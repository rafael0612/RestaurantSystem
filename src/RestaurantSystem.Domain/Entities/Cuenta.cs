using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class Cuenta : AggregateRoot
{
    private Cuenta() { }

    public Cuenta(TipoCuenta tipo, Guid? mesaId, Guid creadaPorUsuarioId, string? observacionGeneral = null)
    {
        if (tipo == TipoCuenta.Salon && mesaId is null)
            throw new DomainException("Cuenta salón requiere MesaId.");

        Tipo = tipo;
        MesaId = mesaId;
        CreadaPorUsuarioId = creadaPorUsuarioId;
        ObservacionGeneral = string.IsNullOrWhiteSpace(observacionGeneral) ? null : observacionGeneral.Trim();

        Estado = EstadoCuenta.Abierta;
        AperturaEn = DateTime.UtcNow;
    }

    public TipoCuenta Tipo { get; private set; }
    public EstadoCuenta Estado { get; private set; }

    public Guid? MesaId { get; private set; }
    public Guid CreadaPorUsuarioId { get; private set; }

    public DateTime AperturaEn { get; private set; }
    public DateTime? CierreEn { get; private set; }

    public string? ObservacionGeneral { get; private set; }

    public IReadOnlyCollection<Comanda> Comandas => _comandas;
    private readonly List<Comanda> _comandas = new();

    public IReadOnlyCollection<Pago> Pagos => _pagos;
    private readonly List<Pago> _pagos = new();

    public void SolicitarCuenta()
    {
        if (Estado != EstadoCuenta.Abierta) return;
        Estado = EstadoCuenta.PorCobrar;
    }

    public void ReabrirDesdePorCobrar()
    {
        if (Estado == EstadoCuenta.PorCobrar)
            Estado = EstadoCuenta.Abierta;
    }

    public void Cerrar()
    {
        if (Estado == EstadoCuenta.Cerrada) return;
        Estado = EstadoCuenta.Cerrada;
        CierreEn = DateTime.UtcNow;
    }

    public void Anular(string motivo)
    {
        if (Estado == EstadoCuenta.Cerrada)
            throw new DomainException("No se puede anular una cuenta cerrada.");
        Estado = EstadoCuenta.Anulada;
        ObservacionGeneral = $"ANULADA: {motivo}";
        CierreEn = DateTime.UtcNow;
    }

    // Helpers de dominio (cálculos)
    public decimal TotalConsumido =>
        _comandas.SelectMany(c => c.Detalles).Where(d => !d.Anulado).Sum(d => d.TotalLinea);

    public decimal TotalPagado =>
        _pagos.Where(p => !p.Anulado).Sum(p => p.Total);

    public decimal SaldoPendiente => TotalConsumido - TotalPagado;

    // Agregados internos
    internal void AddComanda(Comanda comanda) => _comandas.Add(comanda);
    internal void AddPago(Pago pago) => _pagos.Add(pago);
}