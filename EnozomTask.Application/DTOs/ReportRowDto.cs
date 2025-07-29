namespace EnozomTask.Application.DTOs
{
    public class ReportRowDto
    {
        public string User { get; set; }
        public string Project { get; set; }
        public string Task { get; set; }
        public double LocalEstimate { get; set; }
        public double LocalTimeSpent { get; set; }
        public double? ClockifyTimeSpent { get; set; }
    }
} 