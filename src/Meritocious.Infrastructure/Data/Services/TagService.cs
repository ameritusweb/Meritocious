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
    using Meritocious.Core.Exceptions;
    using Microsoft.Extensions.Logging;

    public class TagService : ITagService
    {
        private readonly TagRepository tagRepository;
        private readonly PostRepository postRepository;
        private readonly ILogger<TagService> logger;

        public TagService(
            TagRepository tagRepository,
            PostRepository postRepository,
            ILogger<TagService> logger)
        {
            this.tagRepository = tagRepository;
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public async Task<Tag> CreateTagAsync(string name, string description = null)
        {
            var existingTag = await tagRepository.GetByNameAsync(name);
            if (existingTag != null)
            {
                throw new DuplicateResourceException($"Tag '{name}' already exists");
            }

            var tag = Tag.Create(name, description);
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

        public async Task AddTagToPostAsync(Guid postId, string tagName)
        {
            var post = await postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                throw new ResourceNotFoundException($"Post '{postId}' not found");
            }

            var tag = await tagRepository.GetByNameAsync(tagName);
            if (tag == null)
            {
                tag = await CreateTagAsync(tagName);
            }

            post.AddTag(tag);
            await postRepository.UpdateAsync(post);
        }

        public async Task RemoveTagFromPostAsync(Guid postId, string tagName)
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

        public Task<IEnumerable<object>> GetRelatedTagsAsync(string topic)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }
    }
}