using Meritocious.Core.Features.Tags.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; private set; }
        public string Slug { get; private set; }
        public string Description { get; private set; }
        public Guid? ParentTagId { get; private set; }
        public Tag ParentTag { get; private set; }
        public TagCategory Category { get; private set; }
        public int UseCount { get; private set; }
        public decimal MeritThreshold { get; private set; }
        public TagStatus Status { get; private set; }
        public List<TagSynonym> Synonyms { get; private set; }
        public List<TagRelationship> RelatedTags { get; private set; }
        public List<Tag> ChildTags { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }
        public List<TagWiki> WikiVersions { get; private set; }
        public int FollowerCount { get; internal set; }
        public List<Post> Posts { get; private set; } = new();

        private Tag()
        {
            Synonyms = new List<TagSynonym>();
            RelatedTags = new List<TagRelationship>();
            ChildTags = new List<Tag>();
            Metadata = new Dictionary<string, string>();
            WikiVersions = new List<TagWiki>();
        }

        public static Tag Create(
            string name,
            string description,
            TagCategory category,
            Tag parentTag = null,
            decimal meritThreshold = 0.5m)
        {
            return new Tag
            {
                Name = name,
                Slug = GenerateSlug(name),
                Description = description,
                ParentTagId = parentTag?.Id,
                ParentTag = parentTag,
                Category = category,
                UseCount = 0,
                MeritThreshold = meritThreshold,
                Status = TagStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AddRelatedTag(
            Tag relatedTag,
            TagRelationType relationType,
            decimal strength,
            User creator)
        {
            if (!RelatedTags.Any(r => r.RelatedTagId == relatedTag.Id))
            {
                RelatedTags.Add(TagRelationship.Create(this, relatedTag, relationType, strength, creator));
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void IncrementUseCount()
        {
            UseCount++;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateWiki(string content, User editor, string editReason)
        {
            WikiVersions.Add(TagWiki.Create(this, content, editor, editReason));
            UpdatedAt = DateTime.UtcNow;
        }

        internal static string GenerateSlug(string name)
        {
            return name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-")
                .Where(c => char.IsLetterOrDigit(c) || c == '-')
                .Aggregate("", (current, c) => current + c);
        }
    }
}