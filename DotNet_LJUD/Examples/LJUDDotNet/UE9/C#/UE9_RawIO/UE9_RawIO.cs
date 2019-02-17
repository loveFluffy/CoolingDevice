//---------------------------------------------------------------------------
//
//  UE9_RawIO.cs
// 
///	Simple demonstration of using RAW_OUT and RAW_IN.  Sends a 0x70,0x70
//	which is a do nothing Comm command that simply responds with an echo.
//	Records the time for 1000 iterations and divides by 1000, to determine
//  the basic communication time of the LabJack.
//
//	For more in depth examples of low-level function calls, see the
//	Linux downloads.
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

namespace RawIO
{
	class RawIO
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			RawIO a = new RawIO();
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
			long time=0, i=0;
			long numIterations = 1000;

			double numBytesToWrite;
			double numBytesToRead;
			byte[] writeArray = {0x70,0x70};
			byte[] readArray = {0};

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
				time = Environment.TickCount;

				for(i=0;i<numIterations;i++)
				{
					numBytesToWrite = 2;
					numBytesToRead = 2;

					//Raw Out
					LJUD.eGet(ue9.ljhandle, LJUD.IO.RAW_OUT, 0, ref numBytesToWrite, writeArray);

					//Raw In
					LJUD.eGet(ue9.ljhandle, LJUD.IO.RAW_IN, 0, ref numBytesToRead, readArray);

				}
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			// Display the results
			time = Environment.TickCount - time;

			Console.Out.WriteLine("Milleseconds per iteration = {0:0.###}\n", (double)time / (double)numIterations);

			Console.ReadLine(); // Pause for user
		}
	}
}
