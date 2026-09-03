using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public interface ICoupleService
{
    Task<CoupleResponse?> GetCoupleAsync(string userKey);
    Task<CoupleResponse> LinkCoupleAsync(LinkCoupleRequest request);
    Task<bool> UnlinkCoupleAsync(string userKey);
    Task<CoupleDashboardResponse?> GetCoupleDashboardAsync(string userKey);
}
