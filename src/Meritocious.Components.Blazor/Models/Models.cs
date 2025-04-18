﻿using Microsoft.AspNetCore.Components;
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

    public class ForkNodeData
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string Excerpt { get; set; } = "";
        public double Merit { get; set; }
        public int Forks { get; set; }
        public int Replies { get; set; }
        public List<ForkNodeData> Children { get; set; } = new();
    }

    public enum RoadmapStatus
    {
        Completed,
        InProgress,
        Upcoming
    }

    public class TabItem
    {
        public string Title { get; set; } = "";
        public RenderFragment? Icon { get; set; }
        public RenderFragment Content { get; set; } = builder => { };
    }

    public class RoadmapItem
    {
        public string Title { get; set; } = "";
        public RoadmapStatus Status { get; set; }
        public string? CompletionDate { get; set; }
        public int? Progress { get; set; }
        public string? StartDate { get; set; }
    }

    public class EvolutionNode
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Excerpt { get; set; } = "";
        public double Merit { get; set; }
        public int Forks { get; set; }
        public int Replies { get; set; }
    }

    public class BloomStat
    {
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public double Value { get; set; }
    }

    public class BloomDataPoint
    {
        public double Score { get; set; }
        public string Date { get; set; } = "";
        public int Forks { get; set; }
        public int Replies { get; set; }
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

    public record FilterChip(string Id, string Label, string Type);
}
