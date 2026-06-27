using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Ebooks;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.MotivationalSentences;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Subscriptions;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.Persistence;

public class LearnupDbContext(DbContextOptions<LearnupDbContext> options)
    : DbContext(options), ILearnupDbContext
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Ebook> Ebooks => Set<Ebook>();
    public DbSet<Grammar> Grammars => Set<Grammar>();
    public DbSet<GrammarLesson> GrammarLessons => Set<GrammarLesson>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<MotivationalSentence> MotivationalSentences => Set<MotivationalSentence>();
    public DbSet<LessonVocab> LessonVocabs => Set<LessonVocab>();
    public DbSet<LessonGrammar> LessonGrammars => Set<LessonGrammar>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Vocab> Vocabs => Set<Vocab>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestOption> TestOptions => Set<TestOption>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();
    public DbSet<UserLesson> UserLessons => Set<UserLesson>();
    public DbSet<UserBook> UserBooks => Set<UserBook>();
    public DbSet<UserTestResult> UserTestResults => Set<UserTestResult>();
    public DbSet<LeitnerBox> LeitnerBoxes => Set<LeitnerBox>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionFeature> SubscriptionFeatures => Set<SubscriptionFeature>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnupDbContext).Assembly);
    }
}
