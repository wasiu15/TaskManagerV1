using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface ITokenManager
    {
        string GenerateToken(ref GenericResponse<LoginResponse> loginResponse);
        string GenerateRefreshToken();
    }
}
