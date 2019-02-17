//---------------------------------------------------------------------------
//
//  UE9_AllIO.cs
// 
//	Demonstrates using the add/go/get method to efficiently write and read
//	virtually all analog and digital I/O on the LabJack UE9.
//	Records the time for 1000 iterations and divides by 1000, to allow
//	verification of the basic command/response communication times of the
//	LabJack UE9 as documented in Section 3.1 of the UE9 User's Guide.
//
//  support@labjack.com
//  June 1, 2009
//	Revised December 29, 2010
//----------------------------------------------------------------------
//

using System;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace AllIO
{
	class AllIO
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			AllIO a = new AllIO();
			a.performActions();
		}

		// If error occured print a message indicating which one occurred. If the error is a group error (communication/fatal), quit
		public void showErrorMessage(LabJackUDException e)
		{
			Console.Out.WriteLine("Error: " + e.ToString());
			if (e.LJUDError > UE9.LJUDERROR.MIN_GROUP_ERROR)
			{
				Console.ReadLine(); // Pause for the user
				Environment.Exit(-1);
			}
		}

		public void performActions()
		{
			// Open UE9
			try 
			{
				ue9 = new UE9(LJUD.CONNECTION.USB, "0", true); // Connection through USB
				//ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true); // Connection through ethernet
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			// Variables to pass to and from the driver object
			long ainResolution = 12;
			UE9.IO IOType = (UE9.IO)0;
			UE9.CHANNEL channel = (UE9.CHANNEL) 0;
			double val = 0;
			double dblVal = 0; // Temporary variable required by GetFirstResult
			int intVal = 0; // Temporary variable required by GetFirstResult

			// General variable settings
			long numIterations = 1000;
			long numChannels = 6;  //Number of AIN channels, 0-16.

			// Variables to store results
			double ValueDIPort=0;
			double[] ValueAIN = new double[16];

			try
			{
				// Set DAC0 to 2.5 volts
				LJUD.AddRequest(ue9.ljhandle, UE9.IO.PUT_DAC, 0, 2.5, 0, 0);

				// Set DAC1 to 3.5 volts
				LJUD.AddRequest(ue9.ljhandle, UE9.IO.PUT_DAC, 1, 3.5, 0, 0);

				//Write all digital I/O.  Doing a bunch of bit instructions, rather than
				//the following port instruction, should not make a noticable difference
				//in the overall execution time.
				LJUD.AddRequest (ue9.ljhandle, UE9.IO.PUT_DIGITAL_PORT, 0, 0, 23, 0);
				
				//Configure the desired resolution.  Note that depending on resolution and
				//number of analog inputs, numIterations might need to be reduced from the
				//default above so the program does not take too long to execute.
				LJUD.AddRequest (ue9.ljhandle, UE9.IO.PUT_CONFIG, UE9.CHANNEL.AIN_RESOLUTION, ainResolution, 0, 0);

				//Add analog input requests.
				for(int j=0;j<numChannels;j++)
				{
					LJUD.AddRequest (ue9.ljhandle, UE9.IO.GET_AIN, j, 0, 0, 0);
				}

				//Request a read of all digital I/O.  Doing a bunch of bit instructions,
				//rather than the following port instruction, should not make a noticable
				//difference in the overall execution time.
				LJUD.AddRequest(ue9.ljhandle, UE9.IO.GET_DIGITAL_PORT, 0, 0, 23, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			// Get the current tick count for a reference frame
			double time = System.Environment.TickCount;

			for(int i=0;i<numIterations;i++)
			{
				
				try
				{
					//Execute the requests.
					LJUD.GoOne(ue9.ljhandle);

					//Get all the results.  The input measurement results are stored.  All other
					//results are for configuration or output requests so we are just checking
					//whether there was an error.
					LJUD.GetFirstResult(ue9.ljhandle, ref IOType, ref channel, ref val, ref intVal, ref dblVal);
				}
				catch (LabJackUDException e) 
				{
					showErrorMessage(e);
				}
				
				// Get results until there is no more data available
				bool isFinished = false;
				while(!isFinished)
				{
					switch(IOType)
					{

						case LJUD.IO.GET_AIN:
							ValueAIN[(int)channel]=val;
							break;

						case LJUD.IO.GET_DIGITAL_PORT:
							ValueDIPort=val;
							break;
					}

					try 
					{
						LJUD.GetNextResult(ue9.ljhandle, ref IOType, ref channel, ref val, ref intVal, ref dblVal);
					}
					catch (LabJackUDException e) 
					{
						// If we get an error, report it.  If there is no more data available we are done
						if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
							isFinished = true;
						else
							showErrorMessage(e);
					}
				}

			}

			// Determine how long it took to perform the above tasks and display the millisecounds per iteration along with the readings
			time = System.Environment.TickCount - time;

			Console.Out.WriteLine("Milleseconds per iteration = " + ((double)time / (double)numIterations) + "\n");

			Console.Out.WriteLine("\nDigital Input = " + ValueDIPort + "\n");

			Console.Out.WriteLine("\nAIN readings from last iteration:\n");
			for(int j=0;j<numChannels;j++)
			{
				Console.Out.WriteLine("{0:0.###}\n", ValueAIN[j]);
			}

			Console.ReadLine(); // Pause for the user
		}

	}

}

