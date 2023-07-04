using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Reading : IEntity
{
    [Key]
    public Guid Id { get; set; }
    public string? Type { get; set; }
    public bool IsPrimary { get; set; }
    public string Value { get; set; } = null!;
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character? Character { get; set; }
    #endregion
}