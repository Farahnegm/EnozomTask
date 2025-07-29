using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;
using Microsoft.EntityFrameworkCore;

namespace EnozomTask.InfraStructure.Repositories
{
    public class TimeEntryRepository : ITimeEntryRepository
    {
        private readonly AppDbContext _dbContext;
        public TimeEntryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(TimeEntry timeEntry)
        {
            _dbContext.TimeEntries.Add(timeEntry);
        }
        public async Task<IEnumerable<TimeEntry>> GetAllAsync()
        {
            return await _dbContext.TimeEntries.ToListAsync();
        }
        public async Task<TimeEntry> GetByIdAsync(int id)
        {
            return await _dbContext.TimeEntries.FindAsync(id);
        }
        public void Update(TimeEntry timeEntry)
        {
            _dbContext.TimeEntries.Update(timeEntry);
        }
        public void Delete(TimeEntry timeEntry)
        {
            _dbContext.TimeEntries.Remove(timeEntry);
        }
    }
} 