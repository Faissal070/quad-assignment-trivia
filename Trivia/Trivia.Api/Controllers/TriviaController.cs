using Microsoft.AspNetCore.Mvc;
using Trivia.Api.Services;

namespace Trivia.Api.Controllers;

public class TriviaController : ControllerBase
{
    private readonly ITriviaService _triviaService;

    private const int MinAmount = 1;
    private const int MaxAmount = 50;

    public TriviaController(ITriviaService triviaService)
    {
        _triviaService = triviaService;
    }

    [HttpGet]
    [Route("questions")]
    public async Task<IActionResult> GetQuestions([FromQuery] int amount)
    {
        if(amount < MinAmount || amount > MaxAmount)
        {
            return BadRequest("Amount must be greater than zero and smaller than 50");
        }

        var getQuestions = await _triviaService.GetQuestionsAsync(amount);
        if (!getQuestions.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, getQuestions);
        }

        return Ok(getQuestions);
    }
}