using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Interfaces
{
    using Meritocious.Core.Entities;

    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(string content, string postId, User author, string? parentCommentId = null);
        Task<Comment> UpdateCommentAsync(string commentId, string content);
        Task DeleteCommentAsync(string commentId);
        Task<List<Comment>> GetCommentsByPostAsync(string postId);
        Task<List<Comment>> GetCommentsByUserAsync(string userId);
    }
}