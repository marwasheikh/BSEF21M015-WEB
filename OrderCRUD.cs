using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueenLocalDataHandling
{
    internal class OrderCRUD
    {
        public void InsertOrder(Order order)
        {
            Console.WriteLine("Enter Order Details");
            Console.WriteLine("Order ID");
            order.OrderID =int.Parse( Console.ReadLine());
         
            Console.WriteLine("Customer's CNIC");
            order.CustomerCNIC = Console.ReadLine();
            if (order.CustomerCNIC.Length > 15)
            {
                Console.WriteLine("Customer CNIC exceeds maximum length (15 characters).");
                return;
            }
            Console.WriteLine("Customer's Name");
            order.CustomerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(order.CustomerName))
            {
                Console.WriteLine("Customer name and phone number are required.");
                return;
            }
            Console.WriteLine("Customer's Phone");
            if (string.IsNullOrWhiteSpace(order.CustomerPhone) || !Regex.IsMatch(order.CustomerPhone, @"^\d{10}$"))
            {
                Console.WriteLine("Invalid phone number");
                return;
            }
            order.CustomerPhone = Console.ReadLine();
            Console.WriteLine("Customer's Address");
            order.CustomerAddress = Console.ReadLine();
            Console.WriteLine("Product ID");
            order.ProductID = int.Parse(Console.ReadLine());
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid input for  product ID");
                return;
            }

            Console.WriteLine("Price");
            order.Price = int.Parse(Console.ReadLine());
            if (!int.TryParse(Console.ReadLine(), out int price))
            {
                Console.WriteLine("Invalid input for order ID, product ID, or price.");
                return;
            }

            Console.WriteLine("Size of the Product");
            order.ProductSize = Console.ReadLine();
            string[] validSizes = { "small", "medium", "large" };
            if (!validSizes.Contains(order.ProductSize.ToLower()))
            {
                Console.WriteLine("Invalid product size. Allowed sizes are: small, medium, large.");
                return;
            }

            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Orders;Integrated Security=True;";
            string query = "INSERT INTO [order](OrderId, CustomerCNIC, CustomerName, CustomerPhone, CustomerAddress, ProductID, Price, Size) " +
                  $"VALUES ({order.OrderID}, '{order.CustomerCNIC}', '{order.CustomerName}', '{order.CustomerPhone}', '{order.CustomerAddress}', {order.ProductID}, {order.Price}, '{order.ProductSize}')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                 command.ExecuteNonQuery();

            }
            Console.WriteLine("order placed successfully");

        }
        public void GetAllOrders()
        {
            Console.WriteLine("View Orders Details");
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Orders;Integrated Security=True;";
            string query = "select * from [order]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                    int orderid = reader.GetInt32(0);
                    string cnic = reader.GetString(1);
                    string name = reader.GetString(2);
                    string phone = reader.GetString(3);
                    string address = reader.GetString(4);
                    int proid = reader.GetInt32(5);
                    int price = reader.GetInt32(6);
                    string size = reader.GetString(7);

                        Console.WriteLine("Order ID" + " = "+ " " +orderid);
                        Console.WriteLine("Customer's CNIC" +" = "+ " " + cnic);
                        Console.WriteLine("Customer's Name" + " = " + " " + name);
                        Console.WriteLine("Customer's Phone" + " = " + " " + phone);
                        Console.WriteLine("Customer's Address" + " = " + " " +address );
                        Console.WriteLine("Product ID" + " = " + " " + proid);
                        Console.WriteLine("Price" + " = " + " " + price);
                        Console.WriteLine("Size of the Product" + " = " + " " +size);
                    }
                
            }
        }


        public void UpdateAddress(string CustomerPhone, string newAddress)
        {
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Orders;Integrated Security=True;";
            string query = "UPDATE [order] SET CustomerAddress='" + newAddress + "' WHERE CustomerPhone='" + CustomerPhone + "'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
               

                int rows =command.ExecuteNonQuery();

               if(rows>0)
                {
                    Console.Write("update successfully");

                }
                else
                {
                    Console.WriteLine("customer not exist");
                }

            }
        }
        public void UpdateAddresspara(string CustomerPhone, string newAddress)
        {
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Orders;Integrated Security=True;";
            string query = "UPDATE [order] set CustomerAddress=@newAddress where CustomerPhone=@CustomerPhone";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@newAddress", newAddress);
                command.Parameters.AddWithValue("@CustomerPhone", CustomerPhone);

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.Write("update successfully");

                }
                else
                {
                    Console.WriteLine("customer not exist");
                }

            }
        }
        public void Delete(int OrderId)
        {
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Orders;Integrated Security=True;";
            string query = "delete from [order] where OrderId="+ OrderId;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.Write("delete successfully");

                }
                else
                {
                    Console.WriteLine("customer not exist");
                }




            }
        }
        static void Main(string[] args)
        {
            OrderCRUD oc = new OrderCRUD();
            Order o = new Order();
            int choice;
            do
            {
                Console.WriteLine("-------- Menu --------");
                Console.WriteLine("1. Place Order");
                Console.WriteLine("2. View All Orders");
                Console.WriteLine("3. Update Order");
                Console.WriteLine("4. Delete Order");
                Console.WriteLine("5. Exit");
                Console.Write("Enter your choice: ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            oc.InsertOrder(o);
                            break;
                        case 2:
                            oc.GetAllOrders();
                            break;
                        case 3:
                            Console.Write("Enter Customer's Phone: ");
                            string phone = Console.ReadLine();
                            Console.Write("Enter new Address: ");
                            string address = Console.ReadLine();
                            oc.UpdateAddress(phone, address);
                            break;
                        case 4:
                            Console.Write("Enter Order ID to delete: ");
                            int orderIdToDelete;
                            if (int.TryParse(Console.ReadLine(), out orderIdToDelete))
                            {
                                oc.Delete(orderIdToDelete);
                            }
                            else
                            {
                                Console.WriteLine("Invalid Order ID");
                            }
                            break;
                        case 5:
                            Console.WriteLine("Exiting...");
                            break;
                        default:
                            Console.WriteLine("Invalid choice! Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Please enter a number.");
                }
            } while (choice != 5);
        }
    }
}
 

