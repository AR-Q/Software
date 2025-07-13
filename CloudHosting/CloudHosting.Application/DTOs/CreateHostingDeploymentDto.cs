using System;
using System.ComponentModel.DataAnnotations;
using CloudHosting.Domain.Enums;

namespace CloudHosting.Application.DTOs
{
    public class CreateHostingDeploymentDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public SubscriptionPlan Plan { get; set; }

        [Required]
        [MaxLength(32)]
        [RegularExpression("^(Dockerfile|Source|Compiled)$", 
            ErrorMessage = "SourceType must be either 'Dockerfile', 'Source', or 'Compiled'")]
        public string SourceType { get; set; }

        [Required]
        [MaxLength(256)]
        public string SourcePath { get; set; }
    }
}