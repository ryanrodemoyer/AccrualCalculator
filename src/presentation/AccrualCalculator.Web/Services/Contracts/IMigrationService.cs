using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppName.Web.Services
{
    public interface IMigrationService
    {
        Task ApplyMigrationsAsync();
    }
}