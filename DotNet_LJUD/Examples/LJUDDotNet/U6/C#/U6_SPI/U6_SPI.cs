//---------------------------------------------------------------------------
//
//  U6_SPI.cs
// 
//	Demonstrates SPI communication.
//
//	You can short MOSI to MISO for testing.
//
//	MOSI    FIO2
//	MISO    FIO3
//	CLK     FIO0
//	CS      FIO1
//
//	If you short MISO to MOSI, then you will read back the same bytes that you write.  If you short
//	MISO to GND, then you will read back zeros.  If you short MISO to VS or leave it
//	unconnected, you will read back 255s.
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

namespace SPI
{
	class SPI
	{
		// our U6 variable
		private U6 u6;

		// Variables to satisfy certain method signatures
		int dummyInt = 0;
		double dummyDouble = 0;

		static void Main(string[] args)
		{
			SPI a = new SPI();
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
			double numSPIBytesToTransfer;
			byte[] dataArray = new byte[50];

			//Open the LabJack U6.
			try 
			{
				u6 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB

				//First, configure the SPI communication.

				//Enable automatic chip-select control.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_AUTO_CS,1,0,0);

				//Do not disable automatic digital i/o direction configuration.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_DISABLE_DIR_CONFIG,0,0,0);

				//Mode A:  CPHA=1, CPOL=1.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MODE,0,0,0);

				//125kHz clock.
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLOCK_FACTOR,0,0,0);

				//MOSI is FIO2
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MOSI_PIN_NUM,2,0,0);
	
				//MISO is FIO3
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MISO_PIN_NUM,3,0,0);

				//CLK is FIO0
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLK_PIN_NUM,0,0,0);

				//CS is FIO1
				LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CS_PIN_NUM,1,0,0);


				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
				LJUD.GoOne(u6.ljhandle);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}


			//Get all the results just to check for errors.
			try {LJUD.GetFirstResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
			catch (LabJackUDException e) { showErrorMessage(e); }
			bool finished = false;
			while(!finished)
			{
				try{LJUD.GetNextResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
				
				catch (LabJackUDException e) 
				{
					if(e.LJUDError == LJUD.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						finished = true;
					else
						showErrorMessage(e);
				}
			}


			//This example transfers 4 test bytes.
			numSPIBytesToTransfer = 4;
			dataArray[0] = 175;
			dataArray[1] = 245;
			dataArray[2] = 170;
			dataArray[3] = 240;
	
			//Transfer the data.  The write and read is done at the same time.
			try {LJUD.eGet(u6.ljhandle, LJUD.IO.SPI_COMMUNICATION, 0, ref numSPIBytesToTransfer, dataArray);}
			catch (LabJackUDException e) { showErrorMessage(e); }

			//Display the read data.
			Console.Out.WriteLine("dataArray[0] = {0:0.#}\n",dataArray[0]);
			Console.Out.WriteLine("dataArray[1] = {0:0.#}\n",dataArray[1]);
			Console.Out.WriteLine("dataArray[2] = {0:0.#}\n",dataArray[2]);
			Console.Out.WriteLine("dataArray[3] = {0:0.#}\n",dataArray[3]);


			Console.ReadLine(); // Pause for user

		}
	}
}