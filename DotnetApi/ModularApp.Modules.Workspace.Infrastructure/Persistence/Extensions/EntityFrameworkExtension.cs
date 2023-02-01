using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ModularApp.Modules.Workspace.Domain.Constants;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence.Extensions;

public static class EntityFrameworkExtension
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned() && 
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    
    public static TEntity SetSoftDeletablePropertiesOnEntity<TEntity>(this TEntity entity, Guid? userId)
    {
        DateTimeOffset? now = DateTimeOffset.UtcNow;
        var type = entity?.GetType();

        if (type is null)
            return entity;
        
        type.GetProperty(EntityConstants.NameOfIsDeleted)?.SetValue(entity, true, null);
        type.GetProperty(EntityConstants.NameOfDeletedByEntityId)?.SetValue(entity, userId, null);
        type.GetProperty(EntityConstants.NameOfDeletedAt)?.SetValue(entity, now, null);

        return entity;
    }
}