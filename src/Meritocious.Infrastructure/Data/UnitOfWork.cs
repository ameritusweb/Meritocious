using Meritocious.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly MeritociousDbContext _context;
        private bool _disposed;

        private UserRepository _userRepository;
        private PostRepository _postRepository;
        private CommentRepository _commentRepository;
        private TagRepository _tagRepository;

        public UnitOfWork(MeritociousDbContext context)
        {
            _context = context;
        }

        public UserRepository Users =>
            _userRepository ??= new UserRepository(_context);

        public PostRepository Posts =>
            _postRepository ??= new PostRepository(_context);

        public CommentRepository Comments =>
            _commentRepository ??= new CommentRepository(_context);

        public TagRepository Tags =>
            _tagRepository ??= new TagRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}