namespace FoodOrdering.DTOs
{
    public class OrderQuery
    {
        public int? TableNumber { get; set; }
        public string? Status { get; set; }

        /// <summary>Filter orders whose OrderTime falls on this local calendar day (ignored when <see cref="AllDates"/> is true).</summary>
        public DateOnly? OrderDate { get; set; }

        /// <summary>When true, do not filter by <see cref="OrderDate"/>.</summary>
        public bool AllDates { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }
}
