using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Domain.Interfaces;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ModularApp.Modules.Workspace.Domain.Entities;

[Index(
    nameof(Characters),
    nameof(Type),
    nameof(Source)
    )]
public class Character : IEntity, ICreateable, IModifiable
{
    public Character()
    {
        Meanings = new List<Meaning>();
        Readings = new List<Reading>();
        ContextSentences = new List<ContextSentence>();
    }

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
    public string Type { get; set; } = null!;
    [MaxLength(4000)]
    public string? MeaningMnemonic { get; set; }
    [MaxLength(4000)]
    public string? MeaningMnemonicHint { get; set; }
    [MaxLength(4000)]
    public string? MeaningExplanation { get; set; }
    [MaxLength(4000)]
    public string? MeaningExplanationHint { get; set; }
    [MaxLength(4000)]
    public string? ReadingMnemonic { get; set; }
    [MaxLength(4000)]
    public string? ReadingMnemonicHint { get; set; }
    [MaxLength(4000)]
    public string? ReadingExplanation { get; set; }
    [MaxLength(4000)]
    public string? ReadingExplanationHint { get; set; }
    #endregion
    
    // Metadata
    public int Level { get; set; }
    public int LessonPosition { get; set; }
    public string Source { get; set; } = null!;
    public string? ExternalUrl { get; set; } 
    
    #region NavigationProperties
    public ICollection<Meaning> Meanings { get; set; }
    public ICollection<Reading> Readings { get; set; }
    public ICollection<ContextSentence> ContextSentences { get; set; }
    #endregion
    
    #region Properties Original
    [MaxLength(4000)]
    public string? MeaningMnemonicOriginal { get; set; }
    [MaxLength(4000)]
    public string? MeaningMnemonicHintOriginal { get; set; }
    [MaxLength(4000)]
    public string? MeaningExplanationOriginal { get; set; }
    [MaxLength(4000)]
    public string? MeaningExplanationHintOriginal { get; set; }
    [MaxLength(4000)]
    public string? ReadingMnemonicOriginal { get; set; }
    [MaxLength(4000)]
    public string? ReadingMnemonicHintOriginal { get; set; }
    [MaxLength(4000)]
    public string? ReadingExplanationOriginal { get; set; }
    [MaxLength(4000)]
    public string? ReadingExplanationHintOriginal { get; set; }
    #endregion
}