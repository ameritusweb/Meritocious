namespace Meritocious.Core.Entities;

public class SecurityEvent : BaseEntity
{
    public string EventType { get; private set; }

    public string UserId { get; private set; }

    public User User { get; private set; }

    public string IpAddress { get; private set; }

    public string UserAgent { get; private set; }

    public string Description { get; private set; }

    public int Severity { get; private set; } // 1-5, with 5 being most severe

    public bool RequiresAction { get; private set; }

    public bool IsResolved { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public string ResolvedBy { get; private set; }

    public string ResolutionNotes { get; private set; }
    
    private SecurityEvent()
    {
    }
    
    public static SecurityEvent Create(
        string eventType,
        User user,
        string ipAddress,
        string userAgent,
        string description,
        int severity,
        bool requiresAction = false)
    {
        return new SecurityEvent
        {
            EventType = eventType,
            UserId = user?.Id,
            User = user,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Description = description,
            Severity = Math.Clamp(severity, 1, 5),
            RequiresAction = requiresAction,
            IsResolved = false,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Resolve(string resolvedByUserId, string notes)
    {
        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
        ResolvedBy = resolvedByUserId;
        ResolutionNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}