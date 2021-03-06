//---------------------------------------------------------------------------
//
//  UE9_Simple2.cs
// 
//  Basic command/response UD UE9 example.
//
//  support@labjack.com
//  June 2, 2009
//	Revised December 29, 2010
//----------------------------------------------------------------------
//

using System;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace SimpleTwo
{
	class SimpleTwo
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			SimpleTwo a = new SimpleTwo();
			a.performActions();
		}

		// If error occured print a message indicating which one occurred. If the error is a group error, quit
		public void showErrorMessage(LabJackUDException e)
		{
			Console.Out.WriteLine("Error: " + e.ToString());
			if (e.LJUDError > LJUD.LJUDERROR.MIN_GROUP_ERROR)
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
			double Value2=0,Value3=0;
			double ValueDIBit=0,ValueDIPort=0,ValueCounter=0;

			// dummy variables to satisfy certian method signatures
			double dummyDouble = 0;
			int dummyInt = 0;

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

			try
			{
				//First some configuration commands.  These will be done with the ePut
				//function which combines the add/go/get into a single call.

				//Configure for 16-bit analog input measurements.
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 16, 0);

				//Configure the analog input range on channels 2 and 3 for bipolar gain=1.
				LJUD.ePut (ue9.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 2, (double) LJUD.RANGES.BIP5V, 0);

				LJUD.ePut (ue9.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 3, (double) LJUD.RANGES.BIP5V, 0);

				//Enable Counter0 which will appear on FIO0 (assuming no other
				//program has enabled any timers or Counter1).
				LJUD.ePut (ue9.ljhandle,  LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0);


				//Now we add requests to write and read I/O.  These requests
				//will be processed repeatedly by go/get statements in every
				//iteration of the while loop below.

				//Request AIN2 and AIN3.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0);

				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_AIN, 3, 0, 0, 0);

				//Set DAC0 to 2.5 volts.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.PUT_DAC, 0, 2.5, 0, 0);

				//Read digital input FIO1.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 1, 0, 0, 0);

				//Set digital output FIO2 to output-high.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 2, 1, 0, 0);

				//Read digital inputs FIO3 through FIO7.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_DIGITAL_PORT, 3, 0, 5, 0);

				//Request the value of Counter0.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_COUNTER, 0, 0, 0, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}
			
			bool requestedExit = false;
			while (!requestedExit)
			{
				try
				{
					//Execute the requests.
					LJUD.GoOne (ue9.ljhandle);

					//Get all the results.  The input measurement results are stored.  All other
					//results are for configuration or output requests so we are just checking
					//whether there was an error.
					LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					showErrorMessage(e);
				}

				bool isFinished = false;
				while(!isFinished)
				{
						switch(ioType)
						{

							case LJUD.IO.GET_AIN :
							switch((int)channel)
							{
								case 2:
									Value2=dblValue;
									break;
								case 3:
									Value3=dblValue;
									break;
							}
								break;

							case LJUD.IO.GET_DIGITAL_BIT :
								ValueDIBit=dblValue;
								break;

							case LJUD.IO.GET_DIGITAL_PORT :
								ValueDIPort=dblValue;
								break;

							case LJUD.IO.GET_COUNTER :
								ValueCounter=dblValue;
								break;

						}
						try {LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
						catch (LabJackUDException e) 
						{
							// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
							if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
								isFinished = true;
							else
								showErrorMessage(e);
						}
					}

					// Output the results
					Console.Out.WriteLine("AIN2 = {0:0.00000}\n",Value2);
					Console.Out.WriteLine("AIN3 = {0:0.00000}\n",Value3);
					Console.Out.WriteLine("FIO1 = {0:0.00000}\n",ValueDIBit);
					Console.Out.WriteLine("FIO3-FIO7 = {0:0.00000}\n",ValueDIPort);  //Will read 31 if all 5 lines are pulled-high as normal.
					Console.Out.WriteLine("Counter0 (FIO0) = {0:0.00000}\n",ValueCounter);

					Console.Out.WriteLine("\nPress Enter to go again or (q) to quit\n");
					requestedExit = Console.ReadLine().Equals("q");
				}
			}
		}
	}

