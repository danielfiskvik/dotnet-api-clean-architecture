using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class WorkspaceMember : IEntity
{
    [Key]
    public Guid Id { get; set; }

    #region Relations
    public Guid WorkspaceId { get; set; } 
    public Guid UserId { get; set; }
    #endregion

    #region Navigation Properties
    public Workspace? Workspace { get; set; }
    public User? User { get; set; }
    #endregion
}