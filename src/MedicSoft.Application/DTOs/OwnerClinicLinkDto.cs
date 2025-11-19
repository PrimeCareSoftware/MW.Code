using System;

namespace MedicSoft.Application.DTOs
{
    public class OwnerClinicLinkDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime LinkedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimaryOwner { get; set; }
        public string? Role { get; set; }
        public decimal? OwnershipPercentage { get; set; }
        public DateTime? InactivatedDate { get; set; }
        public string? InactivationReason { get; set; }

        // Related data
        public string? ClinicName { get; set; }
        public string? ClinicCNPJ { get; set; }
        public string? OwnerFullName { get; set; }
        public string? OwnerEmail { get; set; }
    }

    public class CreateOwnerClinicLinkDto
    {
        public Guid OwnerId { get; set; }
        public Guid ClinicId { get; set; }
        public bool IsPrimaryOwner { get; set; } = false;
        public string? Role { get; set; }
        public decimal? OwnershipPercentage { get; set; }
    }

    public class UpdateOwnerClinicLinkDto
    {
        public string? Role { get; set; }
        public decimal? OwnershipPercentage { get; set; }
    }

    public class TransferPrimaryOwnershipDto
    {
        public Guid NewPrimaryOwnerId { get; set; }
    }

    public class DeactivateLinkDto
    {
        public string Reason { get; set; } = null!;
    }

    public class OwnerClinicSummaryDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = null!;
        public string ClinicCNPJ { get; set; } = null!;
        public bool IsPrimaryOwner { get; set; }
        public string? Role { get; set; }
        public decimal? OwnershipPercentage { get; set; }
        
        // Subscription info
        public string? SubscriptionPlanName { get; set; }
        public string? SubscriptionStatus { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public bool IsTrialActive { get; set; }
        public int? TrialDaysRemaining { get; set; }
    }
}
