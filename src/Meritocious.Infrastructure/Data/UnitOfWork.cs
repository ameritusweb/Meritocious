using Meritocious.Core.Entities;
using Meritocious.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Infrastructure.Data
{
    public class UnitOfWork : IDisposable
    {
        private readonly MeritociousDbContext context;
        private bool disposed;

        private UserRepository userRepository;
        private PostRepository postRepository;
        private CommentRepository commentRepository;
        private TagRepository tagRepository;
        private UserManager<User> userManager;

        public UnitOfWork(MeritociousDbContext context)
        {
            this.context = context;
        }

        public UserRepository Users =>
            userRepository ??= new UserRepository(context, userManager);

        public PostRepository Posts =>
            postRepository ??= new PostRepository(context);

        public CommentRepository Comments =>
            commentRepository ??= new CommentRepository(context);

        public TagRepository Tags =>
            tagRepository ??= new TagRepository(context);

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }

            disposed = true;
        }
    }
}