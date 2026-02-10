using Trivia.Api.Storage;

namespace Trivia.Api.Tests.Storage;

public class TokenStoreTests
{
    [Fact]
    public void GetToken_WhenQuizNotExists_ShouldReturnNull()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var store = new TokenStore();

        //Act
        var result = store.GetToken(quizId);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetToken_WhenQuizExists_ShouldReturnToken()
    {
        // Arrange
        var store = new TokenStore();
        var quizId = Guid.NewGuid();
        var expectedToken = "token-123";

        store.SaveToken(quizId, expectedToken);

        // Act
        var result = store.GetToken(quizId);

        // Assert
        Assert.Equal(expectedToken, result);
    }


    [Fact]
    public void SaveToken_WhenTokenExists_ShouldReturnToken()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var token = "token12434";

        var store = new TokenStore();

        store.SaveToken(quizId, token);

        //Act
        var result = store.GetToken(quizId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(token, result);
    }

    [Fact]
    public void SaveToken_WhenValid_ShouldStoreToken()
    {
        // Arrange
        var store = new TokenStore();
        var quizId = Guid.NewGuid();

        // Act
        store.SaveToken(quizId, "token");

        // Assert
        Assert.Equal("token", store.GetToken(quizId));
    }

    [Fact]
    public void ClearQuizToken_WhenTokenExists_ShouldRemoveToken()
    {
        //Arrange
        var store = new TokenStore();
        var quizId = Guid.NewGuid();

        store.SaveToken(quizId, "token-123"); // expliciete beginstaat
        Assert.NotNull(store.GetToken(quizId)); // sanity check (optioneel)

        //Act
        store.RemoveQuizToken(quizId);

        //Assert
        Assert.Null(store.GetToken(quizId));
    }

}