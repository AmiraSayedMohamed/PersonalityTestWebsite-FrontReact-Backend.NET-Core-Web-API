using PersonalityTestAPI.DTOs;
using PersonalityTestAPI.Models;

namespace PersonalityTestAPI.Services;

public interface IPersonalityTestService
{
    Task<SessionResponseDto> CreateSessionAsync(SessionCreateDto sessionData);
    Task<QuestionDto?> GetCurrentQuestionAsync(string sessionId);
    Task<object> SubmitAnswerAsync(AnswerSubmissionDto answer);
    Task<ReportDto?> GetReportAsync(string sessionId);
    Task<Session?> GetSessionAsync(string sessionId);
}
