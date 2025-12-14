using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Mapping;
using ProjectInvoices.API.Services;
using System.Data;

namespace ProjectInvoicesAPI.Tests.Services
{
    public class BankServiceTests
    {
        private readonly IMapper _mapper;

        public BankServiceTests()
        {
            _mapper = CreateMapper();
        }

        // -----------------------------
        // AddBankAsync
        // -----------------------------

        [Fact]
        public async Task AddBankAsync_Persists_Bank()
        {
            var dbName = Guid.NewGuid().ToString();

            // Arrange 
            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await service.AddBankAsync(new BankCreationDto
                {
                    Name = "Bank A"
                });
            }

            // Assert 
            using (var context = CreateContext(dbName))
            {
                var bank = await context.Banks.SingleOrDefaultAsync();

                Assert.NotNull(bank);
                Assert.Equal("Bank A", bank!.Name);
            }
        }

        [Fact]
        public async Task AddBankAsync_Throws_When_Name_Duplicate()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                context.Banks.Add(new Bank { Name = "Bank A" });
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await Assert.ThrowsAsync<DuplicateNameException>(() =>
                    service.AddBankAsync(new BankCreationDto { Name = "Bank A" }));
            }
        }

        // -----------------------------
        // DeleteBankAsync
        // -----------------------------

        [Fact]
        public async Task DeleteBankAsync_Removes_Bank()
        {
            var dbName = Guid.NewGuid().ToString();
            int bankId;

            using (var context = CreateContext(dbName))
            {
                var bank = new Bank { Name = "Bank A" };
                context.Banks.Add(bank);
                await context.SaveChangesAsync();
                bankId = bank.Id;
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);
                await service.DeleteBankAsync(bankId);
            }

            using (var context = CreateContext(dbName))
            {
                Assert.Empty(context.Banks);
            }
        }

        [Fact]
        public async Task DeleteBankAsync_Throws_When_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await Assert.ThrowsAsync<NotFoundException>(() =>
                    service.DeleteBankAsync(999));
            }
        }

        // -----------------------------
        // GetBankByIdAsync
        // -----------------------------

        [Fact]
        public async Task GetBankByIdAsync_Returns_Bank()
        {
            var dbName = Guid.NewGuid().ToString();
            int bankId;

            using (var context = CreateContext(dbName))
            {
                var bank = new Bank { Name = "Bank A" };
                context.Banks.Add(bank);
                await context.SaveChangesAsync();
                bankId = bank.Id;
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                var result = await service.GetBankByIdAsync(bankId);

                Assert.Equal("Bank A", result.Name);
            }
        }

        [Fact]
        public async Task GetBankByIdAsync_Throws_When_NotFound()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await Assert.ThrowsAsync<NotFoundException>(() =>
                    service.GetBankByIdAsync(1));
            }
        }

        // -----------------------------
        // UpdateBankAsync
        // -----------------------------

        [Fact]
        public async Task UpdateBankAsync_Persists_Changes()
        {
            var dbName = Guid.NewGuid().ToString();
            int bankId;

            using (var context = CreateContext(dbName))
            {
                var bank = new Bank { Name = "Old Name" };
                context.Banks.Add(bank);
                await context.SaveChangesAsync();
                bankId = bank.Id;
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await service.UpdateBankAsync(bankId, new BankUpdateDto
                {
                    Name = "New Name"
                });
            }

            using (var context = CreateContext(dbName))
            {
                var bank = await context.Banks.SingleAsync();
                Assert.Equal("New Name", bank.Name);
            }
        }

        [Fact]
        public async Task UpdateBankAsync_Throws_When_Name_Duplicate()
        {
            var dbName = Guid.NewGuid().ToString();
            int bankBId;

            using (var context = CreateContext(dbName))
            {
                context.Banks.AddRange(
                    new Bank { Name = "Bank A" },
                    new Bank { Name = "Bank B" }
                );
                await context.SaveChangesAsync();
                bankBId = context.Banks.Single(b => b.Name == "Bank B").Id;
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                await Assert.ThrowsAsync<DuplicateNameException>(() =>
                    service.UpdateBankAsync(bankBId, new BankUpdateDto
                    {
                        Name = "Bank A"
                    }));
            }
        }

        // -----------------------------
        // GetBanksAsync
        // -----------------------------

        [Fact]
        public async Task GetBanksAsync_Returns_Paginated_Data()
        {
            var dbName = Guid.NewGuid().ToString();

            using (var context = CreateContext(dbName))
            {
                context.Banks.AddRange(
                    new Bank { Name = "A" },
                    new Bank { Name = "B" },
                    new Bank { Name = "C" }
                );
                await context.SaveChangesAsync();
            }

            using (var context = CreateContext(dbName))
            {
                var service = new BankService(context, _mapper);

                var result = await service.GetBanksAsync(1, 2, null);

                Assert.Equal(3, result.TotalRecords);
                Assert.Equal(2, result.Banks.Count());
            }
        }



        //--------------------------------------------------------------
        // Helper methods
        //--------------------------------------------------------------

        private static ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }
        private static IMapper CreateMapper()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            return mapper;
        }
    }
}
