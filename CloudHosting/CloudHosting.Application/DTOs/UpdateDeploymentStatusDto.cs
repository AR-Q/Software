using System.ComponentModel.DataAnnotations;
using CloudHosting.Domain.Enums;

namespace CloudHosting.Application.DTOs
{
    public class UpdateDeploymentStatusDto
    {
        [Required]
        public DeploymentStatus Status { get; set; }
        
        [MaxLength(500)]
        public string? StatusMessage { get; set; }
    }
}