using FluentValidation;
using ServiceLayer.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(dto => dto.Fullname)
                .NotNull().WithMessage("Ad soyad boş ola bilməz.")
                .NotEmpty().WithMessage("Ad soyad boş ola bilməz.")
                .MaximumLength(200).WithMessage("Ad soyad 200 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.Username)
                .NotNull().WithMessage("İstifadəçi adı boş ola bilməz.")
                .NotEmpty().WithMessage("İstifadəçi adı boş ola bilməz.")
                .MaximumLength(100).WithMessage("İstifadəçi adı 100 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.Email)
                .NotNull().WithMessage("E-poçt boş ola bilməz.")
                .NotEmpty().WithMessage("E-poçt boş ola bilməz.")
                .MaximumLength(200).WithMessage("E-poçt 200 simvoldan çox ola bilməz.")
                .EmailAddress().WithMessage("Düzgün bir e-poçt ünvanı daxil edin.");

            RuleFor(dto => dto.Number)
                .NotNull().WithMessage("Telefon nömrəsi boş ola bilməz.")
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz.");

            RuleFor(dto => dto.Password)
                .NotNull().WithMessage("Şifrə boş ola bilməz.")
                .NotEmpty().WithMessage("Şifrə boş ola bilməz.")
                .MinimumLength(3).WithMessage("Şifrə ən azı 3 simvoldan ibarət olmalıdır.");

            RuleFor(dto => dto.ConfrimPassword)
                .NotNull().WithMessage("Şifrənin təkrarı boş ola bilməz.")
                .NotEmpty().WithMessage("Şifrənin təkrarı boş ola bilməz.")
                .Equal(dto => dto.Password).WithMessage("Şifrə və təkrarı eyni olmalıdır.");
        }
    }
}
