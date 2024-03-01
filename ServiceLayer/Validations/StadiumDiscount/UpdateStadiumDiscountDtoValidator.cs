using FluentValidation;
using ServiceLayer.Dtos.StadiumDiscount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class UpdateStadiumDiscountDtoValidator : AbstractValidator<UpdateStadiumDiscountDto>
    {
        public UpdateStadiumDiscountDtoValidator()
        {
            RuleFor(dto => dto.Id)
                .GreaterThan(0).WithMessage("Id 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.StadiumId)
                .GreaterThan(0).WithMessage("Stadyum IDsi 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.Path)
                .NotNull().WithMessage("Path boş ola bilməz.")
                .NotEmpty().WithMessage("Path boş ola bilməz.")
                .MaximumLength(500).WithMessage("Path 500 simvoldan çox ola bilməz.");
        }
    }
}
