using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.Services.Labor
{
    public interface ILaborTemplateService
    {
        Task<List<LaborTemplateDto>> GetAllTemplatesAsync();
        Task<LaborTemplateDto> GetTemplateByIdAsync(int id);
        Task<LaborTemplateDto> CreateTemplateAsync(CreateLaborTemplateDto template);
        Task DeleteTemplateAsync(int id);
        Task<LaborTemplateDto?> GetDefaultTemplateAsync();
        Task<bool> UpdateTemplateAsync(int id, CreateLaborTemplateDto dto);

    }
}
