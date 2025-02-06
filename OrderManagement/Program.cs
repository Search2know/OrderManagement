using System;
using System.Collections.Generic;
using System.IO;
using OrderManagement.Models;
using OrderManagement.Services;

namespace OrderManagement
{ 
    class Program
    {
        private const string ProductsFilePath = "data/products.csv";
        private const string OrdersFilePath = "data/orders.csv";
        
        public static void Main(string[] args)
        {
            List<string> errorMessages = new List<string>();
            
            var dataLoader = new CsvDataLoader();
            var orderProcessor = new OrderProcessor();

            Dictionary<int, Product> products;
            try
            {
                products = dataLoader.LoadProducts(ProductsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ErrorFormatter.FormatError("LoadProducts", ex.Message));
                return;
            }
            
            List<Order> orders = dataLoader.LoadOrders(OrdersFilePath, errorMessages);
            List<Order> validOrders = orderProcessor.ProcessOrders(orders, products, errorMessages);

            string outputDirectory = "Reports";
            
            var reportGenerator = new ReportGenerator(outputDirectory);

            reportGenerator.GenerateReport(validOrders, "report.txt");
            
            reportGenerator.WriteErrors(errorMessages, "errors.log");

            Console.WriteLine("Обработка заказов завершена.");
            Console.WriteLine($"Отчёт и лог ошибок в папке: {Path.Combine(Directory.GetCurrentDirectory(), outputDirectory)}");
            
        }
    }
}