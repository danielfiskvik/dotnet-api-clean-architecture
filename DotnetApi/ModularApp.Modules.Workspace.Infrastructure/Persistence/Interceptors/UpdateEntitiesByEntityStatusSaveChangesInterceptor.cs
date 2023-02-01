using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using ModularApp.Modules.Workspace.Application.Common;
using ModularApp.Core.Extensions;
using ModularApp.Modules.Workspace.Domain.Constants;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Infrastructure.Persistence.Interceptors;

public class UpdateEntitiesByEntityStatusSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    // private readonly IDateTime _dateTime;

    public UpdateEntitiesByEntityStatusSaveChangesInterceptor(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        // _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /**
     * Updates implemented properties defined by interfaces such as:
     * ICreateable, IModifiable, IDeactivateable, ISoftDeletable
     */
    public void UpdateEntities(DbContext? context)
    {
        if (context == null)
            return;

        var utcNow = DateTimeOffset.UtcNow;
        var currentUserId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            var type = entry.Entity.GetType();
            var entity = entry.Entity;
            
            switch (entry.State)
            {
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    continue;
                case EntityState.Added:
                {
                    if (entity.IsImplementingFrom(x => x == EntityConstants.TypeOfCreateable))
                    {
                        type.GetProperty(EntityConstants.NameOfCreatedAt)?.SetValue(entity, utcNow, null);
                        type.GetProperty(EntityConstants.NameOfCreatedByEntityId)?.SetValue(entity, currentUserId, null);
                    }
                    
                    continue;
                } 
                case EntityState.Modified:
                {
                    if (entity.IsImplementingFrom(x => x == EntityConstants.TypeOfSoftDeletable))
                    {
                        var isDeletedCurrent = entity.GetValue(EntityConstants.NameOfIsDeleted)?.ToString()?.ToBoolOrDefault() ?? false;

                        var isDeletedOriginal = entry.OriginalValues[EntityConstants.NameOfIsDeleted]?.ToString()?.ToBoolOrDefault() ?? false;

                        // when deleted we must clear out 'modifiedBy' since it is not modified but 'softDeleted' 
                        if (isDeletedOriginal == false && isDeletedCurrent)
                        {
                            type.GetProperty(EntityConstants.NameOfDeletedByEntityId)?.SetValue(entity, currentUserId, null);
                            type.GetProperty(EntityConstants.NameOfDeletedAt)?.SetValue(entity, utcNow, null);
                            type.GetProperty(EntityConstants.NameOfModifiedByEntityId)?.SetValue(entity, null, null);
                            type.GetProperty(EntityConstants.NameOfModifiedAt)?.SetValue(entity, null, null);

                            continue;
                        }
                    }

                    if (entity.IsImplementingFrom(x => x == EntityConstants.TypeOfDeactivateable))
                    {
                        var isActiveCurrent = entity.GetValue(EntityConstants.NameOfIsActive)?.ToString()?.ToBoolOrDefault() ?? false;

                        var isActiveOriginal = entry.OriginalValues[EntityConstants.NameOfIsActive]?.ToString()?.ToBoolOrDefault() ?? false;

                        // when deactivated we must clear out 'modified' since it is not modified but 'deactivated'
                        if (isActiveOriginal && isActiveCurrent == false)
                        {
                            type.GetProperty(EntityConstants.NameOfDeactivatedByEntityId)?.SetValue(entity, currentUserId, null);
                            type.GetProperty(EntityConstants.NameOfDeactivatedAt)?.SetValue(entity, utcNow, null);
                            type.GetProperty(EntityConstants.NameOfModifiedByEntityId)?.SetValue(entity, null, null);
                            type.GetProperty(EntityConstants.NameOfModifiedAt)?.SetValue(entity, null, null);

                            continue;
                        }
                    }

                    if (entity.IsImplementingFrom(x => x == EntityConstants.TypeOfModifiable))
                    {
                        type.GetProperty(EntityConstants.NameOfModifiedByEntityId)?.SetValue(entity, currentUserId, null);
                        type.GetProperty(EntityConstants.NameOfModifiedAt)?.SetValue(entity, utcNow, null);
                    }
                }
                    continue;
                default:
                    continue;
            }
        }
    }
}
