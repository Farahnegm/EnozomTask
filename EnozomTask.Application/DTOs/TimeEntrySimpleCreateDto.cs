using System;

namespace EnozomTask.Application.DTOs
{
    public class TimeEntrySimpleCreateDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int TaskItemId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
    }
} 