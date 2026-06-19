using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<IGeminiService, GeminiService>();

if (builder.Environment.IsProduction() &&
    !string.IsNullOrEmpty(builder.Configuration["AzureBlobStorage:ConnectionString"]))
    builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
else
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

var app = builder.Build();

// Auto-apply migrations in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

app.Run();
