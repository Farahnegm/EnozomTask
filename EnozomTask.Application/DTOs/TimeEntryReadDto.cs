using System;

namespace EnozomTask.Application.DTOs
{
    public class TimeEntryReadDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
} 