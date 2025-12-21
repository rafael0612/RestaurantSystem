using FluentValidation;
using RestaurantSystem.Application.Dtos;

namespace RestaurantSystem.Application.Validators
{
    public class AgregarItemRequestValidator : AbstractValidator<AgregarItemRequest>
    {
        public AgregarItemRequestValidator()
        {
            RuleFor(x => x.ProductoId).NotEmpty();
            RuleFor(x => x.Cantidad).GreaterThan(0).LessThanOrEqualTo(50);
            RuleFor(x => x.Observacion).MaximumLength(250);
        }
    }
}
