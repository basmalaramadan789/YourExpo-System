using YourExpo.Models;

namespace YourExpo.Services;

public interface IOrderService
{
    void CreateOrder(Order order);
}
