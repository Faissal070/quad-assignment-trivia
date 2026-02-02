using Microsoft.AspNetCore.Mvc;
using Trivia.Api.Services;

namespace Trivia.Api.Controllers
{
    public class TriviaController : ControllerBase
    {
        private readonly ITriviaService _triviaService;

        public TriviaController(ITriviaService triviaService)
        {
            _triviaService = triviaService;
        }

        [HttpGet]
        [Route("questions")]
        public async Task<IActionResult> GetQuestions([FromQuery] int amount)
        {
            if(amount <= 0 || amount > 50)
            {
                return BadRequest("Amount must be greater than zero and smaller than 50");
            }

            var getQuestions = await _triviaService.GetQuestionsAsync(amount);

            return Ok(getQuestions);
        }
    }
}
