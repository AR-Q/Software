using System;
using System.Collections.Generic;

namespace CloudHosting.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailVerified { get; set; }
        public List<string> Roles { get; set; } = new();
        public SubscriptionPlan CurrentPlan { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }
        
        public bool HasActiveSubscription(SubscriptionPlan plan)
        {
            return CurrentPlan >= plan && 
                   (!SubscriptionExpiryDate.HasValue || SubscriptionExpiryDate.Value > DateTime.UtcNow);
        }
    }
}