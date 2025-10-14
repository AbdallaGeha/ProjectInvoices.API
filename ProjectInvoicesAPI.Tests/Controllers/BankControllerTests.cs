using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaklaNew.API.Controllers;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Dtos;
using TaklaNew.API.Mapping;
using TaklaNewAPI.Tests.Helpers;

namespace TaklaNewAPI.Tests.Controllers
{
    /// <summary>
    /// Unit tests for BankController
    /// Ensures correct behavior of API endpoints for managing banks
    /// </summary>
    public class BankControllerTests
    {
        private BankController _controller;
        private Mock<IBankRepository> _bankRepository;
        private IMapper _mapper;
        public BankControllerTests()
        {
            _bankRepository = new Mock<IBankRepository>();
            _controller = new BankController(_bankRepository.Object, GetMapper());
            _mapper = GetMapper();
        }

        /// <summary>
        /// Get AutoMapper IMapper object based on AutoMapper profile
        /// </summary>
        private IMapper GetMapper()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            return mapper;
        }

        /// <summary>
        /// Tests that GetAll action method
        /// returns Ok result containing the correct list of BankDto object
        /// </summary>
        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkResultWithBanksDto()
        {
            //set up
            var banks = new List<Bank>
            {
                new Bank { Id = 1, Name = "Test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new Bank { Id = 2, Name = "Test2", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new Bank { Id = 3, Name = "Test3", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new Bank { Id = 4, Name = "Test4", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now }
            };
            _bankRepository.Setup(x => x.GetAllBanksAsync()).ReturnsAsync(banks);

            var bankDtos = _mapper.Map<List<BankDto>>(banks);

            //call method under test
            var actionResult = await _controller.GetAll();

            //Assertion
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var listOfDtos = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<List<BankDto>>(listOfDtos);

            Assert.Equal(bankDtos.Count, listOfDtos.Count);

            for (int i = 0; i < bankDtos.Count; i++)
            {
                Assert.Equal(bankDtos[i].Id, listOfDtos[i].Id);
                Assert.Equal(bankDtos[i].Name, listOfDtos[i].Name);
                Assert.Equal(bankDtos[i].CreateDate, listOfDtos[i].CreateDate);
                Assert.Equal(bankDtos[i].LastModifiedDate, listOfDtos[i].LastModifiedDate);
            }
        }

        /// <summary>
        /// Tests that Get action method
        /// calls corresponding repository methods
        /// </summary>
        [Theory]
        [InlineData(1, 1, null)]
        [InlineData(2, 2, "test")]
        public async Task Get_WhenCalled_CallRepository(int pageNumber, int pageSize, string? search)
        {
            //call method under test
            await _controller.Get(pageNumber, pageSize, search);

            //Assertion
            _bankRepository.Verify(x => x.GetTotalRecords(search), Times.Once);
            _bankRepository.Verify(x => x.GetBanksAsync(pageNumber, pageSize, search), Times.Once);
        }

        /// <summary>
        /// Tests that Get action method
        /// returns Ok result containing the correct list of BankDto object
        /// </summary>
        [Theory]
        [InlineData(2, 2, "test")]
        public async Task Get_WhenCalled_ReturnsOkResultWithBanksPaginateDto(int pageNumber, int pageSize, string? search)
        {
            //Arrangements
            var banks = new List<Bank>
            {
                new Bank { Id = 1, Name = "Test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new Bank { Id = 2, Name = "Test2", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
            };

            _bankRepository.Setup(x => x.GetTotalRecords(search)).ReturnsAsync(2);
            _bankRepository.Setup(x => x.GetBanksAsync(pageNumber, pageSize, search)).ReturnsAsync(banks);

            //call method under test
            var actionResult = await _controller.Get(pageNumber, pageSize, search);

            //Assertion
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var content = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<BanksPaginateDto>(content);

            Assert.Equal(banks.Count, content.TotalRecords);

            var bankDtos = _mapper.Map<List<BankDto>>(banks);

            var listOfBanksFromContent = content.Banks.ToList();
            for (int i = 0; i < bankDtos.Count; i++)
            {
                Assert.Equal(bankDtos[i].Id, listOfBanksFromContent[i].Id);
                Assert.Equal(bankDtos[i].Name, listOfBanksFromContent[i].Name);
                Assert.Equal(bankDtos[i].CreateDate, listOfBanksFromContent[i].CreateDate);
                Assert.Equal(bankDtos[i].LastModifiedDate, listOfBanksFromContent[i].LastModifiedDate);
            }
        }

        /// <summary>
        /// Tests that Get action method
        /// returns NotFound result if the bank doesn't exist
        /// </summary>
        [Fact]
        public async Task Get_NotExistedId_ReturnsNotFoundResult()
        {
            //Arrangements
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync((Bank?)null);

            //call method under test
            var actionResult = await _controller.Get(1);

            //Assertion
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        /// <summary>
        /// Tests that Get action method
        /// returns Ok result containing the correct BankUpdateGetDto object
        /// </summary>
        [Fact]
        public async Task Get_ExistedId_ReturnsOkResultWithBankUpdateGetDto()
        {
            //Arrangements
            var bank = new Bank { Id = 1, Name = "Test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);

            //call method under test
            var actionResult = await _controller.Get(1);

            //Assertion
            Assert.IsType<OkObjectResult>(actionResult.Result);

            var content = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<BankUpdateGetDto>(content);

            var bankDto = _mapper.Map<BankUpdateGetDto>(bank);

            Assert.Equal(bankDto.Id, content.Id);
            Assert.Equal(bankDto.Name, content.Name);
        }

        /// <summary>
        /// Tests that Post action method
        /// returns BadRequest result if Bank name already exists
        /// </summary>
        [Fact]
        public async Task Post_ExistingBank_ReturnsBadRequest()
        {
            //Arrangements
            var bankCreationDto = new BankCreationDto { Name = "test" };
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(true);

            //call method under test
            var actionResult = await _controller.Post(bankCreationDto);

            //Assertion
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        /// <summary>
        /// Tests that Post action method
        /// calls corresponding repository method if Bank name doesn't exist
        /// </summary>
        [Fact]
        public async Task Post_NotExistedBank_CallingAddBankAsync()
        {
            //Arrangements
            var bankCreationDto = new BankCreationDto { Name = "test" };
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(false);

            //call method under test
            var actionResult = await _controller.Post(bankCreationDto);

            //Assertion
            _bankRepository.Verify(x => x.AddBankAsync(It.IsAny<Bank>()), Times.Once);
        }

        /// <summary>
        /// Tests that Post action method
        /// returns NoContent result if Bank name doesn't exist
        /// </summary>
        [Fact]
        public async Task Post_NotExistedBank_ReturnsNoContent()
        {
            //Arrangements
            var bankCreationDto = new BankCreationDto { Name = "test" };
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(false);

            //call method under test
            var actionResult = await _controller.Post(bankCreationDto);

            //Assertion
            Assert.IsType<NoContentResult>(actionResult);
        }

        /// <summary>
        /// Tests that Put action method
        /// returns NotFound result if Bank doesn't exist
        /// </summary>
        [Fact]
        public async Task Put_NotExistedId_ReturnsNotFound()
        {
            //Arrangements
            var bankUpdateDto = new BankUpdateDto { Name  = "test" };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync((Bank?)null);

            //call method under test
            var actionResult = await _controller.Put(1, bankUpdateDto);

            //Assertion
            Assert.IsType<NotFoundResult>(actionResult);
        }

        /// <summary>
        /// Tests that Put action method
        /// returns BadRequest result if Bank name exists 
        /// </summary>
        [Fact]
        public async Task Put_ExistingBankName_ReturnsBadRequest()
        {
            //Arrangements
            var bankUpdateDto = new BankUpdateDto { Name = "test" };
            var bank = new Bank { Id = 1, Name = "Test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(true);
            
            //call method under test
            var actionResult = await _controller.Put(1, bankUpdateDto);

            //Assertion
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        /// <summary>
        /// Tests that Put action method
        /// returns NoContent result if Bank name and dto bank name are same 
        /// </summary>
        [Fact]
        public async Task Put_ExistingBankNameTrueSameName_ReturnsNoContent()
        {
            //Arrangements
            var bankUpdateDto = new BankUpdateDto { Name = "test" };
            var bank = new Bank { Id = 1, Name = "test", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(true);

            //call method under test
            var actionResult = await _controller.Put(1, bankUpdateDto);

            //Assertion
            Assert.IsType<NoContentResult>(actionResult);
        }

        /// <summary>
        /// Tests that Put action method
        /// calls corresponding repository method if the input is valid
        /// </summary>
        [Fact]
        public async Task Put_ValidInput_CallUpdateBankAsync()
        {
            //Arrangements
            var bankUpdateDto = new BankUpdateDto { Name = "test" };
            var bank = new Bank { Id = 1, Name = "test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(false);

            //call method under test
            await _controller.Put(1, bankUpdateDto);

            //Assertion
            _bankRepository.Verify(x => x.UpdateBankAsync(), Times.Once);
        }

        /// <summary>
        /// Tests that Put action method
        /// returns NoContent result if the input is valid
        /// </summary>
        [Fact]
        public async Task Put_ValidInput_ReturnsNoContent()
        {
            //Arrangements
            var bankUpdateDto = new BankUpdateDto { Name = "test" };
            var bank = new Bank { Id = 1, Name = "test1", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);
            _bankRepository.Setup(x => x.IsExistingBankAsync(It.IsAny<string>())).ReturnsAsync(false);

            //call method under test
            var actionResult =  await _controller.Put(1, bankUpdateDto);

            //Assertion
            Assert.IsType<NoContentResult>(actionResult);
        }

        /// <summary>
        /// Tests that Delete action method
        /// returns NotFound result if Bank doesn't exist
        /// </summary>
        [Fact]
        public async Task Delete_NotExistedId_ReturnsNotFound()
        {
            //Arrangements
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync((Bank?)null);

            //call method under test
            var actionResult = await _controller.Delete(1);

            //Assertion
            Assert.IsType<NotFoundResult>(actionResult);
        }

        /// <summary>
        /// Tests that Delete action method
        /// returns NotContent result if the input is valid
        /// </summary>
        [Fact]
        public async Task Delete_ValidInput_ReturnsNoContent()
        {
            //Arrangements
            var bank = new Bank { Id = 1, Name = "test", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);

            //call method under test
            var actionResult = await _controller.Delete(1);

            //Assertion
            Assert.IsType<NoContentResult>(actionResult);
        }

        /// <summary>
        /// Tests that Delete action method
        /// calls corresponding repository method if the input is valid
        /// </summary>
        [Fact]
        public async Task Delete_ValidInput_CallDeleteBankAsync()
        {
            //Arrangements
            var bank = new Bank { Id = 1, Name = "test", CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now };
            _bankRepository.Setup(x => x.GetBankByIdAsync(It.IsAny<int>())).ReturnsAsync(bank);

            //call method under test
            await _controller.Delete(1);

            //Assertion
            _bankRepository.Verify(x => x.DeleteBankAsync(bank), Times.Once);
        }

    }
}
