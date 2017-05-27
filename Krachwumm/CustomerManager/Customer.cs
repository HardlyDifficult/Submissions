using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HD
{
  [Serializable]
  public class Customer
  {
    #region Data
    public string name { get; private set; }
    public uint ordersFulfilled { get; private set; } = 0;
    public uint ordersFailed { get; private set; } = 0;

    public readonly List<Order> orderList = new List<Order>();

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

    #region Init
    public Customer(
      uint repuration = 100) 
      : this(RandomCustomerName(), repuration) { }

    public Customer(
      string name, 
      uint reputation = 100)
    {
      this.name = name;
      this.reputation = reputation;
    }

    [OnDeserialized]
    void OnDeserialized(
      StreamingContext context)
    {
      for(int i = 0; i < orderList.Count; i++)
      {
        Order order = orderList[i];
        order.Init(this);
      }
    }
    #endregion Construction

    #region Write API

    public void AddOrder(Order order)
    {
      orderList.Add(order);
    }

    public void AddOrder(ProductAttribute productType, uint amount)
    {
      Order newOrder = new Order(productType, amount);
      newOrder.Init(this);
      AddOrder(newOrder);
    }

    public void FulfillOrder(Order order)
    {
      ordersFulfilled++;
      failingStreak = 0;

      reputation++;
    }

    public void FailedOrder(Order order)
    {
      orderList.Remove(order);

      ordersFailed++;
      failingStreak++;

      reputation -= Math.Min(failingStreak, reputation);
      CustomerOrderController.CancelOrder(order.productType);
    }

    #endregion Write API

    internal static string RandomCustomerName()
    {
      //TODO Maybe something like random twitch sub names? e.g. "HardlyDifficult Inc.", "Krachwumm Corp."
      return (new Random()).Next(10000000, 99999999).ToString() + " Inc.";
    }
  }
}
