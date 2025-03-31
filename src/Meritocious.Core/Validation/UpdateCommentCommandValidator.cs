using FluentValidation;
using Meritocious.Core.Features.Comments.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Validation
{
    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(x => x.CommentId).NotEmpty();
            RuleFor(x => x.EditorId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
        }
    }
}
