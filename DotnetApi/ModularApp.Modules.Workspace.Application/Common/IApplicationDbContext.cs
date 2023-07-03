using ModularApp.Modules.Workspace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ModularApp.Modules.Workspace.Application.Common;

public interface IApplicationDbContext
{
    DbSet<Character> Characters { get; }
    DbSet<ContextSentence> ContextSentences { get; }
    DbSet<Meaning> Meanings { get; set; }
    DbSet<Reading> Readings { get; set; }
}