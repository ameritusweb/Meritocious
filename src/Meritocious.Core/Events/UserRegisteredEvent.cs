using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using MediatR;

namespace Meritocious.Core.Events
{
    public class UserRegisteredEvent : INotification
    {
        public string UserId { get; }
        public DateTime RegisteredAt { get; }

        public UserRegisteredEvent(string userId)
        {
            UserId = userId;
            RegisteredAt = DateTime.UtcNow;
        }
    }
}