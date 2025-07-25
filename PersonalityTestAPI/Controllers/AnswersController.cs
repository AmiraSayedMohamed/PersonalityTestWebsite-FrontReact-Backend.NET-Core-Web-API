using Microsoft.AspNetCore.Mvc;
using PersonalityTestAPI.DTOs;
using PersonalityTestAPI.Services;

namespace PersonalityTestAPI.Controllers
{
    [ApiController]
    [Route("api/answers")]
    public class AnswersController : ControllerBase
    {
        private readonly IPersonalityTestService _personalityTestService;

        public AnswersController(IPersonalityTestService personalityTestService)
        {
            _personalityTestService = personalityTestService;
        }

        [HttpPost]
        public async Task<ActionResult> SubmitAnswer([FromBody] AnswerSubmissionDto answer)
        {
            try
            {
                var result = await _personalityTestService.SubmitAnswerAsync(answer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error submitting answer: {ex.Message}" });
            }
        }
    }
}
