namespace EnozomTask.Application.DTOs
{
    public class TaskItemUpdateDto
    {
        public string Title { get; set; }
        public double EstimateHours { get; set; }
        public int ProjectId { get; set; }
        public int AssignedUserId { get; set; }
    }
} 