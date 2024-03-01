using FluentValidation;
using ServiceLayer.Dtos.Reservation.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class UpdateReservationDtoValidator : AbstractValidator<UpdateReservationDto>
    {
        public UpdateReservationDtoValidator()
        {
            RuleFor(dto => dto.Id)
                .GreaterThan(0).WithMessage("Id 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.areaId)
                .GreaterThan(0).WithMessage("Area IDsi 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.ByName)
                .NotEmpty().WithMessage("Reserv eden insanin adi boş ola bilməz.")
                .NotNull().WithMessage("Reserv eden insanin adi boş ola bilməz.")
                .MaximumLength(150).WithMessage("Reserv eden insanin adi 150 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0).WithMessage("Qiymət 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.PhoneNumber)
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz.")
                .NotNull().WithMessage("Telefon nömrəsi boş ola bilməz.");

            RuleFor(dto => dto.Date)
                .NotEmpty().WithMessage("Tarix boş ola bilməz.")
                .NotNull().WithMessage("Tarix boş ola bilməz.")
                .Must(BeValidDateTime).WithMessage("Düzgün bir tarix və vaxt daxil edin.");
        }

        private bool BeValidDateTime(DateTime date)
        {
            return date >= DateTime.Now.AddDays(-1);
        }
    }
}
