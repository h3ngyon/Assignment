using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
  
    class BoardingGate
    {
        private string gateName;
        public string GateName { get;set; }

        private bool supportsCFFT;
        public bool SupportsCFFT {  get; set; }

        private bool supportsDDJB;
        public bool SupportsDDJB { get; set; }

        private bool supportsLWTT;
        public bool SupportsLWTT { get; set; }

        // public Flight flight { get; set; }


        
    }
}
