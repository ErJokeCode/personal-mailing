using System.Threading.Tasks;
using Core.Models;

namespace Core.Abstractions;

public interface IUserAccessor
{
    public Task<Admin?> GetUserAsync();
}