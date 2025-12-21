namespace RestaurantSystem.Application.Abstractions.Printing
{
    public interface IPrinterService
    {
        Task PrintComandaAsync(Guid comandaId, CancellationToken ct);
        Task PrintTicketPagoAsync(Guid pagoId, CancellationToken ct);
    }
}
