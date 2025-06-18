using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Labor
{    
    public interface IBidLaborTemplateService
    {
        Task SaveBidLaborTemplateAsync(BidLaborTemplateDto templateDto);
    }

}
