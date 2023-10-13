using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SitoVetrina.Areas.Identity.Data;
using SitoVetrina.Context;
using SitoVetrina.Contracts;
using SitoVetrina.Data;
using SitoVetrina.Models.ProdottoRepository;
using SitoVetrina.Models.ProdottoViewModels;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SitoVetrinaContextConnection") ?? throw new InvalidOperationException("Connection string 'SitoVetrinaContextConnection' not found.");
var connectionString2 = builder.Configuration.GetConnectionString("SitoVetrinaContextConnectionMongo") ?? throw new InvalidOperationException("Connection string 'SitoVetrinaContextConnection' not found.");

builder.Services.AddDbContext<SitoVetrinaContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SitoVetrinaContext>();

// Add services to the container.
builder.Services.AddTransient<IProdottoRepository,ProdottoRepositoryDapper>();
builder.Services.AddTransient<DapperContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
