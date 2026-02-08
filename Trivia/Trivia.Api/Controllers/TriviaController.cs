using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Trivia.Api.Models.Queries;
using Trivia.Api.Services;

namespace Trivia.Api.Controllers;

public class TriviaController : ControllerBase
{
    private readonly ITriviaService _triviaService;

    public TriviaController(ITriviaService triviaService)
    {
        _triviaService = triviaService;
    }

    [HttpGet]
    [Route("questions")]
    public async Task<IActionResult> GetQuestions([FromQuery] GetQuestionsQuery query, [Required] string sessionId)
    {
        var getQuestions = await _triviaService.GetQuestionsAsync(query, sessionId);

        if (!getQuestions.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, getQuestions);
        }

        return Ok(getQuestions);
    }
}