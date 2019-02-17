//---------------------------------------------------------------------------
//
//  UE9_Memory.cs
// 
//	Demonstrates writing and reading to the user and cal memory on the UE9.
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

namespace UE9_Memory
{
	class UE9_Memory
	{
		// our UE9 variable
		private UE9 ue9;

		static void Main(string[] args)
		{
			UE9_Memory u = new UE9_Memory();
			u.performActions();
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
			double[] achrUserMem = new double[1024];
			double[] adblCalMem = new double[128];

			// Dummy variables that will be used to satisfy certain method signatures but have no other purpose
			double dummyDouble = 0;

			//Make a long parameter which holds the address of the data arrays.  We do this
			//so the compiler does not generate a warning in the eGet call that passes
			//the data.  Note that the x1 parameter  in eGet (and AddRequest) is fairly
			//generic, in that sometimes it could just be a write parameter, and sometimes
			//it has the address of an array.  Since x1 is not declared as a pointer, the
			//compiler will complain if you just pass the array pointer without casting
			//it to a long as follows.
			long pachrUserMem = (long)achrUserMem[0];
			long padblCalMem = (long)adblCalMem[0];

			//Seed the random number function.
			Random rand = new Random(Environment.TickCount);
			
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

			//First a user memory example.  We will read the memory, update a few elements,
			//and write the memory.  The entire memory area is read and written each time.
			//The user memory is just stored as bytes, so almost any information can be
			//put in there such as integers, doubles, or strings.
			
			try
			{
				//Read the user memory.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.USER_MEM, ref dummyDouble, achrUserMem);
				//Display the first 4 elements.
				Console.Out.WriteLine("Read User Mem [0-3] = {0:0}, {1:0}, {2:0}, {3:0}\n",achrUserMem[0],achrUserMem[1],achrUserMem[2],achrUserMem[3]);
				//Create 4 new pseudo-random numbers to write.  We will update the first
				//4 elements of user memory, but the rest will be unchanged.
				for(int i=0 ; i<4 ; i++)
				{
					achrUserMem[i] = rand.Next(100);
				}
				Console.Out.WriteLine("Write User Mem [0-3] = {0:0}, {1:0}, {2:0}, {3:0}\n",achrUserMem[0],achrUserMem[1],achrUserMem[2],achrUserMem[3]);
				LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.USER_MEM, 0, achrUserMem);
				//Re-read the user memory.
				LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.USER_MEM, ref dummyDouble, achrUserMem);
				//Display the first 4 elements.
				Console.Out.WriteLine("Read User Mem [0-3] = {0:0}, {1:0}, {2:0}, {3:0}\n",achrUserMem[0],achrUserMem[1],achrUserMem[2],achrUserMem[3]);
			}
			catch (LabJackUDException e) 
			{
				showErrorMessage(e);
			}
			

			//Now a cal constants example.  The calibration memory is passed as doubles.
			//The memory area consists of 8 blocks (0-7) of 16 doubles each, for a total
			//of 128 elements.  As of this writing, block 7 is not used, so we will
			//use the last 4 elements of block 7 for testing, which is elements 124-127.
			//We will read the constants, update a few elements, and write the constants.  
			//The entire memory area is read and written each time.

			//This cal example is commented out by default, as writing and reading
			//the cal area is an advanced operation.
			/*
			//Read the cal constants.
			LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, ref dummyDouble, adblCalMem);
			//Display the last 4 elements.
			Console.Out.WriteLine("Read Cal Constants [124-127] = {0:0}, {1:0}, {2:0}, {3:0}\n",adblCalMem[124],adblCalMem[125],adblCalMem[126],adblCalMem[127]);
			//Create 4 new pseudo-random numbers to write.  We will update the last
			//4 cal constants, but the rest will be unchanged.
			for(int i=124;i<128;i++)
			{
				adblCalMem[i] = (100*((double)rand.Next(100)/100))-50;
			}
			Console.Out.WriteLine("Write  Cal Constants [124-127] = {0:0}, {1:0}, {2:0}, {3:0}\n",adblCalMem[124],adblCalMem[125],adblCalMem[126],adblCalMem[127]);
			//The special value (0x4C6C) must be put in to write the cal constants.
			LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, 19564, adblCalMem);
			//Re-read the cal constants.
			LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, ref dummyDouble, adblCalMem);
			//Display the first 4 elements.
			Console.Out.WriteLine("Read  Cal Constants [124-127] = {0:0}, {1:0}, {2:0}, {3:0}\n",adblCalMem[124],adblCalMem[125],adblCalMem[126],adblCalMem[127]);
			*/

			Console.ReadLine(); // Pause for user
		}
	}
}
