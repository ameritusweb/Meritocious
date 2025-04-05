namespace Meritocious.Core.Interfaces
{
    using Meritocious.Core.Entities;
    using Meritocious.Common.DTOs.Content;

    public interface IPostService
    {
        Task<Post> GetPostByIdAsync(string postId);
        Task<Post> CreatePostAsync(string title, string content, User author, Post parent = null);
        Task<Post> UpdatePostAsync(string postId, string title, string content);
        Task<Post> ForkPostAsync(string postId, User newAuthor, string newTitle = null);
        Task DeletePostAsync(string postId);
        Task<List<Post>> GetTopPostsAsync(int count = 10);
        Task<List<Post>> GetPostsByUserAsync(string userId);
        Task<List<Post>> GetPostsByTagAsync(string tagName);
        Task UpdatePostActivityAsync(string postId);
    }
}