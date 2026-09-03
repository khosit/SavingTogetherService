using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public interface IUserService
{
    Task<UserResponse?> GetUserAsync(string userKey);
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> CreateOrUpdateUserAsync(string userKey, CreateOrUpdateUserRequest request);
}
