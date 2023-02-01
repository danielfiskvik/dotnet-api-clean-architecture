using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Constants;

public static class EntityConstants
{
    #region ICreateable
    private static Type? _typeOfCreateable;
    public static Type TypeOfCreateable => _typeOfCreateable ??= typeof(ICreateable);
    
    private static string? _nameOfCreatedAt;
    public static string NameOfCreatedAt => _nameOfCreatedAt ??= nameof(ICreateable.CreatedAt);
    
    private static string? _nameOfCreatedByEntityId;
    public static string NameOfCreatedByEntityId => _nameOfCreatedByEntityId ??= nameof(ICreateable.CreatedByEntityId);
    #endregion
    
    #region IModifiable
    private static Type? _typeOfModifiable;
    public static Type TypeOfModifiable => _typeOfModifiable ??= typeof(IModifiable);

    private static string? _nameOfModifiedAt;
    public static string NameOfModifiedAt => _nameOfModifiedAt ??= nameof(IModifiable.ModifiedAt);
    
    private static string? _nameOfModifiedByEntityId;
    public static string NameOfModifiedByEntityId => _nameOfModifiedByEntityId ??= nameof(IModifiable.ModifiedByEntityId);
    #endregion
    
    #region IDeactivateable
    private static Type? _typeOfDeactivateable;
    public static Type TypeOfDeactivateable => _typeOfDeactivateable ??= typeof(IDeactivateable);
    
    private static string? _nameOfDeactivatedAt;
    public static string NameOfDeactivatedAt => _nameOfDeactivatedAt ??= nameof(IDeactivateable.DeactivatedAt);
    
    private static string? _nameOfIsActive;
    public static string NameOfIsActive => _nameOfIsActive ??= nameof(IDeactivateable.IsActive);
    
    private static string? _nameOfDeactivatedByEntityId;
    public static string NameOfDeactivatedByEntityId => _nameOfDeactivatedByEntityId ??= nameof(IDeactivateable.DeactivatedByEntityId);
    #endregion
    
    #region SoftDeletable
    private static Type? _typeOfSoftDeletable;
    public static Type TypeOfSoftDeletable => _typeOfSoftDeletable ??= typeof(ISoftDeletable);

    private static string? _nameOfDeletedAt;
    public static string NameOfDeletedAt => _nameOfDeletedAt ??= nameof(ISoftDeletable.DeletedAt);
    
    private static string? _nameOfIsDeleted;
    public static string NameOfIsDeleted => _nameOfIsDeleted ??= nameof(ISoftDeletable.IsDeleted);
    
    private static string? _nameOfDeletedByEntityId;
    public static string NameOfDeletedByEntityId => _nameOfDeletedByEntityId ??= nameof(ISoftDeletable.DeletedByEntityId);
    #endregion
}