
using System.Threading.Tasks;
using EnozomTask.Domain.Repositories;
using EnozomTask.InfraStructure.persistence;

namespace EnozomTask.InfraStructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IProjectRepository Projects { get; }
        public ITaskItemRepository TaskItems { get; }
        public ITimeEntryRepository TimeEntries { get; }

        public UnitOfWork(
            AppDbContext dbContext,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ITaskItemRepository taskItemRepository,
            ITimeEntryRepository timeEntryRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
            Projects = projectRepository;
            TaskItems = taskItemRepository;
            TimeEntries = timeEntryRepository;
        }
        public int SaveChanges() => _dbContext.SaveChanges();
        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
