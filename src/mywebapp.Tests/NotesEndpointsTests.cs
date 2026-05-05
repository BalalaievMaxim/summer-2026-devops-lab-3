using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using mywebapp.Endpoints;
using mywebapp.Models;

namespace mywebapp.Tests;

public class NotesEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotesEndpointsTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostNote_CreatesNoteAndReturnsIt()
    {
        var newNote = new CreateNoteDto
        {
            Title = "Test Title",
            Content = "Test Content"
        };

        var response = await _client.PostAsJsonAsync("/notes", newNote);

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdNote = await response.Content.ReadFromJsonAsync<Note>();
        Assert.NotNull(createdNote);
        Assert.Equal("Test Title", createdNote.Title);
        Assert.Equal("Test Content", createdNote.Content);
    }

    [Fact]
    public async Task GetNotes_ReturnsEmptyListOrExistingNotes()
    {
        var response = await _client.GetAsync("/notes");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.StartsWith("[", content);
    }

    [Fact]
    public async Task GetNotes_WithHtmlAcceptHeader_ReturnsHtmlTable()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/notes");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("<table", content);
        Assert.Contains("<th>ID</th>", content);
    }

    [Fact]
    public async Task GetNoteById_ExistingNote_ReturnsNote()
    {
        var newNote = new CreateNoteDto { Title = "Fetch Me", Content = "I am here" };
        var postResponse = await _client.PostAsJsonAsync("/notes", newNote);
        var createdNote = await postResponse.Content.ReadFromJsonAsync<Note>();

        Assert.NotNull(createdNote);

        var getResponse = await _client.GetAsync($"/notes/{createdNote.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetchedNote = await getResponse.Content.ReadFromJsonAsync<Note>();
        Assert.NotNull(fetchedNote);
        Assert.Equal(createdNote.Id, fetchedNote.Id);
        Assert.Equal("Fetch Me", fetchedNote.Title);
    }

    [Fact]
    public async Task GetNoteById_NonExistingNote_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/notes/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNoteById_WithHtmlAcceptHeader_ReturnsHtmlTable()
    {
        var newNote = new CreateNoteDto { Title = "HTML Note", Content = "HTML Content" };
        var postResponse = await _client.PostAsJsonAsync("/notes", newNote);
        var createdNote = await postResponse.Content.ReadFromJsonAsync<Note>();

        Assert.NotNull(createdNote);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/notes/{createdNote.Id}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("<table", content);
        Assert.Contains(createdNote.Id.ToString(), content);
        Assert.Contains("HTML Note", content);
    }
}