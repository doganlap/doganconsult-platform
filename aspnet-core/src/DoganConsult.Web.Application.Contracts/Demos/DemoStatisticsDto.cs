namespace DoganConsult.Web.Demos
{
    public class DemoStatisticsDto
    {
        public int ActiveDemos { get; set; }
        public int NewThisWeek { get; set; }
        public int PendingApprovals { get; set; }
        public int SuccessRate { get; set; }
        public int AvgCycleTime { get; set; }
    }
}
