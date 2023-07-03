namespace ModularApp.Modules.Workspace.Domain.Entities;

public class ContextSentence
{
    public string En { get; set; } = null!;
    public string Ja { get; set; } = null!;
    
    #region Relations
    public int CharacterId { get; set; }
    #endregion
    
    #region NavigationProperties
    public Character Character { get; set; }
    #endregion
}