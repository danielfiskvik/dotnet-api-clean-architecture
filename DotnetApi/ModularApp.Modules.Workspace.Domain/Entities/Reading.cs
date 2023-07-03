using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Reading : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public bool IsPrimary { get; set; }
    public string Value { get; set; }
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character Character { get; set; }
    #endregion
}