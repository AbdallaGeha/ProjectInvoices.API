using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Base;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;

namespace ProjectInvoices.API.Services
{
    public class SupplierService : NamedEntityService<Supplier>, ISupplierService
    {
        private readonly IMapper _mapper;
        public SupplierService(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }
        public async Task AddSupplierAsync(SupplierCreationDto Supplier)
        {
            await EnsureNameUniqueAsync(Supplier.Name, null);
            var SupplierEntity = _mapper.Map<Supplier>(Supplier);
            _context.Suppliers.Add(SupplierEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var Supplier = await GetAsync(id);
            _context.Suppliers.Remove(Supplier);
            await _context.SaveChangesAsync();
        }

        public async Task<SupplierUpdateGetDto> GetSupplierByIdAsync(int id)
        {
            var Supplier = await _context.Suppliers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (Supplier == null)
                throw new NotFoundException("Supplier not found.");

            return _mapper.Map<SupplierUpdateGetDto>(Supplier);
        }

        public async Task<SuppliersPaginateDto> GetSuppliersAsync(int page, int pageSize, string? search)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search.ToLower())) ||
                (x.Email != null && x.Email.ToLower().Contains(search.ToLower())) ||
                (x.Address != null && x.Address.ToLower().Contains(search.ToLower()))
                );
            }

            var count = await query.CountAsync();
            var Suppliers = await query.Paginate(page, pageSize).ToListAsync();

            var SuppliersDto = _mapper.Map<IEnumerable<SupplierDto>>(Suppliers);
            var SuppliersPaginateDto = new SuppliersPaginateDto { Suppliers = SuppliersDto, TotalRecords = count };

            return SuppliersPaginateDto;
        }

        public async Task UpdateSupplierAsync(int id, SupplierUpdateDto Supplier)
        {
            var SupplierEntity = await GetAsync(id);
            await EnsureNameUniqueAsync(Supplier.Name, id);
            _mapper.Map(Supplier, SupplierEntity);
            await _context.SaveChangesAsync();
        }
    }
}
