namespace ModularApp.Modules.Workspace.Domain.Interfaces;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedAt { get; set; }

    bool IsDeleted { get; set; }
    
    Guid? DeletedByEntityId { get; set; }
}