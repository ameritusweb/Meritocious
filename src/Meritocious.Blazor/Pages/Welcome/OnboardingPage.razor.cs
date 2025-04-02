using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Meritocious.Web.Pages.Auth;

public partial class Onboarding
{
    private int currentStep;
    private bool isSmallScreen;
    private HashSet<string> selectedInterests = new();
    private HashSet<Guid> selectedSubstacks = new();
    private List<SubstackDto> recommendedSubstacks = new();

    private List<InterestCategory> interestCategories = new()
    {
        new("tech", "Technology", "Software, AI, and digital innovation"),
        new("science", "Science", "Research, discoveries, and breakthroughs"),
        new("philosophy", "Philosophy", "Ideas, ethics, and deep thinking"),
        new("politics", "Politics", "Policy, governance, and current affairs"),
        new("arts", "Arts & Culture", "Literature, music, and creativity"),
        new("business", "Business", "Entrepreneurship and markets"),
    };

    private List<FeedPreference> feedPreferences = new()
    {
        new("clarity", "Content Clarity",
            "How well-written and understandable the content should be", 0.6m),
        new("novelty", "Original Ideas",
            "Preference for unique perspectives and new ideas", 0.5m),
        new("relevance", "Topic Relevance",
            "How closely content should match your interests", 0.7m),
        new("contribution", "Discussion Value",
            "How much the content adds to the conversation", 0.6m)
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadRecommendedSubstacks();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Check screen size
            isSmallScreen = await JS.InvokeAsync<bool>("isSmallScreen");
            StateHasChanged();
        }
    }

    private async Task LoadRecommendedSubstacks()
    {
        try
        {
            recommendedSubstacks = await SubstackService
                .GetRecommendedSubstacksAsync(selectedInterests.ToList());
        }
        catch (Exception ex)
        {
            await MessageService.Error("Failed to load recommendations");
        }
    }

    private void ToggleInterest(string id)
    {
        if (selectedInterests.Contains(id))
            selectedInterests.Remove(id);
        else
            selectedInterests.Add(id);
    }

    private void ToggleSubstack(Guid id)
    {
        if (selectedSubstacks.Contains(id))
            selectedSubstacks.Remove(id);
        else
            selectedSubstacks.Add(id);
    }

    private async Task NextStep()
    {
        if (currentStep == 0 && selectedInterests.Any())
        {
            // Load recommended substacks based on interests
            await LoadRecommendedSubstacks();
        }

        currentStep++;
    }

    private void PreviousStep()
    {
        if (currentStep > 0)
            currentStep--;
    }

    private async Task FinishOnboarding()
    {
        try
        {
            // Save all preferences
            await SubstackService.SaveOnboardingPreferencesAsync(
                new OnboardingPreferences
                {
                    Interests = selectedInterests.ToList(),
                    Substacks = selectedSubstacks.ToList(),
                    FeedPreferences = feedPreferences.ToDictionary(
                        p => p.Key,
                        p => p.Value)
                });

            // Redirect to home page
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            await MessageService.Error("Failed to save preferences");
        }
    }
}

public class InterestCategory
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public InterestCategory(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}

public class FeedPreference
{
    public string Key { get; }
    public string Label { get; }
    public string Description { get; }
    public decimal Value { get; set; }

    public FeedPreference(string key, string label, string description, decimal defaultValue)
    {
        Key = key;
        Label = label;
        Description = description;
        Value = defaultValue;
    }
}

public class OnboardingPreferences
{
    public List<string> Interests { get; set; } = new();
    public List<Guid> Substacks { get; set; } = new();
    public Dictionary<string, decimal> FeedPreferences { get; set; } = new();
}