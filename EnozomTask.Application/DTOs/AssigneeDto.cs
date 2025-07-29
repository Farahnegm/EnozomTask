namespace EnozomTask.Application.DTOs
{
    public class AssigneeDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? ClockifyId { get; set; }
        public bool IsInClockify => !string.IsNullOrEmpty(ClockifyId);
    }
} 