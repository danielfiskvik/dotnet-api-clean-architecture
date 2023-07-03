using System.Net;
using System.Net.Http.Json;
using Faker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ModularApp.Core.Helpers;
using ModularApp.IntegrationTests.Common;
using ModularApp.Modules.Workspace.Domain.Entities;
using ModularApp.Modules.Workspace.Domain.Interfaces;

namespace ModularApp.IntegrationTests;

public class UsersControllerTests : IntegrationTestBase
{
    private const int TenUsersToCreate = 10;

    public UsersControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    
    /*
     * Test that SaveChangesInterceptor sets correct property value on Entities that are Added, Modified, and Deleted.
     */
    
    /*
    [Fact]
    public async Task CreateUsers_Should_Set_Auditable_Properties_On_All_Entities()
    {
        // Arrange
        var httpClient = await InitializeTestAndCreateHttpClientAsync();

        var createUsersRequest = MakeUsers();
        
        // Set the Content-Type header to application/json
        var jsonContent = JsonContent.Create(createUsersRequest);
        
        // Act
        var response = await httpClient.PostAsync("api/Users", jsonContent);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK, "Http Status Code must be OK.");
        
        var responseAsString = await response.Content.ReadAsStringAsync();
        var users = JsonSerializerHelper.DeserializeObject<List<User>>(responseAsString);
        
        Assert.True(users != null && users.Any(), 
            "The list with users is null but should not be empty.");
        
        var expectedUsersCount = TenUsersToCreate + ExistingUserCountSnapshot;
        Assert.True(
            users.Count == expectedUsersCount, 
            $"There should be {expectedUsersCount} number of users in the database.");

        var usersCreatedByIdCount = users
            .Where(x => x.Id != CurrentUserIdSnapshot)
            .Count(x => x.CreatedByEntityId == CurrentUserIdSnapshot);
        
        Assert.True(usersCreatedByIdCount == TenUsersToCreate,
            $"All the {TenUsersToCreate} users created should have the same value of {nameof(ICreateable.CreatedByEntityId)}");
    }

    private static List<User> MakeUsers()
    {
        var createUsersRequest = new List<User>();
        
        for (var i = 0; i < TenUsersToCreate; i++)
        {
            var firstName = Name.First();
            var lastName = Name.Last();
            var fullName = $"{firstName} {lastName}";
            var userName = fullName.Replace(" ", "_").ToLowerInvariant();
            
            var user = new User
            {
                UserName = userName,
                FirstName = firstName,
                SureName = lastName,
            };
            
            createUsersRequest.Add(user);
        }

        return createUsersRequest;
    }
    */
}
