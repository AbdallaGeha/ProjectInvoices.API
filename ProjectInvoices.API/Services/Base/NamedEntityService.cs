using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Base;
using ProjectInvoices.API.Exceptions;
using System.Data;

namespace ProjectInvoices.API.Services.Base
{
    public abstract class NamedEntityService<T> where T : NamedEntity
    {
        protected readonly ApplicationDbContext _context;
        protected NamedEntityService(ApplicationDbContext context)
        {
            _context = context;
        }

        protected virtual async Task<T> GetAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);

            if (entity == null)
                throw new NotFoundException($"{typeof(T).Name} not found.");

            return entity;
        }

        protected async Task EnsureNameUniqueAsync(string name, int? id = null)
        {
            var exists = await _context.Set<T>()
                .AnyAsync(b => b.Name == name && b.Id != id);

            if (exists)
                throw new DuplicateNameException($"{typeof(T).Name} name must be unique.");
        }
    }
}
