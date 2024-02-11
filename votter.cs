using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class votter
    {
        private string votterName;
        private string cnic;
        private string selectedpartyName;
        public votter(string vname=" ",string cni=" ",string selecpname=" ")
        {
            votterName = vname;
            cnic = cni;
            selectedpartyName = selecpname;
        }
        public string SelectedPartyName
        {
            set;
            get;
        }
        public string Cnic
        {
            set;
            get;
        }
        public string Name
        {
            get;
            set;
        }
        public bool HasVoted(string cnic)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Votter;Integrated Security=True;";
            string query = "SELECT COUNT(*) FROM Votter WHERE Cnic = @CNIC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CNIC", cnic);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }




    }
}
