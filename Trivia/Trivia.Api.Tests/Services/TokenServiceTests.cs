using Moq;
using Trivia.Api.Client;
using Trivia.Api.Models.Result;
using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<ITokenStore> _tokenStoreMock;
    private readonly Mock<ITriviaApiClient> _triviaApiClientMock;

    private readonly TriviaTokenService _tokenService;

    public TokenServiceTests()
    {
        _tokenStoreMock = new Mock<ITokenStore>();
        _triviaApiClientMock = new Mock<ITriviaApiClient>();

        _tokenService = new TriviaTokenService(_triviaApiClientMock.Object, _tokenStoreMock.Object);
    }

    [Fact]
    public async Task GetToken_WhenTokenExists_ReturnsExistingToken()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        _tokenStoreMock.Setup(store => store.GetToken(quizId)).Returns("af5a7v7sd8add67");

        //Act
        var token = await _tokenService.GetOrCreateTokenAsync(quizId);

        //Assert    
        Assert.NotNull(token);
        Assert.Equal("af5a7v7sd8add67", token);
    }

    [Fact]
    public async Task GetToken_WhenTokenDoesNotExist_FetchesAndSavesNewToken()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var token = "Token1234";

        _tokenStoreMock.Setup(store => store.GetToken(quizId)).Returns((string?)null);

        _triviaApiClientMock.Setup(client => client.FetchTokenAsync())
            .ReturnsAsync(new TriviaApiTokenResponse
            {
                ResponseCode = 0,
                Token = token
            });

        //Act
        var result = await _tokenService.GetOrCreateTokenAsync(quizId);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(token, result);

        _tokenStoreMock.Verify(store => store.SaveToken(quizId, token), Times.Once);
    }

    [Theory]
    [InlineData(3, null)]
    [InlineData(3, "")]
    [InlineData(0, null)]
    [InlineData(0, "")]
    [InlineData(0, "   ")]
    public async Task GetToken_WhenApiReturnsError_ReturnsNull(int responseCode, string? token)
    {
        //Arrange
        var quizId = Guid.NewGuid();

        _tokenStoreMock.Setup(store => store.GetToken(quizId)).Returns((string?)null);

        _triviaApiClientMock.Setup(client => client.FetchTokenAsync())
            .ReturnsAsync(new TriviaApiTokenResponse
            {
                ResponseCode = responseCode,
                Token = token
            });

        //Act
        var result = await _tokenService.GetOrCreateTokenAsync(quizId);

        //Assert
        Assert.Null(result);

        _tokenStoreMock.Verify(store => store.SaveToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ResetToken_WhenResetSuccessful_ReturnsTrue()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var token = "Token12345";

        _triviaApiClientMock.Setup(client => client.ResetTokenAsync(token))
            .ReturnsAsync(new TriviaApiTokenResponse
            {
                ResponseCode = 0,
                Token = token
            });
        //Act
        var result = await _tokenService.ResetTokenAsync(quizId, token);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ResetToken_WhenResetFails_ReturnsFalse()
    {
        //Arrange
        var quizId = Guid.NewGuid();
        var token = "Token12345";

        _triviaApiClientMock.Setup(client => client.ResetTokenAsync(token))
            .ReturnsAsync(new TriviaApiTokenResponse
            {
                ResponseCode = 3,
                Token = token
            });

        //Act
        var result = await _tokenService.ResetTokenAsync(quizId, token);

        //Assert
        Assert.False(result);
    }

    [Fact]
    public void ClearToken_CallsTokenStoreClearQuizToken()
    {
        //Arrange
        var quizId = Guid.NewGuid();

        //Act
        _tokenService.ClearToken(quizId);

        //Assert
        _tokenStoreMock.Verify(store => store.RemoveQuizToken(quizId), Times.Once);
    }
}

