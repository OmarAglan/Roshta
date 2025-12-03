using Rosheta.Core;
using Rosheta.Infrastructure;
using Rosheta.Core.Application.Settings;
using Rosheta.Filters;
using Rosheta.Presentation.Middleware;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Settings (Options Pattern)
builder.Services.Configure<LicenseSettings>(
    builder.Configuration.GetSection(LicenseSettings.SectionName));

// 2. Register Layer Dependencies (The Clean Way)
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Web Layer Configuration
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        // Since we moved Program.cs to Presentation/, the root is relative to it
        options.RootDirectory = "/Pages";
    })
    .AddMvcOptions(options =>
    {
        options.Filters.Add<ActivationCheckPageFilter>();
    });

var app = builder.Build();

// 4. HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();