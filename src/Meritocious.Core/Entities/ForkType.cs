using Meritocious.Core.Extensions;

namespace Meritocious.Core.Entities;

public class ForkType : BaseEntity<ForkType>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public List<string> Subtypes { get; private set; } = new();
    public bool IsActive { get; private set; }

    private readonly List<Post> posts = new();
    public IReadOnlyCollection<Post> Posts => posts.AsReadOnly();

    private ForkType()
    {
    }

    public static ForkType Create(
        string name,
        string displayName,
        string description,
        List<string> subtypes)
    {
        return new ForkType
        {
            Name = name,
            DisplayName = displayName,
            Description = description,
            Subtypes = subtypes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateSubtypes(List<string> newSubtypes)
    {
        Subtypes = newSubtypes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubtype(string subtype)
    {
        if (!Subtypes.Contains(subtype))
        {
            Subtypes.Add(subtype);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}