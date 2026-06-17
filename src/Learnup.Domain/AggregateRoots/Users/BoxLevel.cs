namespace Learnup.Domain.AggregateRoots.Users;

public class BoxLevel
{
    public int Id { get; private set; }
    public Level Level { get; private set; }
    
    public int LeitnerBoxId { get; private set; }
    public LeitnerBox LeitnerBox { get; private set; }
    
    
    public TimeSpan WillReviewedIn { get; private set; }

    public BoxLevel(TimeSpan willReviewedIn, Level level)
    {
        WillReviewedIn = willReviewedIn;
        Level = level;
    }
}
