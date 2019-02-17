//---------------------------------------------------------------------------
//
//  UE9_Quadrature.cs
// 
//	Enables quadrature input on FIO0/FIO1 and reads the current count
//	every half second, until a key is pressed.  To make something happen
//	without an actual quadrature signal, connect a couple wires to
//	ground and tap them on FIO0 and FIO1.
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
			long lngGetNextIteration;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;

			// Dummy variables to complete certain method signatures
			int dummyInt=0;
			double dummyDouble=0;

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
				//Disable all timers and counters to put everything in a known initial state.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);


				//First enable the quadrature input.

				//Enable 2 timers for phases A and B.  They will use FIO0 and FIO1.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0);

				//Configure Timer0 as quadrature.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double) LJUD.TIMERMODE.QUAD, 0, 0);

				//Configure Timer1 as quadrature.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, (double) LJUD.TIMERMODE.QUAD, 0, 0);

				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//Get all the results just to check for errors.
			try {LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
			catch (LabJackUDException e) {showErrorMessage(e);}
			bool isFinished = false;
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
				while(Win32Interop._kbhit() == 0)	//Program will run until any key is hit
				{

					//Wait 500 milliseconds
					Thread.Sleep(500);

					//Request a read from Timer0.  Timer0 and Timer1 both return the same
					//quadrature value.
					LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_TIMER, 0, ref dblValue, 0);
					Console.Out.WriteLine("Quad Counter = {0:0.0}\n",dblValue);

				}


				//Disable the timers and the FIO lines will return to digital I/O.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			Console.ReadLine(); // Pause for user

		}
	}

	public class Win32Interop
	{
		[DllImport("crtdll.dll")]
		public static extern int _kbhit();
	}
}
