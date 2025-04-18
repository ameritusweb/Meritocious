﻿using Meritocious.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Entities
{
    public class TagWiki : BaseEntity<TagWiki>
    {
        [ForeignKey("FK_TagId")]
        public UlidId<Tag> TagId { get; private set; }

        public Tag Tag { get; private set; }

        public string Content { get; private set; }

        public int VersionNumber { get; private set; }

        [ForeignKey("FK_EditorId")]
        public UlidId<User> EditorId { get; private set; }

        public User Editor { get; private set; }

        public string EditReason { get; private set; }

        public bool IsApproved { get; private set; }

        public DateTime? ApprovedAt { get; private set; }

        [ForeignKey("FK_ApprovedById")]
        public UlidId<User>? ApprovedById { get; private set; }

        public User ApprovedBy { get; private set; }

        private TagWiki()
        {
        }

        public static TagWiki Create(
            Tag tag,
            string content,
            User editor,
            string editReason)
        {
            return new TagWiki
            {
                TagId = tag.Id,
                Tag = tag,
                Content = content,
                VersionNumber = tag.WikiVersions.Count + 1,
                EditorId = editor.Id,
                Editor = editor,
                EditReason = editReason,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Approve(User approver)
        {
            IsApproved = true;
            ApprovedAt = DateTime.UtcNow;
            ApprovedById = approver.Id;
            ApprovedBy = approver;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
