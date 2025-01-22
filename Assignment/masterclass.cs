using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    abstract class Flight
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }
        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }
        public abstract double CalculateFees();
        public override string ToString()
        {
            return $"Fight No: {FlightNumber:-10} Origin: {Origin:-20} Destination: {Destination:-20} Expected Time: {ExpectedTime:-} Status: {Status}";
        }
    }
    class CFFTFlight : Flight
    {
        public double RequestFee { get; set; }
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestfee) : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestfee;
        }
        public override double CalculateFees()
        {
            double baseFee = 300 + 150;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + baseFee;
            }
            else if (Origin == "Singapore (SIN)") // Departing Flights
            { return 800 + baseFee; }
            else
            {
                return baseFee;
            }
        }
        public override string ToString()
        {
            return base.ToString() + "\tRequest Fee: " + RequestFee;
        }
    }
    class DDJBFlight : Flight
    {
        public double RequestFee { get; set; }
        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestfee) : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestfee;
        }
        public override double CalculateFees()
        {
            double baseFee = 300 + 300;
            if (Destination == "Singapore (SIN)") // Arriving flights
            {
                return 500 + baseFee;
            }
            else if (Origin == "Singapore (SIN)") // Departing Flights
            { return 800 + baseFee; }
            else
            {
                return baseFee;
            }
        }
        public override string ToString()
        {
            return base.ToString() + "\tRequest Fee: " + RequestFee;
            Console.WriteLine("I LOVE SUCKING BLACK DICk");
        }
    }

    class NORMFlight : Flight
    {
        public NORMFlight(string fn, string origin, string destination, DateTime et, string status) : base(fn, origin, destination, et, status) { }

        public override double CalculateFees()
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
            return base.ToString();
        }
    }

    class LWTTFlight : Flight
    {
        public double RequestFee { get; set; }
        public LWTTFlight(string fn, string origin, string destination, DateTime et, string status, double ReqFee) : base(fn, origin, destination, et, status)
        {
            RequestFee = ReqFee;
        }

        public override double CalculateFees()
        {
            double baseFee = 300 + 500;
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
            return base.ToString();
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
    }

    class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }
        public Dictionary<string,BoardingGate> BoardingGates { get; set; }
        public Dictionary<string,double> GateFees  { get; set; }



    }
}
    