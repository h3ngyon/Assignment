using Assignment;
using System.ComponentModel.Design;

// Create Dictionaries, Terminal
Terminal T5 = new Terminal("Terminal 5", new Dictionary<string, Airline>(), new Dictionary<string, BoardingGate>(), new Dictionary<string, double>());
Dictionary<string, Airline> airline_dict = new Dictionary<string, Airline>();
Dictionary<string, BoardingGate> boarding_gate_dict = new Dictionary<string, BoardingGate>();
Dictionary<string, BoardingGate> gatesdict = new Dictionary<string, BoardingGate>();


// Basic Feature 1.

// Key for airline_dict will use airline code "SQ"/"MH"

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
        airline.Flights = new Dictionary<string, Flight>();

        airline_dict.Add(airline.Name, airline);
        T5.AddAirline(airline);
    }
}

using (StreamReader sr = new StreamReader("boardinggates.csv"))
{
    string? s = sr.ReadLine();
        
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
        T5.AddBoardingGate(boardinggate);
    }
}

// Basic Feature 2
// Load Flights from flights.csv
Dictionary<string, Flight> flight_dict = new Dictionary<string, Flight>();
using (StreamReader sr = new StreamReader("flights.csv"))
{
    sr.ReadLine();
    string line;

    while ((line = sr.ReadLine()) != null)
    {
        Flight flight;
        string[] data = line.Split(",");

        if (data[4] == "CFFT")
        {
            flight = new CFFTFlight(data[0], data[1], data[2], Convert.ToDateTime(data[3]));
        }
        else if (data[4] == "LWTT")
        {
            flight = new LWTTFlight(data[0], data[1], data[2], Convert.ToDateTime(data[3]));
        }
        else if (data[4] == "DDJB")
        {
            flight = new DDJBFlight(data[0], data[1], data[2], Convert.ToDateTime(data[3]));
        }
        else
        {
            flight = new NORMFlight(data[0], data[1], data[2], Convert.ToDateTime(data[3]));
        }
        flight_dict.Add(flight.FlightNumber, flight);

        string airlineCode = flight.FlightNumber.Substring(0, 2);
        if (airline_dict.ContainsKey(airlineCode))
        {

            airline_dict[airlineCode].Flights.Add(flight.FlightNumber, flight);
        }

    }
}

//Initialize Terminal, T5, properties(dictionaries)
T5.Airlines = airline_dict;
T5.BoardingGates = boarding_gate_dict;
T5.Flights = flight_dict;


// Basic Feature 3: List all Flights with their basic information.
void ListFlightsBasicInfo()
{
    foreach (Flight flight in flight_dict.Values)
    {
        string airlineName = " ";
        foreach (Airline airline in airline_dict.Values)
        {
            string[] flightno = flight.FlightNumber.Split(' ');
            string airlinecode = flightno[0];
            if (airline.Code == airlinecode)
            {
                airlineName = airline.Name;
                break;
            }
        }
        Console.WriteLine($"Fight Number: {flight.FlightNumber,-8} Airline Name: {airlineName,-20} Origin: {flight.Origin,-20} Destination: {flight.Destination,-20} Expected Time: {flight.ExpectedTime,-10}");
    }
}

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

// Basic Feature 5: Assign boarding gate to flight
void AssignBoardingGate()
{
    Console.WriteLine("=============================================\r\nAssign a Boarding Gate to a Flight\r\n=============================================\r\n");

    Flight flight;
    BoardingGate bg;
    while (true)
    {
        Console.Write("Input the flight number: ");
        string flightNo = Console.ReadLine();
        if (flight_dict.ContainsKey(flightNo))
        {
            flight = flight_dict[flightNo];
            break;
        }

    }
    while (true)
    {
        Console.Write("Input the Boarding Gate Name: ");
        string bgateName = Console.ReadLine();
        if (boarding_gate_dict.ContainsKey(bgateName))
        {
            if (boarding_gate_dict[bgateName].Flight != null)
            {
                Console.WriteLine("A flight is already assigned to this boarding gate. Please try again.");
                continue;
            }

            bg = boarding_gate_dict[bgateName];
            break;
        }
        else { Console.WriteLine("Boarding Gate does not exist. Try again."); continue; }
    }

    // Display flight and boarding gate
    Console.WriteLine($"{flight} Boarding Gate: {bg.GateName}");

    // Set status
    Console.WriteLine("Would you like to update the flight status? [Y/N]");
    string option = Console.ReadLine().ToLower();
    if (option == "y")
    {
        Console.WriteLine("1. Delayed\n2. Boarding\n3. On Time");
        Console.WriteLine("Please select the new status of the flight: ");
        string status = Console.ReadLine();
        if (status == "1")
        {
            flight.Status = "Delayed";
        }
        else if (status == "2")
        {
            flight.Status = "Boarding";
        }
        else if (status == "3")
        {
            flight.Status = "On Time";
        }
    }
    else
    {
        flight.Status = "On Time";
    }

    // Assign Flight to Boarding Gate
    bg.Flight = flight;
    Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.");
}



// Basic Feature 6: Create a new Flight
void NewFlight()
{
    Console.Write("Enter Flight Number: ");
    string? flightNo = Console.ReadLine();
    Console.Write("Enter Origin: ");
    string? origin = Console.ReadLine();
    Console.Write("Enter Destination: ");
    string? destination = Console.ReadLine();
    Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
    DateTime time = Convert.ToDateTime(Console.ReadLine());
    Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
    string? code = Console.ReadLine();

    if (code == "CFFT")
    {
        CFFTFlight flight = new CFFTFlight(flightNo, origin, destination, time);
        flight_dict.Add(flight.FlightNumber, flight);
        Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
    }
    if (code == "DDJB")
    {
        DDJBFlight flight = new DDJBFlight(flightNo, origin, destination, time);
        flight_dict.Add(flight.FlightNumber, flight);
        Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
    }
    if (code == "LWTT")
    {
        LWTTFlight flight = new LWTTFlight(flightNo, origin, destination, time);
        flight_dict.Add(flight.FlightNumber, flight);
        Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
    }
}