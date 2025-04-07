using System.Threading.Tasks;
using AntDesign;
using Meritocious.Blazor.Services.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Meritocious.Blazor.Pages.Auth
{
    public partial class RegisterPage
    {
        private int currentStep = 0;
        private bool isGoogleLoading;
        private bool isCheckingUsername;
        private bool? usernameAvailable;

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

        [Inject] public NavigationManager NavigationManager { get; set; } = default!;
        [Inject] public IAuthService AuthService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public MessageService MessageService { get; set; } = default!;
        [Inject] public IFormStatusService FormStatus { get; set; } = default!;
        [Inject] public IValidationService ValidationService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Load topic categories
            topicCategories = await AuthService.GetTopicCategoriesAsync();
        }

        private async Task HandleAccountStep(RegisterModel model)
        {
            if (await ValidateAccountStep())
            {
                currentStep++;
            }
        }

        private async Task HandleProfileStep(ProfileModel model)
        {
            if (ValidateProfileStep())
            {
                currentStep++;
            }
        }

        private async Task HandleInterestsStep(InterestsModel model)
        {
            if (ValidateInterestsStep())
            {
                await CompleteRegistration();
            }
        }

        private async Task ValidateEmail(string value)
        {
            FormStatus.ClearErrors();
            if (!ValidationService.ValidateEmail(value))
            {
                FormStatus.SetFieldError("email", "Please enter a valid email address");
            }
        }

        private async Task ValidateUsername(string value)
        {
            FormStatus.ClearErrors();
            if (string.IsNullOrEmpty(value))
            {
                FormStatus.SetFieldError("username", "Username is required");
                return;
            }

            isCheckingUsername = true;
            try
            {
                usernameAvailable = await ValidationService.ValidateUsernameAvailabilityAsync(value);
                if (!usernameAvailable.Value)
                {
                    FormStatus.SetFieldError("username", "Username is already taken");
                }
            }
            finally
            {
                isCheckingUsername = false;
            }
        }

        private async Task ValidatePassword(string value)
        {
            FormStatus.ClearErrors();
            if (!await ValidationService.ValidatePasswordStrengthAsync(value))
            {
                FormStatus.SetFieldError("password", "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character");
            }
        }

        private void ValidatePasswordMatch(string value)
        {
            FormStatus.ClearErrors();
            if (value != registerModel.Password)
            {
                FormStatus.SetFieldError("confirmPassword", "Passwords do not match");
            }
        }

        private async Task<bool> ValidateAccountStep()
        {
            FormStatus.ClearErrors();
            
            if (string.IsNullOrEmpty(registerModel.Email))
                FormStatus.SetFieldError("email", "Email is required");
            else if (!ValidationService.ValidateEmail(registerModel.Email))
                FormStatus.SetFieldError("email", "Please enter a valid email address");

            if (string.IsNullOrEmpty(registerModel.Username))
                FormStatus.SetFieldError("username", "Username is required");
            else if (!await ValidationService.ValidateUsernameAvailabilityAsync(registerModel.Username))
                FormStatus.SetFieldError("username", "Username is already taken");

            if (string.IsNullOrEmpty(registerModel.Password))
                FormStatus.SetFieldError("password", "Password is required");
            else if (!await ValidationService.ValidatePasswordStrengthAsync(registerModel.Password))
                FormStatus.SetFieldError("password", "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character");

            if (string.IsNullOrEmpty(registerModel.ConfirmPassword))
                FormStatus.SetFieldError("confirmPassword", "Please confirm your password");
            else if (registerModel.Password != registerModel.ConfirmPassword)
                FormStatus.SetFieldError("confirmPassword", "Passwords do not match");

            return !FormStatus.FieldErrors.Any();
        }

        private bool ValidateProfileStep()
        {
            FormStatus.ClearErrors();
            
            if (string.IsNullOrEmpty(profileModel.DisplayName))
            {
                FormStatus.SetFieldError("displayName", "Display name is required");
                return false;
            }

            return true;
        }

        private bool ValidateInterestsStep()
        {
            FormStatus.ClearErrors();
            
            if (!interestsModel.Topics.Any())
            {
                FormStatus.SetFieldError("topics", "Please select at least one topic");
                return false;
            }

            return true;
        }

        private async Task CompleteRegistration()
        {
            await FormStatus.StartProcessingAsync(async () =>
            {
                var registrationRequest = new RegistrationRequest
                {
                    Email = registerModel.Email,
                    Username = registerModel.Username,
                    Password = registerModel.Password,
                    DisplayName = profileModel.DisplayName,
                    Bio = profileModel.Bio,
                    AvatarUrl = profileModel.AvatarUrl,
                    Topics = interestsModel.Topics,
                    ContentPreferences = contentPreferences
                        .ToDictionary(p => p.Key, p => p.Value),
                    GoogleIdToken = await JSRuntime.InvokeAsync<string>("initializeGoogleSignIn")
                };

                var result = await AuthService.RegisterAsync(registrationRequest);

                if (result.Success)
                {
                    if (result.RequiresGoogleLink)
                    {
                        NavigationManager.NavigateTo("/account/linkgoogle");
                    }
                    else
                    {
                        await MessageService.Success("Registration successful!");
                        NavigationManager.NavigateTo("/onboarding");
                    }
                }
                else
                {
                    FormStatus.SetErrorMessage(result.Error ?? "Registration failed");
                }
            });
        }

        private async Task HandleGoogleSignUpAsync()
        {
            isGoogleLoading = true;
            FormStatus.ClearErrors();

            try
            {
                var idToken = await JSRuntime.InvokeAsync<string>("initializeGoogleSignIn");
                if (string.IsNullOrEmpty(idToken))
                {
                    FormStatus.SetErrorMessage("Google sign-in was cancelled");
                    return;
                }

                // First verify the token and get user info
                var googleResult = await AuthService.GoogleLoginAsync(idToken);
                if (!googleResult.Success)
                {
                    FormStatus.SetErrorMessage(googleResult.Error ?? "Failed to verify Google account");
                    return;
                }

                // Pre-fill form with Google data
                registerModel.Email = googleResult.User.Email;
                registerModel.Username = await GenerateUsernameFromEmailAsync(googleResult.User.Email);
                profileModel.DisplayName = googleResult.User.DisplayName;
                profileModel.AvatarUrl = googleResult.User.AvatarUrl;

                // Store token for later use during registration
                await JSRuntime.InvokeVoidAsync("localStorage.setItem", "pendingGoogleToken", idToken);
                
                // Move to next step
                currentStep++;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Google sign-up");
                FormStatus.SetErrorMessage("An unexpected error occurred during Google sign-up");
            }
            finally
            {
                isGoogleLoading = false;
            }
        }

        private async Task HandleAvatarChange(UploadInfo info)
        {
            if (info.File.State == UploadState.Success)
            {
                profileModel.AvatarUrl = info.File.Response;
            }
        }
    }
}
