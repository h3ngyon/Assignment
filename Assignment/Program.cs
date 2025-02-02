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
try
{
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
}
catch(FileNotFoundException ex)
{
    Console.WriteLine("File is not found.");
}

try
{
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
}
catch(FileNotFoundException ex)
{
    Console.WriteLine("File is not found.");
}


// Basic Feature 2
// Load Flights from flights.csv

Dictionary<string, Flight> flight_dict = new Dictionary<string, Flight>();
try
{
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
}
catch(FileNotFoundException ex)
{
    Console.WriteLine("File is not found.");
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
            Airline air = T5.GetAirlineFromFlight(flight);
            string airline = "";        // Flight does not belong to an actual airline
            if (air != null)            // If flight has an airline
            {
                 airline = air.Name;
            }
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
    try
    {
        Console.WriteLine("=============================================\r\nAssign a Boarding Gate to a Flight\r\n=============================================\r");

        // Initialize flight and bg variables
        Flight flight;
        BoardingGate bg;

        // Check input to make sure the flight exists
        while (true)
        {
            Console.Write("Input the flight number: ");
            string flightNo = Console.ReadLine().ToUpper();
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
            string bgateName = Console.ReadLine().ToUpper();
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
        while (true)
        {
            Console.WriteLine("Would you like to update the flight status? [Y/N]");
            string option = Console.ReadLine().ToLower();
            if (option == "y")
            {
                Console.WriteLine("1. Delayed\n2. Boarding\n3. On Time");
                while (true)
                {
                    Console.WriteLine("Please select the new status of the flight: ");
                    string status = Console.ReadLine();
                    if (status == "1")
                    {
                        flight.Status = "Delayed";
                        Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.\n");
                        break;
                    }
                    else if (status == "2")
                    {
                        flight.Status = "Boarding";
                        Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.\n");
                        break;
                    }
                    else if (status == "3")
                    {
                        flight.Status = "On Time";
                        Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }
                    
                }
                break;
            }
            else if (option == "n")
            {
                flight.Status = "On Time";
                Console.WriteLine($"{flight.FlightNumber} has been assigned to Boarding Gate {bg.GateName} successfully.\n");
                break;
            }
            else
            {
                Console.WriteLine("Invalid Option");
            }
        }
        // Assign Flight to Boarding Gate
        bg.Flight = flight;
        flight_dict[flight.FlightNumber].BoardingGate = bg; ;
    }
    catch(FormatException ex)
    {
        Console.WriteLine("Invalid Input");
    }
}



// Basic Feature 6: Create a new Flight
bool NewFlight()
{
    List<Flight> flights = new List<Flight>(); // Create list to display new flights created at the end of the method
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

            // Create the flight subclass based on the special request code 
            Flight newflight;
            if (code == "CFFT")
            {
                CFFTFlight flight = new CFFTFlight(flightNo, origin, destination, time);
                newflight = flight;
            }
            else if (code == "DDJB")
            {
                DDJBFlight flight = new DDJBFlight(flightNo, origin, destination, time);
                newflight = flight;
            }
            else if (code == "LWTT")
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
            newflight.Code = code;
            flight_dict.Add(newflight.FlightNumber, newflight);         // Add new flight to flight_dict
            flights.Add(newflight);                                     // Add new flight to list to be printed at the end of the method


            // Append Flight data to flights.csv
            using (StreamWriter sw = new StreamWriter("flights.csv", true))
            {
                string? s;
                sw.WriteLine($"{newflight.FlightNumber},{newflight.Origin},{newflight.Destination},{newflight.ExpectedTime},{code}");
            }

            // Prompt if user wants to create a new flight
            while (true)
            {
                Console.WriteLine("Would you like to create another flight? [Y/N]: ");
                string option = Console.ReadLine().ToLower();
                if (option == "y")
                {
                    continue;
                }
                else if (option == "n")
                {
                    break;
                }
                else        // Invalid input handling
                {
                    Console.WriteLine("Invalid option");
                }
            }
            // Display success message(s)
            foreach (Flight flight in flights)      // Display all new flights created
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
        try
        {
            Console.Write("Enter Airline Code: ");
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
BoardingGate FindBoardingGate(Flight modify)         // Method to find boarding gate that chosen flight belongs to
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

Flight FindFlight(string modified,string code)                                  // method for finding flight  from flight number
{
    foreach (KeyValuePair<string, Flight> kvp in airline_dict[code].Flights)    // run through each value in the flight dictionary from the selected airline
    {
        if (modified == kvp.Value.FlightNumber)                 // if the code matches a flight number in the flight dictionary within the airline, return the flight
        {
            return kvp.Value;
        }
    }
    return null;                                               // else return null
}
void ModifyFlights()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");            // header for modifying flight method
    Console.WriteLine("=============================================");

    Console.WriteLine($"{"Airline Code",-15} {"Airline Name",-20}");            // header for airline code and name
    foreach (Airline airLine in airline_dict.Values)                            // display all available airlines from the airline dictionary
    {
        Console.WriteLine($"{airLine.Code,-15} {airLine.Name,-20}");
    }
    Console.Write("Enter Airline Code: ");                  // prompts users to select airline that contains flight to modify
    string code = Console.ReadLine().ToUpper();             // upper is used in case the user's input is in lowercase, to match airline dictionary key
    if (string.IsNullOrWhiteSpace(code))                    // if airline code entered is null or blank
    {
        Console.WriteLine("Airline code cannot be empty."); // prints error message 
        return;
    }
    Airline airline;                        // define airline as an Airline object  

    if (airline_dict.ContainsKey(code))     // if airline dictionary contains the airline code  as key
    {
        airline = airline_dict[code];
        Console.WriteLine($"{"Flight Number",-20} {"Airline Name",-19} {"Origin",-19} {"Destination",-19} {"Expected Departure/Arrival Time",-19}");    // header for the flights in selected airline
        foreach (Flight flight in airline.Flights.Values)           // displays all flights and their details from the selected airline
        {
            Console.WriteLine($"{flight.FlightNumber,-20} {airline.Name,-20}{flight.Origin,-20}{flight.Destination,-20}{flight.ExpectedTime,-20}");
        }
        Console.Write("\nChoose a flight to modify or delete: ");       // prompts user for a flight to modify or delete
        string modified = Console.ReadLine().ToUpper();                 // upper is used to match the flight numbers in each flight
        Flight modify = FindFlight(modified, code);                     // use find flight method to find the flight
        BoardingGate gatefound = FindBoardingGate(modify);              // use find boarding gate to find the boarding gate that the flight belongs to
        Console.WriteLine("1. Modify Flight ");                         // give users options to either modify or delete
        Console.WriteLine("2. Delete Flight ");
        Console.Write("Choose an option: ");                            // prompts users to pick either 1 or 2
        string option = Console.ReadLine();

        //MODIFY
        if (option == "1")                                              // if option is 1 (to modify) 
        {
            Console.WriteLine("1. Modify Basic Information");           // give users options to either modify basic info , status , special request code or boarding gate
            Console.WriteLine("2. Modify Status");
            Console.WriteLine("3. Modify Special Request Code");
            Console.WriteLine("4. Modify Boarding Gate");
            Console.Write("Choose an option: ");                        // prompt user to select option
            string choice = Console.ReadLine();

            //MODIFY BASIC FEATURES
            if (choice == "1")                                          // user decided to modify basic details
            {
                Console.Write("Enter new origin: ");            // ask user to enter new origin
                string origin = Console.ReadLine();
                Console.Write("Enter new Destination: ");       // ask user to enter the new destination
                string dest = Console.ReadLine();
                Console.Write("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");        // ask user to enter new expected arrival/depatrture time
                DateTime ex = Convert.ToDateTime(Console.ReadLine());
                modify.Origin = origin;                 // update the flight with the new modifications
                modify.Destination = dest;
                modify.ExpectedTime = ex;
                Console.WriteLine("Flight has been updated!");      // print success message
                Console.WriteLine($"Flight number: {modify.FlightNumber}");     // print details of the newly modified flight
                Console.WriteLine($"Airline Name: {airline.Code}");
                Console.WriteLine($"Origin: {modify.Origin}");
                Console.WriteLine($"Destination: {modify.Destination}");
                Console.WriteLine($"Expected Departure/Arrival Time: {modify.ExpectedTime}");
                Console.WriteLine($"Status: {modify.Status}");
                if (modify is DDJBFlight)           // print the special request code of the flight according to the subclass the 
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
                else                               // if there is no special request code for the flight
                {
                    Console.WriteLine("Special Request Code: None");
                }
                // show flight boarding gate
                if (gatefound == null)              // if there is no boarding gate assigned
                {
                    Console.WriteLine("Boarding Gate: Unassigned");     //print that boarding gate is not assigned
                }
                else            // if there is a boarding gate assigned 
                {
                    string gatename = gatefound.GateName;
                    Console.WriteLine($"Boarding Gate: {gatename}");            // display boarding gate
                }
            }

            // MODIFY STATUS
            else if (choice == "2")             // if user decides to update the status
            {
                Console.Write("Enter new Status: ");    // prompts user to enter new status 
                string statuschange = Console.ReadLine();
                modify.Status = statuschange;         // update new status
                Console.WriteLine("Flight status changed!");
            }

            //MODIFY SPECIAL REQUEST CODE
            else if (choice == "3")
            {
                Console.Write("Enter new special request code: ");      // prompts user to enter special request code to change to 
                string codechange = Console.ReadLine().ToUpper();       // upper to match available special request codes
                if (codechange == "DDJB")                               // change the special request code of the flight according to what user has entered 
                {
                    DDJBFlight newflight = new DDJBFlight(modify.FlightNumber, modify.Origin, modify.Destination, modify.ExpectedTime);
                    flight_dict[modify.FlightNumber] = newflight;       // update the flight dictionary the newly modified flight
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
                Console.WriteLine("Special Request Code updated.");         // display success message
            }

            //MODIFY BOARDING GATE
            else if (choice == "4")         
            {
                Console.Write($"Which gate would you like to assign Flight {modify.FlightNumber} to? ");    //prompts user which gate to assign the flight to
                string chosengate = Console.ReadLine().ToUpper();

                foreach (BoardingGate gate in gatesdict.Values)         // run through the entire gatesdict
                {
                    if (gate.GateName == chosengate)                // if the name of the boarding gate matches the gate entered
                    {
                        if (gate.Flight != null)
                        {
                            Console.WriteLine("This boarding gate is unavailable at the moment.");      // if boarding gate is occupied by a flight, display error
                            break;
                        }
                        else                                    // if not occupied, assign the flight to the boarding gate
                        {
                            gate.Flight = modify;
                            Console.WriteLine($"Flight {modify.FlightNumber} has been assigned to Boarding Gate {chosengate}");     // success message
                            break;
                        }
                    }
                }
            }
        }

        //DELETE
        else if (option == "2") 
        {
            Console.Write($"Are you sure you want to delete flight {modify.FlightNumber}? [Y/N] ");     // ask user for confirmation to delete flight
            string confirm = Console.ReadLine().ToUpper();          // convert to uppercase
            if (confirm == "Y")         // if user confirms to delete
            {
                airline_dict[code].Flights.Remove(modify.FlightNumber);        // remove flight from its respective airline
                Console.WriteLine("Flight Removed Successfully");   //
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
    List<Flight> flights = new List<Flight>();      // Create list to store all flight objects to be sorted later
    foreach (Flight flight in flight_dict.Values)   // Add flight objects into the list
    {
        flights.Add(flight);
    }

    flights.Sort(); // Sort Flights
    Console.WriteLine($"{"FlightNo",-10} {"Airline",-20} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-20} {"Code",-9} {"Status",-10} {"Boarding Gate"}");
    foreach (Flight flight in flights)
    {
        string gate = ""; 
        foreach (BoardingGate bg in gatesdict.Values)
        {
            if (bg.Flight == flight)
            {
                gate = bg.GateName; //Get flight gate
                break;
            }
            gate = "Unassigned";
        }
        try
        {
            Airline air = T5.GetAirlineFromFlight(flight);// Get flight airline name
            string airline = "";        // Flight does not belong to an actual airline
            if (air != null)            // If flight has an airline
            {
                airline = air.Name;
            }
            flight.Airline = airline;

        }
        catch (NullReferenceException)
        {
            continue;
        }
        Console.WriteLine($"{flight.ToString2(),-100}   {flight.Status,-10} {gate}");
    }

    
}

// Advanced Feature A
void ProcessFlights()
{
    Queue<Flight?> unassignedFlights = new Queue<Flight?>();                   // Queue for unassigned flights
    List<BoardingGate> unassignedGates = new List<BoardingGate>();

    BoardingGate SearchGate(Flight flight)
    {
        // o  check if the Flight has a Special Request Code
        string code = flight.Code;
        foreach (BoardingGate bg in unassignedGates)
        {
            if (flight.BoardingGate == bg && bg.Flight == flight) { return bg; }
            else if (flight.BoardingGate == null)
            {
                if (flight.Code == "") { return bg; }
                List<string> supportCode = new List<string>();
                //  if yes, search for an unassigned Boarding Gate that matches the Special Request Code
                if (bg.SupportsDDJB && code == "DDJB") { supportCode.Add("DDJB"); }
                if (bg.SupportsCFFT && code == "CFFT") { supportCode.Add("CFFT"); }
                if (bg.SupportsLWTT && code == "LWTT") { supportCode.Add("LWTT"); }
                //  if no, search for an unassigned Boarding Gate that has no Special Request Code
                if (bg.SupportsDDJB == false && bg.SupportsLWTT == false && bg.SupportsCFFT == false) { supportCode.Add(""); }

                if (supportCode.Contains(code)) { return bg; }
            }

        }
        return null;
    }
    //  for each Flight, check if a Boarding Gate is assigned; if there is none, add it to a queue
    int flightUnassignedCount = 0;
    foreach (Flight flight in flight_dict.Values)
    {
        if (flight.BoardingGate == null)
        {
            unassignedFlights.Enqueue(flight);
            flightUnassignedCount += 1;
        }
    }
    int flightsCount = flight_dict.Count() - flightUnassignedCount;

    //  display the total number of Flights that do not have any Boarding Gate assigned yet
    Console.WriteLine($"{flightUnassignedCount} flights have not been assigned any boarding gates.");

    //  for each Boarding Gate, check if a Flight Number has been assigned
    int bgUnassignedCount = 0;
    foreach (BoardingGate bg in gatesdict.Values)
    {
        if (bg.Flight == null)
        {
            bgUnassignedCount += 1;
            unassignedGates.Add(bg);
        }
    }
    int bgCount = gatesdict.Count() - bgUnassignedCount;

    //  display the total number of Boarding Gates that do not have a Flight Number assigned yet
    Console.WriteLine($"{bgUnassignedCount} Boarding Gates have not been assigned any flights.");


    int autoFlightCount = 0;
    int autoGateCount = 0;
    if (flightUnassignedCount <= flight_dict.Count())
    {
        Console.WriteLine($"{"FlightNo",-10} {"Airline",-20} {"Origin",-18}  {"Destination",-18}  {"ExpectedTime",-20} {"Code",-10} {"Boarding Gate",-10}");
        // for each Flight in the queue, dequeue the first Flight in the queue
        while (unassignedFlights.Count > 0)
        {
            // Check through boarding gates available
            Flight flight = unassignedFlights.Dequeue();
            if (flight.BoardingGate == null)
            {
                string code = flight.Code;
                BoardingGate bg = SearchGate(flight);
                Airline _ = T5.GetAirlineFromFlight(flight);
                if (_ != null)
                {
                    flight.Airline = T5.GetAirlineFromFlight(flight).Name;
                }
                flight.BoardingGate = bg;
                //o  assign the Boarding Gate to the Flight Number
                bg.Flight = flight;

                unassignedGates.Remove(bg);
                /*o  display the Flight details with Basic Information of all Flights, which are the 5 flight specifications 
                 * (i.e. Flight Number, Airline Name, Origin, Destination, and Expected Departure/Arrival Time), Special Request Code (if any) and Boarding Gate*/
                Console.WriteLine($"{flight,-92} {code,-10} {bg.GateName}");

                autoFlightCount += 1;
                autoGateCount += 1;
            }
        }
    }
    Console.WriteLine();
    //  display the total number of Flights and Boarding Gates processed and assigned
    int totalProcessedFlights = 0;
    int totalProcessedGates = 0;
    foreach (Flight flight in flight_dict.Values)
    {
        if (flight.BoardingGate != null) { totalProcessedFlights += 1; }
    }
    foreach (BoardingGate bg in gatesdict.Values)
    {
        if (bg.Flight != null) { totalProcessedGates += 1; }
    }
    Console.WriteLine($"Total number of Flights processed: {totalProcessedFlights}\nTotal number of Boarding Gates processed: {totalProcessedGates}");

    //  display the total number of Flights and Boarding Gates that were processed automatically over those that were already assigned as a percentage
    try
    {
        Console.WriteLine($"Percentage of automatic assignment over previously assigned Flights and Boarding Gates:\nFlights: {autoFlightCount / flightsCount * 100}% Boarding Gates: {autoGateCount / bgCount * 100}%");
    }
    catch (DivideByZeroException)
    {
        Console.WriteLine("There were no flights that were already assigned to a boarding gate.");
    }
}
// Advanced Feature B
bool AirlineFees()
{
    // Check all flights assigned boarding Gates
    foreach (Flight flight in flight_dict.Values)
    {
        if (flight.BoardingGate == null)
        {
            Console.WriteLine("Ensure that all flights are assigned a boarding gate.");
            return false;
        }
        T5.GateFees.Add(flight.BoardingGate.GateName, flight.BoardingGate.CalculateFees());
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
        else if (option == "8")
        {
            ProcessFlights();
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
