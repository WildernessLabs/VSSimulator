# VSSimulator
This tool simulates Visual Studio's debug interface. This code acts like a TCP client used in place of Visual Studio itself. This tool has a simple UI that allows the user to send packets (default of 123 bytes) and display what is received.
The within Meadow.CLI is a debugging server that can receive from and send to this simulator. Meadow.CLI forwards all data received to Meadow which in turn forwards to MONO via another debugging server. The process is reversed by the Mono debugging server sending all Mono debug data to Meadow.CLI which forwards to Visual Studio.
For testing a echo server was built within HCOM that would echo all data received from this VSSimulator. 
