﻿using Assignment;
using System.ComponentModel.Design;

// Create Dictionaries, Terminal


Terminal T5 = new Terminal("Terminal 5", new Dictionary<string, Airline>(), new Dictionary<string, BoardingGate>(), new Dictionary<string, double>());
Dictionary<string, Airline> airline_dict = new Dictionary<string, Airline>();
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

        airline_dict.Add(airline.Code, airline);
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
    // Read Header
    sr.ReadLine();
    string line;

    // Read other lines
    while ((line = sr.ReadLine()) != null)
    {
        Flight flight;
        string[] data = line.Split(",");

        // Check Special request code
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

            airline_dict[airlineCode].AddFlight(flight);
        }

    }
}

//Initialize Terminal, T5, properties(dictionaries)
T5.Airlines = airline_dict;
T5.BoardingGates = gatesdict;
T5.Flights = flight_dict;


// Basic Feature 3: List all Flights with their basic information.
void ListFlightsBasicInfo()
{
    DisplayFlightHeaders();
    foreach (Flight flight in flight_dict.Values)
    {
        string gate = "";
        foreach (BoardingGate bg in gatesdict.Values)
        {
            if (bg.Flight == flight)
            {
                gate = bg.GateName;
                break;
            }
            gate = "Unassigned";
        }

        Console.WriteLine($"{flight}     {gate}");
    }
}

// Basic Feature 4: List all Boarding Gates
void ListAllBoardingGates()
{
    Console.WriteLine("=============================================\r\nList of Boarding Gates for Changi Airport Terminal 5\r\n=============================================\r\n");
    Console.WriteLine($"{"Gate Name",-10} {"DDJB",-10} {"CFFT",-10} {"LWTT",-10}");
    foreach (BoardingGate bg in gatesdict.Values)
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
        //Console.WriteLine($"{bg.GateName,-10} {bg.SupportsDDJB,-10} {bg.SupportsCFFT,-10} {bg.SupportsLWTT,-10}");
        Console.WriteLine(bg);
    }
}
  
// Basic Feature 5: Assign boarding gate to flight
void AssignBoardingGate()
{
    Console.WriteLine("=============================================\r\nAssign a Boarding Gate to a Flight\r\n=============================================\r\n");

    // Initialize flight and bg variables
    Flight flight;
    BoardingGate bg;
    
    // Check input to make sure the flight exists
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
    // Checks that the boarding gate exists
    while (true)
    {
        Console.Write("Input the Boarding Gate Name: ");
        string bgateName = Console.ReadLine();
        if (gatesdict.ContainsKey(bgateName))
        {
            // If boarding gate contains a flight, restart the loop for a new Boarding Gate
            if (gatesdict[bgateName].Flight != null)
            {
                Console.WriteLine("A flight is already assigned to this boarding gate. Please try again.");
                continue;
            }

            // Assign boarding gate to bg
            bg = gatesdict[bgateName];
            break;
        }

        // if boarding gate input does not exist, restart loop
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
    while (true)
    {
        try
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


            string? code = Console.ReadLine().ToLower();


            if (code == "cfft")
            {
                CFFTFlight flight = new CFFTFlight(flightNo, origin, destination, time);
                flight_dict.Add(flight.FlightNumber, flight);
                Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
            }
            if (code == "ddjb")
            {
                DDJBFlight flight = new DDJBFlight(flightNo, origin, destination, time);
                flight_dict.Add(flight.FlightNumber, flight);
                Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
            }
            if (code == "lwtt")
            {
                LWTTFlight flight = new LWTTFlight(flightNo, origin, destination, time);
                flight_dict.Add(flight.FlightNumber, flight);
                Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
            }
            else
            {
                NORMFlight flight = new NORMFlight(flightNo, origin, destination, time);
                flight_dict.Add(flight.FlightNumber, flight);
                Console.WriteLine($"Flight {flight.FlightNumber} has been added!");
            }
            break;
        }
        catch (OverflowException)
        { 
            Console.WriteLine("Please try again.");
            continue;
        }
        catch (FormatException)
        {
            Console.WriteLine("Please try again.");
            continue;
        }    
    }
}



// Basic Feature 7 : Display all flights by airline
void DisplayAirLineFlights()
{
    Console.WriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================\r\n");

    Console.WriteLine($"{"Airline Code",-20} {"Airline Name",-20}");
    foreach (Airline airLine in airline_dict.Values)
    {
        Console.WriteLine($"{airLine.Code,-15} {airLine.Name,-20}");
    }

    Airline airline;
    while (true)
    {
        Console.Write("Enter Airline Code: ");
        try
        {
            string airlineCode = Console.ReadLine().ToUpper();
            if (airline_dict.ContainsKey(airlineCode) == false)
            {
                throw new KeyNotFoundException();
            }
            airline = airline_dict[airlineCode];
            break;
        }
        catch (FormatException fe)
        {
            Console.WriteLine("Input a proper airline code. Try again");
            continue;
        }
        catch (KeyNotFoundException ke)
        {
            Console.WriteLine($"Airline not found.");
            continue;
        }
    }

    Console.WriteLine($"=============================================\r\nList of Flights for {airline.Name}\r\n=============================================\r\n");
    foreach (Flight flight in airline.Flights.Values)
    {
        Console.WriteLine(flight);
    }

}

//Basic Feature 8
//modify flight details
// list all airlines available
void ModifyFlights()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");

    Console.WriteLine($"{"Airline Code",-15} {"Airline Name",-20}");
    foreach (Airline airLine in airline_dict.Values)
        {
         Console.WriteLine($"{airLine.Code,-15} {airLine.Name,-20}");
        }
    Console.Write("Enter Airline Code: ");
    string code = Console.ReadLine().ToUpper();
    Airline airline;
    foreach (KeyValuePair<string, Airline> kvp in airline_dict)
    {
        if (airline_dict.ContainsKey(code))
        {
            airline = airline_dict[code];
            Console.WriteLine($"{"Flight Number",-20}{"Airline Name", -20} {"Origin",-20}{"Destination",-20}{"Expected Departure/Arrival Time",-20}");
            foreach (Flight flight in airline.Flights.Values)
            {
                Console.WriteLine($"{flight.FlightNumber,-20} {airline.Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-20}");
            }
            break;
        }
        Console.WriteLine("Airline not found.");
    }
    Console.Write("Choose a flight to modify or delete: ");
    string modified = Console.ReadLine();
    Flight modify;
   
    Console.WriteLine("1. Modify Flight ");
    Console.WriteLine("2. Delete Flight ");
    Console.Write("Choose an option: ");
    string option =  Console.ReadLine();
    if (option == "1")
    {
        Console.WriteLine("1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        Console.Write("Choose an option: ");
        string choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.Write("Enter new origin: ");
            string origin = Console.ReadLine();
            Console.Write("Enter new Destination: ");
            string dest = Console.ReadLine();
            Console.Write("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            DateTime ex = Convert.ToDateTime(Console.ReadLine());
            foreach (KeyValuePair<string, Flight> kvp in airline_dict[code].Flights)
            {
                if (modified == kvp.Key)
                {
                    modify = kvp.Value;
                    modify.Origin = origin;
                    modify.Destination = dest;
                    modify.ExpectedTime = ex;
                }
            }
            Console.WriteLine("Flight has been updated!");
        }
        else if (choice == "2")
        {
            Console.Write("Enter new Status: ");
            string statuschange = Console.ReadLine();
            foreach (KeyValuePair<string, Flight> kvp in airline_dict[code].Flights)
            {
                if (modified == kvp.Key)
                {
                    modify = kvp.Value;
                    modify.Status = statuschange;
                }
            }
        }
        else if (choice == "3")
        {

        }
    }
    else if (option == "2")
    {

    }
}


// Basic Feature 9: Display flights in chronological order , boarding gates assignments where applicable
void FlightsInOrder()
{
    List<Flight> flights = new List<Flight>();

    foreach (Flight flight in flight_dict.Values)
    {
        flights.Add(flight);
    }
    flights.Sort();
    DisplayFlightHeaders();
    foreach (Flight flight in flights)
    {
        string gate = "";
        foreach (BoardingGate bg in gatesdict.Values)
        {
            if (bg.Flight == flight)
            {
                gate = bg.GateName;
                break;
            }
            gate = "Unassigned";
        }

        Console.WriteLine($"{flight}     {gate}");
    }
}

void DisplayFlightHeaders()
{
    Console.WriteLine($"{"FlightNo",-9} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-7}         Code      Boarding Gate");
}


// WRITING MENU

while (true)
{
    try
    {
        Console.WriteLine("=============================================\r\nWelcome to Changi Airport Terminal 5\r\n=============================================\r\n1. List All Flights\r\n2. List Boarding Gates\r\n3. Assign a Boarding Gate to a Flight\r\n4. Create Flight\r\n5. Display Airline Flights\r\n6. Modify Flight Details\r\n7. Display Flight Schedule\r\n0. Exit\r\n\r\n");
        Console.WriteLine("Please select your option: ");

        string option = Console.ReadLine();

        if (option == "1")
        {
            ListFlightsBasicInfo();
        } 
        else if (option == "2")
        {
            ListAllBoardingGates();
        }
        else if (option == "3")
        {
            AssignBoardingGate();
        }
        else if ( option == "4")
        {
            NewFlight();
        }
        else if ( option == "5")
        {
            DisplayAirLineFlights();
        }
        else if (option == "6")
        {
            ModifyFlights();
        }
        else if (option == "7")
        {
            FlightsInOrder();
        }
        else if (option == "0")
        {
            Console.WriteLine("BYE BYE!!!");
            break;
        }

    }


    catch (FormatException)
    {
        Console.WriteLine("Please input an appropriate option.");
        continue;
    }
}