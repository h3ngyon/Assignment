using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number	: S10268172
// Student Name	: Tan Heng Yong
// Partner Name	: Isaac Leow Yu Jun
//==========================================================


namespace Assignment
{
    abstract class Flight : IComparable<Flight>
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }
        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
        }
        public virtual double CalculateFees()
        {
            double baseFee = 300;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + baseFee;
            }
            else if (Origin == "Singapore (SIN)") // Departing Flights
            { return 800 + baseFee; }
            else { return baseFee; }
        }
        public override string ToString()
        {
            return $"{FlightNumber,-8}  {Origin,-18}  {Destination,-18}  {ExpectedTime,-7} ";
        }

        public int CompareTo(Flight other)
        {
            return ExpectedTime.CompareTo(other.ExpectedTime);
        }
    }
    class CFFTFlight : Flight
    {
        public double RequestFee { get; set; }
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime) : base(flightNumber, origin, destination, expectedTime)
        {
            RequestFee = 150;
        }
        public override double CalculateFees()
        {
            double baseFee = 300 + RequestFee;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + baseFee;
            }
            else  // (Origin == "Singapore (SIN)")  Departing Flights
            { return 800 + baseFee; }

        }
        public override string ToString()
        {
            return $"{base.ToString()}  CFFT ";
        }
    }
    class DDJBFlight : Flight
    {
        public double RequestFee { get; set; }
        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime) : base(flightNumber, origin, destination, expectedTime)
        {
            RequestFee = 300;
        }
        public override double CalculateFees()
        {
            double baseFee = 300 + RequestFee;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + baseFee;
            }
            else // (Origin == "Singapore (SIN)")  Departing Flights
            { 
                return 800 + baseFee;
            }
        }
        public override string ToString()
        {
            return $"{base.ToString()}  DDJB ";
        }
    }

    class NORMFlight : Flight
    {
        public NORMFlight(string fn, string origin, string destination, DateTime et) : base(fn, origin, destination, et) { }

        public override double CalculateFees()
        {
            return base.CalculateFees();
        }

        public override string ToString()
        {
            return base.ToString() + "       ";
        }
    }

    class LWTTFlight : Flight
    {
        public double RequestFee { get; set; }
        public LWTTFlight(string fn, string origin, string destination, DateTime et) : base(fn, origin, destination, et)
        {
            RequestFee = 500;
        }

        public override double CalculateFees()
        {
            double basefee = 300;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + RequestFee + basefee;
            }
            else // Departing Flights
            {
                return 800 + RequestFee + basefee;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}  LWTT ";
        }
    }

    class Airline
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Dictionary<string, Flight> Flights { get; set; }
        public Airline(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public bool AddFlight(Flight x) 
        {
            if (Flights.ContainsKey(x.FlightNumber))
            { return false; }
            Flights.Add(x.FlightNumber, x);
            return true;
        }

        public double CalculateFees()
        {
            double TotalFee = 0;
            // Find Fee for all flights in Flights (dict)
            foreach (KeyValuePair<string, Flight> kvp in Flights)
            {
                TotalFee += (kvp.Value).CalculateFees();
                Flight x = kvp.Value;
                // Discounts for Flights from Dubai, Bangkok, Tokyo
                string[] cities = ["Dubai (DXB", "Bangkok (BKK)", "Tokyo (NRT)"];
                if (cities.Contains(x.Origin))
                {
                    TotalFee -= 25;
                }

                // Discount for Flights before 11am and after 9pm
                TimeSpan ninepm = new TimeSpan(21, 0, 0);
                TimeSpan elevenam = new TimeSpan(11, 0, 0);
                if (x.ExpectedTime.TimeOfDay < elevenam || x.ExpectedTime.TimeOfDay > ninepm)
                {
                    TotalFee -= 110;
                }

                // Check if Flights have special code requests
                if (x is not NORMFlight)
                {
                    TotalFee -= 50;
                }
            }

            // Calculate Discount based on flights in Flights(dict)
            // For every 3 flights arriving/departing, airlines will receive a discount
            if (Flights.Count() / 3 > 1)
            {
                TotalFee -= (Convert.ToInt32(Flights.Count() / 3) * 350);
            }

            // For For more than 5 flights arriving/departing, airlines receive an additional discount
            if (Flights.Count() > 5)
            { return TotalFee * 0.97; }
            else { return TotalFee; }
        }

        public bool RemoveFlight(Flight x)
        {
            if (Flights.ContainsKey(x.FlightNumber))
            {
                Flights.Remove(x.FlightNumber);
                return true;
            }
            else { return false; }
        }

        public override string ToString()
        {
            return $"Name: {Name,-30} Code: {Code,-10} Fee: ${CalculateFees:.2f,-10}";
        }
    }

    class BoardingGate // Havent added Calculate Fees and ToString
    {
        public string GateName {  get; set; }
        public bool SupportsCFFT {  get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }
        public Flight Flight { get; set; }

        public BoardingGate(string gatename)
        {
            GateName = gatename;
        }

        public override string ToString()
        {
            return $"{GateName,-10} {SupportsDDJB,-10} {SupportsCFFT,-10} {SupportsLWTT,-10}";
        }
    }

    class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }
        public Dictionary<string,BoardingGate> BoardingGates { get; set; }
        public Dictionary<string,double> GateFees  { get; set; }

        public Terminal(string terminalName, Dictionary<string, Airline> airlines, Dictionary<string, BoardingGate> boardingGate, Dictionary<string, double> gateFees)
        {
            TerminalName = terminalName;
            Airlines = airlines;
            BoardingGates = boardingGate;
            GateFees = gateFees;
        }


        public bool AddAirline(Airline airline)
        {
            if (Airlines.ContainsKey(airline.Code))
            {
                return false;
            }
            Airlines.Add(airline.Code, airline);
            return true;
        }
        public bool AddBoardingGate(BoardingGate boardinggate)
        {
            if (BoardingGates.ContainsKey(boardinggate.GateName))
            {
                return false;
            }
            BoardingGates.Add(boardinggate.GateName, boardinggate);
            return true;
        }
        public Airline GetAirlineFromFlight(Flight flight) 
        {
            foreach (KeyValuePair<string, Airline> kvp in Airlines)
            {
                string[] flightno = flight.FlightNumber.Split(' ');
                if (flightno[0] == kvp.Key)
                {
                    return kvp.Value;
                }

            }
            return null;
        }
        public void PrintAirlineFees()
        {
            Console.WriteLine("Airline Fees:");
            foreach (Airline airline in Airlines.Values)
            {
                Console.WriteLine($"{airline.Name}: ${airline.CalculateFees():.2f}");
            }
        }
        public override string ToString()
        {
            return $"Terminal: {TerminalName}";
        }
    }
}
    