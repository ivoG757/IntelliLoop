using FFMpegCore;
using IntelliLoop.Core.Entities;
using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Data;
using IntelliLoop.Web.Services.AI;
using IntelliLoop.Web.Services.Background;
using IntelliLoop.Web.Services.Transcription;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OllamaSharp;
using System.Text;
using System.Threading.Channels;
using Whisper.net;
using Whisper.net.Ggml;

var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = Encoding.UTF8;
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPromptService, PromptService>();


// Create Whisper model

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
}); // 

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
