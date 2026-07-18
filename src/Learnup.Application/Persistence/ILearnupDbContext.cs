using Learnup.Domain.AggregateRoots.Chats;
using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.AudioBooks;
using Learnup.Domain.AggregateRoots.Ebooks;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.InvalidData;
using Learnup.Domain.AggregateRoots.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.MotivationalSentences;
using Learnup.Domain.AggregateRoots.Placement;
using Learnup.Domain.AggregateRoots.Conversations;
using Learnup.Domain.AggregateRoots.Subscriptions;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Persistence;

public interface ILearnupDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<AudioBooks> AudioBooks { get; }
    DbSet<AudioBookListItem> AudioBookListItems { get; }
    DbSet<AudioBookListItemExpression> AudioBookListItemExpressions { get; }
    DbSet<Ebook> Ebooks { get; }
    DbSet<Grammar> Grammars { get; }
    DbSet<GrammarLesson> GrammarLessons { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<MotivationalSentence> MotivationalSentences { get; }
    DbSet<LessonVocab> LessonVocabs { get; }
    DbSet<LessonGrammar> LessonGrammars { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Vocab> Vocabs { get; }
    DbSet<Test> Tests { get; }
    DbSet<TestOption> TestOptions { get; }
    DbSet<User> Users { get; }
    DbSet<UserTokenUsage> UserTokenUsages { get; }
    DbSet<Chat> Chats { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<UserOtp> UserOtps { get; }
    DbSet<UserStreak> UserStreaks { get; }
    DbSet<UserLesson> UserLessons { get; }
    DbSet<UserBook> UserBooks { get; }
    DbSet<UserTestResult> UserTestResults { get; }
    DbSet<UserPlacementResult> UserPlacementResults { get; }
    DbSet<UserPlacementAnswer> UserPlacementAnswers { get; }
    DbSet<PlacementTest> PlacementTests { get; }
    DbSet<PlacementQuestion> PlacementQuestions { get; }
    DbSet<PlacementOption> PlacementOptions { get; }
    DbSet<LeitnerBox> LeitnerBoxes { get; }
    DbSet<LeitnerBoxItem> LeitnerBoxItems { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionFeature> SubscriptionFeatures { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }
    DbSet<InvalidDataReport> InvalidDataReports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
