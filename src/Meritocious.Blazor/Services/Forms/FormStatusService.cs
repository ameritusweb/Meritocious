using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meritocious.Blazor.Services.Forms
{
    public interface IFormStatusService
    {
        bool IsLoading { get; }
        string? ErrorMessage { get; }
        Dictionary<string, List<string>> FieldErrors { get; }
        
        Task StartProcessingAsync(Func<Task> action);
        void SetFieldError(string field, string error);
        void ClearErrors();
        void SetErrorMessage(string message);
    }

    public class FormStatusService : IFormStatusService
    {
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Dictionary<string, List<string>> FieldErrors { get; } = new();

        public async Task StartProcessingAsync(Func<Task> action)
        {
            try
            {
                IsLoading = true;
                ClearErrors();
                await action();
            }
            catch (Exception ex)
            {
                SetErrorMessage(ex.Message);
                throw;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SetFieldError(string field, string error)
        {
            if (!FieldErrors.ContainsKey(field))
            {
                FieldErrors[field] = new List<string>();
            }
            FieldErrors[field].Add(error);
        }

        public void ClearErrors()
        {
            ErrorMessage = null;
            FieldErrors.Clear();
        }

        public void SetErrorMessage(string message)
        {
            ErrorMessage = message;
        }
    }
}