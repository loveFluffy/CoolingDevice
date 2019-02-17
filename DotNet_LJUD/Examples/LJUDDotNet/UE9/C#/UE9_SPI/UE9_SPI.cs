//---------------------------------------------------------------------------
//
//  UE9_SPI.cs
// 
// Demonstrates SPI communication.
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
//	Tested with UD driver V2.73, Comm firmware V1.40, and Control firmware V1.77.
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

namespace SPI
{
	class SPI
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			SPI a = new SPI();
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
			double numSPIBytesToTransfer;
			byte[] dataArray = new byte[50];

			// Variables to satisfy certain method signatures
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

			try
			{
				//First, configure the SPI communication.

				//Enable automatic chip-select control.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_AUTO_CS,1,0,0);

				//Do not disable automatic digital i/o direction configuration.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_DISABLE_DIR_CONFIG,0,0,0);

				//Mode A:  CPHA=1, CPOL=1.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MODE,0,0,0);

				//125kHz clock.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLOCK_FACTOR,0,0,0);

				//MOSI is FIO2
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MOSI_PIN_NUM,2,0,0);
				
				//MISO is FIO3
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MISO_PIN_NUM,3,0,0);

				//CLK is FIO0
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLK_PIN_NUM,0,0,0);

				//CS is FIO1
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CS_PIN_NUM,1,0,0);


				//Execute the requests on a single LabJack.  The driver will use a
				//single low-level TimerCounter command to handle all the requests above.
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

			//This example transfers 4 test bytes.
			numSPIBytesToTransfer = 4;
			dataArray[0] = 170;
			dataArray[1] = 240;
			dataArray[2] = 170;
			dataArray[3] = 240;
			
			//Transfer the data.  The write and read is done at the same time.
			try { LJUD.eGet(ue9.ljhandle, LJUD.IO.SPI_COMMUNICATION, 0, ref numSPIBytesToTransfer, dataArray); }
			catch (LabJackUDException e) {showErrorMessage(e);}

			//Display the read data.
			Console.Out.WriteLine("dataArray[0] = {0:0.#}\n",dataArray[0]);
			Console.Out.WriteLine("dataArray[1] = {0:0.#}\n",dataArray[1]);
			Console.Out.WriteLine("dataArray[2] = {0:0.#}\n",dataArray[2]);
			Console.Out.WriteLine("dataArray[3] = {0:0.#}\n",dataArray[3]);

			Console.ReadLine(); // Pause for user
		}
	}
}
