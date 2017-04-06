using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    public class Order
    {
        #region Data

        public Customer customer { get; private set; }

        public Product.ProductType productType { get; private set; }
        public uint amount { get; private set; }
        public uint amountLeft { get; private set; }
        public DateTime deadline { get; private set; } //TODO chance DateTime to something that works with pausing etc.

        #endregion Data

        #region Construction

        public Order(Customer customer, Product.ProductType productType, uint amount)
        {
            this.customer = customer;

            this.productType = productType;
            this.amount = amount;
            this.amountLeft = amount;

            int minutesToDeadline = 5 + (int)amount / 10; //TODO Formula needs tweaking. Currently puts deadline to 5-15 minutes for 5-100 products.

            this.deadline = DateTime.Now.AddMinutes(minutesToDeadline);
        }

        #endregion Construction

        public void Fulfill()
        {
            customer.FulfillOrder(this);
        }

        public void SendProduct(uint amount = 1)
        {
            amountLeft -= amount;

            if (amountLeft <= 0)
            {
                Fulfill();
            }
        }
    }
}
