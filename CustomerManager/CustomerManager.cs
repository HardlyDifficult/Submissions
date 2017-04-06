using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    public class CustomerManager
    {
        #region Data

        private Random rand = new Random();

        private List<Customer> customers = new List<Customer>();

        //TODO Chances need tweaking.
        private double chanceNewOrder = 0.0001;        //Roughly every 90sec

        private double chanceNewCustomer = 0.000005;    //Roughly every hour

        #endregion Data

        public void FixedUpdate()
        {
            foreach (Customer customer in customers)
            {
                foreach (Order order in customer.orders)
                {
                    if (order.deadline < DateTime.Now)
                    {
                        customer.FailedOrder(order);
                    }
                }
            }

            if (rand.NextDouble() < chanceNewCustomer)
            {
                Customer newCustomer = new Customer();
                Order newOrder = new Order(newCustomer, RandomProductType(), RandomProductAmount());

                newCustomer.AddOrder(newOrder);
                customers.Add(newCustomer);

                // Remove an old customer without pending orders after creating a new one to prevent a memory leak
                foreach (Customer customer in customers)
                {
                    if (customer.orders.Count == 0)
                    {
                        customers.Remove(customer);
                        break;
                    }
                }
            }
            else if (rand.NextDouble() < chanceNewOrder)
            {
                Customer newCustomer = customers[rand.Next(0, customers.Count)];
                Order newOrder = new Order(newCustomer, RandomProductType(), RandomProductAmount());

                newCustomer.AddOrder(newOrder);
                customers.Add(newCustomer);
            }
        }

        public Customer AddCustomer(string name, uint reputation = 100)
        {
            Customer customer = new Customer(name, reputation);
            customers.Add(customer);

            return customer;
        }

        #region Write API

        public uint SendProductIfOrdered(Product product, uint amount)
        {
            uint sent = 0;

            foreach (Customer customer in customers)
            {
                foreach (Order order in customer.orders)
                {
                    if (order.productType == product.type)
                    {
                        uint amountSent = SendProduct(order, amount);

                        order.SendProduct(amountSent);

                        if (amountSent == amount)
                        {
                            return amountSent;
                        }
                        amount -= amountSent;
                        sent += amountSent;
                    }
                }
            }

            return sent;
        }

        public uint SendProduct(Order order, uint amount)
        {
            uint amountToSend = Math.Min(amount, order.amountLeft);
            order.SendProduct(amountToSend);

            return amountToSend;
        }

        #endregion Write API

        private uint RandomProductAmount()
        {
            //TODO Needs tweaking. Returns a value between 5 and 100 in steps of 5
            return (uint)rand.Next(1, 21) * 5;
        }

        private Product.ProductType RandomProductType()
        {
            //TODO Make it return a random product type
            return Product.ProductType.whatever;
        }
    }
}
