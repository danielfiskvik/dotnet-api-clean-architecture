using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

[Index(
    nameof(Characters),
    nameof(Source)
)]
public class CharacterMetadata : IEntity, ICreateable, IModifiable
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
    public string Source { get; set; } = null!;
    public bool IsMigrated { get; set; }
    [MaxLength(4000)]
    public string? Status { get; set; }
    [MaxLength(10000)]
    public string? Html { get; set; }
    #endregion
}