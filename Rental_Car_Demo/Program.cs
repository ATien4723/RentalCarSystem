using Microsoft.AspNetCore.Authentication.Cookies;
using NuGet.Protocol.Core.Types;
using Rental_Car_Demo.Repository.CarRepository;
using Rental_Car_Demo.Services;
using Rental_Car_Demo.Models;
using Microsoft.EntityFrameworkCore;
using Rental_Car_Demo.Repository;
using Rental_Car_Demo.Context;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using Org.BouncyCastle.Pqc.Crypto.Lms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ICustomerContext, CustomerContext>();
builder.Services.AddTransient<ITokenGenerator, TokenGenerator>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RentCarDbContext>(options 
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();
builder.Services.AddScoped<ICarRepository, CarRepository> ();
builder.Services.AddScoped<AddressRepository> ();
builder.Services.AddDbContext<RentCarDbContext> (ServiceLifetime.Transient);



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                }
                );
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseDeveloperExceptionPage();
//app.UseDatabaseErrorPage();

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Guest}/{id?}");

app.Run();
