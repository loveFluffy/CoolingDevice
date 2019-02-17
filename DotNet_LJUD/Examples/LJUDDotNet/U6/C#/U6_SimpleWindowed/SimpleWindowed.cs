//---------------------------------------------------------------------------
//
//  U6_SimpleWindowed.cs
// 
//  Basic command/response U6 example using the UD driver and
//	a C# GUI window. Please note that most of this code
//  has been copied directly from the U6_Simple example. 
//  This is because, in most cirumstances, the LabJack related
//  code is the same as it would be without a GUI
//
//  support@labjack.com
//  July 13, 2010
//	Revised December 28, 2010
//----------------------------------------------------------------------
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace U6_SimpleWindowed
{
	/// <summary>
	/// The application's basic form
	/// </summary>
	public class SimpleWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label versionDisplay;
		private System.Windows.Forms.Button goButton;
		private System.Windows.Forms.Label errorLabel;
		private System.Windows.Forms.Label errorDisplay;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label ain2Label;
		private System.Windows.Forms.Label ain2Display;
		private System.Windows.Forms.Label ain3Label;
		private System.Windows.Forms.Label ain3Display;
		private System.Windows.Forms.Label fio1Label;
		private System.Windows.Forms.Label fio1Display;
		private System.Windows.Forms.Label counter0Label;
		private System.Windows.Forms.Label counter0Display;
		private System.Windows.Forms.Label fio2Label;
		private System.Windows.Forms.Label fio2Display;
		
		// our U6 variable
		private U6 u6;

		public SimpleWindow()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
		}

		// If error occured print a message indicating which one occurred
		public void ShowErrorMessage(LabJackUDException exc)
		{
			errorDisplay.Text = "Error: " + exc.ToString();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.versionLabel = new System.Windows.Forms.Label();
			this.versionDisplay = new System.Windows.Forms.Label();
			this.ain2Label = new System.Windows.Forms.Label();
			this.ain2Display = new System.Windows.Forms.Label();
			this.ain3Label = new System.Windows.Forms.Label();
			this.ain3Display = new System.Windows.Forms.Label();
			this.fio1Label = new System.Windows.Forms.Label();
			this.fio1Display = new System.Windows.Forms.Label();
			this.fio2Label = new System.Windows.Forms.Label();
			this.fio2Display = new System.Windows.Forms.Label();
			this.counter0Label = new System.Windows.Forms.Label();
			this.counter0Display = new System.Windows.Forms.Label();
			this.goButton = new System.Windows.Forms.Button();
			this.errorLabel = new System.Windows.Forms.Label();
			this.errorDisplay = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// versionLabel
			// 
			this.versionLabel.Location = new System.Drawing.Point(8, 8);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(136, 16);
			this.versionLabel.TabIndex = 0;
			this.versionLabel.Text = "UD Driver Version:";
			// 
			// versionDisplay
			// 
			this.versionDisplay.Location = new System.Drawing.Point(152, 8);
			this.versionDisplay.Name = "versionDisplay";
			this.versionDisplay.Size = new System.Drawing.Size(136, 16);
			this.versionDisplay.TabIndex = 1;
			// 
			// ain2Label
			// 
			this.ain2Label.Location = new System.Drawing.Point(8, 32);
			this.ain2Label.Name = "ain2Label";
			this.ain2Label.Size = new System.Drawing.Size(136, 16);
			this.ain2Label.TabIndex = 2;
			this.ain2Label.Text = "AIN2:";
			// 
			// ain2Display
			// 
			this.ain2Display.Location = new System.Drawing.Point(152, 32);
			this.ain2Display.Name = "ain2Display";
			this.ain2Display.Size = new System.Drawing.Size(136, 16);
			this.ain2Display.TabIndex = 3;
			// 
			// ain3Label
			// 
			this.ain3Label.Location = new System.Drawing.Point(8, 56);
			this.ain3Label.Name = "ain3Label";
			this.ain3Label.Size = new System.Drawing.Size(136, 16);
			this.ain3Label.TabIndex = 4;
			this.ain3Label.Text = "AIN3:";
			// 
			// ain3Display
			// 
			this.ain3Display.Location = new System.Drawing.Point(152, 56);
			this.ain3Display.Name = "ain3Display";
			this.ain3Display.Size = new System.Drawing.Size(136, 16);
			this.ain3Display.TabIndex = 5;
			// 
			// fio1Label
			// 
			this.fio1Label.Location = new System.Drawing.Point(8, 80);
			this.fio1Label.Name = "fio1Label";
			this.fio1Label.Size = new System.Drawing.Size(136, 16);
			this.fio1Label.TabIndex = 8;
			this.fio1Label.Text = "FIO1:";
			// 
			// fio1Display
			// 
			this.fio1Display.Location = new System.Drawing.Point(152, 80);
			this.fio1Display.Name = "fio1Display";
			this.fio1Display.Size = new System.Drawing.Size(136, 16);
			this.fio1Display.TabIndex = 9;
			// 
			// fio2Label
			// 
			this.fio2Label.Location = new System.Drawing.Point(8, 104);
			this.fio2Label.Name = "fio2Label";
			this.fio2Label.Size = new System.Drawing.Size(136, 16);
			this.fio2Label.TabIndex = 10;
			this.fio2Label.Text = "FIO2-FIO3:";
			// 
			// fio2Display
			// 
			this.fio2Display.Location = new System.Drawing.Point(152, 104);
			this.fio2Display.Name = "fio2Display";
			this.fio2Display.Size = new System.Drawing.Size(136, 16);
			this.fio2Display.TabIndex = 11;
			// 
			// counter0Label
			// 
			this.counter0Label.Location = new System.Drawing.Point(8, 128);
			this.counter0Label.Name = "counter0Label";
			this.counter0Label.Size = new System.Drawing.Size(136, 16);
			this.counter0Label.TabIndex = 12;
			this.counter0Label.Text = "Counter0 (FIO0):";
			// 
			// counter0Display
			// 
			this.counter0Display.Location = new System.Drawing.Point(152, 128);
			this.counter0Display.Name = "counter0Display";
			this.counter0Display.Size = new System.Drawing.Size(136, 16);
			this.counter0Display.TabIndex = 13;
			// 
			// goButton
			// 
			this.goButton.Location = new System.Drawing.Point(8, 208);
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(280, 24);
			this.goButton.TabIndex = 14;
			this.goButton.Text = "Go";
			this.goButton.Click += new System.EventHandler(this.goButton_Click);
			// 
			// errorLabel
			// 
			this.errorLabel.Location = new System.Drawing.Point(8, 152);
			this.errorLabel.Name = "errorLabel";
			this.errorLabel.Size = new System.Drawing.Size(136, 16);
			this.errorLabel.TabIndex = 15;
			this.errorLabel.Text = "Error:";
			// 
			// errorDisplay
			// 
			this.errorDisplay.Location = new System.Drawing.Point(152, 152);
			this.errorDisplay.Name = "errorDisplay";
			this.errorDisplay.Size = new System.Drawing.Size(136, 48);
			this.errorDisplay.TabIndex = 16;
			// 
			// SimpleWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 238);
			this.Controls.Add(this.errorDisplay);
			this.Controls.Add(this.errorLabel);
			this.Controls.Add(this.goButton);
			this.Controls.Add(this.counter0Display);
			this.Controls.Add(this.counter0Label);
			this.Controls.Add(this.fio2Display);
			this.Controls.Add(this.fio2Label);
			this.Controls.Add(this.fio1Display);
			this.Controls.Add(this.fio1Label);
			this.Controls.Add(this.ain3Display);
			this.Controls.Add(this.ain3Label);
			this.Controls.Add(this.ain2Display);
			this.Controls.Add(this.ain2Label);
			this.Controls.Add(this.versionDisplay);
			this.Controls.Add(this.versionLabel);
			this.Name = "SimpleWindow";
			this.Text = "SimpleWindow";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new SimpleWindow());
		}

		/// <summary>
		/// Actually performs actions on the U6 and updates the displaye
		/// </summary>
		/// <param name="sender">The object that executed this method</param>
		/// <param name="e">Event parameters</param>
		private void goButton_Click(object sender, System.EventArgs e)
		{
			double dblDriverVersion;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double Value2=0,Value3=0;
			double ValueDIBit=0,ValueDIPort=0,ValueCounter=0;

			// dummy variables to satisfy certian method signatures
			double dummyDouble = 0;
			int dummyInt = 0;

			//Read and display the UD version.
			dblDriverVersion = LJUD.GetDriverVersion();
			versionDisplay.Text = String.Format("{0:0.000}",dblDriverVersion);

			// Open U6
			try 
			{
				u6 = new U6(LJUD.CONNECTION.USB, "0", true);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}

			try
			{
				//First some configuration commands.  These will be done with the ePut
				//function which combines the add/go/get into a single call.

				//Configure the resolution of the analog inputs (pass a non-zero value for quick sampling). 
                //See section 2.6 / 3.1 for more information.
				LJUD.ePut (u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 0, 0);

				//Configure the analog input range on channels 2 and 3 for bipolar 10v.
				LJUD.ePut (u6.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 2, (double) LJUD.RANGES.BIP10V, 0);

				LJUD.ePut (u6.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 3, (double) LJUD.RANGES.BIP10V, 0);

				//Enable Counter0 which will appear on FIO0 (assuming no other
				//program has enabled any timers or Counter1).
				LJUD.ePut (u6.ljhandle,  LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0);

				//Now we add requests to write and read I/O.  These requests
				//will be processed repeatedly by go/get statements in every
				//iteration of the while loop below.

				//Request AIN2 and AIN3.
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0);
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_AIN, 3, 0, 0, 0);

				//Set DAC0 to 2.5 volts.
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.PUT_DAC, 0, 2.5, 0, 0);

				//Read digital input FIO1.
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 1, 0, 0, 0);

				//Read digital inputs FIO2 through FIO3.
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_DIGITAL_PORT, 2, 0, 2, 0);
				// LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_DIGITAL_PORT, 2, 0, 3, 0); would request through FIO4

				//Request the value of Counter0.
				LJUD.AddRequest (u6.ljhandle, LJUD.IO.GET_COUNTER, 0, 0, 0, 0);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}
			
			try
			{
				//Execute the requests.
				LJUD.GoOne (u6.ljhandle);

				//Get all the results.  The input measurement results are stored.  All other
				//results are for configuration or output requests so we are just checking
				//whether there was an error.
				LJUD.GetFirstResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}

			bool isFinished = false;
			while(!isFinished)
			{
				switch(ioType)
				{

					case LJUD.IO.GET_AIN :
					switch((int)channel)
					{
						case 2:
							Value2=dblValue;
							break;
						case 3:
							Value3=dblValue;
							break;
					}
						break;

					case LJUD.IO.GET_DIGITAL_BIT :
						ValueDIBit=dblValue;
						break;

					case LJUD.IO.GET_DIGITAL_PORT :
						ValueDIPort=dblValue;
						break;

					case LJUD.IO.GET_COUNTER :
						ValueCounter=dblValue;
						break;

				}
				try {LJUD.GetNextResult(u6.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
				catch (LabJackUDException exc) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(exc.LJUDError == U6.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
						ShowErrorMessage(exc);
				}
			}

			// Display results
			ain2Display.Text = String.Format("{0:0.###}",Value2);
			ain3Display.Text = String.Format("{0:0.###}",Value3);
			fio1Display.Text = String.Format("{0:0.###}",ValueDIBit);
			fio2Display.Text = String.Format("{0:0.###}",ValueDIPort);  //Will read 30 (binary 11) if both lines are pulled-high as normal.
			counter0Display.Text = String.Format("{0:0.###}",ValueCounter);
		}
	}
}
