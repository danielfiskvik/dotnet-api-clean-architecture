using System.ComponentModel.DataAnnotations;
using Domain.Interfaces;

namespace Domain.Entities;

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