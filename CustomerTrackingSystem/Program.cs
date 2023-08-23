using CustomerTrackingSystem.Contract;
using CustomerTrackingSystem.Data;
using CustomerTrackingSystem.Models;
using CustomerTrackingSystem.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork<Customer>, CustomerRepository>();
builder.Services.AddScoped<IUnitOfWork<Address>, AddressRepository>();
builder.Services.AddScoped<IUnitOfWork<Contact>, ContactRepository>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CustomerManagement}/{action=ViewCustomers}/{id?}");

app.Run();
