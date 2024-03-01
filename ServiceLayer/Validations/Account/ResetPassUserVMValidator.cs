using FluentValidation;
using ServiceLayer.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class ResetPassUserVMValidator : AbstractValidator<ResetPassUserVM>
    {
        public ResetPassUserVMValidator()
        {
            RuleFor(vm => vm.Username)
                .NotNull().WithMessage("İstifadəçi adı boş ola bilməz.")
                .NotEmpty().WithMessage("İstifadəçi adı boş ola bilməz.")
                .MaximumLength(50).WithMessage("İstifadəçi adı 50 simvoldan çox ola bilməz.");

            RuleFor(vm => vm.OldPassword)
                .NotNull().WithMessage("Köhnə şifrə boş ola bilməz.")
                .NotEmpty().WithMessage("Köhnə şifrə boş ola bilməz.");

            RuleFor(vm => vm.NewPassword)
                .NotEmpty().WithMessage("Yeni şifrə boş ola bilməz.")
                .NotNull().WithMessage("Yeni şifrə boş ola bilməz.")
                .MinimumLength(6).WithMessage("Yeni şifrə ən azı 6 simvoldan ibarət olmalıdır.");

            RuleFor(vm => vm.NewConfrimPassword)
                .NotNull().WithMessage("Yeni şifrənin təkrarı boş ola bilməz.")
                .NotEmpty().WithMessage("Yeni şifrənin təkrarı boş ola bilməz.")
                .Equal(vm => vm.NewPassword).WithMessage("Yeni şifrə və təkrarı eyni olmalıdır.");
        }
    }
}
