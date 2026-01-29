namespace MedicSoft.Application.DTOs.Dashboards
{
    /// <summary>
    /// DTO for dashboard widget display
    /// </summary>
    public class DashboardWidgetDto
    {
        public Guid Id { get; set; }
        public Guid DashboardId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Config { get; set; }
        public string Query { get; set; }
        public int RefreshInterval { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
    }

    /// <summary>
    /// DTO for creating a new widget
    /// </summary>
    public class CreateWidgetDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Config { get; set; }
        public string Query { get; set; }
        public int RefreshInterval { get; set; } = 0;
        public int GridX { get; set; }
        public int GridY { get; set; }
        public int GridWidth { get; set; } = 4;
        public int GridHeight { get; set; } = 3;
    }

    /// <summary>
    /// DTO for updating widget position on grid
    /// </summary>
    public class WidgetPositionDto
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
    }

    /// <summary>
    /// DTO for widget data query results
    /// </summary>
    public class WidgetDataDto
    {
        public Guid WidgetId { get; set; }
        public object Data { get; set; }
        public string Error { get; set; }
    }
}
