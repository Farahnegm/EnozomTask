using System.Threading.Tasks;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EnozomTask.InfraStructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDbContext _dbContext;
        public TaskItemRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TaskItem> GetByNameAndProjectIdAsync(string name, int projectId)
        {
            return await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Name == name && t.ProjectId == projectId);
        }
        public void Add(TaskItem taskItem)
        {
            _dbContext.TaskItems.Add(taskItem);
        }
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _dbContext.TaskItems.ToListAsync();
        }
        public async Task<TaskItem> GetByIdAsync(int id)
        {
            return await _dbContext.TaskItems.FindAsync(id);
        }
        public void Update(TaskItem taskItem)
        {
            _dbContext.TaskItems.Update(taskItem);
        }
        public void Delete(TaskItem taskItem)
        {
            _dbContext.TaskItems.Remove(taskItem);
        }
    }
} 