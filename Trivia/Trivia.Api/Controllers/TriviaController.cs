using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Trivia.Api.Models.Queries;
using Trivia.Api.Services;

namespace Trivia.Api.Controllers;

public class TriviaController : ControllerBase
{
    private readonly ITriviaQuestionService _triviaQuestionService;
    private readonly ITriviaAnswerService _triviaAnswerService;

    public TriviaController(ITriviaQuestionService triviaQuestionService, ITriviaAnswerService triviaAnswerService)
    {
        _triviaQuestionService = triviaQuestionService;
        _triviaAnswerService = triviaAnswerService;
    }

    [HttpGet]
    [Route("questions")]
    public async Task<IActionResult> GetQuestions([FromQuery] GetQuestionsQuery query, [Required] Guid quizId)
    {
       var getQuestions = await _triviaQuestionService.GetQuestionsAsync(query, quizId);

        if (!getQuestions.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, getQuestions);
        }

        return Ok(getQuestions);
    }

    [HttpPost]
    [Route("answers")]
    public async Task<IActionResult> SubmitAnswers([FromBody] SubmitAnswersQuery query)
    {
        var submitAnswers = await _triviaAnswerService.SubmitAnswersAsync(query);

        if (!submitAnswers.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, submitAnswers);
        }

        return Ok(submitAnswers);
    }
}
