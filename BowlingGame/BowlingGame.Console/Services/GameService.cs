public class GameService
{
    private List<Player> _players;
    public List<Player> Players { get { return _players; } }
    private bool _gameEnded = false;
    public bool IsGameFinished { get { return _gameEnded; } }
    private int _currentIndex = 0;

    public GameService(int numberOfPlayers)
    {
        if (numberOfPlayers <= 0 || numberOfPlayers > 10)
        {
            throw new ArgumentOutOfRangeException("numberOfPlayers", "Number of players should be in between 1 and 10");
        }
        _players = CreatePlayers(numberOfPlayers);
    }

    public void Run()
    {
        do
        {
            ExecuteFrame();
            CalculateScores();
            CheckWinner();
            _currentIndex++;
        } while (!_gameEnded);

        AnnounceWinner();
    }

    private void ExecuteFrame()
    {
        var strikeNumber = 1;
        foreach (var player in _players)
        {
            player.Frames.Add(new Frame());
            ExecuteStrike(player, player.Frames[_currentIndex], strikeNumber, new Random().Next(GameConstants.MAX_RANDOM_SCORE));
        }
        Console.WriteLine(); // A blank line between frames in console output
    }

    private void ExecuteStrike(Player player, Frame frame, int strikeNumber, int score)
    {
        frame.Scores.Add(score);
        score = frame.Scores.Sum();

        Console.WriteLine($"Frame: {_currentIndex + 1}, Player: {_players.IndexOf(player)}, Strike: {strikeNumber}, Score: {score}");

        if (_currentIndex < (GameConstants.MAX_FRAME_COUNT - 1) && score != GameConstants.MAX_STRIKE_SCORE && strikeNumber < 2)
        {
            ExecuteStrike(player, frame, strikeNumber + 1, new Random().Next(GameConstants.MAX_RANDOM_SCORE - score));
        }

        if ((_currentIndex == (GameConstants.MAX_FRAME_COUNT - 1) && strikeNumber < 3))
        {
            if (frame.Scores.Sum() >= GameConstants.MAX_STRIKE_SCORE)
            {
                ExecuteStrike(player, frame, strikeNumber + 1, new Random().Next(GameConstants.MAX_RANDOM_SCORE));
            }
            else if (strikeNumber < 2)
            {
                ExecuteStrike(player, frame, strikeNumber + 1, new Random().Next(GameConstants.MAX_RANDOM_SCORE - score));
            }
        }
    }

    private void CalculateScores()
    {
        foreach (var player in _players)
        {
            var currentFrame = player.Frames[_currentIndex];
            var previousFrame = _currentIndex == 0 ? null : player.Frames[_currentIndex - 1];
            var nextFrame = _currentIndex + 1 < player.Frames.Count() ? player.Frames[_currentIndex + 1] : null;

            var currentType = currentFrame.Scores[0] == 10
                ? previousFrame?.Type == FrameType.Strike
                    ? FrameType.Double
                    : FrameType.Strike
                : currentFrame.Scores.Take(2).Sum() == 10
                    ? FrameType.Spare
                    : FrameType.Open;

            currentFrame.Type = currentType;

            foreach (var frame in player.Frames)
            {
                var frameIndex = player.Frames.IndexOf(frame);
                var newNextFrame = frameIndex + 1 < player.Frames.Count() ? player.Frames[frameIndex + 1] : null;

                switch (frame.Type)
                {
                    case FrameType.Open:
                        frame.FrameScore = frame.Scores.Sum();
                        break;
                    case FrameType.Spare:
                        frame.FrameScore = (newNextFrame?.Scores?.First() ?? 0)
                            + (frame.Scores.Sum());
                        break;
                    case FrameType.Strike:
                        frame.FrameScore = (newNextFrame?.Scores?.Take(2)?.Sum() ?? 0)
                            + (frame.Scores.Sum());
                        break;
                    case FrameType.Double:
                        frame.FrameScore = 10
                                + (newNextFrame?.Scores?.Take(2)?.Sum() ?? 0)
                                + (frame.Scores.Sum());
                        break;
                    default: break;
                }
            }
        }
    }

    private void CheckWinner()
    {
        if (_currentIndex == 0) return;
        if (_players.Any(p => p.TotalScore >= 300) || _currentIndex >= 9)
        {
            _gameEnded = true;
        }
    }

    private List<Player> CreatePlayers(int numberOfPlayers)
    {
        var players = new List<Player>();
        for (var i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player { PlayerId = Guid.NewGuid().ToString() });
        }
        return players;
    }

    private void AnnounceWinner()
    {
        foreach (var player in _players)
        {
            Console.WriteLine($"\nPlayer: {_players.IndexOf(player) + 1}");
            foreach (var frame in player.Frames)
            {
                Console.WriteLine($"Frame: {player.Frames.IndexOf(frame) + 1}, Strikes: {string.Join(",", frame.Scores)}, FrameScore: {frame.FrameScore}, FrameType: {frame.Type}");
            }
        }

        var winner = _players.OrderByDescending(p => p.TotalScore).First();

        Console.WriteLine($"\nCongratulations!!! Player {_players.IndexOf(winner) + 1} won with score {winner.TotalScore}");
    }
}
