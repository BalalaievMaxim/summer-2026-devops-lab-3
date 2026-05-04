using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mywebapp.Data;
using mywebapp.Models;
using System.Text;

namespace mywebapp.Endpoints;

public static class NotesEndpoints
{
    public static void MapNotesEndpoints(this WebApplication app)
    {
        app.MapGet("/notes", async (HttpRequest request, AppDbContext dbContext) =>
        {
            var notes = await dbContext.Notes
                .Select(n => new { n.Id, n.Title })
                .ToListAsync();

            if (request.Headers.Accept.ToString().Contains("text/html"))
            {
                var sb = new StringBuilder("<html><body><table border='1'><tr><th>ID</th><th>Title</th></tr>");
                foreach (var note in notes)
                {
                    sb.Append($"<tr><td>{note.Id}</td><td>{note.Title}</td></tr>");
                }
                sb.Append("</table></body></html>");
                return Results.Content(sb.ToString(), "text/html");
            }

            return Results.Ok(notes);
        });

        app.MapGet("/notes/{id:guid}", async (HttpRequest request, AppDbContext dbContext, Guid id) =>
        {
            var note = await dbContext.Notes.FindAsync(id);
            if (note == null) return Results.NotFound();

            if (request.Headers.Accept.ToString().Contains("text/html"))
            {
                var html = $@"
                    <html><body>
                    <table border='1'>
                        <tr><th>ID</th><td>{note.Id}</td></tr>
                        <tr><th>Title</th><td>{note.Title}</td></tr>
                        <tr><th>Created At</th><td>{note.CreatedAt}</td></tr>
                        <tr><th>Content</th><td>{note.Content}</td></tr>
                    </table>
                    </body></html>";
                return Results.Content(html, "text/html");
            }

            return Results.Ok(note);
        });

        app.MapPost("/notes", async (AppDbContext dbContext, [FromBody] CreateNoteDto dto) =>
        {
            var note = new Note
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Notes.Add(note);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/notes/{note.Id}", note);
        });
    }
}

public class CreateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}