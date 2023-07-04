using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Domain.Entities;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence;

public partial class ApplicationDbContext
{
    // Domains
    public DbSet<Character> Characters { get; set; }
    public DbSet<ContextSentence> ContextSentences { get; set;  }
    public DbSet<Meaning> Meanings { get; set; }
    public DbSet<Reading> Readings { get; set; }
    // ReSharper disable once IdentifierTypo
    public DbSet<CharacterMetadata> CharacterMetadatas { get; set; }
}