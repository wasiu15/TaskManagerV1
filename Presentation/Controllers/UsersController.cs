﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Repository.Interfaces;
using TaskManager.Application.Service.Interfaces;
using TaskManager.Domain.Dtos;

namespace TaskManager.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly ITokenManager _tokenManager;
        private readonly IRepositoryManager _repositoryManager;
        public UsersController(IServiceManager serviceManager, ITokenManager tokenManager, IRepositoryManager repositoryManager)
        {
            _serviceManager = serviceManager;
            _tokenManager = tokenManager;
            _repositoryManager = repositoryManager;
        }

        [AllowAnonymous]
        [HttpGet("getAllUser")]
        public async Task<ActionResult> GetAll()
        {
            var response = await _serviceManager.UserService.GetAllUsers();
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getUserById")]
        public async Task<ActionResult> GetUserById([FromQuery] string userId)
        {
            var response = await _serviceManager.UserService.GetByUserId(userId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("loginUser")]
        public async Task<ActionResult> LoginUser(LoginDto loginRequest)
        {
            var userLogin = await _serviceManager.UserService.GetByEmailAndPassword(loginRequest, false);
            if(userLogin.Data != null)
            {
                var token = _tokenManager.GenerateToken(ref userLogin);
                var refreshToken = _tokenManager.GenerateRefreshToken();
                userLogin.Data.AccessToken = token;
                userLogin.Data.RefreshToken = refreshToken;

                var loggedInUser = await _repositoryManager.UserRepository.GetByUserId(userLogin.Data.UserId, true);
                loggedInUser.AccessToken = token;
                loggedInUser.RefreshToken = refreshToken;
                loggedInUser.TokenGenerationTime = DateTime.UtcNow;
                _repositoryManager.UserRepository.UpdateUser(loggedInUser);
                await _repositoryManager.SaveAsync();
                
                return Ok(userLogin);
            }
            return BadRequest(userLogin);
        }

        [AllowAnonymous]
        [HttpPost("registerUser")]
        public async Task<ActionResult> AddUser(RegisterDto project)
        {
            var response = await _serviceManager.UserService.CreateUser(project);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpPost("refreshtoken")]

        public async Task<IActionResult> RefreshToken(RefreshTokenDto request)
        {
            var result = await _serviceManager.UserService.RefreshToken(request);
            if (result.IsSuccessful)
            {
                var loggedInUser = await _repositoryManager.UserRepository.GetByUserId(request.UserId, true);
                loggedInUser.AccessToken = result.Data.AccessToken;
                loggedInUser.RefreshToken = result.Data.RefreshToken;
                loggedInUser.TokenGenerationTime = DateTime.UtcNow;
                _repositoryManager.UserRepository.UpdateUser(loggedInUser);
                await _repositoryManager.SaveAsync();
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("updateUser")]
        public async Task<ActionResult> UpdateUser([FromQuery] string userId, [FromBody] UpdateUserRequest user)
        {
            var response = await _serviceManager.UserService.UpdateUser(userId, user);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser([FromQuery] string userId)
        {
            var response = await _serviceManager.UserService.DeleteUser(userId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
