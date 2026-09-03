using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SavingChallengeService.Data;
using SavingChallengeService.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── EF Core – SQL Server ─────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DailyRecordService>();           // concrete – used by other services
builder.Services.AddScoped<IDailyRecordService>(sp => sp.GetRequiredService<DailyRecordService>());
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICoupleService, CoupleService>();
builder.Services.AddScoped<IStatsService, StatsService>();

// ── CORS – allow local development and the deployed Netlify frontend ──────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:3000",
                "https://reliable-smakager-754cda.netlify.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Saving Challenge API",
        Version     = "v1",
        Description = "Backend API for the SaveTogether couple saving challenge app. " +
                      "Uses SQL Server with pre-seeded sample data.",
    });

    // Include XML comments if present
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Ensure the SQL Server database exists and seed it ─────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("""
        IF COL_LENGTH('dbo.Couples', 'UserKeyA') IS NULL
            ALTER TABLE dbo.Couples ADD UserKeyA nvarchar(50) NOT NULL CONSTRAINT DF_Couples_UserKeyA DEFAULT N'A';
        IF COL_LENGTH('dbo.Couples', 'UserKeyB') IS NULL
            ALTER TABLE dbo.Couples ADD UserKeyB nvarchar(50) NULL;
        ELSE IF EXISTS
        (
            SELECT 1
            FROM sys.columns
            WHERE object_id = OBJECT_ID('dbo.Couples')
              AND name = 'UserKeyB'
              AND is_nullable = 0
        )
            ALTER TABLE dbo.Couples ALTER COLUMN UserKeyB nvarchar(50) NULL;
        """);
    SeedData.Initialize(db);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Saving Challenge API v1");
    c.RoutePrefix = string.Empty; // Swagger at root "/"
});

app.UseCors("FrontendPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
