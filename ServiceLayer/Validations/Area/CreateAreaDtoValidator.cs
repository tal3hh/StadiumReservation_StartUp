using FluentValidation;
using ServiceLayer.Dtos.Area.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validations
{
    public class CreateAreaDtoValidator : AbstractValidator<CreateAreaDto>
    {
        public CreateAreaDtoValidator()
        {
            RuleFor(dto => dto.Name)
                .NotEmpty().WithMessage("Ad boş ola bilməz.")
                .MaximumLength(100).WithMessage("Ad 100 simvoldan çox ola bilməz.");

            RuleFor(dto => dto.widthSize)
                .GreaterThan(0).WithMessage("En ölçüsü 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.lengthtSize)
                .GreaterThan(0).WithMessage("Uzunluq ölçüsü 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.StadiumId)
                .GreaterThan(0).WithMessage("StadiumId 0-dan böyük olmalıdır.");
        }
    }
}
