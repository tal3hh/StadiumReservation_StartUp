using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Dtos.Account;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Validations;

namespace ServiceLayer.Extensions
{
    public static class ValidationExtension
    {
        public static void AddValidation(this IServiceCollection services)
        {
            //Account
            services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<ResetPassUserVM>, ResetPassUserVMValidator>();

            //Area
            services.AddScoped<IValidator<CreateAreaDto>, CreateAreaDtoValidator>();
            services.AddScoped<IValidator<UpdateAreaDto>, UpdateAreaDtoValidator>();

            //Stadum
            services.AddScoped<IValidator<CreateStadiumDto>, CreateStadiumDtoValidator>();
            services.AddScoped<IValidator<UpdateStadiumDto>, UpdateStadiumDtoValidator>();

            //Reservation
            services.AddScoped<IValidator<CreateReservationDto>, CreateReservationDtoValidator>();
            services.AddScoped<IValidator<UpdateReservationDto>, UpdateReservationDtoValidator>();

            //StadiumDiscount
            services.AddScoped<IValidator<CreateStadiumDiscountDto>, CreateStadiumDiscountDtoValidator>();
            services.AddScoped<IValidator<UpdateStadiumDiscountDto>, UpdateStadiumDiscountDtoValidator>();

            //StadiumImage
            services.AddScoped<IValidator<CreateStadiumImageDto>, CreateStadiumImageDtoValidator>();
            services.AddScoped<IValidator<UpdateStadiumImageDto>, UpdateStadiumImageDtoValidator>();
        }

    }
}
