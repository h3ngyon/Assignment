using Assignment;

//==========================================================
// Student Number	: S10268172
// Student Name	: Tan Heng Yong
// Partner Name	: Isaac Leow Yu Jun
//==========================================================


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
        if (flight is NORMFlight) { flight.Code = ""; }
        else { flight.Code = data[4]; }
        flight.Status = "Scheduled";

        string airline_code = flight.FlightNumber.Split(" ")[0];
        if (airline_dict.ContainsKey(airline_code))
        {
            airline_dict[airline_code].AddFlight(flight);
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
    Console.WriteLine($"{"FlightNo",-10} {"Airline",-20} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-20} ");
    foreach (Flight flight in flight_dict.Values)
    {
        try
        {
            string airline = T5.GetAirlineFromFlight(flight).Name;

            flight.Airline = airline;
            Console.WriteLine($"{flight.FlightNumber,-10} {airline,-20} {flight.Origin,-18}  {flight.Destination,-18}  {flight.ExpectedTime,-7} ");
        }
        catch (NullReferenceException)
        {
            continue;
        }
    }
    Console.WriteLine();
}

// Basic Feature 4: List all Boarding Gates
void ListAllBoardingGates()
{
    Console.WriteLine("=============================================\r\nList of Boarding Gates for Changi Airport Terminal 5\r\n=============================================\r");
    Console.WriteLine($"{"Gate Name",-10} {"DDJB",-10} {"CFFT",-10} {"LWTT",-9} {"Flight Number"}");
    foreach (BoardingGate bg in gatesdict.Values)
    {
        string flightnumber = "Unassigned";
        if (bg.Flight != null)
        {
            flightnumber = bg.Flight.FlightNumber;
        }
        Console.WriteLine(bg + flightnumber);
    }
    Console.WriteLine();
}

// Basic Feature 5: Assign boarding gate to flight
void AssignBoardingGate()
{
    Console.WriteLine("=============================================\r\nAssign a Boarding Gate to a Flight\r\n=============================================\r");

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

            // Display flight details
            Console.WriteLine(flight.ToString2());
            break;
        }

    }
    // Checks that the boarding gate exists
    while (true)
    {
        Console.Write("Input the Boarding Gate Name: ");  //Prompt for boarding gate
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
    flight_dict[flight.FlightNumber].BoardingGate = bg; ;
    Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.\n");
}



// Basic Feature 6: Create a new Flight
bool NewFlight()
{
    List<Flight> flights = new List<Flight>();
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


            string? code = Console.ReadLine().ToUpper();

            Flight newflight;
            if (code == "CFFT")
            {
                CFFTFlight flight = new CFFTFlight(flightNo, origin, destination, time);
                newflight = flight;
            }
            if (code == "DDJB")
            {
                DDJBFlight flight = new DDJBFlight(flightNo, origin, destination, time);
                newflight = flight;
            }
            if (code == "LWTT")
            {
                LWTTFlight flight = new LWTTFlight(flightNo, origin, destination, time);
                newflight = flight;
            }
            else
            {
                NORMFlight flight = new NORMFlight(flightNo, origin, destination, time);
                newflight = flight;
                code = "";
            }
            flight_dict.Add(newflight.FlightNumber, newflight);
            flights.Add(newflight);


            // Append Flight data to flights.csv
            using (StreamWriter sw = new StreamWriter("flights.csv", true))
            {
                sw.WriteLine($"{newflight.FlightNumber},{newflight.Origin},{newflight.Destination},{newflight.ExpectedTime},{code}");
            }

            // Prompt if user wants to create a new flight
            Console.WriteLine("Would you like to create another flight? [Y/N]: ");
            string option = Console.ReadLine().ToLower();
            if (option == "y")
            {
                continue;
            }

            // Display success message(s)
            foreach (Flight flight in flights)
            {
                Console.WriteLine($"Flight {newflight.FlightNumber} has been added!");
            }
            return true;
        }



        catch (OverflowException)
        {
            Console.WriteLine("Please try again.");
            continue;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("We could not find that file, can you try again.");
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
    Console.WriteLine("=============================================\r\nList of Airlines for Changi Airport Terminal 5\r\n=============================================\r");

    Console.WriteLine($"{"Airline Code",-20} {"Airline Name",-20}");
    foreach (Airline airLine in airline_dict.Values)
    {
        Console.WriteLine($"{airLine.Code,-10} {airLine.Name,-20}");
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
                Console.WriteLine("Airline code not found.");
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

    Console.WriteLine($"=============================================\r\nList of Flights for {airline.Name}\r\n=============================================\r");
    Console.WriteLine($"{"FlightNo",-10} {"Airline",-20} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-20} ");
    foreach (Flight flight in airline.Flights.Values)
    {
        string airlineName = T5.GetAirlineFromFlight(flight).Name;
        Console.WriteLine($"{flight.FlightNumber,-10} {airlineName,-20} {flight.Origin,-18}  {flight.Destination,-18}  {flight.ExpectedTime,-7} ");
    }
    Console.WriteLine();

}

//Basic Feature 8
//modify flight details
// list all airlines available
BoardingGate FindBoardingGate(Flight modify)
{
    foreach (KeyValuePair<string, BoardingGate> kvp in gatesdict)
    {
        if (kvp.Value.Flight == modify)  // Compare the flight to the boarding gate's flight
        {
            return kvp.Value;  // Return the boarding gate immediately when found
        }
    }

    return null;  // If no match is found, return null
}

Flight FindFlight(string modified, string code)                                  // method for finding flight 
{
    foreach (KeyValuePair<string, Flight> kvp in airline_dict[code].Flights)    // run through each value in the flight dictionary from the selected airline
    {
        if (modified == kvp.Value.FlightNumber)
        {
            return kvp.Value;
        }
    }
    return null;
}
string FindSpecial(BoardingGate found)
{
    if (found.SupportsDDJB == true)
    { return "DDJB"; }
    else if (found.SupportsCFFT == true)
    { return "CFFT"; }
    else if (found.SupportsLWTT == true)
    { return "LWTT"; }
    else
    { return "None"; }
}
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
    if (string.IsNullOrWhiteSpace(code))
    {
        Console.WriteLine("Airline code cannot be empty.");
        return;
    }
    Airline airline;

    if (airline_dict.ContainsKey(code))
    {
        airline = airline_dict[code];
        Console.WriteLine($"{"Flight Number",-20} {"Airline Name",-19} {"Origin",-19} {"Destination",-19} {"Expected Departure/Arrival Time",-19}");
        foreach (Flight flight in airline.Flights.Values)
        {
            Console.WriteLine($"{flight.FlightNumber,-20} {airline.Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-20}");
        }
        Console.Write("\nChoose a flight to modify or delete: ");
        string modified = Console.ReadLine().ToUpper();
        Flight modify = FindFlight(modified, code);
        BoardingGate gatefound = FindBoardingGate(modify);
        Console.WriteLine("1. Modify Flight ");
        Console.WriteLine("2. Delete Flight ");
        Console.Write("Choose an option: ");
        string option = Console.ReadLine();
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
                modify.Origin = origin;
                modify.Destination = dest;
                modify.ExpectedTime = ex;
                Console.WriteLine("Flight has been updated!");
                Console.WriteLine($"Flight number: {modify.FlightNumber}");
                Console.WriteLine($"Airline Name: {airline.Code}");
                Console.WriteLine($"Origin: {modify.Origin}");
                Console.WriteLine($"Destination: {modify.Destination}");
                Console.WriteLine($"Expected Departure/Arrival Time: {modify.ExpectedTime}");
                Console.WriteLine($"Status: {modify.Status}");
                if (modify is DDJBFlight)
                {
                    Console.WriteLine("Special Request Code: DDJB");
                }
                else if (modify is CFFTFlight)
                {
                    Console.WriteLine("Special Request Code: CFFT");
                }
                else if (modify is LWTTFlight)
                {
                    Console.WriteLine("Special Request Code: LWTT");
                }
                else
                {
                    Console.WriteLine("Special Request Code: None");
                }
                // show flight boarding gate
                if (gatefound == null)
                {
                    Console.WriteLine("Boarding Gate: Unassigned");
                }
                else
                {
                    string gatename = gatefound.GateName;
                    Console.WriteLine($"Boarding Gate: {gatename}");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Enter new Status: ");
                string statuschange = Console.ReadLine();
                modify.Status = statuschange;
                Console.WriteLine("Flight status changed!");
            }
            else if (choice == "3")
            {
                Console.Write("Enter new special request code: ");
                string codechange = Console.ReadLine().ToUpper();
                if (codechange == "DDJB")
                {
                    DDJBFlight newflight = new DDJBFlight(modify.FlightNumber, modify.Origin, modify.Destination, modify.ExpectedTime);
                    flight_dict[modify.FlightNumber] = newflight;
                }
                else if (codechange == "CFFT")
                {
                    CFFTFlight newflight = new CFFTFlight(modify.FlightNumber, modify.Origin, modify.Destination, modify.ExpectedTime);
                    flight_dict[modify.FlightNumber] = newflight;
                }
                else if (codechange == "LWTT")
                {
                    LWTTFlight newflight = new LWTTFlight(modify.FlightNumber, modify.Origin, modify.Destination, modify.ExpectedTime);
                    flight_dict[modify.FlightNumber] = newflight;
                }
                else if (codechange == "None")
                {
                    NORMFlight newflight = new NORMFlight(modify.FlightNumber, modify.Origin, modify.Destination, modify.ExpectedTime);
                    flight_dict[modify.FlightNumber] = newflight;
                }
                Console.WriteLine("Special Request Code updated.");
            }
            else if (choice == "4")
            {
                Console.Write($"Which gate would you like to assign Flight {modify.FlightNumber} to? ");
                string chosengate = Console.ReadLine();

                foreach (BoardingGate gate in gatesdict.Values)
                {
                    if (gate.GateName == chosengate)
                    {
                        if (gate.Flight != null)
                        {
                            Console.WriteLine("This boarding gate is unavailable at the moment.");
                            break;
                        }
                        else
                        {
                            gate.Flight = modify;
                            Console.WriteLine($"Flight {modify.FlightNumber} has been assigned to Boarding Gate {chosengate}");
                            break;
                        }
                    }
                }
            }
        }
        else if (option == "2")
        {
            Console.Write($"Are you sure you want to delete flight {modify.FlightNumber}? [Y/N] ");
            string confirm = Console.ReadLine().ToUpper();
            if (confirm == "Y")
            {
                airline_dict[code].Flights.Remove(modified);
                Console.WriteLine("Flight Removed Successfully");
            }
            else if (confirm == "N")
            {
                Console.WriteLine("Flight deletion cancelled.");
            }
        }
    }
    else
    {
        Console.WriteLine("Airline not found.");
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
    Console.WriteLine($"{"FlightNo",-10} {"Airline",-20} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-20} {"Code",-9} {"Status",-10} {"Boarding Gate"}");
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
        try
        {
            flight.Airline = T5.GetAirlineFromFlight(flight).Name;
        }
        catch (NullReferenceException)
        {
            continue;
        }
        Console.WriteLine($"{flight.ToString2(),-100}   {flight.Status,-10} {gate}");
    }


}


// Advanced Feature B
bool AirlineFees()
{
    // Check all flights assigned boarding Gates
    foreach (BoardingGate bg in gatesdict.Values)
    {
        if (bg.Flight == null)
        {
            Console.WriteLine("Ensure that all flights are assigned a boarding gate.");
            return false;
        }
        T5.GateFees.Add(bg.GateName, bg.CalculateFees());
    }

    // Go through airlines
    double total = 0;
    double totalDiscount = 0;
    Console.WriteLine($"{"Airline",-25} {"SubTotal",-15} {"Discount",-15} {"FinalTotal",-15} Discount Percentage of Total");
    foreach (Airline airline in airline_dict.Values)
    {
        double subtotalFee = airline.CalculateFees();
        double discount = airline.CalculateDiscount();

        double airlineTotal = subtotalFee - discount;
        total += airlineTotal;
        totalDiscount += discount;

        double percentageOfDiscount = discount / airlineTotal * 100;

        Console.WriteLine($"{airline.Name,-25} ${subtotalFee,-14:0.00} ${discount,-14:0.00} ${airlineTotal,-14:0.00} {percentageOfDiscount:0.00}%");

    }
    Console.WriteLine($"\n{"Terminal 5",-15} Total Fee: ${total,-10:0.00} Discount: ${totalDiscount,-10:0.00} Final Total: ${total - totalDiscount,-10:0.00} Discount Percentage of Total: {totalDiscount / total * 100:0.00}%");
    return true;

}


// WRITING MENU

while (true)
{
    try
    {
        Console.WriteLine("=============================================\r\nWelcome to Changi Airport Terminal 5\r\n=============================================\r\n1. List All Flights\r\n2. List Boarding Gates\r\n3. Assign a Boarding Gate to a Flight\r\n4. Create Flight\r\n5. Display Airline Flights\r\n6. Modify Flight Details\r\n7. Display Flight Schedule\r");
        Console.WriteLine("8. Advanced Feature 1\r\n9. Advanced Feature 2\r");
        Console.WriteLine("0. Exit\r\n\r\n");
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
        else if (option == "4")
        {
            NewFlight();

        }
        else if (option == "5")
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
        
        else if (option == "9")
        {
            AirlineFees();
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
