using FoodRoasterServer.BackgroundJobs;
using FoodRoasterServer.Data;
using FoodRoasterServer.Exceptions;
using FoodRoasterServer.Mappers;
using FoodRoasterServer.Middleware;
using FoodRoasterServer.Repositories;
using FoodRoasterServer.Services;
using FoodRoasterServer.Services.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure());
    options.EnableSensitiveDataLogging();

});



builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddTransient<TokenCleanUpJob>();
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoasterService, RoasterService>(); 
builder.Services.AddTransient<IFoodMenuService, FoodMenuService>();
builder.Services.AddSingleton<IAuditService, AuditService>();

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});

// Configure Serilog from appsettings.json
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
);

Console.WriteLine($"Current Environment-------------------: {builder.Environment.EnvironmentName}");


var secretKey = builder.Configuration["JwtSettings:Key" ];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHangfire(x => x.UseMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddAuthorization();
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddTransient<DataSeeder>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Build the app
var app = builder.Build();


if (args.Contains("seed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.Seed();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts(); //Only in production to enforce HTTPS strictly
}

app.UseHangfireDashboard();
app.UseHangfireServer();


// this background Job happens for each and every minute
RecurringJob.AddOrUpdate<TokenCleanUpJob>(
    "remove-expired-tokens",
    job => job.RemoveExpiredTokens(),
    "* * * * *"
);
app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditMiddleware>();
app.UseCors();
//app.UseMvc();
app.MapControllers();

app.Run();
