using IntelliLoop.Core.Interfaces;
using IntelliLoop.Web.Services;
using OllamaSharp;
using System.Text;
using System.Threading.Channels;
using Whisper.net.Ggml;

using IntelliLoop.Web.Extensions;


var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = Encoding.UTF8;

builder.Logging.AddConsole();

builder.Services.AddDatabase(builder.Configuration);

builder.Services.AddIdentityOptions();

builder.Services.AddControllersWithViews(); 

builder.Services.AddRazorPages(); 

builder.Services.AddApplicationServices();

await builder.Services.ConfigureWhisperAsync(builder.Configuration);

builder.Services.AddSingleton<ITranscriptionService, TranscriptionService>();

builder.Services.AddSingleton(_ =>
{
    var channel = Channel.CreateBounded<Guid>(new BoundedChannelOptions(100) 
    {
        FullMode = BoundedChannelFullMode.Wait
    });

    return channel;
});

builder.Services.AddOllama();

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
