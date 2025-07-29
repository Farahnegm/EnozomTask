using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnozomTask.Domain.Repositories
{
    public interface IClockifyService
    {
        Task PushSampleDataAsync();
    }
}
