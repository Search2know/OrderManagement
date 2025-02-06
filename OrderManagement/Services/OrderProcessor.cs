using System.Collections.Generic;
using OrderManagement.Models;

namespace OrderManagement.Services
{
    public class OrderProcessor
    {
        public List<Order> ProcessOrders(List<Order> orders, Dictionary<int, Product> products,
            List<string> errorMessages)
        {
            var validOrders = new List<Order>();

            foreach (var order in orders)
            {
                decimal totalPrice = 0;
                var validItems = new List<OrderItem>();
                bool hasInvalidItem = false;

                foreach (var item in order.Items)
                {
                    if (products.ContainsKey(item.ProductId))
                    {
                        Product product = products[item.ProductId];
                        totalPrice += product.Price * item.Quantity;
                        validItems.Add(item);
                    }
                    else
                    {
                        errorMessages.Add(ErrorFormatter.FormatError("Проверка продукта", $"Ошибка в элементе заказа '{item.ProductId}:{item.Quantity}': Товар {item.ProductId} не найден"));
                        hasInvalidItem = true;
                    }
                }

                if (hasInvalidItem)
                {
                    errorMessages.Add(ErrorFormatter.FormatError("Невалидный заказ", $"Заказ {order.Id} содержит невалидные товары, заказ пропущен"));
                    continue;
                }

                order.Items = validItems;
                order.TotalPrice = totalPrice;
                validOrders.Add(order);
            }

            return validOrders;
        }
    }
}