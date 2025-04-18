﻿using System;
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
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly IMeritScorer meritScorer;
        private readonly ILogger<PostService> logger;

        public PostService(
            IPostRepository postRepository,
            IUserRepository userRepository,
            IMeritScorer meritScorer,
            ILogger<PostService> logger)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.meritScorer = meritScorer;
            this.logger = logger;
        }

        public async Task<Post> CreatePostAsync(string title, string content, User author, Post parent = null)
        {
            // Validate content quality using AI
            var contentScore = await meritScorer.ScoreContentAsync(content);
            if (!await meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Content does not meet quality standards");
            }

            var post = Post.Create(title, content, author, parent);

            // TODO: Update merit score.
            // post.UpdateMeritScore(contentScore);
            await postRepository.AddAsync(post);
            return post;
        }

        public async Task<Post> UpdatePostAsync(string postId, string title, string content)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            // Validate content quality
            var contentScore = await meritScorer.ScoreContentAsync(content);
            if (!await meritScorer.ValidateContentAsync(content))
            {
                throw new InvalidOperationException("Content does not meet quality standards");
            }

            post.UpdateContent(title, content);
            
            // TODO: Update merit score
            // post.UpdateMeritScore(contentScore);
            await postRepository.UpdateAsync(post);
            return post;
        }

        public async Task<Post> ForkPostAsync(string postId, User newAuthor, string newTitle = null)
        {
            var originalPost = await postRepository.GetByIdAsync(postId);
            if (originalPost == null)
            {
                throw new KeyNotFoundException("Original post not found");
            }

            var forkedPost = originalPost.CreateFork(newAuthor, newTitle);
            await postRepository.AddAsync(forkedPost);

            return forkedPost;
        }

        public async Task DeletePostAsync(string postId)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            post.Delete();
            await postRepository.UpdateAsync(post);
        }

        public async Task<List<Post>> GetTopPostsAsync(int count = 10)
        {
            return await postRepository.GetTopPostsAsync(count);
        }

        public async Task<List<Post>> GetPostsByUserAsync(string userId)
        {
            return await postRepository.GetPostsByUserAsync(userId);
        }

        public async Task<List<Post>> GetPostsByTagAsync(string tagName)
        {
            return await postRepository.GetPostsByTagAsync(tagName);
        }

        public async Task<Post> GetPostByIdAsync(string postId)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            return post;
        }

        public Task UpdatePostActivityAsync(string postId)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}