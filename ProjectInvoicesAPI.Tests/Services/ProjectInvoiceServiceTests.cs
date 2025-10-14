using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaklaNew.API.Domain;
using TaklaNew.API.Domain.Enums;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Dtos;
using TaklaNew.API.Services;

namespace TaklaNewAPI.Tests.Services
{
    /// <summary>
    /// Unit tests for ProjectInvoiceService
    /// </summary>
    public class ProjectInvoiceServiceTests
    {
        Mock<IProjectInvoiceRepository> _repository;
        ProjectInvoiceService _service;
        Mock<IDbContextTransaction> _transaction; 
        public ProjectInvoiceServiceTests()
        {
            _repository = new Mock<IProjectInvoiceRepository>();
            _service = new ProjectInvoiceService(_repository.Object);
        }

        /// <summary>
        /// Tests that AddProjectInvoiceAsync method
        /// calls requied repository methods
        /// </summary>
        [Fact]
        public async Task AddProjectInvoiceAsync_Called_CallRepositoryMethods()
        {
            //Arrangement
            var order = 0;
            var projectInvoice = new ProjectInvoice();
            _repository.Setup(x => x.AddProjectInvoice(projectInvoice)).Callback(
                () =>
                {
                    order = order + 1;
                    Assert.Equal(1, order);
                });

            _repository.Setup(x => x.SaveAsync()).Callback(
                () =>
                {
                    order = order + 1;
                    Assert.Equal(2, order);
                });

            //call method under test
            await _service.AddProjectInvoiceAsync(projectInvoice);

            //Assertion
            _repository.Verify(x => x.AddProjectInvoice(projectInvoice));
            _repository.Verify(x => x.SaveAsync());
        }

        /// <summary>
        /// Tests that UpdateProjectInvoiceAsync method
        /// calls requied repository method
        /// </summary>
        [Fact]
        public async Task UpdateProjectInvoiceAsync_Called_CallRepositoryMethods()
        {
            //call method under test
            await _service.UpdateProjectInvoiceAsync();

            //Assertion
            _repository.Verify(x => x.SaveAsync());
        }

        /// <summary>
        /// Tests that GetProjectInvoiceWithItemsByIdAsync method
        /// calls requied repository method
        /// </summary>
        [Fact]
        public async Task GetProjectInvoiceWithItemsByIdAsync_Called_CallRepositoryMethod()
        {
            //call method under test
            await _service.GetProjectInvoiceWithItemsByIdAsync(2);

            //Assertion
            _repository.Verify(x => x.GetProjectInvoiceWithItemsByIdAsync(2));
        }

        /// <summary>
        /// Tests that GetProjectInvoiceWithItemsByIdAsync method
        /// returns ProjectInvoice object
        /// </summary>
        [Fact]
        public async Task GetProjectInvoiceWithItemsByIdAsync_Called_ReturnsProjectInvoice()
        {
            //call method under test
            var projectInvoice = new ProjectInvoice();
            _repository.Setup(x => x.GetProjectInvoiceWithItemsByIdAsync(2))
                .ReturnsAsync(projectInvoice);

            var result = await _service.GetProjectInvoiceWithItemsByIdAsync(2);

            //Assertion
            Assert.IsType<ProjectInvoice>(result);
        }

        /// <summary>
        /// Tests that ApproveAsync method
        /// changes projectInvoice object' state
        /// </summary>
        [Fact]
        public async Task ApproveAsync_Called_SetInvoiceState()
        {
            //call method under test
            var projectInvoice = new ProjectInvoice
            {
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };

            await _service.ApproveAsync(projectInvoice);

            //Assertion
            Assert.Equal(ProjectInvoiceState.Approved, projectInvoice.State);
        }

        /// <summary>
        /// Tests that ApproveAsync method
        /// Throws exception when provided with empty ProjectInvoice object
        /// </summary>
        [Fact]
        public void ApproveAsync_CalledWithEmptyInvoice_ThrowException()
        {
            //call method under test
            var projectInvoice = new ProjectInvoice();
            Assert.ThrowsAnyAsync<Exception>(async () => await _service.ApproveAsync(projectInvoice));
        }

        /// <summary>
        /// Tests that ApproveAsync method
        /// calls required repository methods
        /// </summary>
        [Fact]
        public async Task ApproveAsync_Called_CallAddProjectInvoicePayment()
        {

            var projectInvoice = new ProjectInvoice
            {
                Id = 1,
                ReferenceNumber = "r",
                ProjectId = 1,
                SupplierId = 1,
                Date = DateTime.Now,
                Items = new List<ProjectInvoiceItem>()
                {
                    new ProjectInvoiceItem { ItemId = 1, Unit = "Unit1", Quantity = 1, Price = 1},
                    new ProjectInvoiceItem { ItemId = 2, Unit = "Unit2", Quantity = 2, Price = 2}
                }
            };

            //call method under test
            await _service.ApproveAsync(projectInvoice);

            //Assertion
            _repository.Verify(x => x.AddProjectInvoicePayment(
                    It.Is<ProjectInvoicePayment>(m =>
                    (m.ProjectInvoiceId == 1) &&
                    (m.Date.Date.Equals(DateTime.Now.Date)) &&
                    (m.Amount.Equals(projectInvoice.Items.Sum(m => m.Price * (decimal)m.Quantity)))
                    )
                ), Times.Once);

            _repository.Verify(x => x.SaveAsync(), Times.Once);
        }

        /// <summary>
        /// Tests that GetProjectInvoicePaymentByIdAsync method
        /// calls required repository method
        /// </summary>
        [Fact]
        public async Task GetProjectInvoicePaymentByIdAsync_Called_CallRepositoryMethod()
        {
            //call method under test
            await _service.GetProjectInvoicePaymentByIdAsync(2);

            //Assertion
            _repository.Verify(x => x.GetProjectInvoicePaymentByIdAsync(2), Times.Once);
        }

        /// <summary>
        /// Tests that GetProjectInvoicePaymentByIdAsync method
        /// returns ProjectInvoicePayment object
        /// </summary>
        [Fact]
        public async Task GetProjectInvoicePaymentByIdAsync_Called_ReturnsProjectInvoicePayment()
        {
            //call method under test
            var projectInvoicePayment = new ProjectInvoicePayment();
            _repository.Setup(x => x.GetProjectInvoicePaymentByIdAsync(2))
                .ReturnsAsync(projectInvoicePayment);

            var result = await _service.GetProjectInvoicePaymentByIdAsync(2);

            //Assertion
            Assert.IsType<ProjectInvoicePayment>(result);
        }

        /// <summary>
        /// Tests that IsValidPayment method
        /// returns true if the input is valid
        /// </summary>
        [Fact]
        public void IsValidPayment_ValidInput_ReturnsTrue()
        {
            //Arrangements
            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = 1,
                Amount = 100
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            var isValid = _service.IsValidPayment(payment, cashList, checkList);

            //Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Tests that IsValidPayment method
        /// returns false if the input is invalid
        /// </summary>
        [Fact]
        public void IsValidPayment_InvalidInput_ReturnsFalse()
        {
            //Arrangements
            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = 1,
                Amount = 100
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 50 }
            };

            //Call method under test
            var isValid = _service.IsValidPayment(payment, cashList, checkList);

            //Assert
            Assert.False(isValid);
        }

        /// <summary>
        /// Tests that IsValidPayment method
        /// returns false if cash and checks are null
        /// </summary>
        [Fact]
        public void IsValidPayment_NullInput_ReturnsFalse()
        {
            //Arrangements
            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = 1,
                Amount = 100
            };

            //Call method under test
            var isValid = _service.IsValidPayment(payment, null, null);

            //Assert
            Assert.False(isValid);
        }

        /// <summary>
        /// Tests that PayPaymentAsync method
        /// changes payment' Done property to true
        /// </summary>
        [Fact]
        public async Task PayPaymentAsync_Called_SetDonePaymentProperty()
        {
            //Arrangements
            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = 1,
                Amount = 100
            };

            //Call method under test
            await _service.PayPaymentAsync(payment, null, null);

            //Assert
            Assert.True(payment.Done);
        }

        /// <summary>
        /// Tests that PayPaymentAsync method
        /// calls required repository methods
        /// </summary>
        [Fact]
        public async Task PayPaymentAsync_Called_CallRepositoryMethods()
        {
            //Arrangements
            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = 1,
                Amount = 100
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            await _service.PayPaymentAsync(payment, cashList, checkList);

            //Assert
            _repository.Verify(x => x.AddCheckOutMovements(It.Is<List<CheckOutMovement>>(
                m => (m.Count == checkList.Count) && (m.Sum(x => x.Amount) == checkList.Sum(x => x.Amount))
                )), Times.Once);

            _repository.Verify(x => x.AddCashOutMovements(It.Is<List<CashOutMovement>>(
                m => (m.Count == checkList.Count) && (m.Sum(x => x.Amount) == checkList.Sum(x => x.Amount))
                )), Times.Once);

            _repository.Verify(x => x.SaveAsync(), Times.Once);
        }

        /// <summary>
        /// Tests that GetProjectInvoicePaymentsByIdsAsync method
        /// calls required repository method
        /// </summary>
        [Fact]
        public async Task GetProjectInvoicePaymentsByIdsAsync_Called_CallRepositoryMethod()
        {
            //call method under test
            var ids = new List<int> { 1, 2 };
            await _service.GetProjectInvoicePaymentsByIdsAsync(ids);

            //Assertion
            _repository.Verify(x => x.GetProjectInvoicePaymentsByIdsAsync(ids), Times.Once);
        }

        /// <summary>
        /// Tests that GetProjectInvoicePaymentsByIdsAsync method
        /// returns list of ProjectInvoicePayment
        /// </summary>
        [Fact]
        public async Task GetProjectInvoicePaymentsByIdsAsync_Called_ReturnsProjectInvoicePaymentList()
        {
            //call method under test
            var projectInvoicePaymentList = new List<ProjectInvoicePayment>();
            var ids = new List<int> { 1, 2 };
            _repository.Setup(x => x.GetProjectInvoicePaymentsByIdsAsync(ids))
                .ReturnsAsync(projectInvoicePaymentList);

            var result = await _service.GetProjectInvoicePaymentsByIdsAsync(ids);

            //Assertion
            Assert.IsType<List<ProjectInvoicePayment>>(result);
        }

        /// <summary>
        /// Tests that IsValidPaymentGroup method
        /// returns true if the input is valid
        /// </summary>
        [Fact]
        public void IsValidPaymentGroup_ValidInput_ReturnsTrue()
        {
            //Arrangements
            var payments = new List<ProjectInvoicePayment> 
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            var isValid = _service.IsValidPaymentGroup(payments, cashList, checkList);

            //Assert
            Assert.True(isValid);
        }

        /// <summary>
        /// Tests that IsValidPaymentGroup method
        /// returns false if the input is invalid
        /// </summary>
        [Fact]
        public void IsValidPaymentGroup_InvalidInput_ReturnsFalse()
        {
            //Arrangements
            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 100 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 50 }
            };

            //Call method under test
            var isValid = _service.IsValidPaymentGroup(payments, cashList, checkList);

            //Assert
            Assert.False(isValid);
        }

        /// <summary>
        /// Tests that IsValidPaymentGroup method
        /// returns false if cash and checks are null
        /// </summary>
        [Fact]
        public void IsValidPaymentGroup_NullInput_ReturnsFalse()
        {
            //Arrangements
            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 100 },
            };

            //Call method under test
            var isValid = _service.IsValidPaymentGroup(payments, null, null);

            //Assert
            Assert.False(isValid);
        }

        /// <summary>
        /// Tests that PayPaymentGroupAsync method
        /// calls required repository methods if the input is valid
        /// </summary>
        [Fact]
        public async Task PayPaymentGroupAsync_ValidInput_CallRepositoryMethods()
        {
            //Arrangement
            _transaction = new Mock<IDbContextTransaction>();
            _repository.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transaction.Object);
            _transaction.Setup(x => x.CommitAsync(default));

            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 50 }
            };

            //Call method under test
            await _service.PayPaymentGroupAsync(payments, cashList, checkList);

            //Assert
            _repository.Verify(x => x.AddProjectInvoicePaymentGroup(It.IsAny<ProjectInvoicePaymentGroup>()), Times.Once);
            _repository.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _repository.Verify(x => x.SaveAsync(), Times.Exactly(2));
            _repository.Verify(x => x.AddCheckOutMovements(It.Is<List<CheckOutMovement>>(x => x.Count == 2)));
            _repository.Verify(x => x.AddCashOutMovements(It.Is<List<CashOutMovement>>(x => x.Count == 2)));
            _transaction.Verify(x => x.CommitAsync(default), Times.Once);
        }

        /// <summary>
        /// Tests that PayPaymentGroupAsync method
        /// returns true if the input is valid
        /// </summary>
        [Fact]
        public async Task PayPaymentGroupAsync_ValidInput_ReturnsTrue()
        {
            //Arrangement
            _transaction = new Mock<IDbContextTransaction>();
            _repository.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transaction.Object);
            _transaction.Setup(x => x.CommitAsync(default));

            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            var result = await _service.PayPaymentGroupAsync(payments, cashList, checkList);

            //Assert
            Assert.True(result);            
        }

        /// <summary>
        /// Tests that PayPaymentGroupAsync method
        /// returns false if the repository savechanges method throws an exception
        /// </summary>
        [Fact]
        public async Task PayPaymentGroupAsync_ThrowsException_ReturnsFalse()
        {
            //Arrangement
            _transaction = new Mock<IDbContextTransaction>();
            _repository.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transaction.Object);
            _transaction.Setup(x => x.CommitAsync(default));
            _repository.Setup(x => x.SaveAsync()).Throws<Exception>();

            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            var result = await _service.PayPaymentGroupAsync(payments, cashList, checkList);

            //Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests that PayPaymentGroupAsync method
        /// transaction' RollbackAsync method is called if repository savechanges method throws an exception
        /// </summary>
        [Fact]
        public async Task PayPaymentGroupAsync_ThrowsException_RollBackTransaction()
        {
            //Arrangement
            _transaction = new Mock<IDbContextTransaction>();
            _repository.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transaction.Object);
            _transaction.Setup(x => x.CommitAsync(default));
            _repository.Setup(x => x.SaveAsync()).Throws<Exception>();

            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 25 }
            };

            //Call method under test
            await _service.PayPaymentGroupAsync(payments, cashList, checkList);

            //Assert
            _transaction.Verify(x => x.RollbackAsync(default));
        }

        /// <summary>
        /// Tests that PayPaymentGroupAsync method
        /// set group payment properties in the list of payments if the input is valid
        /// </summary>
        [Fact]
        public async Task PayPaymentGroupAsync_ValidInput_UpdateProjectInvoicePaymentList()
        {
            //Arrangement
            _transaction = new Mock<IDbContextTransaction>();
            _repository.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transaction.Object);
            _transaction.Setup(x => x.CommitAsync(default));

            var payments = new List<ProjectInvoicePayment>
            {
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
                new ProjectInvoicePayment { Date = DateTime.Now, ProjectInvoiceId = 1, Amount = 50 },
            };

            var cashList = new List<ProjectInvoiceCashCreationDto>
            {
                new ProjectInvoiceCashCreationDto { Amount = 25 },
                new ProjectInvoiceCashCreationDto { Amount = 25 }
            };

            var checkList = new List<ProjectInvoiceCheckCreationDto>
            {
                new ProjectInvoiceCheckCreationDto { Amount = 25,  },
                new ProjectInvoiceCheckCreationDto { Amount = 50 }
            };

            //Call method under test
            await _service.PayPaymentGroupAsync(payments, cashList, checkList);

            //Assert
            foreach(var payment in payments)
            {
                Assert.True(payment.Done);
                Assert.NotNull(payment.GroupId);
                Assert.True(payment.IsGroup);
            }
        }
    }
}
