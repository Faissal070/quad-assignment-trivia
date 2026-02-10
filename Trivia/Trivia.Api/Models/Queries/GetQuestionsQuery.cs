using System.ComponentModel.DataAnnotations;

namespace Trivia.Api.Models.Queries;

public sealed class GetQuestionsQuery
{
    [Range(1, 50, ErrorMessage = "Amount must be at least 1 and at most 50")]
    public int Amount { get; init; }
    public int? Category { get; init; }
    public string? Difficulty { get; set; }
    public string? Type { get; set; }
}
