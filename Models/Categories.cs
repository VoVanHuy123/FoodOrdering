namespace FoodOrdering.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<MenuItems>? MenuItems { get; set; }
    }
}
