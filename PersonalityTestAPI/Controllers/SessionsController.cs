using Microsoft.AspNetCore.Mvc;
using PersonalityTestAPI.DTOs;
using PersonalityTestAPI.Services;

namespace PersonalityTestAPI.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly IPersonalityTestService _personalityTestService;

        public SessionsController(IPersonalityTestService personalityTestService)
        {
            _personalityTestService = personalityTestService;
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("Sessions controller is working!");
        }

        [HttpPost]
        public async Task<ActionResult<SessionResponseDto>> CreateSession([FromBody] SessionCreateDto sessionData)
        {
            Console.WriteLine($"Creating session for: {sessionData?.Name}");
            try
            {
                var result = await _personalityTestService.CreateSessionAsync(sessionData);
                Console.WriteLine($"Session created with ID: {result.SessionId}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating session: {ex.Message}");
                return StatusCode(500, new { error = $"Error creating session: {ex.Message}" });
            }
        }

        [HttpGet("{sessionId}/question")]
        public async Task<ActionResult<QuestionDto>> GetCurrentQuestion(string sessionId)
        {
            Console.WriteLine($"Getting question for session: {sessionId}");
            try
            {
                var question = await _personalityTestService.GetCurrentQuestionAsync(sessionId);
                Console.WriteLine($"Question found: {question?.QuestionId}");
                
                if (question == null)
                {
                    var session = await _personalityTestService.GetSessionAsync(sessionId);
                    if (session != null && session.Status == "completed")
                    {
                        return Ok(new { isCompleted = true, message = "Test completed" });
                    }
                    return NotFound(new { error = "No more questions or session not found" });
                }

                return Ok(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting question: {ex.Message}");
                return StatusCode(500, new { error = $"Error getting question: {ex.Message}" });
            }
        }

        [HttpGet("{sessionId}/report")]
        public async Task<ActionResult<ReportDto>> GetReport(string sessionId)
        {
            Console.WriteLine($"Generating report for session: {sessionId}");
            try
            {
                var report = await _personalityTestService.GetReportAsync(sessionId);
                Console.WriteLine($"Report generated: {report != null}");
                
                if (report == null)
                    return NotFound(new { error = "Session not found or not completed" });

                return Ok(report);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                return StatusCode(500, new { error = $"Error generating report: {ex.Message}" });
            }
        }
    }
}
