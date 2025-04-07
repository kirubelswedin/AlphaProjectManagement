using Business.Extensions;
using Business.Interfaces;
using Data.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Hubs;
using ASP.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddContexts(builder.Configuration.GetConnectionString("SqlServer")!);
builder.Services.AddLocalIdentity(builder.Configuration);
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.ConfigureApplicationCookie((options => options.LoginPath = "/auth/login"));

var app = builder.Build();
app.UseHsts();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/dashboard"));
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseDefaultRoles(new[] { "Admin", "User" });
app.UseDefaultAdminAccount(
    email: "admin@domain.com",
    password: "BytMig123!",
    firstName: "System",
    lastName: "Administrator",
    role: "Admin"
);

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();