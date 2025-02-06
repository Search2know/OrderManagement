using System.Collections.Generic;
using System.IO;
using System.Linq;
using OrderManagement.Models;

namespace OrderManagement.Services
{
    public class ReportGenerator
    {
        private readonly string _outputDirectory;


        public ReportGenerator(string outputDirectory)
        {
            _outputDirectory = outputDirectory;

            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        public void GenerateReport(List<Order> orders, string reportFileName)
        {
            int totalOrders = orders.Count;
            decimal totalRevenue = orders.Sum(o => o.TotalPrice);
            Order maxOrder = orders.OrderByDescending(o => o.TotalPrice).FirstOrDefault();

            var reportLines = new List<string>
            {
                $"Общее количество заказов: {totalOrders}",
                $"Суммарная выручка: {totalRevenue} руб."
            };

            if (maxOrder != null)
            {
                reportLines.Add($"Самый дорогой заказ: №{maxOrder.Id} ({maxOrder.TotalPrice} руб.)");
            }

            string fullPath = Path.Combine(_outputDirectory, reportFileName);
            File.WriteAllLines(fullPath, reportLines);
        }


        public void WriteErrors(List<string> errorMessages, string errorsFileName)
        {
            string fullPath = Path.Combine(_outputDirectory, errorsFileName);
            File.WriteAllLines(fullPath, errorMessages);
        }
    }
}