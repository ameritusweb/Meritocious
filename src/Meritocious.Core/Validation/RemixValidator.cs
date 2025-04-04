using FluentValidation;
using Meritocious.Core.Interfaces;
using Meritocious.Common.DTOs.Content;
using Meritocious.Common.DTOs.Remix;

namespace Meritocious.Core.Validation;

public class CreateRemixRequestValidator : AbstractValidator<CreateRemixRequest>
{
    public CreateRemixRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title must not be empty and should be less than 200 characters");

        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("Author ID is required");

        RuleFor(x => x.Tags)
            .ForEach(tag => tag.MaximumLength(50))
            .WithMessage("Each tag must be less than 50 characters");

        RuleFor(x => x.InitialSourceIds)
            .Must(sources => sources == null || sources.Count <= 10)
            .WithMessage("Cannot start with more than 10 sources");
    }
}

public class UpdateRemixRequestValidator : AbstractValidator<UpdateRemixRequest>
{
    public UpdateRemixRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title must not be empty and should be less than 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content cannot be empty");

        RuleFor(x => x.Tags)
            .ForEach(tag => tag.MaximumLength(50))
            .WithMessage("Each tag must be less than 50 characters");
    }
}

// TODO: Move Validator
//public class AddSourceRequestValidator : AbstractValidator<AddSourceRequest>
//{
//    private readonly IPostRepository _postRepository;

//    public AddSourceRequestValidator(IPostRepository postRepository)
//    {
//        _postRepository = postRepository;

//        RuleFor(x => x.PostId)
//            .NotEmpty()
//            .MustAsync(async (id, ct) => await _postRepository.ExistsAsync(id))
//            .WithMessage("Referenced post does not exist");

//        RuleFor(x => x.Relationship)
//            .NotEmpty()
//            .Must(r => new[] { "support", "contrast", "example", "question" }.Contains(r.ToLower()))
//            .WithMessage("Invalid relationship type");

//        RuleFor(x => x.Context)
//            .MaximumLength(500)
//            .WithMessage("Context should be less than 500 characters");

//        RuleFor(x => x.InitialQuotes)
//            .Must(quotes => quotes == null || quotes.Count <= 5)
//            .WithMessage("Cannot add more than 5 initial quotes");
//    }
//}

public class AddQuoteRequestValidator : AbstractValidator<AddQuoteRequest>
{
    public AddQuoteRequestValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Quote text must not be empty and should be less than 1000 characters");

        RuleFor(x => x.StartPosition)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Start position must be non-negative");

        RuleFor(x => x.EndPosition)
            .GreaterThan(x => x.StartPosition)
            .WithMessage("End position must be greater than start position");

        RuleFor(x => x.Context)
            .MaximumLength(200)
            .WithMessage("Context should be less than 200 characters");
    }
}

public class RemixSearchRequestValidator : AbstractValidator<RemixSearchRequest>
{
    public RemixSearchRequestValidator()
    {
        RuleFor(x => x.Query)
            .MaximumLength(100)
            .WithMessage("Search query should be less than 100 characters");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 5)
            .WithMessage("Cannot search with more than 5 tags");

        RuleFor(x => x.MinMeritScore)
            .InclusiveBetween(0, 1)
            .When(x => x.MinMeritScore.HasValue)
            .WithMessage("Merit score must be between 0 and 1");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}