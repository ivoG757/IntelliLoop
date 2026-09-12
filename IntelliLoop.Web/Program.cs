using FFMpegCore;
using IntelliLoop.Web.Data;
using IntelliLoop.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
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

if (!File.Exists(builder.Configuration["Whisper:ModelPath"]))
{
    using var modelStream = await WhisperGgmlDownloader.Default.GetGgmlModelAsync(GgmlType.Base); //downloads the base model of whisper
    using var fileWriter = File.Create(builder.Configuration["Whisper:ModelPath"]!);
    await modelStream.CopyToAsync(fileWriter);
}

builder.Services.AddSingleton<TranscriptionService>();

var app = builder.Build();

// TESTING: Transcribe an audio file and print the transcript to the console

var mediaInfo = await FFProbe.AnalyseAsync(@"D:\projects\IntelliLoop\IntelliLoop.Web\test.wav");

Console.WriteLine($"Format: {mediaInfo.Format.FormatName}");
Console.WriteLine($"Duration: {mediaInfo.Duration}");


var transcriptionService = app.Services.GetRequiredService<TranscriptionService>();

var transcript = await transcriptionService.TranscribeAudio(@"D:\projects\IntelliLoop\IntelliLoop.Web\test.wav");

Console.WriteLine(transcript);

// END OF TESTING

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
