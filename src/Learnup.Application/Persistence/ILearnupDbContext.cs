using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.MotivationalSentences;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Subscriptions;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Persistence;

public interface ILearnupDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Grammar> Grammars { get; }
    DbSet<GrammarLesson> GrammarLessons { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<MotivationalSentence> MotivationalSentences { get; }
    DbSet<LessonVocab> LessonVocabs { get; }
    DbSet<LessonGrammar> LessonGrammars { get; }
    DbSet<Story> Stories { get; }
    DbSet<Vocab> Vocabs { get; }
    DbSet<VocabTest> VocabTests { get; }
    DbSet<VocabTestOption> VocabTestOptions { get; }
    DbSet<GrammarTest> GrammarTests { get; }
    DbSet<GrammarTestOption> GrammarTestOptions { get; }
    DbSet<User> Users { get; }
    DbSet<UserStreak> UserStreaks { get; }
    DbSet<UserCourse> UserCourses { get; }
    DbSet<UserLesson> UserLessons { get; }
    DbSet<UserGrammar> UserGrammars { get; }
    DbSet<UserStory> UserStories { get; }
    DbSet<UserVocab> UserVocabs { get; }
    DbSet<UserBook> UserBooks { get; }
    DbSet<UserVocabTestResult> UserVocabTestResults { get; }
    DbSet<UserGrammarTestResult> UserGrammarTestResults { get; }
    DbSet<LeitnerBox> LeitnerBoxes { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionFeature> SubscriptionFeatures { get; }
    DbSet<UserSubscription> UserSubscriptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
