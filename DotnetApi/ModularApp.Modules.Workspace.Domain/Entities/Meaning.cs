using System.ComponentModel.DataAnnotations;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class Meaning {
    [Key]
    public Guid Id { get; set; }
    public string Value { get; set; } // One
    public bool IsPrimary { get; set; } // true
    public bool WordType { get; set; } // 
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character Character { get; set; }
    #endregion
    
}