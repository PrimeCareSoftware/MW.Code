namespace MedicSoft.Application.DTOs.Dashboards
{
    /// <summary>
    /// DTO for widget template display
    /// </summary>
    public class WidgetTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string DefaultConfig { get; set; }
        public string DefaultQuery { get; set; }
        public bool IsSystem { get; set; }
        public string Icon { get; set; }
    }
}
