using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class Comanda : AggregateRoot
{
    private Comanda() { }

    public Comanda(Guid cuentaId, Guid creadaPorUsuarioId, int numeroSecuencia)
    {
        Guard.AgainstNonPositive(numeroSecuencia, "NumeroSecuencia debe ser >= 1.");

        CuentaId = cuentaId;
        CreadaPorUsuarioId = creadaPorUsuarioId;
        NumeroSecuencia = numeroSecuencia;

        Estado = EstadoComanda.Abierta;
        CreadaEn = DateTime.UtcNow;
    }

    public Guid CuentaId { get; private set; }
    public EstadoComanda Estado { get; private set; }
    public DateTime CreadaEn { get; private set; }
    public Guid CreadaPorUsuarioId { get; private set; }
    public int NumeroSecuencia { get; private set; }

    public IReadOnlyCollection<ComandaDetalle> Detalles => _detalles.AsReadOnly();
    private readonly List<ComandaDetalle> _detalles = new();

    public ComandaDetalle AgregarItem(Guid productoId, int cantidad, decimal precioUnitario, decimal costoUnitarioEstandar, string? observacion)
    {
        if (Estado != EstadoComanda.Abierta)
            throw new DomainException("Solo se puede agregar items a una comanda Abierta.");

        Guard.AgainstNonPositive(cantidad, "Cantidad debe ser mayor a 0.");
        Guard.AgainstNegative(precioUnitario, "PrecioUnitario inválido.");
        Guard.AgainstNegative(costoUnitarioEstandar, "CostoUnitarioEstandar inválido.");

        var det = new ComandaDetalle(
            comandaId: Id,
            productoId: productoId,
            cantidad: cantidad,
            precioUnitario: precioUnitario,
            costoUnitarioEstandar: costoUnitarioEstandar,
            observacion: observacion
        );
        _detalles.Add(det);
        return det;
    }

    public void EnviarACocina()
    {
        if (Estado != EstadoComanda.Abierta)
            throw new DomainException("La comanda no está Abierta.");

        if (_detalles.Count == 0)
            throw new DomainException("No se puede enviar una comanda vacía.");

        Estado = EstadoComanda.EnCocina;

        foreach (var d in _detalles.Where(x => !x.Anulado))
            d.MarcarPendiente();
    }

    public void MarcarListoSiCorresponde()
    {
        // regla simple: si todos están Listo/Entregado => comanda Listo
        var activos = _detalles.Where(d => !d.Anulado).ToList();
        if (activos.Count == 0) return;

        if (activos.All(d => d.EstadoCocina is EstadoCocinaItem.Listo or EstadoCocinaItem.Entregado))
            Estado = EstadoComanda.Listo;
        else if (Estado is EstadoComanda.Listo)
            Estado = EstadoComanda.EnCocina;
    }

    public decimal Total => _detalles.Where(d => !d.Anulado).Sum(d => d.TotalLinea);
}

public class ComandaDetalle : Entity
{
    private ComandaDetalle() { }

    internal ComandaDetalle(Guid comandaId, Guid productoId, int cantidad, decimal precioUnitario, decimal costoUnitarioEstandar, string? observacion)
    {
        ComandaId = comandaId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        CostoUnitarioEstandar = costoUnitarioEstandar;
        Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();

        EstadoCocina = EstadoCocinaItem.Pendiente;
        CantidadPagada = 0;
        Anulado = false;
    }

    public Guid ComandaId { get; private set; }
    public Guid ProductoId { get; private set; }

    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal CostoUnitarioEstandar { get; private set; }

    public string? Observacion { get; private set; }
    public EstadoCocinaItem EstadoCocina { get; private set; }

    public int CantidadPagada { get; private set; }
    public bool Anulado { get; private set; }

    public decimal TotalLinea => Cantidad * PrecioUnitario;
    public decimal CostoLinea => Cantidad * CostoUnitarioEstandar;

    public int CantidadPendientePago => Math.Max(0, Cantidad - CantidadPagada);

    internal void MarcarPendiente()
    {
        if (!Anulado) EstadoCocina = EstadoCocinaItem.Pendiente;
    }

    public void CambiarEstadoCocina(EstadoCocinaItem nuevoEstado)
    {
        if (Anulado) throw new DomainException("No se puede cambiar estado de un item anulado.");
        EstadoCocina = nuevoEstado;
    }

    public void AplicarPago(int cantidadPagada)
    {
        Guard.AgainstNonPositive(cantidadPagada, "CantidadPagada debe ser > 0.");

        if (cantidadPagada > CantidadPendientePago)
            throw new DomainException("No se puede pagar más de lo pendiente del ítem.");

        CantidadPagada += cantidadPagada;
    }

    public void Anular(string motivo)
    {
        if (CantidadPagada > 0)
            throw new DomainException("No se puede anular un ítem con pagos registrados.");

        Anulado = true;
        EstadoCocina = EstadoCocinaItem.Anulado;
        Observacion = $"ANULADO: {motivo}";
    }
}