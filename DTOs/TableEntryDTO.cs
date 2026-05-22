namespace FoodOrdering.DTOs
{
    public class TableEntryDTO
    {
        public bool CanEnter { get; set; }
        public string Message { get; set; } = "";
        public TableDTO? Table { get; set; }
    }
}
