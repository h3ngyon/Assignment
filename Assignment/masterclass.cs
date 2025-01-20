using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return base.ToString() + "\tRequest Fee: " + RequestFee;
        }
    }

    class NORMFlight : Flight
    {
        public NORMFlight(string fn, string origin, string destination, DateTime et, string status) : base(fn, origin, destination, et, status) { }

        public override double CalculateFees()
        {
            base.CalculateFees();

      
        }
    }   
    class niiga

}
