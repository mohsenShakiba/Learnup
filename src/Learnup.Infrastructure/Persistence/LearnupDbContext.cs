using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Tests;
using Learnup.Domain.AggregateRoots.Users;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Infrastructure.Persistence;

public class LearnupDbContext(DbContextOptions<LearnupDbContext> options)
    : DbContext(options), ILearnupDbContext
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Grammar> Grammars => Set<Grammar>();
    public DbSet<GrammarLesson> GrammarLessons => Set<GrammarLesson>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonVocab> LessonVocabs => Set<LessonVocab>();
    public DbSet<LessonGrammar> LessonGrammars => Set<LessonGrammar>();
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Vocab> Vocabs => Set<Vocab>();
    public DbSet<VocabTest> VocabTests => Set<VocabTest>();
    public DbSet<VocabTestOption> VocabTestOptions => Set<VocabTestOption>();
    public DbSet<GrammarTest> GrammarTests => Set<GrammarTest>();
    public DbSet<GrammarTestOption> GrammarTestOptions => Set<GrammarTestOption>();
    public DbSet<UserCourse> UserCourses => Set<UserCourse>();
    public DbSet<UserLesson> UserLessons => Set<UserLesson>();
    public DbSet<UserGrammar> UserGrammars => Set<UserGrammar>();
    public DbSet<UserStory> UserStories => Set<UserStory>();
    public DbSet<UserVocab> UserVocabs => Set<UserVocab>();
    public DbSet<UserVocabTestResult> UserVocabTestResults => Set<UserVocabTestResult>();
    public DbSet<UserGrammarTestResult> UserGrammarTestResults => Set<UserGrammarTestResult>();
    public DbSet<LeitnerBox> LeitnerBoxes => Set<LeitnerBox>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnupDbContext).Assembly);
    }
}
