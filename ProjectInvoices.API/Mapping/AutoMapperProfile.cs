using AutoMapper;
using ProjectInvoices.API.Domain;
using ProjectInvoices.API.Domain.Enums;
using ProjectInvoices.API.Dtos;

namespace ProjectInvoices.API.Mapping
{
    /// <summary>
    /// Provides a profile used by AutoMapper to map objects
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Bank, BankDto>();
            CreateMap<Bank, BankUpdateGetDto>();
            CreateMap<BankCreationDto, Bank>();
            CreateMap<BankUpdateDto, Bank>();

            CreateMap<BankAccount, BankAccountDto>().ForMember(x => x.Bank, m => m.MapFrom(x => x.Bank.Name));
            CreateMap<BankAccount, BankAccountUpdateGetDto>();
            CreateMap<BankAccountCreationDto, BankAccount>();
            CreateMap<BankAccountUpdateDto, BankAccount>();

            CreateMap<Item, ItemDto>().ForMember(x => x.AffectInventory, m => m.MapFrom(x => x.AffectInventory ? "Yes": "No"));
            CreateMap<Item, ItemUpdateGetDto>();
            CreateMap<ItemCreationDto, Item>();
            CreateMap<ItemUpdateDto, Item>();

            CreateMap<Project, ProjectDto>().ForMember(x => x.State, m => m.MapFrom(x => x.State.ToString()));
            CreateMap<Project, ProjectUpdateGetDto>().ForMember(x => x.State, m => m.MapFrom(x => (short)x.State)); 
            CreateMap<ProjectCreationDto, Project>().ForMember(x => x.State, m => m.MapFrom(x => (ProjectState)x.State));
            CreateMap<ProjectUpdateDto, Project>().ForMember(x => x.State, m => m.MapFrom(x => (ProjectState)x.State));

            CreateMap<Supplier, SupplierDto>();
            CreateMap<Supplier, SupplierUpdateGetDto>();
            CreateMap<SupplierCreationDto, Supplier>();
            CreateMap<SupplierUpdateDto, Supplier>();

            CreateMap<ProjectInvoiceItemCreationDto, ProjectInvoiceItem>();
            CreateMap<ProjectInvoiceCreationDto, ProjectInvoice>().ForMember(x => x.State, m => m.MapFrom(x => (ProjectInvoiceState)x.State));

            CreateMap<ProjectInvoiceItemUpdateDto, ProjectInvoiceItem>();
            CreateMap<ProjectInvoiceUpdateDto, ProjectInvoice>().ForMember(x => x.Items, m => m.Ignore())
                .ForMember(x => x.State, m => m.MapFrom(x => (ProjectInvoiceState)x.State))
                .AfterMap(MapProjectInvoiceItems);

            CreateMap<ProjectInvoiceItem, ProjectInvoiceItemUpdateGetDto>();
            CreateMap<ProjectInvoice, ProjectInvoiceUpdateGetDto>().ForMember(x => x.State, m => m.MapFrom(x => (short)x.State));
        }

        /// <summary>
        /// Map list of invoice items in the dto to list of items in invoice object 
        /// By update existing ones, add new ones, delete not existed ones
        /// </summary>
        private void MapProjectInvoiceItems(ProjectInvoiceUpdateDto projectInvoiceUpdateDto, ProjectInvoice projectInvoice)
        {
            foreach (var item in projectInvoiceUpdateDto.Items)
            {
                if (item.Id == 0)
                {
                    //Add new item
                    projectInvoice.Items.Add(new ProjectInvoiceItem { ItemId = item.ItemId, Unit = item.Unit, Quantity = item.Quantity, Price = item.Price, ProjectInvoiceId = projectInvoice.Id });
                }
                else
                {
                    var pIItem = projectInvoice.Items.SingleOrDefault(x => x.Id == item.Id);
                    //Update existing item
                    if (pIItem != null)
                    {
                        pIItem.ItemId = item.ItemId;
                        pIItem.Price = item.Price;
                        pIItem.Quantity = item.Quantity;
                        pIItem.Unit = item.Unit;
                    }
                }
            }

            //Get list of invoice item ids that don't exist in dto
            var projectInvoiceItemsToDeleteIds = projectInvoice.Items.Where(x => !projectInvoiceUpdateDto.Items.Select(p => p.Id).Contains(x.Id)).Select(x => x.Id).ToList();

            foreach (var id in projectInvoiceItemsToDeleteIds)
            {
                //Remove deleted item
                projectInvoice.Items.Remove(projectInvoice.Items.First(x => x.Id == id));
            }
        }
    }
}
