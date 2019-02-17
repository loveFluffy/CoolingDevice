//---------------------------------------------------------------------------
//
//  UE9_PMWDutyCycle.cs
// 
//	Demonstrates 1 PWM output and 2 duty cycle inputs.
//	Connect FIO0 to FIO1 to FIO2.
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

namespace PWMDutyCycle
{
	class PWMDutyCycle
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			PWMDutyCycle p = new PWMDutyCycle();
			p.performActions();
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

		//Function used to read and display the duty cycle from a timer.
		private void ReadDutyCycle (int handle, long timerNumber)
		{
			double dblValue=0;
			double highTime,lowTime,dutyCycle;
			double dummyDouble=0; // Satisfies method signatures but does not have any real purpose

			//Read from Timer.  We will go ahead and reset the timer (by writing
			//a value of 0) at the same time as the read.  This way if no new
			//edges occur (e.g. 0% or 100% duty cycle) the next read will return
			//the preset high/low times (0/65535 or 65535/0) rather than returning
			//the old values.
			LJUD.AddRequest(handle, LJUD.IO.GET_TIMER, (LJUD.CHANNEL)timerNumber, 0, 0, 0);
			LJUD.AddRequest(handle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL)timerNumber, 0, 0, 0);
			LJUD.GoOne(handle);
			LJUD.GetResult(handle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL)timerNumber, ref dummyDouble);  //just to check for error
			LJUD.GetResult(handle, LJUD.IO.GET_TIMER, (LJUD.CHANNEL)timerNumber, ref dblValue);
			//High time is LSW
			highTime = (double)(((ulong)dblValue) % (65536));
			//Low time is MSW
			lowTime = (double)(((ulong)dblValue) / (65536));
			//Calculate the duty cycle percentage.
			dutyCycle = 100*highTime/(highTime+lowTime);

			Console.Out.WriteLine("\nHigh clicks Timer{0:0.#} = {1:0.#}",timerNumber,highTime);
			Console.Out.WriteLine("Low clicks Timer{0:0.#} = {1:0.#}",timerNumber,lowTime);
			Console.Out.WriteLine("Duty cycle Timer{0:0.#} = {1:0.#}",timerNumber,dutyCycle);
		}

		public void performActions()
		{
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
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

			//Disable all timers and counters to put everything in a known initial state.
			//Disable the timer and counter, and the FIO lines will return to digital I/O.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0);
			LJUD.GoOne(ue9.ljhandle);


			//Output a PWM output on Timer0 (FIO0) and measure
			//the duty cycle on Timer1 FIO1 and Timer2 FIO2.

			//Use the fixed 750kHz timer clock source.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double) LJUD.TIMERCLOCKS.KHZ750, 0, 0);

			//Set the divisor to 3 so the actual timer clock is 250kHz.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0);

			//Enable 2 timers.  They will use FIO0 and FIO1.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 3, 0, 0);

			//Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double) LJUD.TIMERMODE.PWM8, 0, 0);

			//Set the PWM duty cycle to 50%.  The passed value is the low time.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0);

			//Configure Timer1 and Timer2 as duty cycle measurement.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, (double) LJUD.TIMERMODE.DUTYCYCLE, 0, 0);
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 2, (double) LJUD.TIMERMODE.DUTYCYCLE, 0, 0);


			//Execute the requests on a single LabJack.  The driver will use a
			//single low-level TimerCounter command to handle all the requests above.
			LJUD.GoOne(ue9.ljhandle);


			//Get all the results just to check for errors.
			LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
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

			//Set the PWM duty cycle to 25%.  The passed value is the low time.
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 49152, 0);
			//Now we will reset the duty cycle input timers, so we are sure the
			//reads we do are not old values from before the PWM output was updated.
			Thread.Sleep(10);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 1, 0, 0);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 2, 0, 0);

			//Wait a little so we are sure a duty cycle measurement has occured.
			Thread.Sleep(10);

			//Read from Timer1.
			ReadDutyCycle (ue9.ljhandle, 1);

			//Read from Timer2.
			ReadDutyCycle (ue9.ljhandle, 2);

			//Set the PWM duty cycle to 0%.  The passed value is the low time.
			//We are specifying 65535 out of 65536 clicks to be low.  Since
			//this is 8-bit PWM, we actually get 255 low clicks out of 256 total
			//clicks, so the minimum duty cycle is 0.4%.
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 65535, 0);
			//Now we will reset the duty cycle input timers, so we are sure the
			//reads we do are not old values from before the PWM output was updated.
			Thread.Sleep(10);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 1, 0, 0);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 2, 0, 0);

			//Wait a little so we are sure a duty cycle measurement has occured.
			Thread.Sleep(10);

			//Read from Timer1.
			ReadDutyCycle (ue9.ljhandle, 1);

			//Read from Timer2.
			ReadDutyCycle (ue9.ljhandle, 2);

			//Set the PWM duty cycle to 100%.  The passed value is the low time.
			//We are specifying 0 out of 65536 clicks to be low, so the signal
			//will be high the entire time, meaning there are no edges
			//for the input timers to detect, and no measurement should be made.
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 0, 0);
			//Now we will reset the duty cycle input timers, so we are sure the
			//reads we do are not old values from before the PWM output was updated.
			Thread.Sleep(10);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 1, 0, 0);
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, (LJUD.CHANNEL) 2, 0, 0);

			//Wait a little so we are sure a duty cycle measurement has been attempted.
			Thread.Sleep(10);

			//Read from Timer1.
			ReadDutyCycle (ue9.ljhandle, 1);

			//Read from Timer2.
			ReadDutyCycle (ue9.ljhandle, 2);

			//Disable all timers and counters, and the FIO lines will return to digital I/O.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0);
			LJUD.GoOne(ue9.ljhandle);

			Console.ReadLine(); // Pause for user
		}
	}
}
