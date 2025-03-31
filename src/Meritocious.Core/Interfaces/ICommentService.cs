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
        Task<Comment> AddCommentAsync(string content, Guid postId, User author, Guid? parentCommentId = null);
        Task<Comment> UpdateCommentAsync(Guid commentId, string content);
        Task DeleteCommentAsync(Guid commentId);
        Task<List<Comment>> GetCommentsByPostAsync(Guid postId);
        Task<List<Comment>> GetCommentsByUserAsync(Guid userId);
    }
}