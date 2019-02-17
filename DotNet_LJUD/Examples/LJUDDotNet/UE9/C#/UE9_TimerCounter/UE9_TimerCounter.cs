//---------------------------------------------------------------------------
//
//  UE9_TimerCounter.cs
// 
//	Demonstrates a few of different timer/counter features.
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

namespace TimerCounter
{
	class TimerCounter
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			TimerCounter a = new TimerCounter();
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
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double highTime,lowTime;
			double period16=-1,period32=-1;

			// Variables that satisfy a method signature
			int dummyInt = 0;
			double dummyDouble = 0;

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


			//First we will output a square wave and count the number of pulses for about 1 second.
			//Connect a jumper on the UE9 from FIO0 (PWM output) to
			//FIO1 (Counter0 input).

			//Use the fixed 750kHz timer clock source.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double)LJUD.TIMERCLOCKS.KHZ750, 0, 0);

			//Set the divisor to 3 so the actual timer clock is 250kHz.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0);

			//Enable 1 timer.  It will use FIO0.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 1, 0, 0);

			//Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double)LJUD.TIMERMODE.PWM8, 0, 0);

			//Set the PWM duty cycle to 50%.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0);

			//Enable Counter0.  It will use FIO1 since 1 timer is enabled.
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0, 0);


			//Execute the requests on a single LabJack.  The driver will use a
			//single low-level TimerCounter command to handle all the requests above.
			LJUD.GoOne(ue9.ljhandle);


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

				//Wait 1 second.
				Thread.Sleep(1000);

				//Request a read from the counter.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_COUNTER, 0, ref dblValue, 0);

				//This should read roughly 977 counts.
				Console.Out.WriteLine("Counter = {0:0.0}\n\n",dblValue);

				//Disable the timer and counter, and the FIO lines will return to digital I/O.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);

				//Output a square wave and measure the period.
				//Connect a jumper on the UE9 from FIO0 (PWM8 output) to
				//FIO1 (RISINGEDGES32 input) and FIO2 (RISINGEDGES16).

				//Use the fixed 750kHz timer clock source.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double)LJUD.TIMERCLOCKS.KHZ750, 0, 0);

				//Set the divisor to 3 so the actual timer clock is 250kHz.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0);

				//Enable 3 timers.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 3, 0, 0);

				//Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double)LJUD.TIMERMODE.PWM8, 0, 0);

				//Set the PWM duty cycle to 50%.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0);

				//Configure Timer1 as 32-bit period measurement.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, (double)LJUD.TIMERMODE.RISINGEDGES32, 0, 0);

				//Configure Timer2 as 16-bit period measurement.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 2, (double)LJUD.TIMERMODE.RISINGEDGES16, 0, 0);


				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
				if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
					isFinished = true;
				else
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

			//Wait 1 second.
			Thread.Sleep(1000);

			//Now read the period measurements from the 2 timers.  We
			//will use the Add/Go/Get method so that both
			//reads are done in a single low-level call.

			//Request a read from Timer1
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.GET_TIMER, 1, 0, 0, 0);

			//Request a read from Timer2
			LJUD.AddRequest(ue9.ljhandle, LJUD.IO.GET_TIMER, 2, 0, 0, 0);

			//Execute the requests on a single LabJack.  The driver will use a
			//single low-level TimerCounter command to handle all the requests above.
			LJUD.GoOne(ue9.ljhandle);

			//Get the results of the two read requests.
			LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
			isFinished = false;
			while(!isFinished)
			{
				switch(ioType)
				{
					case LJUD.IO.GET_TIMER :
					switch((int)channel)
					{
						case 1:
							period32=dblValue;
							break;
						case 2:
							period16=dblValue;
							break;
					}
						break;
				}

				try{ LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
				catch (LabJackUDException e) 
				{
					// After encountering a 
					if(e.LJUDError > UE9.LJUDERROR.MIN_GROUP_ERROR)
						isFinished = true;
					else
						showErrorMessage(e);
				}
			}


			try
			{

				//Both period measurements should read about 256.  The timer
				//clock was set to 250 kHz, so each tick equals 4 microseconds, so
				//256 ticks means a period of 1024 microseconds which is a frequency
				//of 977 Hz.
				Console.Out.WriteLine("Period32 = {0:0.0}\n",period32);
				Console.Out.WriteLine("Period16 = {0:0.0}\n\n",period16);

				//Disable the timer and counter, and the FIO lines will return to digital I/O.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);

				//Now we will output a 25% duty-cycle PWM output on Timer0 (FIO0) and measure
				//the duty cycle on Timer1 FIO1.  Requires Control firmware V1.21 or higher.

				//Use the fixed 750kHz timer clock source.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, (double)LJUD.TIMERCLOCKS.KHZ750, 0, 0);

				//Set the divisor to 3 so the actual timer clock is 250kHz.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0);

				//Enable 2 timers.  They will use FIO0 and FIO1.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0);

				//Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, (double)LJUD.TIMERMODE.PWM8, 0, 0);

				//Set the PWM duty cycle to 25%.  The passed value is the low time.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 49152, 0, 0);

				//Configure Timer1 as duty cycle measurement.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, (double)LJUD.TIMERMODE.DUTYCYCLE, 0, 0);


				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
				LJUD.GoOne(ue9.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				// After encountering a 
				if(e.LJUDError > UE9.LJUDERROR.MIN_GROUP_ERROR)
					isFinished = true;
				else
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

			//Wait a little so we are sure a duty cycle measurement has occured.
			Thread.Sleep(100);

			try
			{
				//Request a read from Timer1.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_TIMER, (LJUD.CHANNEL)1, ref dblValue, 0);

				//High time is LSW
				highTime = (double)(((ulong)dblValue) % (65536));
				//Low time is MSW
				lowTime = (double)(((ulong)dblValue) / (65536));

				Console.Out.WriteLine("High clicks = {0:0.0}\n",highTime);
				Console.Out.WriteLine("Low clicks = {0:0.0}\n",lowTime);
				Console.Out.WriteLine("Duty cycle = {0:0.0}\n",100*highTime/(highTime+lowTime));


				//Disable the timers, and the FIO lines will return to digital I/O.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
				LJUD.GoOne(ue9.ljhandle);


				//The PWM output sets FIO0 to output, so we do a read here to set
				//FIO0 to input.
				LJUD.eGet (ue9.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 0, ref dblValue, 0);
			}
			catch (LabJackUDException e) 
			{
				// After encountering a 
				if(e.LJUDError > UE9.LJUDERROR.MIN_GROUP_ERROR)
					isFinished = true;
				else
					showErrorMessage(e);
			}

			Console.ReadLine(); // Pause for user

		}
	}
}
