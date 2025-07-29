using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnozomTask.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? ClockifyId { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }

        public ICollection<TimeEntry> TimeEntries { get; set; }
    }


}
