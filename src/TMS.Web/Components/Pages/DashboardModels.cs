using System.Collections.Generic;

namespace TMS.Web.Components.Pages;

public class DashboardModels
{
    public class StatCardData
    {
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconBg { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty;
        public bool IsPositive { get; set; } = true;
    }

    public class MetricData
    {
        public string Label { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string IconBg { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Percentage { get; set; }
        public string BarColor { get; set; } = string.Empty;
    }

    public class ChartItem
    {
        public string Month { get; set; } = string.Empty;
        public int Words { get; set; }
        public int Height { get; set; }
    }

    public class ActivityItem
    {
        public string Initials { get; set; } = string.Empty;
        public string AvatarBg { get; set; } = string.Empty;
        public string AvatarColor { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }

    public class DeadlineItem
    {
        public string Icon { get; set; } = string.Empty;
        public string IconBg { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public string DueText { get; set; } = string.Empty;
        public string TagClass { get; set; } = string.Empty;
    }

    public static List<StatCardData> GetStatCards() => new()
    {
        new() { Label = "Active Tasks", Icon = "file-text", IconBg = "#dbeafe", IconColor = "#2563eb", Value = "24", Trend = "3 from last week", IsPositive = true },
        new() { Label = "Completed Tasks", Icon = "check-circle", IconBg = "#dcfce7", IconColor = "#16a34a", Value = "156", Trend = "12% this month", IsPositive = true },
        new() { Label = "Pending Tasks", Icon = "clock-circle", IconBg = "#fef3c7", IconColor = "#d97706", Value = "12", Trend = "2 from last week", IsPositive = false },
        new() { Label = "Active Projects", Icon = "folder", IconBg = "#f3e8ff", IconColor = "#9333ea", Value = "8", Trend = "1 new this week", IsPositive = true }
    };

    public static List<MetricData> GetMetrics() => new()
    {
        new() { Label = "Words This Month", Icon = "font-size", IconBg = "#dbeafe", IconColor = "#2563eb", Value = "42.6K", Percentage = 78, BarColor = "#3b82f6" },
        new() { Label = "Productivity Score", Icon = "rise", IconBg = "#dcfce7", IconColor = "#16a34a", Value = "91%", Percentage = 91, BarColor = "#22c55e" },
        new() { Label = "QA Score", Icon = "safety-certificate", IconBg = "#f3e8ff", IconColor = "#9333ea", Value = "96.4%", Percentage = 96, BarColor = "#a855f7" },
        new() { Label = "On-Time Delivery", Icon = "carry-out", IconBg = "#fef3c7", IconColor = "#d97706", Value = "94%", Percentage = 94, BarColor = "#f59e0b" }
    };

    public static List<ChartItem> GetChartData() => new()
    {
        new() { Month = "Jan", Words = 28000, Height = 45 },
        new() { Month = "Feb", Words = 32000, Height = 55 },
        new() { Month = "Mar", Words = 38000, Height = 70 },
        new() { Month = "Apr", Words = 35000, Height = 60 },
        new() { Month = "May", Words = 42000, Height = 78 },
        new() { Month = "Jun", Words = 46000, Height = 85 }
    };

    public static List<ActivityItem> GetRecentActivity() => new()
    {
        new() { Initials = "SK", AvatarBg = "#dbeafe", AvatarColor = "#2563eb", Description = "<strong>Sarah Kim</strong> completed Spanish translation task", Time = "2 min ago" },
        new() { Initials = "MR", AvatarBg = "#dcfce7", AvatarColor = "#16a34a", Description = "<strong>Mike Ross</strong> started a new project review", Time = "15 min ago" },
        new() { Initials = "JL", AvatarBg = "#f3e8ff", AvatarColor = "#9333ea", Description = "<strong>Jane Lee</strong> submitted QA report for approval", Time = "1 hour ago" },
        new() { Initials = "TC", AvatarBg = "#fef3c7", AvatarColor = "#d97706", Description = "<strong>Tom Chen</strong> uploaded new glossary terms", Time = "3 hours ago" }
    };

    public static List<DeadlineItem> GetUpcomingDeadlines() => new()
    {
        new() { Icon = "file-text", IconBg = "#fef2f2", IconColor = "#dc2626", Title = "Medical Report Translation", Meta = "EN → AR · 2,400 words", DueText = "Due Today", TagClass = "tag-urgent" },
        new() { Icon = "file-word", IconBg = "#fffbeb", IconColor = "#d97706", Title = "Legal Contract Review", Meta = "FR → EN · 5 pages", DueText = "Tomorrow", TagClass = "tag-soon" },
        new() { Icon = "book", IconBg = "#f0fdf4", IconColor = "#16a34a", Title = "Technical Manual Update", Meta = "DE → EN · 12,000 words", DueText = "In 3 days", TagClass = "tag-normal" },
        new() { Icon = "global", IconBg = "#eff6ff", IconColor = "#2563eb", Title = "Website Localization", Meta = "ES → PT · 8,500 words", DueText = "Jun 18", TagClass = "tag-normal" }
    };
}
