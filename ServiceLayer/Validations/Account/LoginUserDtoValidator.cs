using FluentValidation;
using ServiceLayer.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(dto => dto.UsernameorEmail)
                .NotNull().WithMessage("İstifadəçi adı və ya e-poçt boş ola bilməz.")
                .NotEmpty().WithMessage("İstifadəçi adı və ya e-poçt boş ola bilməz.")
                .MaximumLength(400).WithMessage("İstifadəçi adı və ya e-poçt 400 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Şifrə boş ola bilməz.")
                .NotNull().WithMessage("Şifrə boş ola bilməz.");
        }
    }
}
