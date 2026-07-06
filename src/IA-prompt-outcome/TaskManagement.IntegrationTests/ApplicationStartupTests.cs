namespace TaskManagement.IntegrationTests;

public sealed class ApplicationStartupTests : IClassFixture<TaskManagementWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public ApplicationStartupTests(TaskManagementWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task SwaggerDocument_IsAvailableInDevelopment()
    {
        var response = await _httpClient.GetAsync("/swagger/v1/swagger.json");

        response.EnsureSuccessStatusCode();
    }
}