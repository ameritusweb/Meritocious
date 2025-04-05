﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class UserSession : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }
}
