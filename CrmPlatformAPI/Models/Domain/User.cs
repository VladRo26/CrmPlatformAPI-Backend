﻿using CrmPlatformAPI.Helpers.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmPlatformAPI.Models.Domain
{
    public class User:IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? SoftwareCompanyId { get; set; } 
        public SoftwareCompany? SoftwareCompany { get; set; }

        public int? BeneficiaryCompanyId { get; set; }
        public BeneficiaryCompany? BeneficiaryCompany { get; set; }

        public UserType UserType { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
