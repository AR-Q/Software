using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CloudHosting.Application.Services;
using CloudHosting.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace CloudHosting.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HostingController : ControllerBase
    {
        private readonly CloudHostingService _service;

        public HostingController(CloudHostingService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Deploy([FromBody] CreateHostingDeploymentDto dto)
        {
            var deployment = await _service.DeployAsync(dto.UserId, dto.Plan, dto.SourceType, dto.SourcePath);
            return Ok(deployment);
        }

        // Add more endpoints as needed (Get, Delete, etc.)
    }

    public enum DeploymentStatus
    {
        Pending,
        Running,
        Failed,
        Stopped
    }

    public class HostingDeployment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionPlan Plan { get; set; }

        [Required]
        [MaxLength(32)]
        public string SourceType { get; set; } // "Dockerfile", "Source", "Compiled"

        [Required]
        public string SourcePath { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DeploymentStatus Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}