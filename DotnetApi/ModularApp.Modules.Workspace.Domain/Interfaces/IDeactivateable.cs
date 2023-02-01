namespace ModularApp.Modules.Workspace.Domain.Interfaces;

public interface IDeactivateable
{
    DateTimeOffset? DeactivatedAt { get; set; }

    bool IsActive { get; set; }
    
    Guid? DeactivatedByEntityId { get; set; }
}