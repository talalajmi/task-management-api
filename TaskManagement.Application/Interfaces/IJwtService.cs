using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser? user);
}
