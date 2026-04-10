using FoodOrdering.DTOs;

namespace FoodOrdering.ViewModels
{
    public class ReportViewModel
    {
        public string Range { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<ReportDTO> Revenue { get; set; }
        public List<ReportDTO> Orders { get; set; }
        public List<ReportDTO> TopFoods { get; set; }
    }
}