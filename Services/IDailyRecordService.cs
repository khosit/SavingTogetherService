using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public interface IDailyRecordService
{
    Task<DailyRecordResponse?> GetOrCreateTodayRecordAsync(string userKey);
    Task<DailyRecordResponse?> GetRecordByDateAsync(string userKey, string date);
    Task<IEnumerable<DailyRecordResponse>> GetMonthRecordsAsync(string userKey, int year, int month);
    Task<IEnumerable<DailyRecordResponse>> GetAllRecordsAsync(string userKey);
    Task<MonthSummaryResponse> GetMonthSummaryAsync(string userKey, int year, int month);
}
