using IntelliLoop.Core.Interfaces;
using IntelliLoop.Core.Repository;
using IntelliLoop.Web.Data;
using IntelliLoop.Web.Identity;
using IntelliLoop.Web.Repositories;
using IntelliLoop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using System.Text;
using System.Threading.Channels;
using Whisper.net.Ggml;
using IntelliLoop.Web.Common;

var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = Encoding.UTF8;

builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<IntelliLoopDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IntelliLoopDbContext>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = Constants.Account.MinimumPasswordLength;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<IntelliLoopDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPromptService, PromptService>();

builder.Services.AddScoped<ILectureGenerationService, LectureGenerationService>();

builder.Services.AddScoped<LectureProcessingService>();

builder.Services.AddScoped<ILectureProcessingRepository, LectureProcessingRepository>();

builder.Services.AddScoped<ILectureRepository, LectureRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IFileStorage, FileStorage>();

builder.Services.AddHostedService<LectureWorker>();


if (!File.Exists(builder.Configuration["Whisper:ModelPath"]))
{
    using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Small); //downloads the small model of whisper
    using var fileWriter = File.Create(builder.Configuration["Whisper:ModelPath"]!);
    await modelStream.CopyToAsync(fileWriter);
}

builder.Services.AddSingleton<ITranscriptionService, TranscriptionService>();

builder.Services.AddSingleton(_ =>
{
    var channel = Channel.CreateBounded<Guid>(new BoundedChannelOptions(100) 
    {
        FullMode = BoundedChannelFullMode.Wait
    });

    return channel;
});

var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(10)
};

var ollamaClient = new OllamaApiClient(httpClient)
{
    SelectedModel = "qwen3:8b"
}; 
// TODO: Move this to a configuration file or environment variable later

builder.Services.AddChatClient(ollamaClient);
builder.Services.AddScoped<ILlmService, LocalLlmService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
