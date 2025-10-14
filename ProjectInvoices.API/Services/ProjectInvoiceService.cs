using TaklaNew.API.Domain;
using TaklaNew.API.Domain.Enums;
using TaklaNew.API.Domain.IRepository;
using TaklaNew.API.Dtos;

namespace TaklaNew.API.Services
{
    /// <summary>
    /// Concrete implementation of IProjectInvoiceService
    /// </summary>
    public class ProjectInvoiceService : IProjectInvoiceService
    {
        private readonly IProjectInvoiceRepository _repository;

        public ProjectInvoiceService(IProjectInvoiceRepository projectInvoiceRepository)
        {
            //initialize repository
            _repository = projectInvoiceRepository;
        }

        /// <inheritdoc/>
        public async Task AddProjectInvoiceAsync(ProjectInvoice projectInvoice)
        {
            //Add invoice to repository
            _repository.AddProjectInvoice(projectInvoice);
            
            //Save changes to DB
            await _repository.SaveAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateProjectInvoiceAsync()
        {
            //Save changes to DB
            await _repository.SaveAsync();
        }

        /// <inheritdoc/>
        public async Task<ProjectInvoice?> GetProjectInvoiceWithItemsByIdAsync(int id)
        {
            //Retrieve project invoice with all its items by its id
            return await _repository.GetProjectInvoiceWithItemsByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task ApproveAsync(ProjectInvoice invoice)
        {
            //Change invoice state
            invoice.State = ProjectInvoiceState.Approved;

            //Calculate invoice total price
            var amount = invoice.Items.Sum(x => (decimal)x.Quantity * x.Price);

            //Create suggested payment object
            var invoicePayment = new ProjectInvoicePayment
            {
                Date = DateTime.Now,
                ProjectInvoiceId = invoice.Id,
                Amount = amount
            };

            //Add payment to repository
            _repository.AddProjectInvoicePayment(invoicePayment);
            
            //Save all changes to Db
            await _repository.SaveAsync();
        }

        /// <inheritdoc/>
        public async Task<ProjectInvoicePayment?> GetProjectInvoicePaymentByIdAsync(int id)
        {
            //Retrieve payment by its id
            return await _repository.GetProjectInvoicePaymentByIdAsync(id);
        }

        /// <inheritdoc/>
        public bool IsValidPayment(ProjectInvoicePayment payment, List<ProjectInvoiceCashCreationDto>? cashList,
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
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task PayPaymentAsync(ProjectInvoicePayment payment, List<ProjectInvoiceCashCreationDto>? cashList,
            List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            //Build list of checks from dto
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
                        PaymentId = payment.Id,
                        TransactionKind = TransactionKind.ProjectInvoice
                    });
                }
            }

            //Build cash list from dto
            var cashOutMovementsList = new List<CashOutMovement>();

            if (cashList != null)
            {
                foreach (var cashDto in cashList)
                {
                    cashOutMovementsList.Add(new CashOutMovement
                    {
                        Date = cashDto.Date,
                        Amount = cashDto.Amount,
                        PaymentId = payment.Id,
                        TransactionKind = TransactionKind.ProjectInvoice
                    });
                }
            }

            //Change payment state
            payment.Done = true;

            //Add list of checks to repository
            _repository.AddCheckOutMovements(checkOutMovementsList);
            
            //Add cash list to repository
            _repository.AddCashOutMovements(cashOutMovementsList);

            //Save changes to DB
            await _repository.SaveAsync();
        }

        /// <inheritdoc/>
        public async Task<List<ProjectInvoicePayment>> GetProjectInvoicePaymentsByIdsAsync(List<int> ids)
        {
            //Retrieve list of payments by their ids
            return await _repository.GetProjectInvoicePaymentsByIdsAsync(ids);
        }

        /// <inheritdoc/>
        public bool IsValidPaymentGroup(List<ProjectInvoicePayment> payments, List<ProjectInvoiceCashCreationDto>? cashList,
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
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> PayPaymentGroupAsync(List<ProjectInvoicePayment> payments, List<ProjectInvoiceCashCreationDto>? cashList,
            List<ProjectInvoiceCheckCreationDto>? checkList)
        {
            //Create PaymentGroup object
            var paymentGroup = new ProjectInvoicePaymentGroup { Date = DateTime.Now };

            //Add PaymentGroup to repository
            _repository.AddProjectInvoicePaymentGroup(paymentGroup);

            //Start DB transaction
            using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                //Save current changes to DB
                await _repository.SaveAsync();

                //Build list of checks from dto
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
                            PaymentId = paymentGroup.Id,
                            TransactionKind = TransactionKind.ProjectInvoice,
                            IsGroup = true
                        });
                    }
                }

                //Build cash list from dto
                var cashOutMovementsList = new List<CashOutMovement>();

                if (cashList != null)
                {
                    foreach (var cashDto in cashList)
                    {
                        cashOutMovementsList.Add(new CashOutMovement
                        {
                            Date = cashDto.Date,
                            Amount = cashDto.Amount,
                            PaymentId = paymentGroup.Id,
                            TransactionKind = TransactionKind.ProjectInvoice,
                            IsGroup = true
                        });
                    }
                }

                //Assign group to every payment in the list and change each payment state
                foreach (var payment in payments)
                {
                    payment.IsGroup = true;
                    payment.GroupId = paymentGroup.Id;
                    payment.Done = true;
                }

                //Add list of checks to repository
                _repository.AddCheckOutMovements(checkOutMovementsList);
                
                //Add cash list to repository
                _repository.AddCashOutMovements(cashOutMovementsList);

                //Save current changes to DB
                await _repository.SaveAsync();

                //Commit DB transaction
                await transaction.CommitAsync();

                return true;
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();
            }

            return false;
        }
    }
}
