using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    public class Product
    {
        //This class is temporary.
        //Replace its references with the real way of accessing the product/productTypes
        public enum ProductType
        {
            whatever
        }

        public ProductType type { get; private set; }
    }
}
