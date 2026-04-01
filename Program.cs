using FoodOrdering.App_Start;
using FoodOrdering.Context;
using FoodOrdering.Extentions;
using FoodOrdering.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<FoodOrderingContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
builder.Services.AddApplicationServices();
builder.Services.AddSession();
builder.Services.AddSignalR();

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

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.MapHub<OrderHub>("/orderHub");
RouteConfig.RegisterRoutes(app);
app.MapStaticAssets();

app.UseSession();


app.Run();
