using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnozomTask.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? ClockifyId { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public User AssignedUser { get; set; }


        public double EstimateHours { get; set; }

        public Project Project { get; set; }

        public ICollection<TimeEntry> TimeEntries { get; set; }
    }


}
