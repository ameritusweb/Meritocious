using System.Threading.Tasks;
using AntDesign;
using Meritocious.Blazor.Services.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Meritocious.Blazor.Pages.Posts
{
    public partial class PostEditorPage
    {
        [Parameter] public string SubstackSlug { get; set; } = "";
        [Parameter] public string? PostId { get; set; }

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public IPostService PostService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public MessageService MessageService { get; set; } = default!;
        [Inject] public IFormStatusService FormStatus { get; set; } = default!;
        [Inject] public ISubstackService SubstackService { get; set; } = default!;

        private bool isEditing => !string.IsNullOrEmpty(PostId);
        private bool isAnalyzing;
        private bool isPreviewVisible;
        private bool ShowVersionHistory => isEditing;
    
        private PostDto? originalPost;
        private PostModel model = new();
        private List<string> availableTags = new();
        private PostAnalysis? analysis;
        private System.Timers.Timer? analysisDebounceTimer;
        private WordCountComponent wordCounter = default!;
        private string versionNotes = string.Empty;
    
        private bool IsValid => !string.IsNullOrWhiteSpace(model.Title) && 
                            !string.IsNullOrWhiteSpace(model.Content);

        protected override async Task OnInitializedAsync()
        {
            await LoadAvailableTags();

            if (isEditing)
            {
                await LoadPost();
            }

            // Set up content analysis timer
            analysisDebounceTimer = new System.Timers.Timer(1000);
            analysisDebounceTimer.Elapsed += async (sender, e) => await AnalyzeContent();
            analysisDebounceTimer.AutoReset = false;
        }

        private async Task LoadAvailableTags()
        {
            availableTags = await PostService.GetAvailableTagsAsync(SubstackSlug);
        }

        private async Task LoadPost()
        {
            await FormStatus.StartProcessingAsync(async () =>
            {
                var post = await PostService.GetPostAsync(SubstackSlug, PostId!);
                
                model = new PostModel
                {
                    Title = post.Title,
                    Content = post.Content,
                    Tags = post.Tags,
                    Visibility = post.Visibility,
                    IsFork = post.IsFork
                };

                if (post.IsFork)
                {
                    originalPost = await PostService.GetOriginalPostAsync(PostId!);
                }
            });
        }

        private void HandleContentChange(string value)
        {
            model.Content = value;
            analysisDebounceTimer?.Stop();
            analysisDebounceTimer?.Start();
        }
    
        private async Task HandleInsertLink(MarkdownToolbarComponent.LinkInfo linkInfo)
        {
            if (!string.IsNullOrWhiteSpace(linkInfo.Url))
            {
                var markdown = $"[{linkInfo.Text}]({linkInfo.Url})";
                await JSRuntime.InvokeVoidAsync("insertTextAtSelection", markdown);
            }
        }
    
        private void HandleTagsChanged(List<string> tags)
        {
            model.Tags = tags;
        }

        private async Task HandleSubmit()
        {
            await SavePost(isDraft: false);
        }

        private async Task HandleSaveDraft()
        {
            await SavePost(isDraft: true);
        }

        private async Task SavePost(bool isDraft)
        {
            if (!IsValid) return;

            await FormStatus.StartProcessingAsync(async () =>
            {
                if (isEditing)
                {
                    await PostService.UpdatePostAsync(SubstackSlug, PostId!, new UpdatePostRequest
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Tags = model.Tags,
                        Visibility = model.Visibility,
                        IsDraft = isDraft,
                        VersionNotes = versionNotes
                    });
                }
                else
                {
                    var postId = await PostService.CreatePostAsync(SubstackSlug, new CreatePostRequest
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Tags = model.Tags,
                        Visibility = model.Visibility,
                        IsDraft = isDraft
                    });

                    PostId = postId;
                }

                await MessageService.Success(isDraft ? "Draft saved" : "Post published");

                if (!isDraft)
                {
                    NavigationManager.NavigateTo($"/s/{SubstackSlug}/{PostId}");
                }
            });
        }

        private async Task AnalyzeContent()
        {
            if (string.IsNullOrWhiteSpace(model.Content))
                return;

            try
            {
                isAnalyzing = true;
                analysis = await PostService.AnalyzePostAsync(new AnalyzePostRequest
                {
                    Title = model.Title,
                    Content = model.Content,
                    Tags = model.Tags
                });
            }
            finally
            {
                isAnalyzing = false;
                StateHasChanged();
            }
        }

        private string GetMeritColor(decimal score)
        {
            return score switch
            {
                >= 0.8m => "#52c41a",
                >= 0.6m => "#1890ff",
                >= 0.4m => "#faad14",
                _ => "#ff4d4f"
            };
        }

        private void NavigateBack()
        {
            NavigationManager.NavigateTo($"/s/{SubstackSlug}");
        }

        public void Dispose()
        {
            analysisDebounceTimer?.Dispose();
        }
    }