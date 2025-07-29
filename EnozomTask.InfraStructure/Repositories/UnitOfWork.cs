
using System;
using System.Threading.Tasks;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EnozomTask.InfraStructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        
        private IUserRepository _users;
        private IProjectRepository _projects;
        private ITaskItemRepository _taskItems;
        private ITimeEntryRepository _timeEntries;

        public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public IUserRepository Users => _users ??= _serviceProvider.GetRequiredService<IUserRepository>();
        public IProjectRepository Projects => _projects ??= _serviceProvider.GetRequiredService<IProjectRepository>();
        public ITaskItemRepository TaskItems => _taskItems ??= _serviceProvider.GetRequiredService<ITaskItemRepository>();
        public ITimeEntryRepository TimeEntries => _timeEntries ??= _serviceProvider.GetRequiredService<ITimeEntryRepository>();

        public int SaveChanges() => _dbContext.SaveChanges();
        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
