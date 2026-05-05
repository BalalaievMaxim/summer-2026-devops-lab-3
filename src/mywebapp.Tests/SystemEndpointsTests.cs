namespace mywebapp.Tests;

public class SystemEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SystemEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthAlive_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/alive");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("\"OK\"", content);
    }

    [Fact]
    public async Task GetHealthReady_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/ready");
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetRoot_ReturnsHtmlWithInstructions()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("API Endpoints", content);
    }
}