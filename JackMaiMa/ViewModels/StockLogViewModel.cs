using JackMaiMa.Models;

namespace JackMaiMa.ViewModels
{
    public class StockLogViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public StockLog StockLog { get; set; }
    }
}