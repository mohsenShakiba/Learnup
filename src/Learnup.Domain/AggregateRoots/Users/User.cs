namespace Learnup.Domain.AggregateRoots.Users;

public class User
{
    public int Id { get; private set; }
    public string DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public UserStatus Status { get; private set; }

    public ICollection<UserLesson> Lessons { get; private set; } = new List<UserLesson>();
    public ICollection<UserGrammar> Grammars { get; private set; } = new List<UserGrammar>();
    public ICollection<UserStory> Stories { get; private set; } = new List<UserStory>();
    public ICollection<UserVocab> Vocabs { get; private set; } = new List<UserVocab>();
}
