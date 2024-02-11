using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Diagnostics.Metrics;

namespace ConsoleApp1
{
    internal class votermachine
    {
        private List<candidate> candidates;
        public votermachine()
        {
            candidates = new List<candidate>();
        }
        public void CastVote(candidate c)
        {
            c.IncrementVotes();
            Console.WriteLine("Vote cast successfully for candidate: " + c.Name);

           
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;";
            string query = "UPDATE candidate SET Votes = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    command.ExecuteNonQuery();
                }
            }
        }


        public void AddVoter()
        {
            votter v = new votter();
            Console.WriteLine("..................................");
            Console.WriteLine("1 : Add Voter Details ");
            Console.WriteLine("..................................");
            Console.WriteLine("Enter the voter Details");
            Console.WriteLine("Name");
            string voterName = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("CNIC");
            string cnic = Console.ReadLine();
            if (v.HasVoted(cnic))
            {
                Console.WriteLine("This voter has already cast their vote.");
                return;
            }
            Console.WriteLine("Selecetd Party");
            string selectedParty = Console.ReadLine();
            votter newvot = new votter(voterName, cnic, selectedParty);
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Votter;Integrated Security=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "INSERT INTO Voter (Cnic,VoterName,SelectedParty) VALUES (@CNIC,@VoterName,@SelectedParty)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CNIC", cnic);
                command.Parameters.AddWithValue("@VoterName", voterName);
                command.Parameters.AddWithValue("@SelectedParty", selectedParty);


                 command.ExecuteNonQuery();

            }

            string path = "vo.txt";
            using (StreamWriter sw = new StreamWriter(path, append: true))
            {
                sw.WriteLine($"Name: {voterName}, CNIC: {cnic}, Party: {selectedParty}");

            }
            Console.WriteLine("Voter added successfully");

        }
        public void UpdateVotter(string cnic)
        {
            string connctionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Votter;Integrated Security=True;";
            string query = "select VoterName from Voter where cnic=@CNIC";
            using (SqlConnection connection = new SqlConnection(connctionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CNIC", cnic);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                string voterName = null;
                string SelectedParty = null;
                if (reader.Read())
                {
                    voterName = reader.GetString(0);
                    SelectedParty = reader.GetString(1);
                  
                }
                reader.Close();

                if (voterName == null)
                {
                    Console.WriteLine("Voter with CNIC " + cnic + " not found.");
                    return;
                }
                Console.WriteLine("Enter Updated voter Details");
                Console.WriteLine("Name");
                string newVotterName = Console.ReadLine();
               

                voterName = newVotterName;
              
                string updateQuery = "UPDATE Voter SET VoterName = @NewVoterName WHERE cnic = @CNIC";
                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@NewVoterName", newVotterName);
                updateCommand.Parameters.AddWithValue("@CNIC", cnic);
                updateCommand.ExecuteNonQuery();


                string filePath = "v.txt";
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(cnic))
                    {
                        lines[i] = $"Name: {newVotterName}, CNIC: {cnic}";
                        break;
                    }
                }
                File.WriteAllLines(filePath, lines);

                Console.WriteLine("Voter details updated successfully.");
            }

        }
        public void displayVoters()
        {
            string connctionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=Votter;Integrated Security=True;";
            string query = "select * from Voter";
            using (SqlConnection connection = new SqlConnection(connctionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("..........................................");
                Console.WriteLine("Votter Details");
                Console.WriteLine("..........................................");
                while (reader.Read())
                {
                    string cnic = reader.GetString(0);
                    string VoterName = reader.GetString(1);
                    Console.WriteLine("CNIC");
                    Console.WriteLine(cnic);
                    Console.WriteLine();
                    Console.WriteLine("Name");
                    Console.WriteLine(VoterName);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("----------------------------------------------");
                }

            }


        }
        public void insertCandidate(candidate c)
        {
            Console.WriteLine("..................................");
            Console.WriteLine("1 : Add Candidate Details ");
            Console.WriteLine("..................................");
            Console.WriteLine("Enter the Candidates Details");
            Console.WriteLine("Candidate ID");
            Random rand = new Random();
            int candidateId = rand.Next(1, 1000);
            c.CandidateID = candidateId;
            Console.WriteLine("Name");
            string Name = Console.ReadLine();
            c.Name = Name;
            Console.WriteLine("Party");
            string Party = Console.ReadLine();
            c.Party = Party;
            int vote = 0;
            c.Votes = vote;
            //candidate cand = new candidate(Name, Party);
            string connctionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=candidate;Integrated Security=True;";
            SqlConnection connection = new SqlConnection(connctionString);
            connection.Open();
            string query = "INSERT INTO Candidate(candidateId,Name,party,votes) VALUES(@CandidateId,@Name,@Party,@Votes)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", Name);
                command.Parameters.AddWithValue("@Party", Party);
                command.Parameters.AddWithValue("@CandidateId", candidateId);
                command.Parameters.AddWithValue("@Votes", vote);

                command.ExecuteNonQuery();
            }
            string path = "cjson.txt";
            using (StreamWriter sw = new StreamWriter(path, append: true))
            {
                sw.WriteLine($"Candidate ID: {candidateId}, Name: {Name}, Party: {Party}");

            }
            string binaryFilePath = "c.txt";
            string jsonstring = JsonSerializer.Serialize(c);
            using (StreamWriter sww = new StreamWriter(binaryFilePath, append: true))
            {

                sww.WriteLine(jsonstring);
                sww.Close();
            }
            Console.WriteLine("Candidate added successfully");

        }
        public void displayCandidates()
        {
            string connectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=candidate;Integrated Security=True;";

            string query = "select Name, Party, Votes from candidate";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        Console.WriteLine("Candidates:");
                        Console.WriteLine("Name\t\tParty\t\tVotes");
                        Console.WriteLine("--------------------------------------");
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string party = reader.GetString(1);
                            int votes = reader.GetInt32(2);


                            Console.WriteLine($"{name}\t{party}\t\t{votes}");
                        }
                    }
                }


                int totalVotesForIK = GetTotalVotesForParty(connection, "Imran Khan");

                int totalVotesForNS = GetTotalVotesForParty(connection, "Nawaz Sharif");

                Console.WriteLine("\nTotal Votes:");
                Console.WriteLine($"Imran Khan: {totalVotesForIK}");
                Console.WriteLine($"Nawaz Sharif: {totalVotesForNS}");
            }
        }

        private int GetTotalVotesForParty(SqlConnection connection, string partyName)
        {
            string query = "SELECT SUM(Votes) FROM candidate WHERE Party = @PartyName";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PartyName", partyName);
                object result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }
        public void DeclareWinner()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;";
            string query = "SELECT TOP 1 Name, Party, SUM(Votes) AS TotalVotes FROM candidate GROUP BY Name, Party ORDER BY TotalVotes DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string party = reader.GetString(1);
                            int votes = reader.GetInt32(2);

                            Console.WriteLine("Winner Candidate:");

                            Console.WriteLine($"Name: {name}");
                            Console.WriteLine($"Party: {party}");
                            Console.WriteLine($"Votes: {votes}");
                        }
                        else
                        {
                            Console.WriteLine("No winner found. No candidates registered.");
                        }
                    }
                }
            }
        }
        public void UpdateCandidate(candidate c, int ID)
        {
            Console.WriteLine("................................");
            Console.WriteLine("Update Candidate Details");
            Console.WriteLine("..................................");
            Console.WriteLine("Enter Name to Update");
            string newName = Console.ReadLine();
            c.Name = newName;
            Console.WriteLine("Enter Party to Update");
            string newParty = Console.ReadLine();
            c.Party = newParty;
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;";
            string query = "UPDATE Candidate SET Name = @Name, Party = @Party,Votes=@Votes WHERE CandidateId = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", newName);
                    command.Parameters.AddWithValue("@Party", newParty);
                    command.Parameters.AddWithValue("@Votes", 0);
                    command.Parameters.AddWithValue("@ID", ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Candidate with ID {ID} updated successfully in the database.");
                    }
                    else
                    {
                        Console.WriteLine($"Candidate with ID {ID} not found in the database.");
                        return;
                    }
                }
            }


            string filePath = "cand.txt";
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains($"Candidate ID: {ID}"))
                {
                    lines[i] = $"Candidate ID: {ID}, Name: {c.Name}, Party: {c.Party}, Votes: {c.Votes}";
                    File.WriteAllLines(filePath, lines);
                    Console.WriteLine($"Candidate with ID {ID} updated successfully in the file.");
                    return;
                }
            }

            Console.WriteLine($"Candidate with ID {ID} not found in the file.");
        }
        public void DeleteCandidate(int ID)
        {

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;";
            string query = "DELETE FROM Candidate WHERE CandidateId = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Candidate with ID {ID} deleted successfully from the database.");
                    }
                    else
                    {
                        Console.WriteLine($"Candidate with ID {ID} not found in the database.");
                        return;
                    }
                }
            }
            string filePath = "cand.txt";
            List<string> lines = File.ReadAllLines(filePath).ToList();
            bool candidateRemoved = false;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains($"Candidate ID: {ID}"))
                {
                    lines.RemoveAt(i);
                    candidateRemoved = true;
                    break;
                }
            }

            if (candidateRemoved)
            {
                File.WriteAllLines(filePath, lines);
                Console.WriteLine($"Candidate with ID {ID} deleted successfully from the file.");
            }
            else
            {
                Console.WriteLine($"Candidate with ID {ID} not found in the file.");
            }
        }

        public void ReadCandidate(int id)
        {
            // Read candidate data from the database
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Candidate;Integrated Security=True;";
            string query = "SELECT * FROM Candidate WHERE CandidateId = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int candidateId = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            string party = reader.GetString(2);
                            int votes = reader.GetInt32(3);
                            Console.WriteLine("Read from database");
                            Console.WriteLine($"Candidate ID: {candidateId}, Name: {name}, Party: {party}, Votes: {votes}");
                        }
                    }
                }
            }

            // Read candidate data from the JSON file
            string filePath = "candidate.txt";
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader fileReader = new StreamReader(filePath))
                    {
                        string jsonstring = fileReader.ReadToEnd();
                        candidate candi = JsonSerializer.Deserialize<candidate>(jsonstring);
                        Console.WriteLine(candi.Name + " " + candi.CandidateID + " " + candi.Party + " " + candi.Votes);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading candidate data from file: {ex.Message}");
                }
            }
        }


        static void Main(string[] argv)
        {

            votermachine vm = new votermachine();
            while (true)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Voter");
                Console.WriteLine("2. Update Voter");
                Console.WriteLine("3. Delete Voter");
                Console.WriteLine("4. Display Voters");
                Console.WriteLine("5. Cast Vote");
                Console.WriteLine("6. Insert Candidate");
                Console.WriteLine("7. Update Candidate");
                Console.WriteLine("8. Display Candidates");
                Console.WriteLine("9. Delete Candidate");
                Console.WriteLine("10. Declare Winner");
                Console.WriteLine("11. Exit");
                Console.Write("Enter your choice from 1 to 11: ");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        vm.AddVoter();
                        break;
                    case 2:
                        Console.Write("Enter CNIC to update: ");
                        string cnic = Console.ReadLine();
                        vm.UpdateVotter(cnic);
                        break;
                    case 3:
                        Console.Write("Enter CNIC to delete: ");
                        string cnicToDelete = Console.ReadLine();
                        vm.DeleteVoter(cnicToDelete);
                        break;
                    case 4:
                        vm.displayVoters();
                        break;
                    case 5:
                        // Call method to cast vote
                        break;
                    case 6:
                        vm.insertCandidate();
                        break;
                    case 7:
                        // Call method to update candidate
                        break;
                    case 8:
                        // Call method to display candidates
                        break;
                    case 9:
                        // Call method to delete candidate
                        break;
                    case 10:
                        // Call method to declare winner
                        break;
                    case 11:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 11.");
                        break;
                }
            }
        }
    }
    
}
