namespace ModularApp.Modules.Workspace.Domain.Interfaces;

/// <summary>
/// CreatedByEntityId is nullable in case it will be possible no hard delete entities
/// </summary>
public interface ICreateable
{
    DateTimeOffset? CreatedAt { get; set; }
    
    Guid? CreatedByEntityId { get; set; }
}