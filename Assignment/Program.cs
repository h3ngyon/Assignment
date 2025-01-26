using Assignment;
using System.ComponentModel.Design;

// Basic Feature 1.

// Key for airline_dict will use airline code "SQ"/"MH"
Dictionary<string,Airline> airline_dict = new Dictionary<string,Airline>();
using (StreamReader sr = new StreamReader("airlines.csv"))
{
    sr.ReadLine();
    string line;
    while ((line= sr.ReadLine()) != null)
    {
        string[] airline_array = line.Split(",");
        Airline new_airline = new Airline(airline_array[0], airline_array[1]);
        // Key for Airline_dict will be airline code
        airline_dict.Add(airline_array[1], new_airline);
    }
}

// Create Boarding Gates dictionary
Dictionary<string,BoardingGate> boarding_gate_dict = new Dictionary<string,BoardingGate>();
using (StreamReader sr = new StreamReader("boardinggates.csv"))
{
    sr.ReadLine();
    string line;

    while ((line = sr.ReadLine()) != null)
    {
        string[] boarding_gate_array = line.Split(",");
        BoardingGate boardingGate = new BoardingGate(boarding_gate_array[0]);

        // Supports DDJB
        if (boarding_gate_array[1] == "True") 
        { boardingGate.SupportsDDJB = true; }

        // Supports CFFT
        if (boarding_gate_array[2] == "True") 
        { boardingGate.SupportsCFFT = true;}

        // Supports LWTT
        if (boarding_gate_array[3] == "True")
        {
            boardingGate.SupportsLWTT = true;
        }

        // Add new BoardingGate object to dict
        boarding_gate_dict.Add(boarding_gate_array[0], boardingGate);
    }
}

// Basic Feature 2
// Load Flights from flights.csv

using (StreamReader sr = new StreamReader("flights.csv"))
{
    sr.ReadLine();
    string line;

    while ((line = sr.ReadLine()) != null)
    {

    }
}