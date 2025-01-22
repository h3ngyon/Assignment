using Assignment;

// Basic Feature 1.

// Key for airline_dict will use airline code "SQ"/"MH"
Dictionary<string,Airline> airline_dict = new Dictionary<string,Airline>();
using (StreamReader sr = new StreamReader("airlines.csv"))
{
    sr.ReadLine();
    string line;
    while ((line= sr.ReadLine()) != null)
    {
        string[] airline = line.Split(",");
        Airline new_airline = new Airline(airline[0], airline[1]);
        // Key for Airline_dict will be airline code
        airline_dict.Add(airline[1], new_airline);
    }
}

// Create Boarding Gates dictionary
Dictionary<string,BoardingGate> boarding_gate_dict = new Dictionary<string,BoardingGate>();