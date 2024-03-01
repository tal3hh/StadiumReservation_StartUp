using FluentValidation;
using ServiceLayer.Dtos.StadiumImage;

namespace ServiceLayer.Validations
{
    public class CreateStadiumImageDtoValidator : AbstractValidator<CreateStadiumImageDto>
    {
        public CreateStadiumImageDtoValidator()
        {
            RuleFor(dto => dto.s)
                .GreaterThan(0).WithMessage("Stadyum IDsi 0-dan böyük olmalıdır.");

            RuleFor(dto => dto.Path)
                .NotNull().WithMessage("Path boş ola bilməz.")
                .NotEmpty().WithMessage("Path boş ola bilməz.")
                .MaximumLength(500).WithMessage("Path 500 simvoldan çox ola bilməz.");
        }
    }
}
