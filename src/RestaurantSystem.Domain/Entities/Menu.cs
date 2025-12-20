using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class MenuDia : AggregateRoot
{
    private MenuDia() { }

    public MenuDia(DateOnly fecha)
    {
        Fecha = fecha;
        Activo = true;
    }

    public DateOnly Fecha { get; private set; }
    public bool Activo { get; private set; }

    public IReadOnlyCollection<MenuDiaItem> Items => _items;
    private readonly List<MenuDiaItem> _items = new();

    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;

    public void AgregarProducto(Guid productoId)
    {
        if (_items.Any(i => i.ProductoId == productoId))
            return;

        _items.Add(new MenuDiaItem(Id, productoId));
    }
}

public class MenuDiaItem : Entity
{
    private MenuDiaItem() { }

    public MenuDiaItem(Guid menuDiaId, Guid productoId)
    {
        Guard.AgainstNull(menuDiaId, "MenuDiaId requerido.");
        Guard.AgainstNull(productoId, "ProductoId requerido.");
        MenuDiaId = menuDiaId;
        ProductoId = productoId;
    }

    public Guid MenuDiaId { get; private set; }
    public Guid ProductoId { get; private set; }
}