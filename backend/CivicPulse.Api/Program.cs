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

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetValue<string>("FrontendUrl") ?? "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed roles, admin user, and ensure search index exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);

    var searchService = services.GetRequiredService<ISearchService>();
    await searchService.EnsureIndexAsync();
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
