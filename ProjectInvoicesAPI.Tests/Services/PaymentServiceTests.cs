using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Mapping;
using ProjectInvoices.API.Services;

namespace ProjectInvoicesAPI.Tests.Services
{
    public class PaymentServiceTests
    {

        [Fact]
        public async Task GetProjectInvoicePaymentReadyToPayAsync_ShouldReturnOnlyUnpaidPayments()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-001",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                State = ProjectInvoiceState.Created
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ProjectInvoicePayments.AddRange(
                new ProjectInvoicePayment
                {
                    Date = DateTime.Today,
                    Amount = 100,
                    ProjectInvoiceId = invoice.Id,
                    Done = false
                },
                new ProjectInvoicePayment
                {
                    Date = DateTime.Today,
                    Amount = 200,
                    ProjectInvoiceId = invoice.Id,
                    Done = true
                }
            );

            await context.SaveChangesAsync();

            // Act
            var result = await service.GetProjectInvoicePaymentReadyToPayAsync();

            // Assert (force DB read)
            context.ChangeTracker.Clear();

            Assert.Single(result);
            Assert.Equal(100, result[0].Amount);
            Assert.Equal(invoice.Id, result[0].InvoiceId);
        }

        [Fact]
        public async Task GetProjectInvoicePaymentsByIdsAsync_ShouldReturnOnlyMatchingUnpaidPayments()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            // Arrange: create required project and supplier
            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            // Create a project invoice
            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-002",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                Supplier = supplier,
                State = ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>
        {
            new ProjectInvoiceItem { ItemId = 1, Unit = "pcs", Quantity = 1, Price = 100 } // dummy item
        }
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            // Create payments
            var unpaid = new ProjectInvoicePayment
            {
                Date = DateTime.Today,
                Amount = 50,
                ProjectInvoiceId = invoice.Id,
                Done = false
            };

            var paid = new ProjectInvoicePayment
            {
                Date = DateTime.Today,
                Amount = 60,
                ProjectInvoiceId = invoice.Id,
                Done = true
            };

            context.ProjectInvoicePayments.AddRange(unpaid, paid);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            // Act
            var result = await service.GetProjectInvoicePaymentsByIdsAsync(
                new List<int> { unpaid.Id, paid.Id });

            // Assert
            Assert.Single(result);
            Assert.Equal(unpaid.Id, result[0].Id);
            Assert.Equal(invoice.Id, result[0].InvoiceId);
            Assert.Equal(project.Name, result[0].Project);
            Assert.Equal(supplier.Name, result[0].Supplier);
        }

        [Fact]
        public async Task PayPaymentAsync_ShouldMarkPaymentDone_AndCreateCashMovements()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-003",
                Date = DateTime.Today,
                ProjectId = 1,
                SupplierId = 1,
                State = ProjectInvoiceState.Approved
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Today,
                Amount = 300,
                ProjectInvoiceId = invoice.Id,
                Done = false
            };

            context.ProjectInvoicePayments.Add(payment);
            await context.SaveChangesAsync();

            var dto = new ProjectInvoicePaymentCreationDto
            {
                PaymentId = payment.Id,
                CashList = new()
            {
                new() { Amount = 100, Date = DateTime.Today },
                new() { Amount = 200, Date = DateTime.Today }
            }
            };

            // Act
            await service.PayPaymentAsync(dto);

            // Assert (force DB read)
            context.ChangeTracker.Clear();

            var persistedPayment = await context.ProjectInvoicePayments.SingleAsync();
            Assert.True(persistedPayment.Done);

            var cashMovements = await context.CashOutMovements.ToListAsync();
            Assert.Equal(2, cashMovements.Count);
            Assert.All(cashMovements, c =>
            {
                Assert.False(c.IsGroup);
                Assert.Equal(TransactionKind.ProjectInvoice, c.TransactionKind);
            });
        }

        [Fact]
        public async Task PayPaymentAsync_ShouldThrow_WhenPaidAmountDoesNotMatch()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-004",
                Date = DateTime.Today,
                ProjectId = 1,
                SupplierId = 1,
                State = ProjectInvoiceState.Approved
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            var payment = new ProjectInvoicePayment
            {
                Date = DateTime.Today,
                Amount = 500,
                ProjectInvoiceId = invoice.Id,
                Done = false
            };

            context.ProjectInvoicePayments.Add(payment);
            await context.SaveChangesAsync();

            var dto = new ProjectInvoicePaymentCreationDto
            {
                PaymentId = payment.Id,
                CashList = new()
            {
                new() { Amount = 100 }
            }
            };

            await Assert.ThrowsAsync<ValidationException>(() =>
                service.PayPaymentAsync(dto));
        }

        [Fact]
        public async Task PayPaymentGroupAsync_Should_CreateGroup_InsertMovements_And_MarkPaymentsDone()
        {
            using var context = CreateContext();

            // ---------- Arrange ----------
            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-G1",
                Date = DateTime.Today,
                ProjectId = 1,
                SupplierId = 1,
                State = ProjectInvoiceState.Approved,
                Items = new List<ProjectInvoiceItem>()
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            var payments = new List<ProjectInvoicePayment>
        {
            new() { Date = DateTime.Today, Amount = 200, ProjectInvoiceId = invoice.Id, Done = false },
            new() { Date = DateTime.Today, Amount = 300, ProjectInvoiceId = invoice.Id, Done = false }
        };

            context.ProjectInvoicePayments.AddRange(payments);
            await context.SaveChangesAsync();

            var paymentIds = payments.Select(x => x.Id).ToList();

            context.ChangeTracker.Clear();

            var service = CreateService(context);

            var dto = new ProjectInvoiceGroupPaymentCreationDto
            {
                PaymentIds = paymentIds,
                CashList = new List<ProjectInvoiceCashCreationDto>
            {
                new() { Amount = 500, Date = DateTime.Today }
            }
            };

            // ---------- Act ----------
            await service.PayPaymentGroupAsync(dto);

            context.ChangeTracker.Clear();

            // ---------- Assert ----------
            var group = await context.ProjectInvoicePaymentGroups.SingleAsync();
            var savedPayments = await context.ProjectInvoicePayments.ToListAsync();
            var cashMovements = await context.CashOutMovements.ToListAsync();

            Assert.All(savedPayments, p =>
            {
                Assert.True(p.Done);
                Assert.True(p.IsGroup);
                Assert.Equal(group.Id, p.GroupId);
            });

            Assert.Single(cashMovements);
            Assert.True(cashMovements[0].IsGroup);
            Assert.Equal(group.Id, cashMovements[0].PaymentId);
        }

        [Fact]
        public async Task PayPaymentGroupAsync_Should_Insert_Cash_And_Check_Movements()
        {
            using var context = CreateContext();

            // ---------- Arrange ----------
            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-G2",
                Date = DateTime.Today,
                ProjectId = 1,
                SupplierId = 1,
                State = ProjectInvoiceState.Approved,
                Items = new List<ProjectInvoiceItem>()
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            var payments = new List<ProjectInvoicePayment>
            {
                new() { Date = DateTime.Today, Amount = 400, ProjectInvoiceId = invoice.Id, Done = false }
            };

            context.ProjectInvoicePayments.AddRange(payments);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var service = CreateService(context);

            var dto = new ProjectInvoiceGroupPaymentCreationDto
            {
                PaymentIds = payments.Select(x => x.Id).ToList(),
                CashList = new()
                {
                    new() { Amount = 100, Date = DateTime.Today }
                },
                ChecksList = new()
                {
                    new()
                        {
                            Amount = 300,
                            Date = DateTime.Today,
                            BankAccountId = 1,
                            CheckNumber = "CHK-G1"
                        }
                }
            };

            // ---------- Act ----------
            await service.PayPaymentGroupAsync(dto);

            context.ChangeTracker.Clear();

            // ---------- Assert ----------
            Assert.Single(context.ProjectInvoicePaymentGroups);
            Assert.Single(context.CashOutMovements);
            Assert.Single(context.CheckOutMovements);

            Assert.True(context.CashOutMovements.Single().IsGroup);
            Assert.True(context.CheckOutMovements.Single().IsGroup);
        }

        [Fact]
        public async Task PayPaymentGroupAsync_ShouldThrow_When_TotalAmountMismatch()
        {
            using var context = CreateContext();

            // ---------- Arrange ----------
            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-G3",
                Date = DateTime.Today,
                ProjectId = 1,
                SupplierId = 1,
                State = ProjectInvoiceState.Approved,
                Items = new List<ProjectInvoiceItem>()
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            var payments = new List<ProjectInvoicePayment>
            {
                new() { Date = DateTime.Today, Amount = 200, ProjectInvoiceId = invoice.Id, Done = false },
                new() { Date = DateTime.Today, Amount = 300, ProjectInvoiceId = invoice.Id, Done = false }
            };

            context.ProjectInvoicePayments.AddRange(payments);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var service = CreateService(context);

            var dto = new ProjectInvoiceGroupPaymentCreationDto
            {
                PaymentIds = payments.Select(x => x.Id).ToList(),
                CashList = new()
                {
                    new() { Amount = 400, Date = DateTime.Today } 
                }
            };

            // ---------- Act + Assert ----------
            await Assert.ThrowsAsync<ValidationException>(() =>
                service.PayPaymentGroupAsync(dto));

            context.ChangeTracker.Clear();

            // Ensure no partial data saved
            Assert.Empty(context.ProjectInvoicePaymentGroups);
            Assert.All(context.ProjectInvoicePayments, p => Assert.False(p.Done));
            Assert.Empty(context.CashOutMovements);
            Assert.Empty(context.CheckOutMovements);
        }

        //--------------------------------------------------------------
        // Helper methods
        //--------------------------------------------------------------

        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
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

        private static PaymentService CreateService(ApplicationDbContext context)
        {
            return new PaymentService(context, CreateMapper());
        }
    }
}
