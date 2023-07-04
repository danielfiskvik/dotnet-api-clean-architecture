using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class ContextSentence : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string En { get; set; } = null!;
    public string Ja { get; set; } = null!;
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character? Character { get; set; }
    #endregion
}