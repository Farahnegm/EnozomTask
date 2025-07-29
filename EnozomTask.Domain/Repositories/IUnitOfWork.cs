using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnozomTask.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        ITaskItemRepository TaskItems { get; }
        ITimeEntryRepository TimeEntries { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
