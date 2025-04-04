
namespace Meritocious.Core.Interfaces
{
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Tags.Models;

    public interface ITagService
    {
        Task<Tag> CreateTagAsync(string name, TagCategory category, string description = null);
        Task<Tag> GetTagByNameAsync(string name);
        Task<List<Tag>> GetPopularTagsAsync(int count = 10);
        Task<List<Tag>> SearchTagsAsync(string searchTerm);
        Task AddTagToPostAsync(Guid postId, string tagName, TagCategory category);
        Task RemoveTagFromPostAsync(Guid postId, string tagName);
        Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames);
        Task<IEnumerable<Tag>> GetRelatedTagsAsync(string topic);
    }
}