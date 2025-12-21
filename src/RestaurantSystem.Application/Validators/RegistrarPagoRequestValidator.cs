using FluentValidation;
using RestaurantSystem.Shared.Contracts;

namespace RestaurantSystem.Application.Validators
{
    public class RegistrarPagoRequestValidator : AbstractValidator<RegistrarPagoRequest>
    {
        public RegistrarPagoRequestValidator()
        {
            RuleFor(x => x.CuentaId).NotEmpty();
            RuleFor(x => x.Detalles).NotNull().Must(x => x.Count > 0).WithMessage("Debe seleccionar al menos un ítem.");
            RuleFor(x => x.Metodos).NotNull().Must(x => x.Count > 0).WithMessage("Debe registrar al menos un método de pago.");

            RuleForEach(x => x.Detalles).ChildRules(d =>
            {
                d.RuleFor(x => x.ComandaDetalleId).NotEmpty();
                d.RuleFor(x => x.CantidadPagada).GreaterThan(0).LessThanOrEqualTo(50);
            });

            RuleForEach(x => x.Metodos).ChildRules(m =>
            {
                m.RuleFor(x => x.Monto).GreaterThanOrEqualTo(0);
                m.RuleFor(x => x.ReferenciaOperacion).MaximumLength(60);
            });
        }
    }
}
