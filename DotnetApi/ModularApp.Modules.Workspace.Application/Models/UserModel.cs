namespace ModularApp.Modules.Workspace.Application.Models;

public class UserModel
{
    public string? UserName { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? SureName { get; set; }
    
    public string? FullName { get; set; }
}

public record UserModel2(string? UserName);