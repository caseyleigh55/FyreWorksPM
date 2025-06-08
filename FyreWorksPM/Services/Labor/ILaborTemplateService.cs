using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Labor
{
    public interface ILaborTemplateService
    {
        Task<List<LaborTemplateDto>> GetAllTemplatesAsync();
        Task<LaborTemplateDto> GetTemplateByIdAsync(Guid id);
        Task<LaborTemplateDto> CreateTemplateAsync(CreateLaborTemplateDto template);
        Task DeleteTemplateAsync(Guid id);
        Task<LaborTemplateDto?> GetDefaultTemplateAsync();
    }
}
