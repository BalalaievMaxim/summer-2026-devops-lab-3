using System.Text;
using mywebapp.Data;

namespace mywebapp.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this WebApplication app)
    {
        app.MapGet("/health/alive", () => Results.Ok("OK"));

        app.MapGet("/health/ready", async (AppDbContext dbContext) =>
        {
            try
            {
                bool canConnect = await dbContext.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Results.Ok("OK");
                }
                return Results.Problem("Database connection failed", statusCode: 500);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        });

        app.MapGet("/", (HttpRequest request) =>
        {
            string html = @"
                <html>
                <body>
                    <h1>API Endpoints</h1>
                    <ul>
                        <li>GET /notes - вивести список усих нотаток</li>
                        <li>POST /notes - створити нову нотатку</li>
                        <li>GET /notes/{id} - вивести повний вміст нотатки</li>
                    </ul>
                </body>
                </html>";

            return Results.Content(html, "text/html", Encoding.UTF8);
        });
    }
}