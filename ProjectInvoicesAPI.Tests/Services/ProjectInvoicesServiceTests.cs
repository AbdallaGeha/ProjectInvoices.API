using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services;
using ProjectInvoices.API.Mapping;

namespace ProjectInvoicesAPI.Tests.Services
{
    public class ProjectInvoicesServiceTests
    {
        // -------------------------------------------------
        // AddProjectInvoiceAsync
        // -------------------------------------------------
        [Fact]
        public async Task AddProjectInvoiceAsync_ShouldPersistInvoice()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var dto = new ProjectInvoiceCreationDto
            {
                ReferenceNumber = "INV-001",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id
            };

            await service.AddProjectInvoiceAsync(dto);

            context.ChangeTracker.Clear();

            var invoice = await context.ProjectInvoices
                .Include(x => x.Project)
                .Include(x => x.Supplier)
                .SingleAsync();

            Assert.Equal("INV-001", invoice.ReferenceNumber);
            Assert.Equal(project.Id, invoice.ProjectId);
            Assert.Equal(supplier.Id, invoice.SupplierId);
            Assert.Equal("Project A", invoice.Project.Name);
            Assert.Equal("Supplier A", invoice.Supplier.Name);
        }

        // -------------------------------------------------
        // ApproveAsync
        // -------------------------------------------------
        [Fact]
        public async Task ApproveAsync_ShouldApproveAndCreatePayment()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-APP",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                State = ProjectInvoiceState.Created,
                Items = new List<ProjectInvoiceItem>
                {
                    new() { Quantity = 2, Price = 100, ItemId = 1, Unit = "unit1" },
                    new() { Quantity = 1, Price = 50, ItemId = 2, Unit = "unit2" }
                }
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            await service.ApproveAsync(invoice.Id);

            context.ChangeTracker.Clear();

            var updatedInvoice = await context.ProjectInvoices.SingleAsync();
            Assert.Equal(ProjectInvoiceState.Approved, updatedInvoice.State);

            var payment = await context.ProjectInvoicePayments.SingleAsync();
            Assert.Equal(250m, payment.Amount);
            Assert.False(payment.Done);
            Assert.Equal(invoice.Id, payment.ProjectInvoiceId);
        }

        [Fact]
        public async Task ApproveAsync_ShouldThrow_WhenStateIsNotCreated()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-APP",
                Date = DateTime.Today,
                State = ProjectInvoiceState.Approved,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                Supplier = supplier
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            await Assert.ThrowsAsync<BusinessException>(() =>
                service.ApproveAsync(invoice.Id));
        }

        // -------------------------------------------------
        // GetProjectInvoiceWithItemsByIdAsync
        // -------------------------------------------------
        [Fact]
        public async Task GetProjectInvoiceWithItemsByIdAsync_ShouldReturnInvoice()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-GET",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                Supplier = supplier,
                Items = new List<ProjectInvoiceItem>
                {
                    new() { Quantity = 1, Price = 10, ItemId = 1, Unit = "unit" }
                }
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var result = await service.GetProjectInvoiceWithItemsByIdAsync(invoice.Id);

            Assert.Single(result.Items);
            Assert.Equal("INV-GET", result.ReferenceNumber);
        }

        [Fact]
        public async Task GetProjectInvoiceWithItemsByIdAsync_ShouldThrow_WhenNotFound()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.GetProjectInvoiceWithItemsByIdAsync(999));
        }

        // -------------------------------------------------
        // UpdateProjectInvoiceAsync
        // -------------------------------------------------
        [Fact]
        public async Task UpdateProjectInvoiceAsync_ShouldUpdateInvoiceAndItems()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            // Arrange: create project, supplier, and items
            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            var item1 = new Item { Name = "Item 1", Unit = "unit1" };
            var item2 = new Item { Name = "Item 2", Unit = "unit2" };
            var item3 = new Item { Name = "Item 3", Unit = "unit3" }; 
            context.AddRange(project, supplier, item1, item2, item3);
            await context.SaveChangesAsync();

            // Existing invoice with 2 items
            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "OLD",
                Date = DateTime.Today,
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                Supplier = supplier,
                Items = new List<ProjectInvoiceItem>
                {
                    new ProjectInvoiceItem { ItemId = item1.Id, Item = item1, Unit = "kg", Quantity = 5, Price = 10 },
                    new ProjectInvoiceItem { ItemId = item2.Id, Item = item2, Unit = "m", Quantity = 3, Price = 20 }
                }
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            // Prepare update DTO
            var dto = new ProjectInvoiceUpdateDto
            {
                ReferenceNumber = "NEW",
                Date = DateTime.Today.AddDays(1),
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                State = (short)ProjectInvoiceState.Approved,
                Items = new List<ProjectInvoiceItemUpdateDto>
                {
                    // Update existing item1
                    new ProjectInvoiceItemUpdateDto { Id = invoice.Items[0].Id, ItemId = item1.Id, Unit = "kg", Quantity = 6, Price = 12 },
                    // Remove item2 (by not including it in DTO)
                    // Add new item3
                    new ProjectInvoiceItemUpdateDto { Id = 0, ItemId = item3.Id, Unit = "pcs", Quantity = 2, Price = 50 }
                }
            };

            // Act
            await service.UpdateProjectInvoiceAsync(invoice.Id, dto);

            context.ChangeTracker.Clear();

            // Assert
            var updated = await context.ProjectInvoices
                .Include(x => x.Items)
                .SingleAsync();

            Assert.Equal("NEW", updated.ReferenceNumber);
            Assert.Equal(DateTime.Today.AddDays(1), updated.Date);
            Assert.Equal(2, updated.Items.Count); // item2 removed, item1 updated, item3 added

            // Validate updated item1
            var updatedItem1 = updated.Items.Single(x => x.ItemId == item1.Id);
            Assert.Equal(6, updatedItem1.Quantity);
            Assert.Equal(12m, updatedItem1.Price);
            Assert.Equal("kg", updatedItem1.Unit);

            // Validate added item3
            var addedItem3 = updated.Items.Single(x => x.ItemId == item3.Id);
            Assert.Equal(2, addedItem3.Quantity);
            Assert.Equal(50m, addedItem3.Price);
            Assert.Equal("pcs", addedItem3.Unit);
        }

        // -------------------------------------------------
        // GetProjectInvoiceView
        // -------------------------------------------------
        [Fact]
        public async Task GetProjectInvoiceView_ShouldApplyFiltersAndPaginate()
        {
            using var context = CreateContext();
            var service = CreateService(context);

            var project = new Project { Name = "Project A" };
            var supplier = new Supplier { Name = "Supplier A" };
            context.AddRange(project, supplier);
            await context.SaveChangesAsync();

            var invoice = new ProjectInvoice
            {
                ReferenceNumber = "INV-100",
                ProjectId = project.Id,
                SupplierId = supplier.Id,
                Project = project,
                Supplier = supplier,
                State = ProjectInvoiceState.Created,
                Date = DateTime.Today,
                Items = new List<ProjectInvoiceItem>
            {
                new() { Quantity = 2, Price = 100, ItemId = 1, Unit = "unit" }
            }
            };

            context.ProjectInvoices.Add(invoice);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var request = new ProjectInvoiceViewRequestDto
            {
                Page = 1,
                PageSize = 10,
                Reference = "INV"
            };

            var response = await service.GetProjectInvoiceView(request);

            Assert.Single(response.Data);
            Assert.Equal(1, response.TotalRecords);
            Assert.Equal("Project A", response.Data.First().Project);
            Assert.Equal(200m, response.Data.First().Amount);
        }

        // -------------------------------------------------
        // Helpers
        // -------------------------------------------------
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            return configuration.CreateMapper();
        }

        private static ProjectInvoicesService CreateService(ApplicationDbContext context)
            => new ProjectInvoicesService(context, CreateMapper());
    }
}
