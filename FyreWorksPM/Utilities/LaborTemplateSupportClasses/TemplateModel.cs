namespace FyreWorksPM.Utilities.LaborTemplateSupportClasses
{
    public class TemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public Dictionary<string, LaborCategory> Categories { get; set; }
        public LaborRate Journeyman { get; set; }
        public LaborRate Apprentice { get; set; }
    }
}
