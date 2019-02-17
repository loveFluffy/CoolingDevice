//---------------------------------------------------------------------------
//
//  U6_Simple.cs
// 
//  Basic command/response U6 example using the UD driver.
//
//  support@labjack.com
//  June 5, 2009
//	Revised December 28, 2010
//----------------------------------------------------------------------
//

using System;
using System.Threading;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace Simple
{
	class Simple
	{
		String str1; 
		// our U6 variable
		private U6 u6;

		// Variables to satisfy certain method signatures
		int dummyInt = 0;
		double dummyDouble = 0;

		static void Main(string[] args)
		{
			Simple a = new Simple();
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
			double dblDriverVersion;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double Value0=9999,Value1=9999,Value2=9999;
			double ValueDIBit=9999,ValueDIPort=9999,ValueCounter=9999;


			//Read and display the UD version.
			dblDriverVersion = LJUD.GetDriverVersion();
			Console.Out.WriteLine("UD Driver Version = {0:0.000}\n\n",dblDriverVersion);


			//Open the first found LabJack U6.
			try 
			{
				u6 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}


			//First some configuration commands.  These will be done with the ePut
			//function which combines the add/go/get into a single call.

			//Set the timer/counter pin offset to 3, which will put the first
			//timer/counter on FIO3.
			LJUD.ePut (u6.ljhandle,  LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 3, 0);

			//Enable Counter1 (FIO3).
			LJUD.ePut (u6.ljhandle,  LJUD.IO.PUT_COUNTER_ENABLE, (LJUD.CHANNEL)1, 1, 0);


			//The following commands will use the add-go-get method to group
			//multiple requests into a single low-level function.

			//Request a single-ended reading from AIN0.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, 0, 0, 0, 0);

			//Request a single-ended reading from AIN1.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, 1, 0, 0, 0);

			//Request a reading from AIN2 using the Special range.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN_DIFF, 2, 0, 15, 0);

			//Set DAC0 to 3.5 volts.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.PUT_DAC, 0, 3.5, 0, 0);

			//Set digital output FIO0 to output-high.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 0, 1, 0, 0);

			//Read digital input FIO1.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 1, 0, 0, 0);

			//Read digital inputs FIO1 through FIO2.
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_DIGITAL_PORT, 1, 0, 2, 0);

			//Request the value of Counter1 (FIO3).
			LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_COUNTER, 1, 0, 0, 0);


			bool requestedExit = false;
			while (!requestedExit)
			{
				try
				{
					//Execute the requests.
					LJUD.GoOne (u6.ljhandle);

					//Get all the results.  The input measurement results are stored.  All other
					//results are for configuration or output requests so we are just checking
					//whether there was an error.
					LJUD.GetFirstResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					showErrorMessage(e);
				}

				bool finished = false;
				while(!finished)
				{
					switch(ioType)
					{

						case LJUD.IO.GET_AIN :
						switch((int)channel)
						{
							case 0:
								Value0=dblValue;
								break;
							case 1:
								Value1=dblValue;
								break;
						}
							break;

						case LJUD.IO.GET_AIN_DIFF :
							Value2=dblValue;
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
					try {LJUD.GetNextResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
					catch (LabJackUDException e) 
					{
						// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
						if(e.LJUDError == U6.LJUDERROR.NO_MORE_DATA_AVAILABLE)
							finished = true;
						else
							showErrorMessage(e);
					}
				}

				Console.Out.WriteLine("AIN0 = {0:0.###}\n",Value0);
				Console.Out.WriteLine("AIN1 = {0:0.###}\n",Value1);
				Console.Out.WriteLine("AIN2 = {0:0.###}\n",Value2);
				Console.Out.WriteLine("FIO1 = {0:0.###}\n",ValueDIBit);
				Console.Out.WriteLine("FIO1-FIO2 = {0:0.###}\n",ValueDIPort);  //Will read 3 (binary 11) if both lines are pulled-high as normal.
				Console.Out.WriteLine("Counter1 (FIO3) = {0:0.###}\n",ValueCounter);

				Console.Out.WriteLine("\nPress Enter to go again or (q) to quit\n");
				str1 = Console.ReadLine(); // Pause for user
				requestedExit = str1 == "q";
			}


		}
	}
}
