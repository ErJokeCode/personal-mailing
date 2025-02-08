using System.Threading.Tasks;
using Core.Models;

namespace Core.Infrastructure.Services;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}