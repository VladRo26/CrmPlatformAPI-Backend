﻿using CrmPlatformAPI.Helpers.Enums;
using CrmPlatformAPI.Models.Domain;
using CrmPlatformAPI.Models.DTO;

namespace CrmPlatformAPI.Repositories.Interface
{
    public interface IRepositoryTicketStatusHistory
    {

        Task<IEnumerable<TicketStatusHistory>> GetHistoryByTicketIdAsync(int ticketId);

        Task AddHistoryAsync(int ticketId, TicketStatusHistoryDTO dto);

    }
}