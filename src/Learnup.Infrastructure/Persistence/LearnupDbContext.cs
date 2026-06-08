using Learnup.Application.Persistence;
using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Languages;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Stories;
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
    public DbSet<Story> Stories => Set<Story>();
    public DbSet<Vocab> Vocabs => Set<Vocab>();
    public DbSet<UserCourse> UserCourses => Set<UserCourse>();
    public DbSet<UserLesson> UserLessons => Set<UserLesson>();
    public DbSet<UserGrammar> UserGrammars => Set<UserGrammar>();
    public DbSet<UserStory> UserStories => Set<UserStory>();
    public DbSet<UserVocab> UserVocabs => Set<UserVocab>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnupDbContext).Assembly);
    }

}
