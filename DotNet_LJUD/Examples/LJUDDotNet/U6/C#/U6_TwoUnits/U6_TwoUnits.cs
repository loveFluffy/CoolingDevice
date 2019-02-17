//---------------------------------------------------------------------------
//
//  U6_TwoUnits.cs
// 
//  Simple example demonstrates communication with 2 U6s.
//
//  support@labjack.com
//  June 5, 2009
//----------------------------------------------------------------------
//

using System;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace TwoUnits
{
	class TwoUnits
	{
		// our U6 variable
		private U6 unit2, unit3;

		static void Main(string[] args)
		{
			TwoUnits a = new TwoUnits();
			a.preformActions();
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

		public void preformActions()
		{
			long lngGetNextIteration;
			double dblDriverVersion;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double Value12=9999,Value22=9999,Value32=9999;
			double Value13=9999,Value23=9999,Value33=9999;

			//Read and display the UD version.
			dblDriverVersion = LJUD.GetDriverVersion();
			Console.Out.WriteLine("UD Driver Version = {0:0.000}\n\n",dblDriverVersion);

			// Variables to satisfy certain method signatures
			int dummyInt = 0;
			double dummyDouble = 0;

			//Open the U6 with local ID 2.
			try 
			{
				unit2 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}
			//Open the U6 with local ID 3.
			try 
			{
				unit3 = new U6(LJUD.CONNECTION.USB, "0", true); // Connection through USB
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			try
			{
				//The following commands will use the add-go-get method to group
				//multiple requests into a single low-level function.

				//Request a single-ended reading from AIN1.
				LJUD.AddRequest (unit2.ljhandle, LJUD.IO.GET_AIN, 1, 0, 0, 0);
				LJUD.AddRequest (unit3.ljhandle, LJUD.IO.GET_AIN, 1, 0, 0, 0);

				//Request a single-ended reading from AIN2.
				LJUD.AddRequest (unit2.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0);
				LJUD.AddRequest (unit3.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}

			bool isFinished = false;
			while (!isFinished)
			{

				try
				{
					//Execute all requests on all open LabJacks.
					LJUD.Go();

					//Get all the results for unit 2.  The input measurement results are stored.
					LJUD.GetFirstResult(unit2.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
				}
				catch (LabJackUDException e) 
				{
					showErrorMessage(e);
				}

				bool unit2Finished = false;
				while(!unit2Finished)
				{
					switch(ioType)
					{

						case LJUD.IO.GET_AIN :
						switch((int)channel)
						{
							case 1:
								Value12=dblValue;
								break;
							case 2:
								Value22=dblValue;
								break;
						}
							break;

						case LJUD.IO.GET_AIN_DIFF :
							Value32=dblValue;
							break;

					}

					try { LJUD.GetNextResult(unit2.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
					catch (LabJackUDException e)
					{
						// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
						if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
							unit2Finished = true;
						else
							showErrorMessage(e);
					}

				}


				//Get all the results for unit 3.  The input measurement results are stored.
				try { LJUD.GetFirstResult(unit3.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
				catch (LabJackUDException e)  { showErrorMessage(e); }

				bool unit3Finished = false;
				while(!unit3Finished)
				{
					switch(ioType)
					{

						case LJUD.IO.GET_AIN :
						switch((int)channel)
						{
							case 1:
								Value13=dblValue;
								break;
							case 2:
								Value23=dblValue;
								break;
						}
							break;

						case LJUD.IO.GET_AIN_DIFF :
							Value33=dblValue;
							break;

					}

					try { LJUD.GetNextResult(unit3.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble); }
					catch (LabJackUDException e)
					{
						// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
						if(e.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
							unit3Finished = true;
						else
							showErrorMessage(e);
					}

				}



				Console.Out.WriteLine("AIN1 (Unit 2) = {0:0.###}\n",Value12);
				Console.Out.WriteLine("AIN1 (Unit 3) = {0:0.###}\n",Value13);
				Console.Out.WriteLine("AIN2 (Unit 2) = {0:0.###}\n",Value22);
				Console.Out.WriteLine("AIN2 (Unit 3) = {0:0.###}\n",Value23);
				Console.Out.WriteLine("AIN3 (Unit 2) = {0:0.###}\n",Value32);
				Console.Out.WriteLine("AIN3 (Unit 3) = {0:0.###}\n",Value33);
	
				Console.Out.WriteLine("\nPress Enter to go again or (q) to quit\n");
				String str1 = Console.ReadLine(); // Pause for user
				isFinished = str1 == "q";
			}

		}
	}
}
