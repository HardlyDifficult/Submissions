using System;
using System.Collections.Generic;

namespace HD
{
  [Serializable]
  public class CustomerManager : AbstractCustomerManager, IFixedUpdate
  {
    #region Data
    readonly List<Customer> customers = new List<Customer>();

    // .02 is the tickRate in FixedUpdate
    const double chanceNewOrder = .02 / 90;  // Roughly every 90sec
    const double chanceNewCustomer = .02 / 3600;    // Roughly every hour ( =3600sec )

    #endregion Data

    #region Init
    public CustomerManager()
    {
      AddCustomer();
      TimeController.Sub(this);
    }
    #endregion

    #region Events
    void IFixedUpdate.FixedUpdate(
      int timeStep)
    {
      for(int i = 0; i < timeStep; i++)
      {
        foreach(Customer customer in customers)
        {
          foreach(Order order in customer.orderList)
          {
            if(order.deadline < TimeController.totalDeltaTime)
            {
              customer.FailedOrder(order);
            }
          }
        }

        if(Rng.Double() < chanceNewCustomer) // TODO change to Rng next customer time
        {
          Customer newCustomer = AddCustomer();
          Order newOrder = new Order(RandomProductType(), RandomProductAmount());
          newOrder.Init(newCustomer);

          newCustomer.AddOrder(newOrder);

          // Remove an old customer without pending orders after creating a new one to prevent a memory leak
          foreach(Customer customer in customers)
          {
            if(customer.orderList.Count == 0)
            {
              customers.Remove(customer);
              break;
            }
          }
        }
        else if(Rng.Double() < chanceNewOrder && customers.Count > 0) // TODO change to cache RNG
        {
          Customer newCustomer = customers[Rng.Int(customers.Count)];
          Order newOrder = new Order(RandomProductType(), RandomProductAmount());
          newOrder.Init(newCustomer);

          newCustomer.AddOrder(newOrder);
          customers.Add(newCustomer);
        }
      }
    }
    #endregion

    #region Write API
    public Customer AddCustomer(
      uint reputation = 100)
    {
      return AddCustomer(Customer.RandomCustomerName(), reputation);
    }
    public Customer AddCustomer(
      string name,
      uint reputation = 100)
    {
      Customer customer = new Customer(name, reputation);
      customers.Add(customer);

      return customer;
    }

    protected override void Ship(
      Product product)
    {
      Order orderToSendThisTo = null;
      foreach(Customer customer in customers)
      {
        foreach(Order order in customer.orderList)
        {
          if(order.productType == product.attribute)
          {
            orderToSendThisTo = order;
            break; // TODO sub method to break both
          }
        }
        if (orderToSendThisTo != null) break;
      }

      if(orderToSendThisTo != null)
      {
        SendProduct(orderToSendThisTo, product);
      }
    }

    public void SendProduct(
      Order order,
      Product product)
    {
      order.SendProduct();
      order.customer.orderList.Remove(order);
      product.Destroy();
    }
    #endregion

    #region Read API
    uint RandomProductAmount()
    {
      //TODO Needs tweaking. Returns a value between 5 and 100 in steps of 5
      return Rng.Uint(1, 21) * 5;
    }

    ProductAttribute RandomProductType()
    {
      int productIndex = Rng.Int(ProductWrapper.productList.Count);
      return ProductWrapper.productList[productIndex].attribute;
    }
    #endregion
  }
}
