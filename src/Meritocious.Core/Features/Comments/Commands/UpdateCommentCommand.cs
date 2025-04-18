﻿using MediatR;
using Meritocious.Common.DTOs.Content;
using Meritocious.Core.Entities;
using Meritocious.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Core.Features.Comments.Commands
{
    public record UpdateCommentCommand : IRequest<Result<CommentDto>>
    {
        public string CommentId { get; init; }
        public string EditorId { get; init; }
        public string Content { get; init; }
    }
}
