//---------------------------------------------------------------------------
//
//  UE9_SimpleStream.cs
// 
//  2-channel stream of AIN0 and AIN1.
//
//  support@labjack.com
//  June 2, 2009
//	Revised December 29, 2010
//----------------------------------------------------------------------
//

using System;
using System.Threading;
using System.Runtime.InteropServices;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace SimpleStream
{
	class SimpleStream
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			SimpleStream a = new SimpleStream();
			a.performActions();
		}

		// If error occured print a message indicating which one occurred. If the error is a group error, quit
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
			long i=0,k=0;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0, dblCommBacklog=0, dblUDBacklog=0;
			double scanRate = 1000;  //scan rate = sample rate / #channels
			int delayms = 1000;
			double numScans = 2000;  //Max number of scans per read.  2x the expected # of scans (2*scanRate*delayms/1000).
			double numScansRequested;
			double[] adblData = new double[4000];  //Max buffer size (#channels*numScansRequested)

			// Dummy variables to satisfy certain method signatures
			double dummyDouble = 0;
			double[] dummyDoubleArray = {0};
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
				//Configure the stream:
				//Configure all analog inputs for 12-bit resolution
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 12, 0, 0);
				
				//Configure the analog input range on channel 0 for bipolar +-5 volts.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 0, (double)LJUD.RANGES.BIP5V, 0, 0);
				
				//Set the scan rate.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_SCAN_FREQUENCY, scanRate, 0, 0);
				
				//Give the driver a 5 second buffer (scanRate * 2 channels * 5 seconds).
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_BUFFER_SIZE, scanRate*2*5, 0, 0);
				
				//Configure reads to retrieve whatever data is available without waiting (wait mode LJUD.STREAMWAITMODES.NONE).
				//See comments below to change this program to use LJUD.STREAMWAITMODES.SLEEP mode.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_WAIT_MODE, (double)LJUD.STREAMWAITMODES.NONE, 0, 0);
				
				//Define the scan list as AIN0 then AIN1.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.CLEAR_STREAM_CHANNELS, 0, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 0, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 1, 0, 0, 0);
			    
				//Execute the list of requests.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}
		    
			// Get results until there is no more data available for error checking
			bool isFinished = false;
			while(!isFinished)
			{
				try { LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dummyDouble, ref dummyInt, ref dummyDouble); }
				catch (LabJackUDException e) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
						showErrorMessage(e);
				}
			}
		    
			//Start the stream.
			LJUD.eGet(ue9.ljhandle, LJUD.IO.START_STREAM, 0, ref dblValue, 0);

			//The actual scan rate is dependent on how the desired scan rate divides into
			//the LabJack clock.  The actual scan rate is returned in the value parameter
			//from the start stream command.
			Console.Out.WriteLine("Actual Scan Rate = {0:0.###}\n",dblValue);
			Console.Out.WriteLine("Actual Sample Rate = {0:0.###}\n",2*dblValue);

			//Read data
			while(Win32Interop._kbhit() == 0)	//Loop will run until any key is hit
			{
				//Since we are using wait mode LJUD.STREAMWAITMODES.NONE, we will wait a little, then
				//read however much data is available.  Thus this delay will control how
				//fast the program loops and how much data is read each loop.  An
				//alternative common method is to use wait mode LJUD.STREAMWAITMODES.SLEEP where the
				//stream read waits for a certain number of scans.  In such a case
				//you would not have a delay here, since the stream read will actually
				//control how fast the program loops.
				//
				//To change this program to use sleep mode,
				//	-change numScans to the actual number of scans desired per read,
				//	-change wait mode addrequest value to LJUD.STREAMWAITMODES.SLEEP,
				//	-comment out the following Thread.Sleep command.

				Thread.Sleep(delayms);	//Remove if using LJUD.STREAMWAITMODES.SLEEP.

				//init array so we can easily tell if it has changed
				for(k=0;k<numScans*2;k++)
				{
					adblData[k] = 9999.0;
				}

				//Read the data.  We will request twice the number we expect, to
				//make sure we get everything that is available.
				//Note that the array we pass must be sized to hold enough SAMPLES, and
				//the Value we pass specifies the number of SCANS to read.
				numScansRequested=numScans;
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_STREAM_DATA, LJUD.CHANNEL.ALL_CHANNELS, ref numScansRequested, adblData);
				
				//The displays the number of scans that were actually read.
				Console.Out.WriteLine("\nIteration # {0:0}\n",i);
				Console.Out.WriteLine("Number scans read = {0:0}\n",numScansRequested);

				//Display just the first scan.
				Console.Out.WriteLine("First scan = {0:0.###}, {1:0.###}\n",adblData[0],adblData[1]);

				//Retrieve the current Comm backlog.  The UD driver retrieves stream data from
				//the UE9 in the background, but if the computer is too slow for some reason
				//the driver might not be able to read the data as fast as the UE9 is
				//acquiring it, and thus there will be data left over in the UE9 buffer.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_COMM, ref dblCommBacklog, 0);
				Console.Out.WriteLine("Comm Backlog = {0:0.###}\n",dblCommBacklog);

				//Retrieve the current UD driver backlog.  If this is growing, then the application
				//software is not pulling data from the UD driver fast enough.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_UD, ref dblUDBacklog, 0);
				Console.Out.WriteLine("UD Backlog = {0:0.###}\n",dblUDBacklog);

				i++;
			}

		   
			//Stop the stream
			LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, 0, ref dummyDouble, dummyDoubleArray);


			Console.Out.WriteLine("\nDone");
			Console.ReadLine(); // Pause for user
		}
	}

	public class Win32Interop
	{
		[DllImport("crtdll.dll")]
		public static extern int _kbhit();
	}
}