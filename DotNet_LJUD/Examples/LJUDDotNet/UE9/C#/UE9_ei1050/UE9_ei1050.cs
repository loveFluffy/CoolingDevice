//---------------------------------------------------------------------------
//
//  UE9_ei1050.cs
// 
//	Demonstrates talking to 1 or 2 EI-1050 probes.
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

namespace EIExample
{
	class EIExample
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			EIExample a = new EIExample();
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
			double dblValue=0;

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

			///*
			//Use this code if only a single EI-1050 is connected.
			//	Connections for the probe:
			//	Red (Power)         FIO2
			//	Black (Ground)      GND
			//	Green (Data)        FIO0
			//	White (Clock)       FIO1
			//	Brown (Enable)      FIO2
			
			try
			{
				//Set the Data line to FIO0, which is the default anyway. 
				LJUD.ePut(ue9.ljhandle, LJUD.IO.SHT_DATA_CHANNEL, 0, 0, 0);

				//Set the Clock line to FIO1, which is the default anyway. 
				LJUD.ePut (ue9.ljhandle, LJUD.IO.SHT_CLOCK_CHANNEL, (LJUD.CHANNEL) 1, 0, 0);

				//Set FIO2 to output-high to provide power to the EI-1050s. 
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, (LJUD.CHANNEL) 2, 1, 0);

				//Now, an add/go/get block to get the temp & humidity at the same time.
				//Request a temperature reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0);

				//Request a humidity reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0);

				//Execute the requests.  Will take about 0.5 seconds with a USB high-high
				//or Ethernet connection, and about 1.5 seconds with a normal USB connection.
				LJUD.GoOne (ue9.ljhandle);

				//Get the temperature reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, ref dblValue);
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg K\n",dblValue);
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg C\n",(dblValue-273.15));
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg F\n",(((dblValue-273.15)*1.8)+32));

				//Get the humidity reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, ref dblValue);
				Console.Out.WriteLine("RH Probe A = {0:0.###} percent\n\n",dblValue);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//End of single probe code.
			//*/


			/*
			//Use this code if two EI-1050 probes are connected.
			//	Connections for both probes:
			//	Red (Power)         FIO2
			//	Black (Ground)      GND
			//	Green (Data)        FIO0
			//	White (Clock)       FIO1
			//
			//	Probe A:
			//	Brown (Enable)    FIO3
			//
			//	Probe B:
			//	Brown (Enable)    DAC0
			try
			{
				//Set FIO3 to output-low to disable probe A. 
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, (LJUD.CHANNEL) 3, 0, 0);

				//Set DAC0 to 0 volts to disable probe B.
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DAC, (LJUD.CHANNEL) 0, 0.0, 0);

				//Set FIO3 to output-high to enable probe A. 
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, (LJUD.CHANNEL) 3, 1, 0);

				//Now, an add/go/get block to get the temp & humidity at the same time.
				//Request a temperature reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0);

				//Request a humidity reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0);

				//Execute the requests.  Will take about 0.5 seconds with a USB high-high
				//or Ethernet connection, and about 1.5 seconds with a normal USB connection.
				LJUD.GoOne (ue9.ljhandle);

				//Get the temperature reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, ref dblValue);
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg K\n",dblValue);
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg C\n",(dblValue-273.15));
				Console.Out.WriteLine("Temp Probe A = {0:0.###} deg F\n",(((dblValue-273.15)*1.8)+32));

				//Get the humidity reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, ref dblValue);
				Console.Out.WriteLine("RH Probe A = {0:0.###} percent\n\n",dblValue);

				//Set FIO3 to output-low to disable probe A. 
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, (LJUD.CHANNEL) 3, 0, 0);

				//Set DAC0 to 3.3 volts to enable probe B.
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DAC, 0, 3.3, 0);

				//Now, an add/go/get block to get the temp & humidity at the same time.
				//Request a temperature reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0);

				//Request a humidity reading from the EI-1050.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0);

				//Execute the requests.  Will take about 0.5 seconds with a USB high-high
				//or Ethernet connection, and about 1.5 seconds with a normal USB connection.
				LJUD.GoOne (ue9.ljhandle);

				//Get the temperature reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, ref dblValue);
				Console.Out.WriteLine("Temp Probe B = {0:0.###} deg K\n",dblValue);
				Console.Out.WriteLine("Temp Probe B = {0:0.###} deg C\n",(dblValue-273.15));
				Console.Out.WriteLine("Temp Probe B = {0:0.###} deg F\n",(((dblValue-273.15)*1.8)+32));

				//Get the humidity reading.
				LJUD.GetResult (ue9.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, ref dblValue);
				Console.Out.WriteLine("RH Probe B = {0:0.###} percent\n\n",dblValue);

				//Set DAC0 to 0 volts to disable probe B.
				LJUD.ePut (ue9.ljhandle, LJUD.IO.PUT_DAC, 0, 0.0, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//End of dual probe code.
			*/

			Console.ReadLine(); // Pause for user

		}
	}
}
