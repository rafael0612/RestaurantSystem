using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Abstractions.Persistence;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Repositories
{
    public class CuentaRepository : ICuentaRepository
    {
        private readonly RestaurantSystemDbContext _db;
        public CuentaRepository(RestaurantSystemDbContext db) => _db = db;

        public async Task<Cuenta?> GetByIdAsync(Guid cuentaId, bool includeDetails, CancellationToken ct)
        {
            IQueryable<Cuenta> q = _db.Cuentas;

            if (includeDetails)
            {
                // OJO: navegaciones por backing fields (string include)
                //q = q
                //    .Include("_comandas")
                //    .Include("_comandas._detalles")
                //    .Include("_pagos")
                //    .Include("_pagos._detalles")
                //    .Include("_pagos._metodos");
                q = q
                    .Include(c => c.Comandas)
                        .ThenInclude(cmd => cmd.Detalles)
                    .Include(c => c.Pagos)
                        .ThenInclude(p => p.Detalles)
                    .Include(c => c.Pagos)
                        .ThenInclude(p => p.Metodos);
            }

            return await q.FirstOrDefaultAsync(c => c.Id == cuentaId, ct);
        }

        public Task<Cuenta?> GetCuentaActivaPorMesaAsync(Guid mesaId, CancellationToken ct)
        {
            return _db.Cuentas
                .Where(c => c.MesaId == mesaId && (c.Estado == EstadoCuenta.Abierta || c.Estado == EstadoCuenta.PorCobrar))
                .OrderByDescending(c => c.AperturaEn)
                .FirstOrDefaultAsync(ct);
        }

        public Task AddAsync(Cuenta cuenta, CancellationToken ct)
        {
            _db.Cuentas.Add(cuenta);
            return Task.CompletedTask;
        }
    }
}
