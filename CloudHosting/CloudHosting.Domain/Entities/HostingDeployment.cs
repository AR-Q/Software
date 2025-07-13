using System;
using System.ComponentModel.DataAnnotations;
using CloudHosting.Domain.Enums;

namespace CloudHosting.Domain.Entities
{
    public class HostingDeployment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionPlan Plan { get; set; }

        [Required]
        [MaxLength(32)]
        public string SourceType { get; set; }

        [Required]
        public string SourcePath { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DeploymentStatus Status { get; set; }
        public string? StatusMessage { get; set; }
        public bool IsDeleted { get; set; }
    }
}