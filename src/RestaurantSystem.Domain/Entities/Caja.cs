using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class CajaSesion : AggregateRoot
{
    private CajaSesion() { }

    public CajaSesion(Guid usuarioId, decimal montoApertura)
    {
        Guard.AgainstNegative(montoApertura, "MontoApertura no puede ser negativo.");

        UsuarioId = usuarioId;
        MontoApertura = montoApertura;
        AperturaEn = DateTime.UtcNow;
        Estado = EstadoCajaSesion.Abierta;
    }

    public Guid UsuarioId { get; private set; }
    public EstadoCajaSesion Estado { get; private set; }

    public DateTime AperturaEn { get; private set; }
    public decimal MontoApertura { get; private set; }

    public DateTime? CierreEn { get; private set; }
    public decimal? MontoCierreContado { get; private set; }
    public decimal? EfectivoEsperado { get; private set; }
    public decimal? Diferencia { get; private set; }

    public void Cerrar(decimal montoContado, decimal efectivoEsperado, string? motivo)
    {
        if (Estado != EstadoCajaSesion.Abierta)
            throw new DomainException("La caja no está abierta.");

        Guard.AgainstNegative(montoContado, "MontoContado no puede ser negativo.");

        Estado = EstadoCajaSesion.Cerrada;
        CierreEn = DateTime.UtcNow;

        MontoCierreContado = montoContado;
        EfectivoEsperado = efectivoEsperado;
        Diferencia = montoContado - efectivoEsperado;

        // motivo se guarda en auditoría/movimientos en capas superiores
        _ = motivo;
    }

    public void Anular() => Estado = EstadoCajaSesion.Anulada;
}

public class Pago : AggregateRoot
{
    private Pago() { }

    public Pago(Guid cuentaId, Guid cajaSesionId, DateTime pagadoEn)
    {
        CuentaId = cuentaId;
        CajaSesionId = cajaSesionId;
        PagadoEn = pagadoEn;

        Total = 0;
        Anulado = false;
    }

    public Guid CuentaId { get; private set; }
    public Guid CajaSesionId { get; private set; }
    public DateTime PagadoEn { get; private set; }

    public decimal Total { get; private set; }

    public bool Anulado { get; private set; }
    public string? MotivoAnulacion { get; private set; }

    public IReadOnlyCollection<PagoMetodo> Metodos => _metodos;
    private readonly List<PagoMetodo> _metodos = new();

    public IReadOnlyCollection<PagoDetalle> Detalles => _detalles;
    private readonly List<PagoDetalle> _detalles = new();

    public void AgregarMetodo(MetodoPago metodo, decimal monto, string? referencia)
    {
        Guard.AgainstNegative(monto, "Monto de método no puede ser negativo.");
        if (monto == 0) return;

        _metodos.Add(new PagoMetodo(Id, metodo, monto, referencia));
        RecalcularTotal();
    }

    public void AgregarDetalle(Guid comandaDetalleId, int cantidadPagada, decimal precioUnitario)
    {
        Guard.AgainstNonPositive(cantidadPagada, "CantidadPagada debe ser > 0.");
        Guard.AgainstNegative(precioUnitario, "PrecioUnitario inválido.");

        var monto = cantidadPagada * precioUnitario;
        _detalles.Add(new PagoDetalle(Id, comandaDetalleId, cantidadPagada, monto));
        RecalcularTotal();
    }

    public void ValidarConsistencia()
    {
        if (_detalles.Count == 0) throw new DomainException("Pago debe tener al menos un detalle.");
        if (_metodos.Count == 0) throw new DomainException("Pago debe tener al menos un método.");

        var sumMetodos = _metodos.Sum(m => m.Monto);
        var sumDetalles = _detalles.Sum(d => d.MontoAsignado);

        if (sumMetodos != sumDetalles)
            throw new DomainException("La suma de métodos debe igualar el subtotal cobrado.");
    }

    public void Anular(string motivo)
    {
        Guard.AgainstNullOrEmpty(motivo, "Motivo de anulación requerido.");
        Anulado = true;
        MotivoAnulacion = motivo.Trim();
    }

    private void RecalcularTotal()
    {
        // Total se define por los detalles (lo realmente cobrado por ítems)
        Total = _detalles.Sum(d => d.MontoAsignado);
    }
}

public class PagoMetodo : Entity
{
    private PagoMetodo() { }

    internal PagoMetodo(Guid pagoId, MetodoPago metodo, decimal monto, string? referenciaOperacion)
    {
        PagoId = pagoId;
        Metodo = metodo;
        Monto = monto;
        ReferenciaOperacion = string.IsNullOrWhiteSpace(referenciaOperacion) ? null : referenciaOperacion.Trim();
    }

    public Guid PagoId { get; private set; }
    public MetodoPago Metodo { get; private set; }
    public decimal Monto { get; private set; }
    public string? ReferenciaOperacion { get; private set; }
}

public class PagoDetalle : Entity
{
    private PagoDetalle() { }

    internal PagoDetalle(Guid pagoId, Guid comandaDetalleId, int cantidadPagada, decimal montoAsignado)
    {
        PagoId = pagoId;
        ComandaDetalleId = comandaDetalleId;
        CantidadPagada = cantidadPagada;
        MontoAsignado = montoAsignado;
    }

    public Guid PagoId { get; private set; }
    public Guid ComandaDetalleId { get; private set; }
    public int CantidadPagada { get; private set; }
    public decimal MontoAsignado { get; private set; }
}

public class MovimientoCaja : AggregateRoot
{
    private MovimientoCaja() { }

    public MovimientoCaja(Guid cajaSesionId, Guid usuarioId, TipoMovimientoCaja tipo, decimal monto, string motivo)
    {
        Guard.AgainstNullOrEmpty(motivo, "Motivo requerido.");
        Guard.AgainstNegative(monto, "Monto no puede ser negativo.");

        CajaSesionId = cajaSesionId;
        UsuarioId = usuarioId;
        Tipo = tipo;
        Monto = monto;
        Motivo = motivo.Trim();
        Fecha = DateTime.UtcNow;
    }

    public Guid CajaSesionId { get; private set; }
    public Guid UsuarioId { get; private set; }

    public TipoMovimientoCaja Tipo { get; private set; }
    public decimal Monto { get; private set; }
    public string Motivo { get; private set; } = default!;
    public DateTime Fecha { get; private set; }
}