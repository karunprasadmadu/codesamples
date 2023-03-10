using Moq;
using FluentAssertions;

public class GameServiceTests
{
    [Fact]
    public void GameRun_Success()
    {
        // Arrange
        var expectedPalyerCount = 2;

        var gameService = new GameService(expectedPalyerCount);

        // Act
        gameService.Run();

        // Assert
        gameService.IsGameFinished.Should().Be(true);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void GameRun_ThrowsException(int playerCount)
    {
        // Arrange
        var expectedErrorMessage = "Number of players should be in between 1 and 10 (Parameter 'numberOfPlayers')";

        // Act
        Action gameService = () => { new GameService(playerCount); };

        // Assert
        gameService.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void GameRun_PlayerCount_Success(int playerCount)
    {
        // Arrange
        var gameService = new GameService(playerCount);

        // Act
        gameService.Run();

        // Assert
        gameService.IsGameFinished.Should().Be(true);
        gameService.Players.Count().Should().Be(playerCount);


    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void GameRun_PlayerFramesCount_Success(int playerCount)
    {
        // Arrange
        var gameService = new GameService(playerCount);

        // Act
        gameService.Run();
        var playerFramesCount = gameService.Players.First().Frames.Count();

        // Assert
        gameService.IsGameFinished.Should().Be(true);
        gameService.Players.Count().Should().Be(playerCount);
        gameService.Players.All(p => p.Frames.Count() == playerFramesCount).Should().Be(true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void GameRun_StrikeScoreShouldBelow10_Success(int playerCount)
    {
        // Arrange
        var gameService = new GameService(playerCount);

        // Act
        gameService.Run();

        // Assert
        gameService.IsGameFinished.Should().Be(true);
        gameService.Players.Count().Should().Be(playerCount);
        gameService.Players.All(p => p.Frames.All(f => f.Scores.All(s => s <= GameConstants.MAX_STRIKE_SCORE))).Should().Be(true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void GameRun_PlayerIdsGenerated_Success(int playerCount)
    {
        // Arrange
        var gameService = new GameService(playerCount);

        // Act
        gameService.Run();

        // Assert
        gameService.IsGameFinished.Should().Be(true);
        gameService.Players.Count().Should().Be(playerCount);
        gameService.Players.All(p => !string.IsNullOrEmpty(p.PlayerId)).Should().Be(true);
    }
}