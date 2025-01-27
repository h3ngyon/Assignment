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

Dictionary<string, Airline> airlinedict = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> gatesdict = new Dictionary<string, BoardingGate>();

using (StreamReader sr = new StreamReader("airlines.csv"))
{
    string? s = sr.ReadLine();
    if (s != null)
    {
        string[] heading = s.Split(',');
    }
    while ((s = sr.ReadLine()) != null)
    {
        string[] airlines = s.Split(",");
        string airlinename = airlines[0];
        string airlinecode = airlines[1];
        Airline airline = new Airline(airlinename, airlinecode);
        airlinedict.Add(airline.Name, airline);
    }
}

using (StreamReader sr = new StreamReader("boardinggates.csv"))
{
    string? s = sr.ReadLine();
    if (s != null)
    {
        string[] heading = s.Split(",");
    }
    while ((s = sr.ReadLine()) != null)
    {
        string[] gates = s.Split(",");
        BoardingGate boardinggate = new BoardingGate(gates[0]);

        if (gates[1] == "True")
        {
            boardinggate.SupportsDDJB = true;
        }
        if (gates[2] == "True")
        {
            boardinggate.SupportsCFFT = true;
        }
        if (gates[3] == "True")
        {
            boardinggate.SupportsLWTT = true;
        }
        gatesdict.Add(boardinggate.GateName, boardinggate);
    }
}

// Basic Feature 3
// display flight details


// Basic Feature 4: List all Boarding Gates
void ListAllBoardingGates()
{
    Console.WriteLine("=============================================\r\nList of Boarding Gates for Changi Airport Terminal 5\r\n=============================================\r\n");
    Console.WriteLine($"{"Gate Name",-10} {"DDJB",-10} {"CFFT",-10} {"LWTT",-10}");
    foreach (BoardingGate bg in boarding_gate_dict.Values)
    {
        string supports = "";
        if (bg.SupportsCFFT == true)
        {
            supports += "CFFT ";
        }
        if (bg.SupportsLWTT == true)
        {
            supports += "LWTT ";
        }
        if (bg.SupportsDDJB == true)
        {
            supports += "DDJB ";
        }
        Console.WriteLine($"{bg.GateName,-10} {bg.SupportsDDJB,-10} {bg.SupportsCFFT,-10} {bg.SupportsLWTT,-10}");
    }
}
//havjvx