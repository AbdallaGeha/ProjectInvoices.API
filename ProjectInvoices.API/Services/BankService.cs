using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Base;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;
using System.Data;

namespace ProjectInvoices.API.Services
{
    public class BankService : NamedEntityService<Bank>, IBankService
    {
        private readonly IMapper _mapper;
        public BankService(ApplicationDbContext context, IMapper mapper): base(context)
        {
            _mapper = mapper;
        }
        public async Task AddBankAsync(BankCreationDto bank)
        {
            await EnsureNameUniqueAsync(bank.Name, null);
            var bankEntity = _mapper.Map<Bank>(bank);
            _context.Banks.Add(bankEntity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBankAsync(int id)
        {
            var bank = await GetAsync(id);
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();
        }

        public async Task<BankUpdateGetDto> GetBankByIdAsync(int id)
        {
            var bank = await _context.Banks
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            if (bank == null)
                throw new NotFoundException("Bank not found.");

            return _mapper.Map<BankUpdateGetDto>(bank);
        }

        public async Task<BanksPaginateDto> GetBanksAsync(int page, int pageSize, string? search)
        {
            var query = _context.Banks.AsQueryable();

            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(search.ToLower()));
            }

            var count = await query.CountAsync();
            var banks = await query.Paginate(page, pageSize).ToListAsync();

            var banksDto = _mapper.Map<IEnumerable<BankDto>>(banks);
            var banksPaginateDto = new BanksPaginateDto { Banks = banksDto, TotalRecords = count };

            return banksPaginateDto;
        }

        public async Task UpdateBankAsync(int id, BankUpdateDto bank)
        {
            var bankEntity = await GetAsync(id);
            await EnsureNameUniqueAsync(bank.Name, id);
            _mapper.Map(bank, bankEntity);
            await _context.SaveChangesAsync();
        }
    }
}
