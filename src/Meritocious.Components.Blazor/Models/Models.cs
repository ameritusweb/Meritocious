using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meritocious.Components.Blazor.Models
{
    public enum ButtonVariant
    {
        Primary,
        Secondary,
        Teal,
        Danger
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
        Fork
    }

    public class NavItem
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public bool IsActive { get; set; }
        public decimal? Merit { get; set; }
        public int NotificationCount { get; set; }
    }

    public class SelectOption
    {
        public string Value { get; set; } = "";
        public string Label { get; set; } = "";
    }

    public class EvolutionNodeData
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string Excerpt { get; set; } = "";
        public double Merit { get; set; }
        public int Forks { get; set; }
        public int Replies { get; set; }
        public List<EvolutionNodeData> Children { get; set; } = new();
    }
}
