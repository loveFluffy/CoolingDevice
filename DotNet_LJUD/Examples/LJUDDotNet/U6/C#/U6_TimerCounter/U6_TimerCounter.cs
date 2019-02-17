//---------------------------------------------------------------------------
//
//  U6_TimerCounter.cs
// 
//  Basic U6 example does a PWM output and a counter input, using AddGoGet method.
//
//  Please jumper FIO0 to FIO1
//
//  support@labjack.com
//  June 5, 2009
//	Revised March 24, 2015
//----------------------------------------------------------------------
//

using System;
using System.Threading;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace TimerCounter
{
	class TimerCounter
	{
		// our U6 variable
		private U6 u6;

		static void Main(string[] args)
		{
			TimerCounter a = new TimerCounter();
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
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;

			// Variables to satisfy certain method signatures
			int dummyInt = 0;
			double dummyDouble = 0;
			double[] dummyDoubleArray = {0};

			//Open the Labjack with id 2
			try 
			{
				u6 = new U6(LJUD.CONNECTION.USB, "1", true); // Connection through USB

				//First requests to configure the timer and counter.  These will be
				//done with and add/go/get block.

				//Set the timer/counter pin offset to 0, which will put the first
				//timer/counter on FIO0.
				LJUD.AddRequest (u6.ljhandle,  LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 0, 0, 0);

				//Use the 48 MHz timer clock base with divider.  Since we are using clock with divisor
				//support, Counter0 is not available.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double)LJUD.TIMERCLOCKS.MHZ48_DIV, 0, 0);

				//Set the divisor to 48 so the actual timer clock is 1 MHz.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 48, 0, 0);

				//Enable 1 timer.  It will use FIO0.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 1, 0, 0);

				//Make sure Counter0 is disabled.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);

				//Enable Counter1.  It will use FIO1 since 1 timer is enabled.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 1, 0, 0);

				//Configure Timer0 as 8-bit PWM.  Frequency will be 1M/256 = 3906 Hz.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double)LJUD.TIMERMODE.PWM8, 0, 0);

				//Set the PWM duty cycle to 50%.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0);

				//Execute the requests.
				LJUD.GoOne (u6.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			//Get all the results just to check for errors.
			try {LJUD.GetFirstResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
			catch (LabJackUDException e) {showErrorMessage(e);}
			bool finished = false;
			while(!finished)
			{
				try{LJUD.GetNextResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
				catch (LabJackUDException e) 
				{
					if (e.LJUDError == LJUD.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						finished = true;
					else
						showErrorMessage(e);
				}
			}

			try
			{
				//Wait 1 second.
				Thread.Sleep(1000);

				//Request a read from the counter.
				LJUD.eGet(u6.ljhandle, LJUD.IO.GET_COUNTER, (LJUD.CHANNEL)1, ref dblValue, dummyDoubleArray);
  
				//This should read roughly 4k counts if FIO0 is shorted to FIO1.
				Console.Out.WriteLine("Counter = {0:0.0}\n",dblValue);

				//Wait 1 second.
				Thread.Sleep(1000);

				//Request a read from the counter.
				LJUD.eGet(u6.ljhandle, LJUD.IO.GET_COUNTER, (LJUD.CHANNEL)1, ref dblValue, dummyDoubleArray);

				//This should read about 3906 counts more than the previous read.
				Console.Out.WriteLine("Counter = {0:0.0}\n",dblValue);

				//Reset all pin assignments to factory default condition.
				LJUD.ePut (u6.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0);

				//The PWM output sets FIO0 to output, so we do a read here to set
				//it to input.
				LJUD.eGet (u6.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 0, ref dblValue, 0);
			}
			catch (LabJackUDException e) 
			{
				if (e.LJUDError == LJUD.LJUDERROR.NO_MORE_DATA_AVAILABLE)
					finished = true;
				else
					showErrorMessage(e);
			}

			Console.ReadLine(); // Pause for user	
		}
	}
}
