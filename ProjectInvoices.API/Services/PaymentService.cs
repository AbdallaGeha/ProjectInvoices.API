using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectInvoices.API.Data;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Dtos;
using ProjectInvoices.API.Exceptions;
using ProjectInvoices.API.Services.Interfaces;
using ProjectInvoices.API.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectInvoices.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PaymentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProjectInvoicePaymentReadyToPayDto>> GetProjectInvoicePaymentReadyToPayAsync()
        {
            return await _context.ProjectInvoicePayments.Where(x => x.Done == false).Join(
                    _context.ProjectInvoices.Include(x => x.Project).Include(x => x.Supplier),
                    x => x.ProjectInvoiceId,
                    x => x.Id,
                    (p, i) => new ProjectInvoicePaymentReadyToPayDto
                    {
                        Id = p.Id,
                        Date = p.Date,
                        Amount = p.Amount,
                        InvoiceId = i.Id,
                        InvoiceReference = i.ReferenceNumber,
                        InvoiceDate = i.Date,
                        SupplierId = i.SupplierId,
                        Supplier = i.Supplier.Name,
                        ProjectId = i.ProjectId,
                        Project = i.Project.Name
                    }
                ).ToListAsync();
        }

        public async Task<List<ProjectInvoicePaymentReadyToPayDto>> GetProjectInvoicePaymentsByIdsAsync(List<int> paymentsIds)
        {
            return await _context.ProjectInvoicePayments.Where(x => x.Done == false && paymentsIds.Contains(x.Id)).Join(
                    _context.ProjectInvoices.Include(x => x.Project).Include(x => x.Supplier),
                    x => x.ProjectInvoiceId,
                    x => x.Id,
                    (p, i) => new ProjectInvoicePaymentReadyToPayDto
                    {
                        Id = p.Id,
                        Date = p.Date,
                        Amount = p.Amount,
                        InvoiceId = i.Id,
                        InvoiceReference = i.ReferenceNumber,
                        InvoiceDate = i.Date,
                        SupplierId = i.SupplierId,
                        Supplier = i.Supplier.Name,
                        ProjectId = i.ProjectId,
                        Project = i.Project.Name
                    }
                ).ToListAsync();
        }

        public async Task PayPaymentAsync(ProjectInvoicePaymentCreationDto paymentDto)
        {
            var payment = await GetProjectInvoicPaymentAsync(paymentDto.PaymentId);
            EnsureIsValidPayment(payment, paymentDto.CashList, paymentDto.ChecksList);

            //Build list of checks from dto
            var checkOutMovementsList = MapChecksForPayment(payment.Id, paymentDto.ChecksList);

            //Build cash list from dto
            var cashOutMovementsList = MapCashForPayment(payment.Id, paymentDto.CashList);

            //Change payment state
            payment.Done = true;

            //Add list of checks to context
            _context.CheckOutMovements.AddRange(checkOutMovementsList);

            //Add cash list to context
            _context.CashOutMovements.AddRange(cashOutMovementsList);

            //Save changes to DB
            await _context.SaveChangesAsync();
        }

        public async Task PayPaymentGroupAsync(ProjectInvoiceGroupPaymentCreationDto paymentDto)
        {
            var payments = await _context.ProjectInvoicePayments
                .Where(x => paymentDto.PaymentIds.Contains(x.Id))
                .ToListAsync();

            //Check if the retrieved list doesn't match dto info
            if (payments == null || payments.Count != paymentDto.PaymentIds.Count)
            {
                throw new ValidationException("retrieved payments doesn't match Payments ids");
            }

            EnsureIsValidPaymentGroup(payments, paymentDto.CashList, paymentDto.ChecksList);

            //Create PaymentGroup object
            var paymentGroup = new ProjectInvoicePaymentGroup { Date = DateTime.Now };

            //Add PaymentGroup to repository
            _context.ProjectInvoicePaymentGroups.Add(paymentGroup);

            //Start DB transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //Save current changes to DB
                await _context.SaveChangesAsync();

                //Build list of checks from dto
                var checkOutMovementsList = MapChecksForPaymentGroup(paymentGroup.Id, paymentDto.ChecksList);

                //Build cash list from dto
                var cashOutMovementsList = MapCashForPaymentGroup(paymentGroup.Id, paymentDto.CashList);

                //Assign group to every payment in the list and change each payment state
                foreach (var payment in payments)
                {
                    payment.IsGroup = true;
                    payment.GroupId = paymentGroup.Id;
                    payment.Done = true;
                }

                //Add list of checks to repository
                _context.CheckOutMovements.AddRange(checkOutMovementsList);

                //Add cash list to repository
                _context.CashOutMovements.AddRange(cashOutMovementsList);

                //Save current changes to DB
                await _context.SaveChangesAsync();

                //Commit DB transaction
                await transaction.CommitAsync();
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
        }

        // -----------------------------
        // Private helper methods
        // -----------------------------

        private async Task<ProjectInvoicePayment> GetProjectInvoicPaymentAsync(int id)
        {
            var projectInvoicePayment = await _context.ProjectInvoicePayments
                .SingleOrDefaultAsync(x => x.Id == id);

            if (projectInvoicePayment == null)
                throw new NotFoundException("Project Invoice Payment not found.");

            return projectInvoicePayment;
        }

        private void EnsureIsValidPayment(ProjectInvoicePayment payment, List<ProjectInvoiceCashCreationDto>? cashList,
            List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            decimal checks = 0;
            decimal cash = 0;

            //Calculate checks amount
            if (checkList != null && checkList.Count > 0)
            {
                checks = checkList.Sum(x => x.Amount);
            }

            //Calculate cash amount
            if (cashList != null && cashList.Count > 0)
            {
                cash = cashList.Sum(x => x.Amount);
            }

            //validate payment amount
            if (checks + cash != payment.Amount)
            {
                throw new ValidationException("Payment amount should be equal to paid amount");
            }
        }

        private void EnsureIsValidPaymentGroup(List<ProjectInvoicePayment> payments, List<ProjectInvoiceCashCreationDto>? cashList,
            List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            decimal checks = 0;
            decimal cash = 0;

            //Calculate checks amount
            if (checkList != null && checkList.Count > 0)
            {
                checks = checkList.Sum(x => x.Amount);
            }

            //Calculate cash amount
            if (cashList != null && cashList.Count > 0)
            {
                cash = cashList.Sum(x => x.Amount);
            }

            //Calcualte payment total from list of payments
            var paymentAmount = payments.Sum(x => x.Amount);
            var paidAmount = checks + cash;

            //Compare payment total with paid total
            if (paymentAmount != paidAmount)
            {
                throw new ValidationException("Payments amount should be equal to paid amount");
            }
        }

        private List<CheckOutMovement> MapChecksForPayment(int paymentId, List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            var checkOutMovementsList = new List<CheckOutMovement>();

            if (checkList != null)
            {
                foreach (var checkDto in checkList)
                {
                    checkOutMovementsList.Add(new CheckOutMovement
                    {
                        Date = checkDto.Date,
                        Amount = checkDto.Amount,
                        BankAcountId = checkDto.BankAccountId,
                        CheckNumber = checkDto.CheckNumber,
                        PaymentId = paymentId,
                        TransactionKind = TransactionKind.ProjectInvoice
                    });
                }
            }

            return checkOutMovementsList;
        }

        private List<CashOutMovement> MapCashForPayment(int paymentId, List<ProjectInvoiceCashCreationDto>? cashList)
        {
            var cashOutMovementsList = new List<CashOutMovement>();

            if (cashList != null)
            {
                foreach (var cashDto in cashList)
                {
                    cashOutMovementsList.Add(new CashOutMovement
                    {
                        Date = cashDto.Date,
                        Amount = cashDto.Amount,
                        PaymentId = paymentId,
                        TransactionKind = TransactionKind.ProjectInvoice
                    });
                }
            }

            return cashOutMovementsList;
        }

        private List<CheckOutMovement> MapChecksForPaymentGroup(int groupId, List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            var checkOutMovementsList = new List<CheckOutMovement>();

            if (checkList != null)
            {
                foreach (var checkDto in checkList)
                {
                    checkOutMovementsList.Add(new CheckOutMovement
                    {
                        Date = checkDto.Date,
                        Amount = checkDto.Amount,
                        BankAcountId = checkDto.BankAccountId,
                        CheckNumber = checkDto.CheckNumber,
                        PaymentId = groupId,
                        TransactionKind = TransactionKind.ProjectInvoice,
                        IsGroup = true
                    });
                }
            }

            return checkOutMovementsList;
        }

        private List<CashOutMovement> MapCashForPaymentGroup(int groupId, List<ProjectInvoiceCashCreationDto>? cashList)
        {
            var cashOutMovementsList = new List<CashOutMovement>();

            if (cashList != null)
            {
                foreach (var cashDto in cashList)
                {
                    cashOutMovementsList.Add(new CashOutMovement
                    {
                        Date = cashDto.Date,
                        Amount = cashDto.Amount,
                        PaymentId = groupId,
                        TransactionKind = TransactionKind.ProjectInvoice,
                        IsGroup = true
                    });
                }
            }

            return cashOutMovementsList;
        }

        
    }
}
