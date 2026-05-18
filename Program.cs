using DotNetEnv;
using FoodOrdering.App_Start;
using FoodOrdering.Context;
using FoodOrdering.Extentions;
using FoodOrdering.Services.Interfaces;
using FoodOrdering.Services.Implementations;
using FoodOrdering.Hubs;
using FoodOrdering.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using FoodOrdering.Settings;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

Env.Load(); // đọc biến từ file .env

var defaultConnection = Environment.GetEnvironmentVariable("DefaultConnection");
var cloudName = Environment.GetEnvironmentVariable("CloudName");
var cloudApiKey = Environment.GetEnvironmentVariable("CloudApiKey");
var cloudApiSecret = Environment.GetEnvironmentVariable("CloudApiSecret");
var appUrl = Environment.GetEnvironmentVariable("AppUrl");

// DbContext
builder.Services.AddDbContext<FoodOrderingContext>(options =>
    options.UseSqlServer(defaultConnection)
);

builder.Services.Configure<NetworkAccessSettings>(builder.Configuration.GetSection("NetworkAccess"));

//CloudinarySettings
builder.Services.Configure<CloudinarySettings>(options =>
{
    options.CloudName = cloudName;
    options.ApiKey = cloudApiKey;
    options.ApiSecret = cloudApiSecret;
});


// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();
builder.Services.AddScoped<FoodOrdering.Services.Implementations.NetworkAccessService>();
builder.Services.AddSession();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";     
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// ===== Add Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Food Ordering API",
        Version = "v1",
        Description = "API Managerment Orders, MenuItems, Tables..."
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.


// ===== Enable Swagger =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Food Ordering API v1");
        c.RoutePrefix = "apis"; // mở swagger ở trang gốc: http://localhost:5000/
    });

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// app.UseMiddleware<FoodOrdering.Middleware.NetworkAccessMiddleware>();

app.UseSession();

app.UseCors("AllowAll");

app.UseAuthentication();   // SAU Routing
app.UseAuthorization();    // SAU Authentication

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<OrderHub>("/orderHub");
app.MapHub<NotifyHub>("/notifyHub");
app.MapHub<MenuItemHub>("/menuItemHub");
//RouteConfig.RegisterRoutes(app);
app.MapStaticAssets();

app.UseSession();


app.Run();
