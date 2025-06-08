namespace FyreWorksPM.DataAccess.DTO
{
    public class CreateLaborTemplateDto
    {
        public string TemplateName { get; set; }
        public bool IsDefault { get; set; }
        public List<LaborRateDto> LaborRates { get; set; } = new();
        public List<LocationHourDto> LocationHours { get; set; } = new();
    }

}
