using System.Runtime.InteropServices.JavaScript;
using Learnup.Domain.AggregateRoots.Vocabularies;

namespace Learnup.Domain.AggregateRoots.Users;

public class LeitnerBoxItem
{
    public int Id { get; private set; }
    public int LeitnerBoxId { get; private set; }
    public LeitnerBox LeitnerBox { get; private set; } = null!;
    public int VocabId { get; private set; }
    public Vocab Vocab { get; private set; } = null!;
    public int BoxLevelId { get; private set; }
    public BoxLevel BoxLevel { get; private set; }
    public DateTime AddedAt { get; private set; }
    public DateTime? NextReviewAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    
    private LeitnerBoxItem() { }

    public LeitnerBoxItem(int vocabId, int boxLevelId)
    {
        VocabId = vocabId;
        AddedAt = DateTime.UtcNow;
        BoxLevelId = boxLevelId;
    }

    public void ChangeBoxLevel(BoxLevel boxLevel)
    {
        BoxLevelId = boxLevel.Id;
        NextReviewAt = DateTime.UtcNow + boxLevel.WillReviewedIn;
        ReviewedAt = DateTime.UtcNow;
    }
}
