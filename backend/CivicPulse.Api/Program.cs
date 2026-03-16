using System.Text;
using CivicPulse.Api.Configuration;
using CivicPulse.Api.Data;
using CivicPulse.Api.Models;
using CivicPulse.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<AzureAISearchConfig>(builder.Configuration.GetSection("AzureAISearch"));
builder.Services.Configure<AzureAIVisionConfig>(builder.Configuration.GetSection("AzureAIVision"));
builder.Services.Configure<OpenAIConfig>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
    };
});

// Services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Create database, seed roles/admin, ensure search index, and index seed events
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    await SeedData.InitializeAsync(services);

    var searchService = services.GetRequiredService<ISearchService>();
    await searchService.EnsureIndexAsync();

    // Index all events that aren't yet in the search index
    try
    {
        var embeddingService = services.GetRequiredService<IEmbeddingService>();
        var events = await db.Events.ToListAsync();
        foreach (var ev in events)
        {
            try
            {
                var textToEmbed = $"{ev.Title} {ev.Description} {ev.Category} {ev.AiSummary}";
                var vector = await embeddingService.VectorizeTextAsync(textToEmbed);
                await searchService.IndexEventAsync(
                    ev.Id, ev.Title, ev.Description, ev.AiSummary,
                    ev.Category.ToString(), ev.EventDate, ev.Location,
                    ev.OrganizerId, vector);
                logger.LogInformation("Indexed event {EventId}: {Title}", ev.Id, ev.Title);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to index event {EventId}", ev.Id);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to index seed events — search may not work until events are re-indexed");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
