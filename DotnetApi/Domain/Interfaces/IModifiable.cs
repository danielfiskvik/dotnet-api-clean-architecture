namespace Domain.Interfaces;

public interface IModifiable
{
    DateTimeOffset? ModifiedAt { get; set; }
    
    Guid? ModifiedByEntityId { get; set; }
}