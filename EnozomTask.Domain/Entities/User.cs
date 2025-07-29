
using System.ComponentModel.DataAnnotations;


namespace EnozomTask.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? ClockifyId { get; set; }

        public ICollection<TimeEntry> TimeEntries { get; set; }
    }

}
