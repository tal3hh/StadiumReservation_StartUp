using AutoMapper;
using DomainLayer.Entities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Contexts;
using ServiceLayer.AutoMapper;
using ServiceLayer.Extensions;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//EXTENSION     
builder.Services.AddServices();
builder.Services.AddValidation();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


#region JWT

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audince"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ClockSkew = TimeSpan.Zero  // remove delay of token when expire
    };
});
#endregion


#region Identity

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;  //Simvollardan biri olmalidir(@,/,$) 
    opt.Password.RequireLowercase = false;       //Mutleq Kicik herf
    opt.Password.RequireUppercase = false;       //Mutleq Boyuk herf 
    opt.Password.RequiredLength = 4;            //Min. simvol sayi
    opt.Password.RequireDigit = false;

    opt.User.RequireUniqueEmail = true;

    opt.SignIn.RequireConfirmedEmail = false;
    opt.SignIn.RequireConfirmedAccount = false;

    //opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //Sifreni 5 defe sehv girdikde hesab 1dk baglanir.
    //opt.Lockout.MaxFailedAccessAttempts = 5;                      //Sifreni max. 5defe sehv girmek olar.

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

#endregion


#region Cookie

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Strict;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opt.Cookie.Name = "AshionIdentity";
    opt.LoginPath = new PathString("/Account/Login");
    opt.AccessDeniedPath = new PathString("/Account/AccessDenied");

});

#endregion


#region Context

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:Mssql"]);
});

#endregion


#region AutoMapper

var configuration = new MapperConfiguration(x =>
{
    x.AddProfile(new MappingProofile());
});
var mapper = configuration.CreateMapper();
builder.Services.AddSingleton(mapper);

#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
