using AntDesign;
using Meritocious.Blazor.Services.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Meritocious.Blazor.Pages.Auth
{
    public partial class Register
    {
        private int currentStep;
        private bool isLoading;
        private bool isGoogleLoading;
        private bool isCheckingUsername;
        private bool? usernameAvailable;
        private string? errorMessage;
        private System.Timers.Timer? usernameDebounceTimer;

        private RegisterModel registerModel = new();
        private ProfileModel profileModel = new();
        private InterestsModel interestsModel = new();

        private List<TopicCategory> topicCategories = new();
        private List<ContentPreference> contentPreferences = new()
    {
        new("clarity", "Content Clarity", 0.6m),
        new("novelty", "Original Ideas", 0.5m),
        new("relevance", "Topic Relevance", 0.7m)
    };

        protected override async Task OnInitializedAsync()
        {
            // Load topic categories
            topicCategories = await AuthService.GetTopicCategoriesAsync();
        }

        private async Task HandleAccountStep()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // Validate passwords match
                if (registerModel.Password != registerModel.ConfirmPassword)
                {
                    errorMessage = "Passwords do not match";
                    return;
                }

                // Check username availability one final time
                var isAvailable = await AuthService.CheckUsernameAvailabilityAsync(
                    registerModel.Username);

                if (!isAvailable)
                {
                    errorMessage = "Username is no longer available";
                    return;
                }

                // Move to next step
                currentStep++;
            }
            catch (Exception)
            {
                errorMessage = "An error occurred. Please try again.";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleProfileStep()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // Optional: Upload avatar if provided
                if (profileModel.AvatarFile != null)
                {
                    var avatarUrl = await AuthService.UploadAvatarAsync(
                        profileModel.AvatarFile);
                    profileModel.AvatarUrl = avatarUrl;
                }

                // Move to next step
                currentStep++;
            }
            catch (Exception)
            {
                errorMessage = "Failed to save profile. Please try again.";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleInterestsStep()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // Combine all registration data
                var registrationRequest = new CompleteRegistrationRequest
                {
                    Email = registerModel.Email,
                    Username = registerModel.Username,
                    Password = registerModel.Password,
                    DisplayName = profileModel.DisplayName,
                    Bio = profileModel.Bio,
                    AvatarUrl = profileModel.AvatarUrl,
                    Topics = interestsModel.Topics,
                    ContentPreferences = contentPreferences
                        .ToDictionary(p => p.Key, p => p.Value)
                };

                // Complete registration
                var result = await AuthService.CompleteRegistrationAsync(
                    registrationRequest);

                if (result.Success)
                {
                    // Show success message and redirect
                    await MessageService.Success("Registration successful!");
                    NavigationManager.NavigateTo("/onboarding");
                }
                else
                {
                    errorMessage = result.Error ?? "Registration failed";
                }
            }
            catch (Exception)
            {
                errorMessage = "Registration failed. Please try again.";
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task HandleGoogleSignUp()
        {
            try
            {
                isGoogleLoading = true;
                errorMessage = null;

                var idToken = await JSRuntime.InvokeAsync<string>("initializeGoogleSignIn");

                if (!string.IsNullOrEmpty(idToken))
                {
                    var result = await AuthService.GoogleSignUpAsync(idToken);

                    if (result.Success)
                    {
                        // Pre-fill some fields from Google data
                        if (result.GoogleData != null)
                        {
                            registerModel.Email = result.GoogleData.Email;
                            profileModel.DisplayName = result.GoogleData.Name;
                            profileModel.AvatarUrl = result.GoogleData.Picture;
                        }

                        // Move to next step
                        currentStep++;
                    }
                    else
                    {
                        errorMessage = result.Error ?? "Google sign-up failed";
                    }
                }
            }
            catch (Exception)
            {
                errorMessage = "An error occurred during Google sign-up";
            }
            finally
            {
                isGoogleLoading = false;
            }
        }

        private async Task ValidateUsername(string username)
        {
            // Reset timer if it exists
            usernameDebounceTimer?.Stop();
            usernameDebounceTimer?.Dispose();

            if (string.IsNullOrWhiteSpace(username))
            {
                usernameAvailable = null;
                return;
            }

            // Create new timer
            usernameDebounceTimer = new System.Timers.Timer(500);
            usernameDebounceTimer.Elapsed += async (sender, e) =>
            {
                await InvokeAsync(async () =>
                {
                    isCheckingUsername = true;
                    StateHasChanged();

                    try
                    {
                        usernameAvailable = await AuthService
                            .CheckUsernameAvailabilityAsync(username);
                    }
                    finally
                    {
                        isCheckingUsername = false;
                        StateHasChanged();
                    }
                });
            };

            usernameDebounceTimer.AutoReset = false;
            usernameDebounceTimer.Start();
        }

        private async Task HandleAvatarChange(UploadChangeEventArgs args)
        {
            if (args.File.State == UploadState.Success)
            {
                profileModel.AvatarFile = args.File.ObjectURL;
            }
        }

        public void Dispose()
        {
            usernameDebounceTimer?.Dispose();
        }
    }
}
