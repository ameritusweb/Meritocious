using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Services
{
    using Meritocious.Core.Entities;
    using Meritocious.Core.Interfaces;
    using Meritocious.Infrastructure.Data.Repositories;
    using Meritocious.AI.MeritScoring.Interfaces;
    using Microsoft.Extensions.Logging;

    public class PostService : IPostService
    {
        private readonly PostRepository _postRepository;
        private readonly UserRepository _userRepository;
        private readonly IMeritScorer _meritScorer;
        private readonly ILogger<PostService> _logger;

        public PostService(
            PostRepository postRepository,
            UserRepository userRepository,
            IMeritScorer meritScorer,
            ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _meritScorer = meritScorer;
            _logger = logger;
        }

        public async Task<Post> CreatePostAsync(string title, string content, User author, Post parent = null)
        {
            // Validate content quality using AI
            var contentScore = await _meritScorer.ScoreContentAsync(content);
            if (!await _meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Content does not meet quality standards");
            }

            var post = Post.Create(title, content, author, parent);
            post.UpdateMeritScore(contentScore.FinalScore);

            await _postRepository.AddAsync(post);
            return post;
        }

        public async Task<Post> UpdatePostAsync(Guid postId, string title, string content)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found");

            // Validate content quality
            var contentScore = await _meritScorer.ScoreContentAsync(content);
            if (!await _meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Content does not meet quality standards");
            }

            post.UpdateContent(title, content);
            post.UpdateMeritScore(contentScore.FinalScore);

            await _postRepository.UpdateAsync(post);
            return post;
        }

        public async Task<Post> ForkPostAsync(Guid postId, User newAuthor, string newTitle = null)
        {
            var originalPost = await _postRepository.GetByIdAsync(postId);
            if (originalPost == null)
                throw new KeyNotFoundException("Original post not found");

            var forkedPost = originalPost.CreateFork(newAuthor, newTitle);
            await _postRepository.AddAsync(forkedPost);

            return forkedPost;
        }

        public async Task DeletePostAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found");

            post.Delete();
            await _postRepository.UpdateAsync(post);
        }

        public async Task<List<Post>> GetTopPostsAsync(int count = 10)
        {
            return await _postRepository.GetTopPostsAsync(count);
        }

        public async Task<List<Post>> GetPostsByUserAsync(Guid userId)
        {
            return await _postRepository.GetPostsByUserAsync(userId);
        }

        public async Task<List<Post>> GetPostsByTagAsync(string tagName)
        {
            return await _postRepository.GetPostsByTagAsync(tagName);
        }
    }
}