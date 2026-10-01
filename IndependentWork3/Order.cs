using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork3;

public class Order
{
    private readonly List<OrderItem> _orderItems = [];

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public DateTime OrderDate { get; } = DateTime.UtcNow;

    public void AddOrderItem(int productId, string productName, decimal unitPrice, int units = 1)
    {
        var existingOrderForProduct = _orderItems.SingleOrDefault(o => o.ProductId == productId);

        if (existingOrderForProduct != null)
        {
            existingOrderForProduct.AddUnits(units);
        }
        else
        {
            var orderItem = new OrderItem(productId, productName, unitPrice, units);
            _orderItems.Add(orderItem);
        }
    }
}