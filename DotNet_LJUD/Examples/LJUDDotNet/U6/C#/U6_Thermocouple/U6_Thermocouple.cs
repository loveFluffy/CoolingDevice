//Demonstrates using the TCVoltsToTemp function to yield temperature
//  readings for a thermocouple directly connected to a U6.
//
//  Setup:
//  TC plus to AIN0.
//  TC minus to GND.
//
//  If you do not want all the thermocouple minus leads tied together
//  at GND, you can use differential channels instead.  Connect the minus
//  to AIN1 in that case, and use a 10k resistor from AIN1 to GND to
//  provide a path for bias currents.
//
//support@labjack.com
//December 29, 2009
//Revised December 28, 2010

using System;
using System.Threading;
using System.Runtime.InteropServices;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace Thermocouple
{
	class Thermocouple
	{
		// our U6 variable
		private U6 u6;

		// Variables to satisfy certain method signatures
		int dummyInt = 0;
		double dummyDouble = 0;

		static void Main(string[] args)
		{
			Thermocouple a = new Thermocouple();
			a.performActions();
		}

		// If error occured print a message indicating which one occurred. If the error is a group error (communication/fatal), quit
		public void showErrorMessage(LabJackUDException e)
		{
			Console.Out.WriteLine("Error: " + e.ToString());
			if (e.LJUDError > U6.LJUDERROR.MIN_GROUP_ERROR)
			{
				Console.ReadLine(); // Pause for the user
				Environment.Exit(-1);
			}
		}

		public void performActions()
		{
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double valueAIN = 0;  //Analog Voltage Value
			LJUD.CHANNEL tempChannel = 0;  //Channel which the TC/LJTIA is on (AIN0).
			double ainResolution = 0;	//Configure resolution of the analog inputs (pass a non-zero value for quick sampling). 										//See section 2.6 / 3.1 for more information.
			double dblInternal = 0;
			double range = (double)LJUD.RANGES.BIPP1V;

			// Variables to satisfy certain method signatures
			int dummyInt = 0;
			double dummyDouble = 0;

			double tcVolts = 0, cjTempK = 0, pTCTempK = 0;
			LJUD.THERMOCOUPLETYPE tcType = LJUD.THERMOCOUPLETYPE.K;
			//Set the temperature sensor to a k type thermocouple
			//Possible Thermocouple types are:
			//B = 6001
			//E = 6002
			//J = 6003
			//K = 6004
			//N = 6005
			//R = 6006
			//S = 6007
			//T = 6008

			//Open the first found LabJack U6 via USB.
			try 
			{
				u6 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			try
			{
				//Configure the desired resolution. See section 2.6 / 3.1 of the User's Guide
				LJUD.eGet(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, ref ainResolution, 0);
				
				// Set the range on the ananlog input channel to +/- 0.1 volts (x100 gain)
				LJUD.eGet(u6.ljhandle, LJUD.IO.PUT_AIN_RANGE, channel, ref range, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			Console.Out.WriteLine("Press any key to quit\n");

			//Constantly acquire temperature readings until a key is pressed
			bool keyPressed = false;
			while(!keyPressed) 
			{
				ioType = 0;
				channel = 0;
				tcVolts = 0;
				cjTempK = 0;
				pTCTempK = 0;
				
				try
				{
					//Add analog input requests.
					LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, (LJUD.CHANNEL)tempChannel, 0, 0, 0);

					//Add request for internal temperature reading -- Internal temp sensor uses 
					//analog input channel 14.
					LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, 14, 0, 0, 0);

					//Execute all requests on the labjack u6.ljhandle.
					LJUD.GoOne (u6.ljhandle);

					//Get all the results.  The first result should be the voltage reading of the 
					//temperature channel.
					LJUD.GetFirstResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					showErrorMessage(e);
				}

				//Get the rest of the results.  There should only be one more on the request 
				//queue.
				bool finished = false;
				while(!finished) 
				{
					if(ioType == LJUD.IO.GET_AIN) 
					{
						if(channel == tempChannel)
							tcVolts = dblValue;
				
						if(channel == (LJUD.CHANNEL)14)
							dblInternal = dblValue;
					}	
			
					try { LJUD.GetNextResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
					catch (LabJackUDException e)
					{
						if (e.LJUDError == LJUD.LJUDERROR.NO_DATA_AVAILABLE)
							finished = true;
						else if (e.LJUDError > LJUD.LJUDERROR.MIN_GROUP_ERROR)
							finished = true;
						else
							showErrorMessage(e);
					}
				}

				//The cold junction is the screw-terminal block where the thermocouple
				//is connected.  As discussed in the U6 User's Guide, add 2.5 degrees C
				//to the internal temp sensor reading.  If using the CB37 rather than
				//the built-in screw terminals, just add 1.0 degrees C.
				cjTempK = dblInternal + 2.5;

				//Display Voltage Reading
				Console.Out.WriteLine("Analog {0:0}:           {1:0.######}\n", (int)tempChannel, valueAIN);

				//Display the internal temperature sensor reading.  This example uses
				//that value for cold junction compensation.
				Console.Out.WriteLine("U6 internal sensor:  {0:0.0} deg K\n", (double)dblInternal);

				//Convert TC voltage to temperature.
				LJUD.TCVoltsToTemp(tcType, tcVolts, cjTempK, ref pTCTempK);

				//Display Temperature
				Console.Out.WriteLine("Thermocouple sensor:  {0:0.0} deg K\n\n", pTCTempK);

				Thread.Sleep(1500); // Short pause

				keyPressed = Win32Interop._kbhit() != 0; // If a key was hit break out of the loop
			}
	
		}
	}
	public class Win32Interop
	{
		[DllImport("crtdll.dll")]
		public static extern int _kbhit();
	}
}
