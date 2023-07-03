using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Character : IEntity, ICreateable, IModifiable
{
    [Key]
    public Guid Id { get; set; }

    #region ICreateable
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset? CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByEntityId { get; set; }
    #endregion

    #region IModifiable
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset? ModifiedAt { get; set; }
    public Guid? ModifiedByEntityId { get; set; }
    #endregion

    #region Properties
    [MaxLength(200)]
    public string Characters { get; set; } = null!;
    [MaxLength(200)]
    public string Type { get; set; } = null!; // "radical" | "kanji" | ""
    [MaxLength(4000)]
    public string? MeaningMnemonic { get; set; }
    [MaxLength(4000)]
    public string? MeaningHint { get; set; }
    [MaxLength(4000)]
    public string? ReadingMnemonic { get; set; }
    [MaxLength(4000)]
    public string? ReadingHint { get; set; }
    
    // Metadata
    public int Level { get; set; }
    public int LessonPosition { get; set; }
    public string Source { get; set; } = null!; // WaniKani
    public string? ExternalUrl { get; set; } 
    
    #region NavigationProperties
    public ICollection<Meaning> Meanings { get; set; }
    public ICollection<Reading> Readings { get; set; }
    public ICollection<ContextSentence> ContextSentences { get; set; }
    #endregion
    #endregion
}