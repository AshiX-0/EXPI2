using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace TESTS_PI2
{
    public class Order : Entry
    {
        private bool isBuy;
        private double executedVolume;
        private double sendVolume;
        private double price;

        public Order() : base()
        {
            isBuy = true;
            executedVolume = 0;
            sendVolume = 0;
            price = 0;
        }

        public Order(string source, string symbol, DateTime timestamp, double bid, double ask, bool isBuy, double executedVolume, double sendVolume, double price) : base(source, symbol, timestamp, bid, ask)
        {
            this.isBuy = isBuy;
            this.executedVolume = executedVolume;
            this.sendVolume = sendVolume;
            this.price = price;
        }

        public Order(Entry entry, bool isBuy, double executedVolume, double sendVolume, double price) : base(entry.Source(), entry.Symbol(), entry.Timestamp(), entry.Bid(), entry.Ask())
        {
            this.isBuy = isBuy;
            this.executedVolume = executedVolume;
            this.sendVolume = sendVolume;
            this.price = price;
        }


        public bool IsBuy { get { return isBuy; } }
        public double ExecutedVolume { get { return executedVolume; } }
        public double SendVolume { get { return sendVolume; } }
        public double Price { get { return price; } }

        public override string ToString()
        {
            if (isBuy)
            {
                return base.ToString() + " ,Buy Volume : " + sendVolume.ToString() + " ,Buy Volume Executed : " + executedVolume.ToString() + " ,at : " + price.ToString();
            }
            else
            {
                return base.ToString() + " ,Sell Volume : " + sendVolume.ToString() + " ,Sell Volume Executed : " + executedVolume.ToString() + " ,at : " + price.ToString();
            }
        }

        public static List<Order> ReadingOrders(string filePath)
        {
            List<Order> orders = new List<Order>();

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] cells = lines[i].Split(',');

                string source = cells[0];
                string symbol = cells[1];

                // Parse timestamp
                DateTime timestamp;
                string format = "yyyy-MM-dd HH:mm:ss.fffZ";
                if (!DateTime.TryParseExact(cells[2], format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out timestamp))
                {
                    Console.WriteLine("Invalid timestamp format: " + cells[2]);
                    continue;
                }

                double bid, ask, executedVolume, sendVolume, price;

                if (!double.TryParse(cells[3], NumberStyles.Float, CultureInfo.InvariantCulture, out bid))
                {
                    Console.WriteLine("Invalid bid format: " + cells[3]);
                    continue;
                }

                if (!double.TryParse(cells[4], NumberStyles.Float, CultureInfo.InvariantCulture, out ask))
                {
                    Console.WriteLine("Invalid ask format: " + cells[4]);
                    continue;
                }

                bool isBuy = cells[5].Replace("\"", "").ToUpper() == "BUY";


                if (!double.TryParse(cells[6], NumberStyles.Float, CultureInfo.InvariantCulture, out executedVolume))
                {
                    Console.WriteLine("Invalid volume format: " + cells[6]);
                    continue;
                }

                if (!double.TryParse(cells[7], NumberStyles.Float, CultureInfo.InvariantCulture, out sendVolume))
                {
                    Console.WriteLine("Invalid volume format: " + cells[7]);
                    continue;
                }
                if (!double.TryParse(cells[8], NumberStyles.Float, CultureInfo.InvariantCulture, out price))
                {
                    Console.WriteLine("Invalid volume format: " + cells[8]);
                    continue;
                }

                Order order = new Order(source, symbol, timestamp, bid, ask, isBuy, executedVolume, sendVolume, price);
                orders.Add(order);
            }
            return orders;
        }
        public static List<Order> FilterOrdersBySymbol(List<Order> orders, string symbol)
        {
            List<Order> filteredOrders = new List<Order>();
            foreach (Order order in orders)
            {
                if (order.Symbol().ToLower() == symbol)
                {
                    filteredOrders.Add(order);
                }
            }
            return filteredOrders;
        }

        public static List<Order> FilterOrdersByDate(List<Order> orders, DateTime startDate, DateTime endDate)
        {
            List<Order> filteredOrders = new List<Order>();
            foreach (Order order in orders)
            {
                if (order.Timestamp() >= startDate && order.Timestamp() <= endDate)
                {
                    filteredOrders.Add(order);
                }
            }
            return filteredOrders;
        }

        public static List<Order> FilterOrdersByHour(List<Order> orders, int startHour, int endHour)
        {
            List<Order> filteredOrders = new List<Order>();

            // Validate the input
            if (startHour < 0 || startHour > 23)
            {
                throw new ArgumentException("startHour must be between 0 and 23.");
            }

            if (endHour < 0 || endHour > 23)
            {
                throw new ArgumentException("endHour must be between 0 and 23.");
            }

            if (endHour < startHour)
            {
                throw new ArgumentException("endHour must be greater than or equal to startHour.");
            }

            // Filter the orders by hour
            foreach (Order order in orders)
            {
                int orderHour = order.Timestamp().Hour;

                if (orderHour >= startHour && orderHour <= endHour)
                {
                    filteredOrders.Add(order);
                }
            }

            return filteredOrders;
        }

        public static List<Order> FilterOrdersByType(List<Order> orders, bool isBuy)
        {
            List<Order> filteredOrders = new List<Order>();

            foreach (Order order in orders)
            {
                if (order.IsBuy == isBuy)
                {
                    filteredOrders.Add(order);
                }
            }

            return filteredOrders;
        }

        public static List<Order> FilterOrdersByExecutedVolumeProportion(List<Order> orders, double threshold)
        {
            // Validate the input
            if (threshold < 0 || threshold > 100)
            {
                throw new ArgumentException("threshold must be between 0 and 100.");
            }

            List<Order> filteredOrders = new List<Order>();
            foreach (Order order in orders)
            {
                double proportion = order.ExecutedVolume / order.SendVolume;
                if (proportion >= threshold / 100)
                {
                    filteredOrders.Add(order);
                }
            }
            return filteredOrders;
        }
        public static List<Order> FilterAny(List<Order> orders, bool isBuy, double minfillrate = 0, string symbol = "", DateTime? startDate = null, DateTime? endDate = null, int startHour = 0, int endHour = 23 )
        {
            List<Order> filteredOrders = new List<Order>();
            if (minfillrate > 0)
            {
                filteredOrders = FilterOrdersByExecutedVolumeProportion(orders, minfillrate);
            }
            if (symbol != "")
            {
                filteredOrders = FilterOrdersBySymbol(filteredOrders, symbol);
            }
            if (startDate != null && endDate != null)
            {
                filteredOrders = FilterOrdersByDate(filteredOrders, (DateTime)startDate, (DateTime)endDate);
            }
            if (startHour != 0 || endHour != 23)
            {
                filteredOrders = FilterOrdersByHour(filteredOrders, startHour, endHour);
            }
            if (isBuy != true)
            {
                filteredOrders = FilterOrdersByType(filteredOrders, isBuy);
            }
            return filteredOrders;
        }
    }
}
