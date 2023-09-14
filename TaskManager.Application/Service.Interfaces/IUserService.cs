using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Dtos;

namespace TaskManager.Application.Service.Interfaces
{
    public interface IUserService
    {
        Task<GenericResponse<IEnumerable<UserWithIdDto>>> GetAllUsers();
        Task<GenericResponse<UserDto>> GetByUserId(string userId);
        Task<GenericResponse<Response>> CreateUser(RegisterDto task);
        Task<GenericResponse<Response>> UpdateUser(string userIdString, UpdateUserRequest request);
        Task<GenericResponse<Response>> DeleteUser(string userId);
        Task<GenericResponse<Response>> AssignTask(string userId, AddOrDelete operation, string taskId);
    }
}
