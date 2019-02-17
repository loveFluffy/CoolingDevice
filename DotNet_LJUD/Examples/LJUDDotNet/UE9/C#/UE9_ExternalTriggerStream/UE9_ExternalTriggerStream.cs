//---------------------------------------------------------------------------
//
//  ExternalTriggerStream.cs
// 
//	Does a 6-channel externally triggered stream of AIN0, AIN1, EIO_FIO,
//	MIO_CIO, Timer0_Low, and Timer0_High.
//
//	Timer0 (FIO0) is enabled as SystemTimerLow (mode 10).  With an externally
//	triggered stream, the timing is determined by the external trigger signal,
//	so this timer value can be used to timestamp each scan.
//
//	Timer1 (FIO1) is enabled as frequency output at 1 kHz, and can be
//	used as the external trigger signal for testing.
//
//	External triggering is enabled for the stream, so the trigger signal should
//	be connected to FIO2.
//
//	For testing, connect FIO1 to FIO2.
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
			double dblValue=0, dblCommBacklog=0;

			// Dummy variables to satisfy certain method signatures
			double dummyDouble = 0;
			int dummyInt = 0;
			double[] dummyDoubleArray = {0.0};

			//The actual scan rate is determined by the external clock, but we need
			//an idea of how fast the scan rate will be so that we can make
			//the buffers big enough.  Also, the driver needs to have an idea of the
			//expected scan rate to help it decide how big of packets to transfer.
			double scanRate = 1000;
			int delayms = 1000;
			double numScans = 2000;  //2x the expected # of scans (2*scanRate*delayms/1000)
			double numScansRequested;
			double[] adblData = new double[12000];  //Max buffer size (#channels*numScansRequested)

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
				//Make sure the UE9 is not streaming.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, (LJUD.CHANNEL) 0, ref dummyDouble, 0);
			}
			catch (LabJackUDException e) 
			{
				// If the error indicates that the stream could not be stopped it is because the stream has not started yet and can be ignored 
				if (e.LJUDError != LJUD.LJUDERROR.UNABLE_TO_STOP_STREAM)
					showErrorMessage(e);
			}

			try
			{
				//Disable all timers and counters to put everything in a known initial state.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);


				//First we will configure Timer0 as system timer low and configure Timer1 to
				//output a 1000 Hz square wave.

				//Use the fixed 750kHz timer clock source.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double) LJUD.TIMERCLOCKS.KHZ750, 0, 0);

				//Set the divisor to 3 so the actual timer clock is 250 kHz.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0);

				//Enable 2 timers.  They will use FIO0-FIO1.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0);

				//Configure Timer0 as system timer low.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, (LJUD.CHANNEL) 0, (double) LJUD.TIMERMODE.SYSTIMERLOW, 0, 0);

				//Configure Timer1 as frequency output.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, (LJUD.CHANNEL) 1, (double) LJUD.TIMERMODE.FREQOUT, 0, 0);

				//Set the frequency output on Timer1 to 1000 Hz (250000/(2*125) = 1000).
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 1, 125, 0, 0);

				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//Get all the results just to check for errors.
			bool isFinished = false;
			try { LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
			catch (LabJackUDException e) {showErrorMessage(e);}
			while(!isFinished)
			{
				try
				{
					LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
						showErrorMessage(e);
				}
			}
			
			try
			{
				//Configure the stream:
				//Configure resolution for all analog inputs.  Since the test external clock
				//is at 1000 Hz, and we are scanning 6 channels, we will have a
				//sample rate of 6000 samples/second.  That means the maximum resolution
				//we could use is 13-bit.  We will use 12-bit in this example.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 12, 0, 0);
				//Configure the analog input range on channel 0 for bipolar +-5 volts.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 0, (double) LJUD.RANGES.BIP5V, 0, 0);
				//Configure the analog input range on channel 1 for bipolar +-5 volts.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 1, (double) LJUD.RANGES.BIP5V, 0, 0);
				//Give the driver a 5 second buffer (scanRate * 6 channels * 5 seconds).
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_BUFFER_SIZE, scanRate*6*5, 0, 0);
				//Configure reads to retrieve whatever data is available without waiting (wait mode LJ_swNONE).
				//See comments below to change this program to use LJ_swSLEEP mode.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_WAIT_MODE, (double) LJUD.STREAMWAITMODES.NONE, 0, 0);
				//Configure for external triggering.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_EXTERNAL_TRIGGER, 1, 0, 0);
				//Define the scan list.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.CLEAR_STREAM_CHANNELS, 0, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 0, 0, 0, 0);	//AIN0
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 1, 0, 0, 0);	//AIN1
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 193, 0, 0, 0);	//EIO_FIO
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 194, 0, 0, 0);	//MIO_CIO
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 200, 0, 0, 0);	//Timer0 LSW
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 224, 0, 0, 0);	//Timer0 MSW
			    
				//Execute the list of requests.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//Get all the results just to check for errors.
			try {LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
			catch (LabJackUDException e) {showErrorMessage(e);}
			isFinished = false;
			while(!isFinished)
			{
				try
				{
					LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
						showErrorMessage(e);
				}
			}
		    
			try
			{
				//Start the stream.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.START_STREAM, 0, ref dblValue, 0);

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
					//	-change wait mode addrequest value to LJ_swSLEEP,
					//	-comment out the following Sleep command.

					Thread.Sleep(delayms);	//Remove if using LJUD.STREAMWAITMODES.SLEEP
					
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
					//This displays the number of scans that were actually read.
					Console.Out.WriteLine("\nIteration # {0:0.###}\n",i);
					Console.Out.WriteLine("Number read = {0:0.###}\n",numScansRequested);
					//This displays just the first scan.
					Console.Out.WriteLine("First scan = {0:0.###},{0:0.###},{0:0.###},{0:0.###},{0:0.###},{0:0.###}\n",adblData[0],adblData[1],adblData[2],adblData[3],adblData[4],adblData[5]);
					//Retrieve the current Comm backlog.  The UD driver retrieves stream data from
					//the UE9 in the background, but if the computer is too slow for some reason
					//the driver might not be able to read the data as fast as the UE9 is
					//acquiring it, and thus there will be data left over in the UE9 buffer.
					LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_COMM, ref dblCommBacklog, 0);
					Console.Out.WriteLine("Comm Backlog = {0:0.###}\n",dblCommBacklog);
					i++;
				}

			   
				//Stop the stream
				LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, 0, ref dummyDouble, 0);

				//Disable the timers.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			Console.Out.WriteLine("\nDone");
			Console.ReadLine(); // Pause for user
			return;

		}
	}

	public class Win32Interop
	{
		[DllImport("crtdll.dll")]
		public static extern int _kbhit();
	}
}
