using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    public class Customer
    {
        #region Data

        public string name { get; private set; }
        public uint ordersFulfilled { get; private set; } = 0;
        public uint ordersFailed { get; private set; } = 0;

        public List<Order> orders { get; private set; } = new List<Order>();

        public uint failingStreak { get; private set; } = 0;

        /// <summary>
        /// The reputation starts off at 100.
        /// Fulfilling an order adds 1.
        /// Failing an order subtracts however many you've failed back-to-back.
        /// </summary>
        public uint reputation { get; private set; }

        #endregion Data

        #region Properties

        public uint TotalOrders
        {
            get
            {
                return ordersFulfilled + ordersFailed;
            }
        }

        #endregion Properties

        #region Construction
        public Customer(uint repuration = 100)
        {
            this.name = RandomCustomerName();
            this.reputation = reputation;
        }
        public Customer(string name, uint reputation = 100)
        {
            this.name = name;
            this.reputation = reputation;
        }

        #endregion Construction

        #region Write API

        public void AddOrder(Order order)
        {
            orders.Add(order);
        }
        public void AddOrder(Product.ProductType productType, uint amount)
        {
            AddOrder(new Order(this, productType, amount));
        }

        public void FulfillOrder(Order order)
        {
            orders.Remove(order);

            ordersFulfilled++;
            failingStreak = 0;

            reputation++;
        }

        public void FailedOrder(Order order)
        {
            orders.Remove(order);

            ordersFailed++;
            failingStreak++;

            reputation -= Math.Min(failingStreak, reputation);
        }

        #endregion Write API

        private static string RandomCustomerName()
        {
            //TODO Maybe something like random twitch sub names? e.g. "HardlyDifficult Inc.", "Krachwumm Corp."
            return (new Random()).Next(10000000, 99999999).ToString() + " Inc.";
        }
    }
}
