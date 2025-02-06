using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OrderManagement.Models;

namespace OrderManagement.Services
{
    public class CsvDataLoader
    {
        public Dictionary<int, Product> LoadProducts(string filePath)
        {
            var products = new Dictionary<int, Product>();

            if (!File.Exists(filePath))
                throw new FileNotFoundException(ErrorFormatter.FormatError("Файл", $"Файл {filePath} не найден."));

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');
                if (parts.Length != 3)
                    throw new FormatException(ErrorFormatter.FormatError("Формат строки",
                        $"Неверный формат строки: {line}"));

                if (!int.TryParse(parts[0], out int id))
                    throw new FormatException(ErrorFormatter.FormatError("Парсинг Id",
                        $"Ошибка при парсинге Id в строке: {line}"));

                string name = parts[1];

                if (!decimal.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    throw new FormatException(ErrorFormatter.FormatError("Парсинг цены",
                        $"Ошибка при парсинге цены в строке: {line}"));

                if (!products.ContainsKey(id))
                    products.Add(id, new Product { Id = id, Name = name, Price = price });
                else
                    throw new Exception(ErrorFormatter.FormatError("Дублирование товара",
                        $"Дублирование товара с Id {id}."));
            }

            return products;
        }


        public List<Order> LoadOrders(string filePath, List<string> errorMessages)
        {
            var orders = new List<Order>();

            if (!File.Exists(filePath))
            {
                errorMessages.Add(ErrorFormatter.FormatError("Файл", $"Файл {filePath} не найден."));
                return orders;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(';');
                if (parts.Length != 3)
                {
                    errorMessages.Add(ErrorFormatter.FormatError("Формат строки заказа",
                        $"Неверный формат строки заказа: {line}"));
                    continue;
                }

                Order order = new Order();

                if (!int.TryParse(parts[0], out int orderId))
                {
                    errorMessages.Add(ErrorFormatter.FormatError("Парсинг OrderId",
                        $"Ошибка при парсинге OrderId в строке: {line}"));
                    continue;
                }

                order.Id = orderId;
                order.CustomerName = parts[1];

                string itemsPart = parts[2];
                if (string.IsNullOrWhiteSpace(itemsPart))
                {
                    errorMessages.Add(ErrorFormatter.FormatError("Элементы заказа",
                        $"Ошибка: заказ {order.Id} не содержит элементов заказа."));
                    continue;
                }

                var itemStrings = itemsPart.Split(',');
                bool hasInvalidItem = false;

                foreach (var itemStr in itemStrings)
                {
                    if (string.IsNullOrWhiteSpace(itemStr))
                    {
                        errorMessages.Add(ErrorFormatter.FormatError("Элемент заказа",
                            $"Ошибка в заказе {order.Id}: пустой элемент заказа."));
                        hasInvalidItem = true;
                        continue;
                    }

                    var itemParts = itemStr.Split(':');
                    if (itemParts.Length != 2)
                    {
                        errorMessages.Add(ErrorFormatter.FormatError("Формат элемента заказа",
                            $"Ошибка в элементе заказа '{itemStr}': Неверный формат."));
                        hasInvalidItem = true;
                        continue;
                    }

                    if (!int.TryParse(itemParts[0], out int productId))
                    {
                        errorMessages.Add(ErrorFormatter.FormatError("Парсинг ProductId",
                            $"Ошибка в элементе заказа '{itemStr}': Значение '{itemParts[0]}' не является числом."));
                        hasInvalidItem = true;
                        continue;
                    }

                    if (!int.TryParse(itemParts[1], out int quantity))
                    {
                        errorMessages.Add(ErrorFormatter.FormatError("Парсинг Quantity",
                            $"Ошибка в элементе заказа '{itemStr}': Значение '{itemParts[1]}' не является числом."));
                        hasInvalidItem = true;
                        continue;
                    }

                    order.Items.Add(new OrderItem { ProductId = productId, Quantity = quantity });
                }

                if (hasInvalidItem)
                {
                    errorMessages.Add(ErrorFormatter.FormatError("Невалидный заказ",
                        $"Заказ {order.Id} содержит невалидные товары, заказ пропущен"));
                    continue;
                }

                orders.Add(order);
            }

            return orders;
        }
    }
}