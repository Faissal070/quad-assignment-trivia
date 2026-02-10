using Trivia.Api.Storage;

namespace Trivia.Api.Tests.Storage;

public class CorrectAnswerStoreTests
{
    [Fact]
    public void AddCorrectAnswer_WhenQuizIdNotExists_ShouldSaveQuestion()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var correctAnswer = "Correct Answer";

        var store = new CorrectAnswerStore();

        store.AddCorrectAnswer(quizId, questionId, correctAnswer);

        //Act
        var result = store.GetCorrectAnswer(quizId, questionId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(correctAnswer, result);
    }

    [Fact]
    public void GetCorrectAnswer_WhenQuestionDoesNotExist_ShouldReturnNull()
    {
        //Arrange
        var store = new CorrectAnswerStore();
        var quizId = Guid.NewGuid();

        store.AddCorrectAnswer(quizId, Guid.NewGuid(), "A");

        //Act
        var result = store.GetCorrectAnswer(quizId, Guid.NewGuid());

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCorrectAnswer_WhenQuizDoesNotExist_ShouldReturnNull()
    {
        //Arrange
        var store = new CorrectAnswerStore();

        //Act
        var result = store.GetCorrectAnswer(Guid.NewGuid(), Guid.NewGuid());

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCorrectAnswer_WhenQuizAndQuestionExist_ShouldReturnCorrectAnswer()
    {
        //Arrange
        var store = new CorrectAnswerStore();
        var quizId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        var expectedAnswer = "Correct Answer";

        store.AddCorrectAnswer(quizId, questionId, expectedAnswer);

        //Act
        var result = store.GetCorrectAnswer(quizId, questionId);

        //Assert
        Assert.Equal(expectedAnswer, result);
    }

    [Fact]
    public void QuizExists_WhenQuizExists_ShouldReturnTrue()
    {
        // Arrange
        var store = new CorrectAnswerStore();
        var quizId = Guid.NewGuid();

        store.AddCorrectAnswer(quizId, Guid.NewGuid(), "A");

        // Act
        var result = store.QuizExists(quizId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void QuizExists_WhenQuizDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var store = new CorrectAnswerStore();
        var quizId = Guid.NewGuid();

        // Act
        var result = store.QuizExists(quizId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveQuiz_WhenQuizIdNotExists_ShouldReturnFalse()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var store = new CorrectAnswerStore();

        //Act
        var result = store.RemoveQuiz(quizId);

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void RemoveQuiz_WhenQuizExists_ShouldReturnTrue()
    {
        // Arrange
        var store = new CorrectAnswerStore();
        var quizId = Guid.NewGuid();

        store.AddCorrectAnswer(quizId, Guid.NewGuid(), "A");

        // Act
        var result = store.RemoveQuiz(quizId);

        // Assert
        Assert.True(result);
    }
}
