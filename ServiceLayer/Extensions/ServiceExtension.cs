using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer.Repositories;
using RepositoryLayer.Repository;
using ServiceLayer.Services;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.Pagine;

namespace ServiceLayer.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStadiumService, StadiumService>();
            services.AddScoped<IStadiumImageService, StadiumImageService>();
            services.AddScoped<IStadiumDetailService, StadiumDetailService>();
            services.AddScoped<IStadiumDiscountService, StadiumDiscountService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IReservationService, ReservationService>();

            services.AddTransient<ITokenService, TokenService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IPaginate<>), typeof(Paginate<>));
        }
    }
}
