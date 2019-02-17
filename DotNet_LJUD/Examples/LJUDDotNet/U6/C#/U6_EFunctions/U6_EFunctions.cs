//---------------------------------------------------------------------------
//
//  U6_EFunctions.cs
// 
//  Demonstrates the UD E-functions with the LabJack U6.
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

namespace U6_EFunctions
{
	class U6_EFunctions
	{
		// our U6 variable
		private U6 u6;

		static void Main(string[] args)
		{
			U6_EFunctions a = new U6_EFunctions();
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
			double dblValue=0;
			int intValue=0;

			int binary;
			int[] aEnableTimers = new int[2];
			int[] aEnableCounters = new int[2];
			int[] aTimerModes = new int[2];
			double[] adblTimerValues = new double[2];
			int[] aReadTimers = new int[2];
			int[] aUpdateResetTimers = new int[2];
			int[] aReadCounters = new int[2];
			int[] aResetCounters = new int[2];
			double[] adblCounterValues = {0,0};
			
			try 
			{
				//Open the first found LabJack U6.
				u6 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB

				//Take a single-ended measurement from AIN3.
				binary = 0;
				LJUD.eAIN( u6.ljhandle, 3, 199, ref dblValue, -1, -1, -1, binary);
				Console.Out.WriteLine("AIN3 = {0:0.###}\n",dblValue);

				//Set DAC0 to 3.0 volts.
				dblValue = 3.0;
				binary = 0;
				LJUD.eDAC (u6.ljhandle, 0, dblValue, binary, 0, 0);
				Console.Out.WriteLine("DAC0 set to {0:0.###} volts\n",dblValue);

				//Read state of FIO0.
				LJUD.eDI (u6.ljhandle, 0, ref intValue);
				Console.Out.WriteLine("FIO0 = {0:0.#}\n",intValue);

				//Set the state of FIO3.
				intValue = 1;
				LJUD.eDO (u6.ljhandle, 3, intValue);
				Console.Out.WriteLine("FIO3 set to = {0:0.#}\n\n",intValue);

			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}
			Console.ReadLine(); // Pause for user
		}
	}
}

