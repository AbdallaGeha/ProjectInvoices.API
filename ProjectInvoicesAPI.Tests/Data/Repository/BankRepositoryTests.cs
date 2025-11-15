using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Data.Repository;
using ProjectInvoices.API.Domain;

namespace ProjectInvoicesAPI.Tests.Data.Repository
{
    /// <summary>
    /// Unit tests for BankRepository
    /// </summary>
    public class BankRepositoryTests
    {
        private BankRepository _repository;
        private ApplicationDbContext _context;
        private DbContextOptions _options;
        
        public BankRepositoryTests()
        {
            //using InMemory DB 
            _options = new DbContextOptionsBuilder().UseInMemoryDatabase("TESTDB").Options;
            _context = new ApplicationDbContext(_options);
            _repository = new BankRepository(_context);
            
            //To make sure DB is deleted and created on every test
            SetUpDB();
        }

        /// <summary>
        /// This method ensures that InMemory Db is deleted and created
        /// </summary>
        private void SetUpDB()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Tests that AddBankAsync method
        /// adds bank to DB
        /// </summary>
        [Fact]
        public async Task AddBankAsync_WhenAddingValidBank_AddBankToDb()
        {
            //Call method under test
            await _repository.AddBankAsync(new Bank { Name = "Test Bank" });

            //Check banks count
            var newContext = new ApplicationDbContext(_options);
            var count = await newContext.Banks.CountAsync();
            Assert.Equal(1, count);

            //Check bank name
            var bank = await newContext.Banks.SingleOrDefaultAsync();
            Assert.Equal("Test Bank", bank?.Name);

            newContext.Dispose();
        }

        /// <summary>
        /// Tests that DeleteBankAsync method
        /// removes bank from DB if bank exists
        /// </summary>
        [Fact]
        public async Task DeleteBankAsync_WhenPassingExistingBank_RemoveBankFromDb()
        {
            //Add a Bank
            var bank = new Bank { Name = "new bank" };
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();

            //Call method under test
            await _repository.DeleteBankAsync(bank);

            //Check bank is removed
            var newContext = new ApplicationDbContext(_options);
            var count = await newContext.Banks.CountAsync();

            Assert.Equal(0, count);

            newContext.Dispose();
        }

        /// <summary>
        /// Tests that GetAllBanksAsync method
        /// returns all banks from DB
        /// </summary>
        [Fact]
        public async Task GetAllBanksAsync_WhenCalled_ReturnsAllBanksFromDb()
        {
            //add dummy Banks
            var newContext = new ApplicationDbContext(_options);
            newContext.Banks.Add(new Bank { Name = "Bank1" });
            newContext.Banks.Add(new Bank { Name = "Bank2" });
            newContext.Banks.Add(new Bank { Name = "Bank3" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var banks = await _repository.GetAllBanksAsync();

            //Check banks count
            Assert.Equal(3, banks.Count);
        }

        /// <summary>
        /// Tests that GetBankByIdAsync method
        /// returns bank if bank with provided id exists
        /// </summary>
        [Fact]
        public async Task GetBankByIdAsync_PassingExistingId_ReturnsBank()
        {
            //add dummy Bank
            var bank = new Bank { Name = "new bank" };
            var newContext = new ApplicationDbContext(_options);
            newContext.Banks.Add(bank);

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var bankInDb = await _repository.GetBankByIdAsync(bank.Id);

            //Check bank            
            Assert.NotNull(bankInDb);
            Assert.Equal(bank.Id, bankInDb.Id);
            Assert.Equal(bank.Name, bankInDb.Name);
        }

        /// <summary>
        /// Tests that GetBankByIdAsync method
        /// returns null if bank with provided id doesn't exist
        /// </summary>
        [Fact]
        public async Task GetBankByIdAsync_PassingNotExistedId_ReturnsNull()
        {
            //add dummy Bank
            var bank = new Bank { Name = "new bank" };
            var newContext = new ApplicationDbContext(_options);
            newContext.Banks.Add(bank);

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var bankInDb = await _repository.GetBankByIdAsync(bank.Id + 1);

            //Check bank is null            
            Assert.Null(bankInDb);
        }

        /// <summary>
        /// Tests that GetBanksAsync method
        /// returns valid result when provided with paging info without search
        /// </summary>
        [Theory]
        [InlineData(1, 3, 3, new[] { "Bank1", "Bank2", "Bank3" })]
        [InlineData(2, 3, 3, new[] { "Bank4", "Bank5", "Bank6" })]
        [InlineData(3, 3, 3, new[] { "Bank7", "Bank8", "Bank9" })]
        [InlineData(4, 3, 1, new[] { "Bank10" })]
        [InlineData(1, 20, 10, new[] { "Bank1", "Bank2", "Bank3", "Bank4", "Bank5", "Bank6", "Bank7", "Bank8", "Bank9", "Bank10" })]
        [InlineData(1, -1, 0, null)]
        [InlineData(1, 0, 0, null)]
        [InlineData(-1, 3, 3, new[] { "Bank1", "Bank2", "Bank3" })]
        [InlineData(0, 3, 3, new[] { "Bank1", "Bank2", "Bank3" })]
        [InlineData(0, 0, 0, new[] { "Bank1", "Bank2", "Bank3" })]
        public async Task GetBanksAsync_PagingInfoWithoutSearch_ReturnsValidResults
            (int page, int pageSize, int expectedcount, string[] expectedBankNames)
        {
            //add dummy Banks
            var newContext = new ApplicationDbContext(_options);
            
            newContext.Banks.Add(new Bank { Name = "Bank1" });
            newContext.Banks.Add(new Bank { Name = "Bank2" });
            newContext.Banks.Add(new Bank { Name = "Bank3" });
            newContext.Banks.Add(new Bank { Name = "Bank4" });
            newContext.Banks.Add(new Bank { Name = "Bank5" });
            newContext.Banks.Add(new Bank { Name = "Bank6" });
            newContext.Banks.Add(new Bank { Name = "Bank7" });
            newContext.Banks.Add(new Bank { Name = "Bank8" });
            newContext.Banks.Add(new Bank { Name = "Bank9" });
            newContext.Banks.Add(new Bank { Name = "Bank10" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var banks = await _repository.GetBanksAsync(page, pageSize, null);

            //Check banks
            banks = banks.OrderBy(x => x.CreateDate).ToList();
            Assert.Equal(expectedcount, banks.Count);
            for(int i = 0; i < banks.Count; i++)
            {
                Assert.Equal(expectedBankNames[i], banks[i].Name);
            }
        }

        /// <summary>
        /// Tests that GetBanksAsync method
        /// returns valid result when provided with paging info with search
        /// </summary>
        [Theory]
        [InlineData(1, 3, "Bank", 3, new[] { "Bank one", "Bank two", "Bank three" })]
        [InlineData(1, 3, "Bank t", 3, new[] { "Bank two", "Bank three", "Bank ten" })]
        [InlineData(1, 3, "Bank s", 2, new[] { "Bank six", "Bank seven" })]
        [InlineData(2, 3, "Bank s", 0, new[] { "Bank six", "Bank seven" })]
        public async Task GetBanksAsync_PagingInfoWithSearch_ReturnsValidResults
            (int page, int pageSize, string search, int expectedcount, string[] expectedBankNames)
        {
            //add dummy Banks
            var newContext = new ApplicationDbContext(_options);

            newContext.Banks.Add(new Bank { Name = "Bank one" });
            newContext.Banks.Add(new Bank { Name = "Bank two" });
            newContext.Banks.Add(new Bank { Name = "Bank three" });
            newContext.Banks.Add(new Bank { Name = "Bank four" });
            newContext.Banks.Add(new Bank { Name = "Bank five" });
            newContext.Banks.Add(new Bank { Name = "Bank six" });
            newContext.Banks.Add(new Bank { Name = "Bank seven" });
            newContext.Banks.Add(new Bank { Name = "Bank eight" });
            newContext.Banks.Add(new Bank { Name = "Bank nine" });
            newContext.Banks.Add(new Bank { Name = "Bank ten" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var banks = await _repository.GetBanksAsync(page, pageSize, search);

            //Check banks
            banks = banks.OrderBy(x => x.CreateDate).ToList();
            Assert.Equal(expectedcount, banks.Count);
            for (int i = 0; i < banks.Count; i++)
            {
                Assert.Equal(expectedBankNames[i], banks[i].Name);
            }
        }

        /// <summary>
        /// Tests that GetTotalRecords method
        /// returns valid result when provided with search value
        /// </summary>
        [Theory]
        [InlineData(null, 10)]
        [InlineData("B", 10)]
        [InlineData("ban", 10)]
        [InlineData("Ba", 10)]
        [InlineData("ank", 10)]
        [InlineData("ank t", 3)]
        [InlineData("Bank t", 3)]
        [InlineData("two", 1)]
        public async Task GetTotalRecords_PassingSearch_ReturnsCount(string? search, int expectedCount)
        {
            //add dummy Banks
            var newContext = new ApplicationDbContext(_options);

            newContext.Banks.Add(new Bank { Name = "Bank one" });
            newContext.Banks.Add(new Bank { Name = "Bank two" });
            newContext.Banks.Add(new Bank { Name = "Bank three" });
            newContext.Banks.Add(new Bank { Name = "Bank four" });
            newContext.Banks.Add(new Bank { Name = "Bank five" });
            newContext.Banks.Add(new Bank { Name = "Bank six" });
            newContext.Banks.Add(new Bank { Name = "Bank seven" });
            newContext.Banks.Add(new Bank { Name = "Bank eight" });
            newContext.Banks.Add(new Bank { Name = "Bank nine" });
            newContext.Banks.Add(new Bank { Name = "Bank ten" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var count = await _repository.GetTotalRecords(search);

            //Check count
            Assert.Equal(expectedCount, count);
        }

        /// <summary>
        /// Tests that IsExistingBankAsync method
        /// returns valid result when provided with bank name
        /// </summary>
        [Theory]
        [InlineData("Bank one", true)]
        [InlineData("Bank1", false)]
        [InlineData("", false)]
        [InlineData("Bank five", true)]
        public async Task IsExistingBankAsync_PassingBankName_ReturnsValidResult(string name, bool expectedResult)
        {
            //add dummy Banks
            var newContext = new ApplicationDbContext(_options);

            newContext.Banks.Add(new Bank { Name = "Bank one" });
            newContext.Banks.Add(new Bank { Name = "Bank two" });
            newContext.Banks.Add(new Bank { Name = "Bank three" });
            newContext.Banks.Add(new Bank { Name = "Bank four" });
            newContext.Banks.Add(new Bank { Name = "Bank five" });
            newContext.Banks.Add(new Bank { Name = "Bank six" });
            newContext.Banks.Add(new Bank { Name = "Bank seven" });
            newContext.Banks.Add(new Bank { Name = "Bank eight" });
            newContext.Banks.Add(new Bank { Name = "Bank nine" });
            newContext.Banks.Add(new Bank { Name = "Bank ten" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //Call method under test
            var isExisted = await _repository.IsExistingBankAsync(name);

            //Check value
            Assert.Equal(expectedResult, isExisted);
        }

        /// <summary>
        /// Tests that UpdateBankAsync method
        /// saves changes in context to DB
        /// </summary>
        [Fact]
        public async Task UpdateBankAsync_WhenCalled_SaveContextChanges()
        {
            //add dummy Bank to DB
            var newContext = new ApplicationDbContext(_options);

            newContext.Banks.Add(new Bank { Name = "Bank test" });

            await newContext.SaveChangesAsync();
            newContext.Dispose();

            //get bank using main context
            var bank = _context.Banks.First();

            //update bank name
            bank.Name = "Bank test update";

            //call the method under test
            await _repository.UpdateBankAsync();

            //get bank using new context
            newContext = new ApplicationDbContext(_options);
            var bankToCheck = newContext.Banks.First();

            //Check Bank is updated
            Assert.Equal("Bank test update", bankToCheck.Name);
        }
    }
}
