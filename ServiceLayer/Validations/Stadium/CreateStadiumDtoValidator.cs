using FluentValidation;
using ServiceLayer.Dtos.Stadium.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class CreateStadiumDtoValidator : AbstractValidator<CreateStadiumDto>
    {
        public CreateStadiumDtoValidator()
        {
            RuleFor(dto => dto.Username)
                .NotEmpty().WithMessage("Username boş ola bilməz.")
                .NotNull().WithMessage("Username boş ola bilməz.");

            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Stadium adı boş ola bilməz.")
                .NotNull().WithMessage("Stadium adı boş ola bilməz.")
                .MaximumLength(100).WithMessage("Stadium adı 100 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.City)
                .NotEmpty().WithMessage("Şəhər boş ola bilməz.")
                .NotNull().WithMessage("Şəhər boş ola bilməz.")
                .MaximumLength(50).WithMessage("Şəhər adı 50 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.minPrice)
                .GreaterThan(0).WithMessage("Minimum qiymət 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.maxPrice)
                .GreaterThan(0).WithMessage("Maksimum qiymət 0-dan böyük olmalıdır.")
                .GreaterThanOrEqualTo(dto => dto.minPrice).WithMessage("Maksimum qiymət, minimum qiymətdən böyük və ya bərabər olmalıdır.");

            RuleFor(dto => dto.Address)
                .NotEmpty().WithMessage("Ünvan boş ola bilməz.")
                .MaximumLength(400).WithMessage("Ünvan 400 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz.")
                .NotNull().WithMessage("Telefon nömrəsi boş ola bilməz.");

            RuleFor(dto => dto.OpenCloseDay)
                .NotEmpty().WithMessage("Açılış-Bağlanış günü boş ola bilməz.")
                .NotNull().WithMessage("Açılış-Bağlanış günü boş ola bilməz.");

            RuleFor(dto => dto.OpenCloseHour)
                .NotNull().WithMessage("Açılış-Bağlanış vaxtı boş ola bilməz.")
                .NotEmpty().WithMessage("Açılış-Bağlanış vaxtı boş ola bilməz.");
        }
    }
}
