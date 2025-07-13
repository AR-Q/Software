using System;
using System.ComponentModel.DataAnnotations;
using CloudHosting.Domain.Entities;
using CloudHosting.Domain.Enums;

namespace CloudHosting.Application.DTOs
{
    public class HostingDeploymentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionPlan Plan { get; set; }
        public string SourceType { get; set; }
        public string SourcePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DeploymentStatus Status { get; set; }
        public string? StatusMessage { get; set; }
    }

    public class CreateHostingDeploymentDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public SubscriptionPlan Plan { get; set; }

        [Required]
        [MaxLength(32)]
        public string SourceType { get; set; }

        [Required]
        public string SourcePath { get; set; }
    }
}