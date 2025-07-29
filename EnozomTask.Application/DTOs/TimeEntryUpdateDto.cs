using System;

namespace EnozomTask.Application.DTOs
{
    public class TimeEntryUpdateDto
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int TaskItemId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
} 