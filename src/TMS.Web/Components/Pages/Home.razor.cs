using Microsoft.AspNetCore.Components;
using TMS.Web.Models.ViewModels;

namespace TMS.Web.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<DashboardModels.StatCardData> statCards = DashboardModels.GetStatCards();
    private List<DashboardModels.MetricData> metrics = DashboardModels.GetMetrics();
    private List<DashboardModels.ChartItem> chartData = DashboardModels.GetChartData();
    private List<DashboardModels.ActivityItem> recentActivity = DashboardModels.GetRecentActivity();
    private List<DashboardModels.DeadlineItem> upcomingDeadlines = DashboardModels.GetUpcomingDeadlines();
}
