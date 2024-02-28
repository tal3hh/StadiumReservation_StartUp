using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Services.Interface;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStadiumService, StadiumService>();
            services.AddScoped<IStadiumImageService, StadiumImageService>();
            services.AddScoped<IStadiumDiscountService, StadiumDiscountService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IReservationService, ReservationService>();

            services.AddTransient<ITokenService, TokenService>();
        }
    }
}
