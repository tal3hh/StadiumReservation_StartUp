using FluentValidation;
using ServiceLayer.Dtos.StadiumImage;

namespace ServiceLayer.Validations
{
    public class UpdateStadiumImageDtoValidator : AbstractValidator<UpdateStadiumImageDto>
    {
        public UpdateStadiumImageDtoValidator()
        {
            RuleFor(dto => dto.StadiumId)
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
