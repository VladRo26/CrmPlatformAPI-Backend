﻿namespace CrmPlatformAPI.Models.Domain
{
    public class BeneficiaryCompanies
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? ActivityDomain { get; set; }
        public string? Address { get; set; }
        public DateOnly EstablishmentDate { get; set; }


    }
}

