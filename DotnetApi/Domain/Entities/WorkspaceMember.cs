using System.ComponentModel.DataAnnotations;

using Domain.Interfaces;

namespace Domain.Entities;

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