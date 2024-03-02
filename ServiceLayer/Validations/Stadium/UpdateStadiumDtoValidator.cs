using FluentValidation;
using ServiceLayer.Dtos.Stadium.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class UpdateStadiumDtoValidator : AbstractValidator<UpdateStadiumDto>
    {
        public UpdateStadiumDtoValidator()
        {
            RuleFor(dto => dto.Id)
                .GreaterThan(0).WithMessage("Id 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.AppUserId)
                .NotEmpty().WithMessage("AppUserId boş ola bilməz.")
                .NotNull().WithMessage("AppUserId boş ola bilməz.");

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

            RuleFor(dto => dto.Description)
                .MaximumLength(800).WithMessage("Təsvir 800 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.OpenDay)
                .NotEmpty().WithMessage("Açılış günü boş ola bilməz.")
                .NotNull().WithMessage("Açılış günü boş ola bilməz.");

            RuleFor(dto => dto.CloseDay)
                .NotEmpty().WithMessage("Bağlanış günü boş ola bilməz.")
                .NotNull().WithMessage("Bağlanış günü boş ola bilməz.");

            RuleFor(dto => dto.OpenTime)
                .NotNull().WithMessage("Açılış vaxtı boş ola bilməz.")
                .NotEmpty().WithMessage("Açılış vaxtı boş ola bilməz.");

            RuleFor(dto => dto.CloseTime)
                .NotNull().WithMessage("Bağlanış vaxtı boş ola bilməz.")
                .NotEmpty().WithMessage("Bağlanış vaxtı boş ola bilməz.")
                .GreaterThan(dto => dto.OpenTime).WithMessage("Bağlanış vaxtı, açılış vaxtından böyük olmalıdır.");
        }
    }
}
