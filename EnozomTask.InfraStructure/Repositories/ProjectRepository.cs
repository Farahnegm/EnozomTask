using System.Threading.Tasks;
using EnozomTask.Domain.Entities;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EnozomTask.InfraStructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _dbContext;
        public ProjectRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Project> GetByNameAsync(string name)
        {
            return await _dbContext.Projects.FirstOrDefaultAsync(p => p.Name == name);
        }
        public void Add(Project project)
        {
            _dbContext.Projects.Add(project);
        }
        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dbContext.Projects.ToListAsync();
        }
        public async Task<Project> GetByIdAsync(int id)
        {
            return await _dbContext.Projects.FindAsync(id);
        }
        public void Update(Project project)
        {
            _dbContext.Projects.Update(project);
        }
        public void Delete(Project project)
        {
            _dbContext.Projects.Remove(project);
        }
    }
} 