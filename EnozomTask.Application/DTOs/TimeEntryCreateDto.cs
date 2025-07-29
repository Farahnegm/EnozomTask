namespace EnozomTask.Application.DTOs
{
    public class TimeEntryCreateDto
    {
        public string UserFullName { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public double EstimateHours { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
} 