using Meritocious.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meritocious.Core.Entities;

public class ForkRequest : BaseEntity<ForkRequest>
{
    [ForeignKey("FK_SubmitterId")]
    public UlidId<User> SubmitterId { get; private set; }
    public User Submitter { get; private set; }

    [ForeignKey("FK_ExternalForkSourceId")]
    public UlidId<ExternalForkSource> ExternalForkSourceId { get; private set; }
    public ExternalForkSource ExternalForkSource { get; private set; }

    public string SuggestedFocus { get; private set; }
    public string? Notes { get; private set; }

    [ForeignKey("FK_ClaimedById")]
    public UlidId<User>? ClaimedById { get; private set; }
    public User? ClaimedBy { get; private set; }

    public DateTime? ClaimedAt { get; private set; }
    public string Status { get; private set; } // open, claimed, fulfilled

    [ForeignKey("FK_FulfilledByPostId")]
    public UlidId<Post>? FulfilledByPostId { get; private set; }
    public Post? FulfilledByPost { get; private set; }

    private ForkRequest()
    {
    }

    public static ForkRequest Create(
        User submitter,
        ExternalForkSource source,
        string suggestedFocus,
        string? notes = null)
    {
        return new ForkRequest
        {
            SubmitterId = submitter.Id,
            Submitter = submitter,
            ExternalForkSourceId = source.Id,
            ExternalForkSource = source,
            SuggestedFocus = suggestedFocus,
            Notes = notes,
            Status = "open",
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Claim(User claimer)
    {
        if (Status != "open")
        {
            throw new InvalidOperationException("Request is not open for claiming");
        }

        ClaimedById = claimer.Id;
        ClaimedBy = claimer;
        ClaimedAt = DateTime.UtcNow;
        Status = "claimed";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fulfill(Post post)
    {
        if (Status != "claimed")
        {
            throw new InvalidOperationException("Request must be claimed before fulfilling");
        }

        FulfilledByPostId = post.Id;
        FulfilledByPost = post;
        Status = "fulfilled";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unclaim()
    {
        if (Status != "claimed")
        {
            throw new InvalidOperationException("Request is not currently claimed");
        }

        ClaimedById = null;
        ClaimedBy = null;
        ClaimedAt = null;
        Status = "open";
        UpdatedAt = DateTime.UtcNow;
    }
}