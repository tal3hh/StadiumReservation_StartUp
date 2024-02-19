using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


#region Identity

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;  //Simvollardan biri olmalidir(@,/,$) 
    opt.Password.RequireLowercase = false;       //Mutleq Kicik herf
    opt.Password.RequireUppercase = false;       //Mutleq Boyuk herf 
    opt.Password.RequiredLength = 4;            //Min. simvol sayi
    opt.Password.RequireDigit = false;

    opt.User.RequireUniqueEmail = true;

    opt.SignIn.RequireConfirmedEmail = true;
    opt.SignIn.RequireConfirmedAccount = false;

    //opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); //Sifreni 5 defe sehv girdikde hesab 1dk baglanir.
    //opt.Lockout.MaxFailedAccessAttempts = 5;                      //Sifreni max. 5defe sehv girmek olar.

}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
#endregion

#region Context

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["ConnectionStrings:Mssql"]);
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();