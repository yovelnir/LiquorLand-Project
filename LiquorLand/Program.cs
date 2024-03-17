using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LiquorLand.Areas.Identity.Data;
using LiquorLand.Models;
using LiquorLand.Data;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UserDbContextConnection") ?? throw new InvalidOperationException("Connection string 'UserDbContextConnection' not found.");

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<Users>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/AccessDenied";
});


//for me

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600); // Session timeout of 1 hour
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "ProductPage",
    pattern: "item/{ProductName?}",
    defaults: new { controller = "Product", action="productsShow"});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "User" };

    foreach(var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

    string email = "admin@admin.com";
    string password = "Admin123.";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new Users();
        user.Email = email;
        user.UserName = "System_Admin";
        user.FirstName = "admin";
        user.LastName = "admin";

        var result = await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
        await userManager.AddToRoleAsync(user, "User");
    }
}

app.Use(async (context, next) =>
{
    // Store Model
    if (context.Session.GetString("cart") == null)
    {
        var cart = new ShoppingCart(); // Initialize your model
        context.Session.SetString("cart", JsonConvert.SerializeObject(cart));
    }

    await next();   
});

app.UseStaticFiles();

app.Run();
