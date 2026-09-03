using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public interface IStatsService
{
    Task<StreakResponse?> GetStreakAsync(string userKey);
    Task<DashboardStatsResponse?> GetDashboardStatsAsync(string userKey);
}
