using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Labor
{
    public interface IManualLaborHourService
    {
        Task<List<ManualLaborHourDto>> GetHoursByBidIdAsync(int bidId);
        Task SaveHoursAsync(Guid bidId, List<ManualLaborHourDto> hours);
    }
}
