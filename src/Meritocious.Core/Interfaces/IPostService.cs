namespace Meritocious.Core.Interfaces
{
    using Meritocious.Core.Entities;
    using Meritocious.Common.DTOs.Content;

    public interface IPostService
    {
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<Post> CreatePostAsync(string title, string content, User author, Post parent = null);
        Task<Post> UpdatePostAsync(Guid postId, string title, string content);
        Task<Post> ForkPostAsync(Guid postId, User newAuthor, string newTitle = null);
        Task DeletePostAsync(Guid postId);
        Task<List<Post>> GetTopPostsAsync(int count = 10);
        Task<List<Post>> GetPostsByUserAsync(Guid userId);
        Task<List<Post>> GetPostsByTagAsync(string tagName);
        Task UpdatePostActivityAsync(Guid postId);
    }
}