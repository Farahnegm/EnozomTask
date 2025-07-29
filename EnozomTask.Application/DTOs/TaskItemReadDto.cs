namespace EnozomTask.Application.DTOs
{
    public class TaskItemReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ProjectName { get; set; }
        public string? AssignedUserName { get; set; }
        public string? AssignedUserClockifyId { get; set; }
        public double EstimateHours { get; set; }
    }
} 