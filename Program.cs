using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SnakeServerAPI;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.DataBase.Data;
using SnakeServerAPI.DiscordApi;
using SnakeServerAPI.Service;
using System.Text;


//CheckMemberRole CheckMemberRole = new();
//CheckMemberRole.RoleCheckerBot();

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHttpLogging(options => // <--- Setup logging
{
    // Specify all that you need here:
    options.LoggingFields = HttpLoggingFields.RequestHeaders |
                            HttpLoggingFields.RequestBody |
                            HttpLoggingFields.ResponseHeaders |
                            HttpLoggingFields.ResponseBody;
});

// Add services to the container.

string connStr = string.Format("Server={0};Port={1};Database={2};User ID={3};Password={4};Pooling=true;Connection Lifetime=0;SslMode=Disable;SslMode=Disable;",
builder.Configuration.GetSection("PrimaryDB:host").Value,
builder.Configuration.GetSection("PrimaryDB:port").Value,
builder.Configuration.GetSection("PrimaryDB:database").Value,
builder.Configuration.GetSection("PrimaryDB:user").Value,
builder.Configuration.GetSection("PrimaryDB:password").Value);


builder.Services.AddDbContext<SnakeDB>(options =>
{
    options.UseNpgsql(connStr)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();

}, ServiceLifetime.Scoped);

builder.Services.AddSingleton<Random>();
builder.Services.AddSingleton<CheckMemberRole>();
builder.Services.AddRoutinesHandler();


builder.Services.AddAuthentication(p =>
{
    p.DefaultAuthenticateScheme = "Cookie";
    p.DefaultSignInScheme = "Cookie";
    p.DefaultChallengeScheme = "Discord";
})
        .AddScheme<TokenAuthOptions, TokenAuthHandler>("Token", _ => { })
        .AddCookie("Cookie")
        .AddDiscord("Discord", oauth =>
        {
            oauth.ClientId = builder.Configuration["Discord:ClientId"];
            oauth.ClientSecret = builder.Configuration["Discord:ClientSecret"];
            oauth.CallbackPath = "/auth/callback";
            oauth.AccessDeniedPath = "/test";
        });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Snake.API", Version = "v1" });
});
builder.Services.AddApplicationInsightsTelemetry();




var app = builder.Build();
//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseHttpLogging();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


#region Starting Discord
using (var scope = app.Services.CreateScope())
{
    var discordClient = scope.ServiceProvider.GetService<CheckMemberRole>();
    discordClient.Start();

}
#endregion


#region CheckApp
//using (var scope = app.Services.CreateScope())
//{
//    var Path = "Main.exe";
//    byte[] Byte = await File.ReadAllBytesAsync(Path);
//    var snakeDB = scope.ServiceProvider.GetService<SnakeDB>();
//    string Base64Byte = Convert.ToBase64String(Byte);


//    var dbUserRoles = snakeDB.MainAppByte.Where(p => p.Data == Base64Byte).ToList();
//    if (!dbUserRoles.Any() && snakeDB != null)
//    {
//        snakeDB.MainAppByte.Add((
//                new BytesApplication
//                {
//                    Data = Base64Byte,
//                    DateTime = DateTime.Now,

//                }));
//        await snakeDB.SaveChangesAsync();
//    }
//    await Task.Delay(360 * 10000);
//    Console.Write("await Task.Delay(360*10000);");



//}
#endregion









app.Run();
