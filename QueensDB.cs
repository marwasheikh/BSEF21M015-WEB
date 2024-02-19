using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenLocalDataHandling
{
    internal class Order
    {
        private int orderID;
        public int OrderID { get; set; }

        private string customerCNIC;
        public string CustomerCNIC { get; set; }

        private string customerName;
        public string CustomerName { get; set; }

        private string customerPhone;
        public string CustomerPhone { get; set; }

        private string customerAddress;
        public string CustomerAddress { get; set; }

        private int productID;
        public int ProductID { get; set; }

        private int price;
        public int Price { get; set; }

        private string productSize;
        public string ProductSize { get; set; }


    }
}
