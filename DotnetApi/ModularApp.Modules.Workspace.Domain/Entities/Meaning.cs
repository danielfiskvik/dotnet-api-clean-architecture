using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Meaning : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Value { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public string? WordType { get; set; }
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character? Character { get; set; }
    #endregion
}