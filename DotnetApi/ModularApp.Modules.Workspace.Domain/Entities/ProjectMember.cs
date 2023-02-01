using System.ComponentModel.DataAnnotations;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.Modules.Workspace.Domain.Entities;

public class ProjectMember : IEntity
{
    [Key]
    public Guid Id { get; set; }

    #region Relations
    public Guid WorkspaceMemberId { get; set; } 
    public Guid ProjectId { get; set; }
    #endregion

    #region Navigation Properties
    public WorkspaceMember? WorkspaceMember { get; set; }
    public Project? Project { get; set; }
    #endregion
}