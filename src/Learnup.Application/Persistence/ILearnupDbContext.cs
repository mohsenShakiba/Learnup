using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Stories;
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
    DbSet<Story> Stories { get; }
    DbSet<Vocab> Vocabs { get; }
    DbSet<VocabTransaction> VocabTransactions { get; }
    DbSet<UserCourse> UserCourses { get; }
    DbSet<UserLesson> UserLessons { get; }
    DbSet<UserGrammar> UserGrammars { get; }
    DbSet<UserStory> UserStories { get; }
    DbSet<UserVocab> UserVocabs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
