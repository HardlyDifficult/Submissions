using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HD
{
  [Serializable]
  public class Order 
  {
    #region Data
    [NonSerialized]
    public Customer customer;

    public ProductAttribute productType { get; private set; }
    public uint deadline { get; private set; }
    #endregion Data

    #region Init
    public Order(
      ProductAttribute productType,
      uint amount)
    {
      this.productType = productType;

      //TODO Formula needs tweaking. Currently puts deadline to 5-15 minutes for 5-100 products.
      uint minutesToDeadline = 5u + amount / 10; 

      CustomerOrderController.PlaceOrder(productType);

      this.deadline = TimeController.totalDeltaTime + minutesToDeadline * 60 * 1000; // ish?
    }
    
    public void Init(
      Customer customer)
    {
      this.customer = customer;
    }
    #endregion 

    public void SendProduct()
    {
      customer.FulfillOrder(this);
    }
  }
}
