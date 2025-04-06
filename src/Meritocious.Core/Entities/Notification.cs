using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public class Notification : BaseEntity<Notification>
{
    [ForeignKey("FK_UserId")]
    public UlidId<User> UserId { get; private set; }
    public User User { get; private set; }
    
    public string Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string Link { get; private set; }
    public bool IsRead { get; private set; }

    // Optional related entities
    [ForeignKey("FK_PostId")]
    public UlidId<Post>? PostId { get; private set; }
    public Post? Post { get; private set; }

    [ForeignKey("FK_CommentId")]
    public UlidId<Comment>? CommentId { get; private set; }
    public Comment? Comment { get; private set; }
    
    private Notification()
    {
    }
    
    public static Notification Create(
        string userId,
        string type,
        string title,
        string message,
        string link = null,
        Post post = null,
        Comment comment = null)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Link = link,
            PostId = post?.Id ?? string.Empty,
            Post = post,
            CommentId = comment?.Id ?? string.Empty,
            Comment = comment,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void MarkAsRead()
    {
        IsRead = true;
        UpdatedAt = DateTime.UtcNow;
    }
}