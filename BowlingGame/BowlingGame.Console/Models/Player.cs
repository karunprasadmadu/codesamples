using System.Linq;
using System.Collections.Generic;

public class Player
{
    public string PlayerId { get; set; } = null!;
    public List<Frame> Frames { get; set; } = new List<Frame>();
    public int TotalScore { get { return Frames.Sum(f => f.FrameScore); } }
}