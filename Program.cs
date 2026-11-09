using netgram.Data;
using Microsoft.EntityFrameworkCore;
using netgram.Models;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;
using CloudinaryDotNet;

Env.Load();


var builder = WebApplication.CreateBuilder(args);

var cloudinarySettings = new CloudinarySettings
{
    CloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME") ?? "",
    ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY") ?? "",
    ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET") ?? ""
};

builder.Services.AddSingleton(cloudinarySettings);

var account = new CloudinaryDotNet.Account(
   cloudinarySettings.CloudName,
   cloudinarySettings.ApiKey,
   cloudinarySettings.ApiSecret
);
var cloudinary = new CloudinaryDotNet.Cloudinary(account);
builder.Services.AddSingleton(cloudinary);
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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


app.Run();
