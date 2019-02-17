//---------------------------------------------------------------------------
//
//  UE9_Functions.cs
// 
//	Demonstrates the UD E-functions with the LabJack UE9.
//
//  support@labjack.com
//  June 2, 2009
//	Revised December 29, 2010
//----------------------------------------------------------------------
//

using System;
using System.Threading;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace UE9_EFunctions
{
	class UE9_EFunctions
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			UE9_EFunctions u = new UE9_EFunctions();
			u.performActions();
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
			int intValue=0;
			LJUD.RANGES range;
			int intResolution;
			int intBinary;
			int[] aintEnableTimers = new int[6];
			int[] aintEnableCounters = new int[2];
			int intTimerClockBaseIndex; 
			int intTimerClockDivisor;
			int[] aintTimerModes = new int[6];
			double[] adblTimerValues = new double[6];
			int[] aintReadTimers = new int[6];
			int[] aintUpdateResetTimers = new int[6];
			int[] aintReadCounters = new int[2];
			int[] aintResetCounters = new int[2];
			double[] adblCounterValues = {0,0};
			double highTime, lowTime, dutyCycle;

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
				//Take a measurement from AIN3.
				range = LJUD.RANGES.BIP5V;
				intResolution = 17;
				intBinary = 0;
				LJUD.eAIN(ue9.ljhandle, 3, 0, ref dblValue, (int)range, (int)intResolution, 0, 0);
				Console.Out.WriteLine("AIN3 = {0:0.###}\n",dblValue);

				//Set DAC0 to 3.0 volts.
				dblValue = 3.0;
				intBinary = 0;
				LJUD.eDAC(ue9.ljhandle, 0, dblValue, intBinary, 0, 0);
				Console.Out.WriteLine("DAC0 set to {0:0.###} volts\n",dblValue);

				//Read state of FIO2.
				LJUD.eDI(ue9.ljhandle, 2, ref intValue);
				Console.Out.WriteLine("FIO2 = {0:0.###}\n",intValue);

				//Set the state of FIO3.
				intValue = 0;
				LJUD.eDO(ue9.ljhandle, 3, intValue);
				Console.Out.WriteLine("FIO3 set to = {0:0.###}\n\n",intValue);

				//Timers and Counters example.
				//First, a call to eTCConfig.  Fill the arrays with the desired values, then make the call.
				intTimerClockBaseIndex = (int) LJUD.TIMERCLOCKS.KHZ750;  //Choose 750 kHz base clock.
				intTimerClockDivisor = 3; //Divide by 3, thus timer clock is 250 kHz.
				aintEnableTimers[0] = 1; //Enable Timer0 (uses FIO0).
				aintEnableTimers[1] = 1; //Enable Timer1 (uses FIO1).
				aintEnableTimers[2] = 1; //Enable Timer2 (uses FIO2).
				aintEnableTimers[3] = 1; //Enable Timer3 (uses FIO3).
				aintEnableTimers[4] = 0; //Disable Timer4.
				aintEnableTimers[5] = 0; //Disable Timer5.
				aintTimerModes[0] = (int) LJUD.TIMERMODE.PWM8; //Timer0 is 8-bit PWM output.  Frequency is 250k/256 = 977 Hz.
				aintTimerModes[1] = (int) LJUD.TIMERMODE.DUTYCYCLE; //Timer1 is duty cyle input.
				aintTimerModes[2] = (int) LJUD.TIMERMODE.FIRMCOUNTER; //Timer2 is firmware counter input.
				aintTimerModes[3] = (int) LJUD.TIMERMODE.RISINGEDGES16; //Timer3 is 16-bit period measurement.
				aintTimerModes[4] = 0; //Timer4 not enabled.
				aintTimerModes[5] = 0; //Timer5 not enabled.
				adblTimerValues[0] = 16384; //Set PWM8 duty-cycle to 75%.
				adblTimerValues[1] = 0;
				adblTimerValues[2] = 0;
				adblTimerValues[3] = 0;
				adblTimerValues[4] = 0;
				adblTimerValues[5] = 0;
				aintEnableCounters[0] = 1; //Enable Counter0 (uses FIO4).
				aintEnableCounters[1] = 1; //Enable Counter1 (uses FIO5).
				LJUD.eTCConfig(ue9.ljhandle, aintEnableTimers, aintEnableCounters, 0, (int) intTimerClockBaseIndex, intTimerClockDivisor, aintTimerModes, adblTimerValues, 0, 0);
				Console.Out.WriteLine("Timers and Counters enabled.\n");

				Thread.Sleep(1000); //Wait 1 second.

				//Now, a call to eTCValues.
				aintReadTimers[0] = 0; //Don't read Timer0 (output timer).
				aintReadTimers[1] = 1; //Read Timer1;
				aintReadTimers[2] = 1; //Read Timer2;
				aintReadTimers[3] = 1; //Read Timer3;
				aintReadTimers[4] = 0; //Timer4 not enabled.
				aintReadTimers[5] = 0; //Timer5 not enabled.
				aintUpdateResetTimers[0] = 1; //Update Timer0;
				aintUpdateResetTimers[1] = 1; //Reset Timer1;
				aintUpdateResetTimers[2] = 1; //Reset Timer2;
				aintUpdateResetTimers[3] = 1; //Reset Timer3;
				aintUpdateResetTimers[4] = 0; //Timer4 not enabled.
				aintUpdateResetTimers[5] = 0; //Timer5 not enabled.
				aintReadCounters[0] = 1; //Read Counter0;
				aintReadCounters[1] = 1; //Read Counter1;
				aintResetCounters[0] = 1; //Reset Counter0.
				aintResetCounters[1] = 1; //Reset Counter1.
				adblTimerValues[0] = 32768; //Change Timer0 duty-cycle to 50%.
				adblTimerValues[1] = 0;
				adblTimerValues[2] = 0;
				adblTimerValues[3] = 0;
				adblTimerValues[4] = 0;
				adblTimerValues[5] = 0;
				LJUD.eTCValues(ue9.ljhandle, aintReadTimers, aintUpdateResetTimers, aintReadCounters, aintResetCounters, adblTimerValues, adblCounterValues, 0, 0);
				Console.Out.WriteLine("Timer1 value = {0:0.###}",adblTimerValues[1]);
				Console.Out.WriteLine("Timer2 value = {0:0.###}",adblTimerValues[2]);
				Console.Out.WriteLine("Timer3 value = {0:0.###}",adblTimerValues[3]);
				Console.Out.WriteLine("Counter0 value = {0:0.###}",adblCounterValues[0]);
				Console.Out.WriteLine("Counter1 value = {0:0.###}",adblCounterValues[1]);

				//Convert Timer1 value to duty-cycle percentage.
				//High time is LSW
				highTime = (double)(((ulong)adblTimerValues[1]) % (65536));
				//Low time is MSW
				lowTime = (double)(((ulong)adblTimerValues[1]) / (65536));
				//Calculate the duty cycle percentage.
				dutyCycle = 100*highTime/(highTime+lowTime);
				Console.Out.WriteLine("\nHigh clicks Timer1 = {0:0.###}",highTime);
				Console.Out.WriteLine("Low clicks Timer1 = {0:0.###}",lowTime);
				Console.Out.WriteLine("Duty cycle Timer1 = {0:0.###}",dutyCycle);


				//Disable all timers and counters.
				aintEnableTimers[0] = 0;
				aintEnableTimers[1] = 0;
				aintEnableTimers[2] = 0;
				aintEnableTimers[3] = 0;
				aintEnableTimers[4] = 0;
				aintEnableTimers[5] = 0;
				aintEnableCounters[0] = 0;
				aintEnableCounters[1] = 0;
				LJUD.eTCConfig(ue9.ljhandle, aintEnableTimers, aintEnableCounters, 0, intTimerClockBaseIndex, intTimerClockDivisor, aintTimerModes, adblTimerValues, 0, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			// Pause for the user
			Console.ReadLine();
		}

	}

}

