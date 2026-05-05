using System.Net;
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
}