using YourExpo.Models;

namespace YourExpo.ViewModels;

public class SupplierDashboardViewModel
{
    

    public int PendingOrdersCount { get; set; }
    public int ShippedOrdersCount { get; set; }
    public int DeliveredOrdersCount { get; set; }
    public Dictionary<string, int> ProductCategoryDistribution { get; set; }
    
}
