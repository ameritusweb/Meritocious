using System.Text.RegularExpressions;
using MediatR;
using Microsoft.Extensions.Logging;
using Meritocious.Core.Features.Substacks.Services;
using Meritocious.Core.Features.Posts.Commands;
using Meritocious.Core.Results;
using Meritocious.Core.Features.Substacks.Models;
using Meritocious.Core.Commands;

namespace Meritocious.Core.Features.Substacks.Commands;

public class ImportSubstackPostCommand : IRequest<Result<Guid>>
{
    public string PostUrl { get; set; }
    public string SubstackName { get; set; }
    public string UserId { get; set; }
    public bool ImportAsRemix { get; set; }
    public string RemixNotes { get; set; }
}

public class ImportSubstackPostCommandHandler : IRequestHandler<ImportSubstackPostCommand, Result<Guid>>
{
    private readonly ISubstackFeedService substackService;
    private readonly IMediator mediator;
    private readonly ILogger<ImportSubstackPostCommandHandler> logger;

    public ImportSubstackPostCommandHandler(
        ISubstackFeedService substackService,
        IMediator mediator,
        ILogger<ImportSubstackPostCommandHandler> logger)
    {
        this.substackService = substackService;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task<Result<Guid>> Handle(ImportSubstackPostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate the URL
            if (!await substackService.ValidateSubstackUrlAsync(request.PostUrl))
            {
                return Result<Guid>.Failure("Invalid Substack post URL");
            }

            // Get the post content
            var content = await substackService.GetPostContentAsync(request.PostUrl);
            if (string.IsNullOrEmpty(content))
            {
                return Result<Guid>.Failure("Failed to fetch post content");
            }

            // Clean up the HTML content
            var cleanContent = CleanHtmlContent(content);

            // TODO: Make this work.
            // if (request.ImportAsRemix)
            // {
            //    var remixCommand = new ForkPostCommand
            //    {
            //        OriginalPostUrl = request.PostUrl,
            //        Content = cleanContent,
            //        UserId = request.UserId,
            //        Source = "substack",
            //        SourceName = request.SubstackName,
            //        Notes = request.RemixNotes
            //    };

            // var remixResult = await _mediator.Send(remixCommand, cancellationToken);
            //    return remixResult;
            // }
            // else
            // {
            //    var createCommand = new CreatePostCommand
            //    {
            //        Content = cleanContent,
            //        UserId = request.UserId,
            //        Source = "substack",
            //        SourceUrl = request.PostUrl,
            //        SourceName = request.SubstackName
            //    };
            //    var createResult = await _mediator.Send(createCommand, cancellationToken);
            //    return createResult;
            // }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error importing Substack post from {Url}", request.PostUrl);
            return Result<Guid>.Failure("Failed to import Substack post");
        }

        // TODO: Remove this.
        return Result<Guid>.Success(Guid.NewGuid());
    }

    private string CleanHtmlContent(string html)
    {
        // Remove script tags
        html = Regex.Replace(html, @"<script.*?</script>", "", 
            RegexOptions.Singleline);

        // Remove style tags
        html = Regex.Replace(html, @"<style.*?</style>", "", 
            RegexOptions.Singleline);

        // Convert divs to paragraphs
        html = Regex.Replace(html, @"<div.*?>", "<p>", 
            RegexOptions.Singleline);
        html = Regex.Replace(html, @"</div>", "</p>", 
            RegexOptions.Singleline);

        // Remove class attributes
        html = Regex.Replace(html, @"\s+class="".*?""", "");

        // Remove data attributes
        html = Regex.Replace(html, @"\s+data-[a-zA-Z0-9-]+="".*?""", "");

        // Remove empty paragraphs
        html = Regex.Replace(html, @"<p>\s*</p>", "");

        // Convert double line breaks to paragraphs
        html = Regex.Replace(html, @"\n\n", "</p><p>");

        // Clean up any remaining HTML special characters
        html = System.Web.HttpUtility.HtmlDecode(html);

        return html.Trim();
    }
}