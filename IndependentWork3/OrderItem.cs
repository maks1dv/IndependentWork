using System;

namespace IndependentWork3;

public class OrderItem(int productId, string productName, decimal unitPrice, int units = 1)
{
    public int ProductId { get; } = productId;
    public string ProductName { get; } = productName;
    public decimal UnitPrice { get; } = unitPrice;
    public int Units { get; private set; } = units;

    public void AddUnits(int addedUnits)
    {
        if (addedUnits < 0)
        {
            throw new ArgumentException("Кількість товару не може бути від'ємною.");
        }

        Units += addedUnits;
    }
}