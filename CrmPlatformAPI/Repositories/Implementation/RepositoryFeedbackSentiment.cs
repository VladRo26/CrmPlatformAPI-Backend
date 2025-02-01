﻿using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;
using CrmPlatformAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatformAPI.Repositories.Implementation
{
    public class RepositoryFeedbackSentiment : IRepositoryFeedbackSentiment
    {

        private readonly ApplicationDbContext _context;

        public RepositoryFeedbackSentiment(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSentimentAsync(FeedBackSentiment sentiment)
        {
            _context.FeedbackSentiments.Add(sentiment);
            await _context.SaveChangesAsync();
        }

        public async Task<FeedBackSentiment> GetSentimentByFeedbackIdAsync(int feedbackId)
        {
            if (_context == null)
            {
                return null;
            }
            return await _context.FeedbackSentiments
                .Include(s => s.Feedback) // Include Feedback navigation property if needed
                .FirstOrDefaultAsync(s => s.FeedbackId == feedbackId);
        }

        public async Task<AverageFeedbackSentimentDTO?> GetAverageSentimentByUsernameAsync(string username)
        {
            // Fetch the user by username to get their ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return null; // User not found
            }

            // Get all feedback sentiments for the given user (ToUserId)
            var feedbackSentiments = await _context.FeedbackSentiments
                .Where(s => s.Feedback.ToUserId == user.Id)
                .ToListAsync();

            if (!feedbackSentiments.Any())
            {
                return null; // No sentiment data available
            }

            // Calculate the average sentiment scores
            var averagePositive = feedbackSentiments.Average(s => s.Positive);
            var averageNeutral = feedbackSentiments.Average(s => s.Neutral);
            var averageNegative = feedbackSentiments.Average(s => s.Negative);

            // Return the new DTO without FeedbackId
            return new AverageFeedbackSentimentDTO
            {
                Positive = averagePositive,
                Neutral = averageNeutral,
                Negative = averageNegative
            };
        }



    }
}
