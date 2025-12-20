namespace RestaurantSystem.Domain.Enums;

public enum RolUsuario
{
    Admin = 1,
    Mesero = 2,
    Cocinero = 3,
    Caja = 4
}

public enum TipoCuenta
{
    Salon = 1,
    Llevar = 2
}

public enum EstadoCuenta
{
    Abierta = 1,
    PorCobrar = 2,
    Cerrada = 3,
    Anulada = 4
}

public enum EstadoMesa
{
    Libre = 1,
    Ocupada = 2,
    PorCobrar = 3
}

public enum EstadoComanda
{
    Abierta = 1,
    EnCocina = 2,
    Listo = 3,
    Entregado = 4,
    Cerrada = 5,
    Anulada = 6
}

public enum EstadoCocinaItem
{
    Pendiente = 1,
    Preparando = 2,
    Listo = 3,
    Entregado = 4,
    Anulado = 5
}

public enum MetodoPago
{
    Efectivo = 1,
    Yape = 2,
    Plin = 3
}

public enum EstadoCajaSesion
{
    Abierta = 1,
    Cerrada = 2,
    Anulada = 3
}

public enum TipoMovimientoCaja
{
    Ingreso = 1,
    Egreso = 2,
    Ajuste = 3,
    Anulacion = 4
}