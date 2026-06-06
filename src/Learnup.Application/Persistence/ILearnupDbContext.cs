using Learnup.Domain.AggregateRoots.Courses;
using Learnup.Domain.AggregateRoots.Grammars;
using Learnup.Domain.AggregateRoots.Lessons;
using Learnup.Domain.AggregateRoots.Stories;
using Learnup.Domain.AggregateRoots.Vocabularies;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Persistence;

public interface ILearnupDbContext
{
    DbSet<Course> Courses { get; }
    DbSet<Grammar> Grammars { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<Story> Stories { get; }
    DbSet<Vocab> Vocabs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
