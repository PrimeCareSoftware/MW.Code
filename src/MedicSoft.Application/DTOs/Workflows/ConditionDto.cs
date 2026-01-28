namespace MedicSoft.Application.DTOs.Workflows
{
    public class ConditionDto
    {
        public string Field { get; set; }
        public string Operator { get; set; } // ==, !=, >, >=, <, <=, contains
        public object Value { get; set; }
    }
}
