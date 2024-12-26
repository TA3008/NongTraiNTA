using RauSach.Web.Helpers;
using RauSach.Web.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();
builder.AddMongoDatabase();
builder.Services.RegisterServices();

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    //builder.AddRazorRuntimeCompilation();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context => context.Context.Response.Headers.Add("Cache-Control", "public, max-age=28800")
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "admin/{controller=Home}/{action=Index}"
);

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "danhsachtin",
    pattern: FriendlyUrl.ArticleListFrm,
    defaults: new { controller = "Article", action = "List" });

app.MapControllerRoute(
    name: "tinchitiet",
    pattern: FriendlyUrl.ArticleDetailFrm,
    defaults: new {controller = "Article", action = "Detail"});

app.MapControllerRoute(
    name: "dathang",
    pattern: FriendlyUrl.ShopFrm,
    defaults: new { controller = "Order", action = "Index" });

app.MapControllerRoute(
    name: "donhangcuatoi",
    pattern: FriendlyUrl.MyOrderFrm,
    defaults: new { controller = "Order", action = "MyOrder" });

app.MapControllerRoute(
    name: "cacvuoncuatoi",
    pattern: FriendlyUrl.MyGardensFrm,
    defaults: new { controller = "Order", action = "MyGardens" });

app.MapControllerRoute(
    name: "vuoncuatoi",
    pattern: FriendlyUrl.MyGardenDetailFrm,
    defaults: new { controller = "Order", action = "MyGarden" });

app.MapControllerRoute(
    name: "vuonchitiet",
    pattern: FriendlyUrl.GardenDetailFrm,
    defaults: new { controller = "Garden", action = "Detail" });

app.MapControllerRoute(
    name: "danhsachvuon",
    pattern: FriendlyUrl.GardenListFrm,
    defaults: new { controller = "Garden", action = "List" });

app.MapControllerRoute(
    name: "aboutus",
    pattern: FriendlyUrl.AboutUsFrm,
    defaults: new { controller = "Home", action = "AboutUs" });

app.MapControllerRoute(
    name: "contact",
    pattern: FriendlyUrl.ContactFrm,
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name: "profile",
    pattern: FriendlyUrl.ProfileFrm,
    defaults: new { controller = "Account", action = "Profile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
