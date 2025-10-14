using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaklaNew.API.Controllers;
using TaklaNew.API.Data.Repository;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Dtos;
using TaklaNew.API.Mapping;
using TaklaNew.API.Services;
using TaklaNewAPI.Tests.Helpers;

namespace TaklaNewAPI.Tests.Controllers
{
    public class ProjectInvoiceControllerTests
    {
        private ProjectInvoiceController _controller;
        private Mock<IProjectInvoiceService> _projectInvoiceService;
        private Mock<IProjectInvoiceQueries> _projectInvoiceQueries;
        private IMapper _mapper;
        public ProjectInvoiceControllerTests()
        {
            _projectInvoiceService = new Mock<IProjectInvoiceService>();
            _projectInvoiceQueries = new Mock<IProjectInvoiceQueries>();
            _mapper = GetMapper();
            _controller = new ProjectInvoiceController(_projectInvoiceService.Object, _projectInvoiceQueries.Object, _mapper);
        }

        private IMapper GetMapper()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));
            var mapper = new Mapper(configuration);
            return mapper;
        }

        [Fact]
        public async Task Post_Call_CallServiceMethod()
        {
            var dto = new ProjectInvoiceCreationDto
            {
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                Items = new List<ProjectInvoiceItemCreationDto>()
                {
                    new ProjectInvoiceItemCreationDto { ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItemCreationDto { ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };

            //call method under test
            await _controller.Post(dto);

            //Assert
            _projectInvoiceService.Verify(x => x.AddProjectInvoiceAsync(It.IsAny<ProjectInvoice>()));
        }

        [Fact]
        public async Task Post_Success_ReturnsNoContent()
        {
            var dto = new ProjectInvoiceCreationDto
            {
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                Items = new List<ProjectInvoiceItemCreationDto>()
                {
                    new ProjectInvoiceItemCreationDto { ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItemCreationDto { ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };

            //call method under test
            var actionResult = await _controller.Post(dto);

            //Assertion
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task PutGet_NoInvoice_ReturnsNotFound()
        {
            //Preperation
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync((ProjectInvoice?)null);

            //Call method under test
            var actionResult = await _controller.PutGet(1);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutGet_InvoiceNotNull_ReturnsOkWithDto()
        {
            //Preperation
            var invoice = new ProjectInvoice
            {
                Id = 1,
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { Id = 2, ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync(invoice);

            //Call method under test
            var actionResult = await _controller.PutGet(1);

            //Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var content = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<ProjectInvoiceUpdateGetDto>(content);
        }

        [Fact]
        public async Task Put_NoInvoice_ReturnsNotFound()
        {
            //Preperation
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync((ProjectInvoice?)null);

            //Call method under test
            var actionResult = await _controller.Put(1, new ProjectInvoiceUpdateDto { });

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task Put_InvoiceNotNull_CallUpdateAndReturnsNoContent()
        {
            //Preperation
            var invoice = new ProjectInvoice
            {
                Id = 1,
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { Id = 2, ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync(invoice);

            var updateDto = new ProjectInvoiceUpdateDto
            {
                ReferenceNumber = "r-update",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = (short)TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItemUpdateDto>()
                {
                    new ProjectInvoiceItemUpdateDto { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 10, Price = 1},
                    new ProjectInvoiceItemUpdateDto { Id = 2, ItemId = 2, Unit = "Unit2", Quantity = 20, Price = 2}
                }
            };

            //Call method under test
            var actionResult = await _controller.Put(1, updateDto);

            //Assert
            _projectInvoiceService.Verify(x => x.UpdateProjectInvoiceAsync(), Times.Once);
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task Put_InvoiceNotNull_InvoiceObjectUpdated()
        {
            //Preperation
            var invoice = new ProjectInvoice
            {
                Id = 1,
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { Id = 2, ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync(invoice);

            var updateDto = new ProjectInvoiceUpdateDto
            {
                ReferenceNumber = "r-update",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = (short)TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItemUpdateDto>()
                {
                    new ProjectInvoiceItemUpdateDto { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 10, Price = 1},
                    new ProjectInvoiceItemUpdateDto { Id = 0, ItemId = 3, Unit = "Unit3", Quantity = 30, Price = 3},
                }
            };

            //Call method under test
            var actionResult = await _controller.Put(1, updateDto);

            //Assert
            Assert.Equal("r-update", invoice.ReferenceNumber);
            Assert.Equal(10, invoice.Items[0].Quantity);
            Assert.Equal(30, invoice.Items[1].Quantity);
        }

        [Fact]
        public async Task Approve_NoInvoice_ReturnsNotFound()
        {
            //Preperation
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync((ProjectInvoice?)null);

            //Call method under test
            var actionResult = await _controller.Approve(1);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task Approve_InvoiceNotNull_CallServiceAndReturnsNoContent()
        {
            //Preperation
            var invoice = new ProjectInvoice
            {
                Id = 1,
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                State = TaklaNew.API.Domain.Enums.ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { Id = 1, ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { Id = 2, ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };
            
            _projectInvoiceService.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(1)).ReturnsAsync(invoice);

            //Call method under test
            var actionResult = await _controller.Approve(1);

            //Assert
            _projectInvoiceService.Verify(x => x.ApproveAsync(invoice), Times.Once);
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task PaymentsToPay_Called_ReturnsOkwithPayments()
        {
            //Preperation
            var payments = new List<ProjectInvoicePaymentReadyToPayDto>();
            _projectInvoiceQueries.Setup(x => x.GetProjectInvoicePaymentReadyToPayAsync()).ReturnsAsync(payments);

            //Call method under test
            var actionResult = await _controller.PaymentsToPay();

            //Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var content = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<List<ProjectInvoicePaymentReadyToPayDto>?> (content);
        }

        [Fact]
        public async Task PaymentsToPayByIds_Called_ReturnsOkwithPayments()
        {
            //Preperation
            var payments = new List<ProjectInvoicePaymentReadyToPayDto>();
            var ids = new List<int> { 1, 2 };
            _projectInvoiceQueries.Setup(x => x.GetProjectInvoicePaymentToPayByIdsAsync(ids)).ReturnsAsync(payments);

            //Call method under test
            var actionResult = await _controller.PaymentsToPayByIds(ids);

            //Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var content = Helper.GetObjectResultContent(actionResult);
            Assert.IsType<List<ProjectInvoicePaymentReadyToPayDto>?>(content);
        }

        [Fact]
        public async Task Payment_NoPayment_ReturnsNotFound()
        {
            //Preperation
            var dto = new ProjectInvoicePaymentCreationDto { PaymentId = 1 };
            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentByIdAsync(1)).ReturnsAsync((ProjectInvoicePayment?)null);

            //Call method under test
            var actionResult = await _controller.Payment(dto);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task Payment_NotValidPayment_ReturnsBadRequest()
        {
            //Preperation
            var dto = new ProjectInvoicePaymentCreationDto { PaymentId = 1, CashList = null, ChecksList = null };
            var payment = new ProjectInvoicePayment { Id = 1 };
            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentByIdAsync(1)).ReturnsAsync(payment);
            _projectInvoiceService.Setup(x => x.IsValidPayment(payment, null, null)).Returns(false);
            
            //Call method under test
            var actionResult = await _controller.Payment(dto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Payment_ValidPayment_CallServiceAndReturnsNoContent()
        {
            //Preperation
            var dto = new ProjectInvoicePaymentCreationDto { PaymentId = 1, CashList = null, ChecksList = null };
            var payment = new ProjectInvoicePayment { Id = 1 };
            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentByIdAsync(1)).ReturnsAsync(payment);
            _projectInvoiceService.Setup(x => x.IsValidPayment(payment, null, null)).Returns(true);

            //Call method under test
            var actionResult = await _controller.Payment(dto);

            //Assert
            _projectInvoiceService.Verify(x => x.PayPaymentAsync(payment, null, null), Times.Once);
            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task PaymentGroup_NoPayment_ReturnsNotFound()
        {
            //Preperation
            var dto = new ProjectInvoiceGroupPaymentCreationDto { PaymentIds = new List<int> { 1, 2}, CashList = null, ChecksList = null };
            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentsByIdsAsync(dto.PaymentIds)).ReturnsAsync(new List<ProjectInvoicePayment>());

            //Call method under test
            var actionResult = await _controller.PaymentGroup(dto);

            //Assert
            Assert.IsType<NotFoundResult>(actionResult);
        }

        [Fact]
        public async Task PaymentGroup_NotValidPayment_ReturnsBadRequest()
        {
            //Preperation
            var dto = new ProjectInvoiceGroupPaymentCreationDto { PaymentIds = new List<int> { 1, 2 }, CashList = null, ChecksList = null };
            var payments = new List<ProjectInvoicePayment>()
            {
                new ProjectInvoicePayment { Id = 1},
                new ProjectInvoicePayment { Id = 2}
            };

            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentsByIdsAsync(dto.PaymentIds)).ReturnsAsync(payments);
            _projectInvoiceService.Setup(x => x.IsValidPaymentGroup(payments, null, null)).Returns(false);

            //Call method under test
            var actionResult = await _controller.PaymentGroup(dto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task PaymentGroup_ValidPaymentAndNotSuccessPayCalling_ReturnsBadRequest()
        {
            //Preperation
            var dto = new ProjectInvoiceGroupPaymentCreationDto { PaymentIds = new List<int> { 1, 2 }, CashList = null, ChecksList = null };
            var payments = new List<ProjectInvoicePayment>()
            {
                new ProjectInvoicePayment { Id = 1},
                new ProjectInvoicePayment { Id = 2}
            };

            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentsByIdsAsync(dto.PaymentIds)).ReturnsAsync(payments);
            _projectInvoiceService.Setup(x => x.IsValidPaymentGroup(payments, null, null)).Returns(true);
            _projectInvoiceService.Setup(x => x.PayPaymentGroupAsync(payments, null, null)).ReturnsAsync(false);

            //Call method under test
            var actionResult = await _controller.PaymentGroup(dto);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task PaymentGroup_ValidPaymentAndSuccessPayCalling_ReturnsNoContent()
        {
            //Preperation
            var dto = new ProjectInvoiceGroupPaymentCreationDto { PaymentIds = new List<int> { 1, 2 }, CashList = null, ChecksList = null };
            var payments = new List<ProjectInvoicePayment>()
            {
                new ProjectInvoicePayment { Id = 1},
                new ProjectInvoicePayment { Id = 2}
            };

            _projectInvoiceService.Setup(x => x.GetProjectInvoicePaymentsByIdsAsync(dto.PaymentIds)).ReturnsAsync(payments);
            _projectInvoiceService.Setup(x => x.IsValidPaymentGroup(payments, null, null)).Returns(true);
            _projectInvoiceService.Setup(x => x.PayPaymentGroupAsync(payments, null, null)).ReturnsAsync(true);

            //Call method under test
            var actionResult = await _controller.PaymentGroup(dto);

            //Assert
            Assert.IsType<NoContentResult>(actionResult);
        }
    }
}
