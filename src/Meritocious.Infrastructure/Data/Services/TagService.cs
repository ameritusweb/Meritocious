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
    using Meritocious.Core.Exceptions;
    using Microsoft.Extensions.Logging;
    using Meritocious.Core.Features.Tags.Models;

    public class TagService : ITagService
    {
        private readonly ITagRepository tagRepository;
        private readonly IPostRepository postRepository;
        private readonly ILogger<TagService> logger;

        public TagService(
            ITagRepository tagRepository,
            IPostRepository postRepository,
            ILogger<TagService> logger)
        {
            this.tagRepository = tagRepository;
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Tag> CreateTagAsync(string name, TagCategory category, string description = null)
        {
            var existingTag = await tagRepository.GetByNameAsync(name);
            if (existingTag != null)
            {
                throw new DuplicateResourceException($"Tag '{name}' already exists");
            }

            var tag = Tag.Create(name, description, category);
            await tagRepository.AddAsync(tag);
            return tag;
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            var tag = await tagRepository.GetByNameAsync(name);
            if (tag == null)
            {
                throw new ResourceNotFoundException($"Tag '{name}' not found");
            }

            return tag;
        }

        public async Task<List<Tag>> GetPopularTagsAsync(int count = 10)
        {
            return await tagRepository.GetPopularTagsAsync(count);
        }

        public async Task<List<Tag>> SearchTagsAsync(string searchTerm)
        {
            return await tagRepository.SearchTagsAsync(searchTerm);
        }

        public async Task AddTagToPostAsync(string postId, string tagName, TagCategory category)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new ResourceNotFoundException($"Post '{postId}' not found");
            }

            var tag = await tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                tag = await CreateTagAsync(tagName, category);
            }

            post.AddTag(tag);
            await postRepository.UpdateAsync(post);
        }

        public async Task RemoveTagFromPostAsync(string postId, string tagName)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new ResourceNotFoundException($"Post '{postId}' not found");
            }

            var tag = await tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                throw new ResourceNotFoundException($"Tag '{tagName}' not found");
            }

            // Note: AddTag/RemoveTag logic is handled in the Post entity
            // We might need to add a RemoveTag method to the Post entity
            await postRepository.UpdateAsync(post);
        }

        public Task<IEnumerable<Tag>> GetOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }

        Task<IEnumerable<Tag>> ITagService.GetRelatedTagsAsync(string topic)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}