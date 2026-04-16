namespace FoodOrdering.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int ProductCount { get; set; }
    }

    public class CategoriesQuery
    {
        public string? Name { get; set; }
    }
}