using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Commands
{
    using FluentValidation;
    using MediatR;
    using Meritocious.Core.Entities;
    using Meritocious.Core.Features.Tags.Models;

    public class CreatePostCommand : IRequest<Post>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public string? ParentPostId { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<TagCategory> TagCategories { get; set; } = new();
    }

    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(5, 255);

            RuleFor(x => x.Content)
                .NotEmpty()
                .MinimumLength(20);

            RuleFor(x => x.AuthorId)
                .NotEmpty();

            RuleForEach(x => x.Tags)
                .Must(tag => !string.IsNullOrWhiteSpace(tag))
                .WithMessage("Tags cannot be empty");
        }
    }
}