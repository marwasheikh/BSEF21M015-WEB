using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [Serializable]
    internal class candidate
    {
        private int candidateID;
        private string name;
        private string party;
        private int votes;

        private int GenerateCandidateID()
        {
            Random rand = new Random();
            return rand.Next(1, 1000);
        }
        public candidate(string n, string p)
        {
            candidateID = GenerateCandidateID();
            name = n;
            party = p;
            votes = 0;
        }
        public int CandidateID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Party
        {
            get;
            set;
        }
        public int Votes
        {
            get;
            set;
        }
        public void IncrementVotes()
        {
            votes++;
        }
        public override string ToString()
        {
            return name + " " + party + " " + votes + "" + GenerateCandidateID();
        }


 
    }

 
}
