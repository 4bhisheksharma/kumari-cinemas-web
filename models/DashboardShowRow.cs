namespace KumariCinemas.Web.Models;

public class DashboardShowRow
{
    public string MovieTitle { get; set; } = string.Empty;
    public string HallName { get; set; } = string.Empty;
    public string ShowTimeStr { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Sold { get; set; }
    public int Available => Capacity - Sold;
    public (string Label, string CssClass) Status => Available <= 0
        ? ("Houseful", "kc-status-full")
        : Available < Capacity * 0.2m
            ? ("Filling Fast", "kc-status-filling")
            : ("Open", "kc-status-open");
}
