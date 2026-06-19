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
    public BoxLevel BoxLevel { get; private set; } = null!;
    public DateTime AddedAt { get; private set; }
    public DateTime? NextReviewAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    private LeitnerBoxItem() { }

    public LeitnerBoxItem(int vocabId, BoxLevel boxLevel)
    {
        VocabId = vocabId;
        AddedAt = DateTime.UtcNow;
        BoxLevel = boxLevel;
        BoxLevelId = boxLevel.Id;
        NextReviewAt = AddedAt + boxLevel.WillReviewedIn;
    }

    internal void AssignToBox(LeitnerBox leitnerBox)
    {
        LeitnerBox = leitnerBox;
        LeitnerBoxId = leitnerBox.Id;
    }

    public void ChangeBoxLevel(BoxLevel boxLevel)
    {
        if (BoxLevel.Id != boxLevel.Id)
        {
            BoxLevel.RemoveItem(this);
            boxLevel.AddItem(this);

            BoxLevel = boxLevel;
            BoxLevelId = boxLevel.Id;
        }

        var reviewedAt = DateTime.UtcNow;
        ReviewedAt = reviewedAt;
        NextReviewAt = reviewedAt + boxLevel.WillReviewedIn;
    }

    public void UpdateNextReviewAt(TimeSpan reviewInterval)
    {
        if (reviewInterval < TimeSpan.Zero)
        {
            throw new InvalidOperationException("Review interval cannot be negative.");
        }

        var nextReviewBase = ReviewedAt ?? DateTime.UtcNow;
        NextReviewAt = nextReviewBase + reviewInterval;
    }
}
