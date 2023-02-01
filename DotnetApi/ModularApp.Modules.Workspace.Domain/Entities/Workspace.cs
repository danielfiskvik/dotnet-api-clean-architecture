using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Workspace : IEntity, ICreateable, IModifiable
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
    public string? Name { get; set; }
    #endregion
}