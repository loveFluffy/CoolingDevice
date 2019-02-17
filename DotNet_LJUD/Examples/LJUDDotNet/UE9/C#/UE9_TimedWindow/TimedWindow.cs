//---------------------------------------------------------------------------
//
//  UE9_SimpleWindowed.cs
// 
//  Basic command/response UE9 example using the UD driver and
//	a C# GUI window that periodically updatea using a timer.
//  Please note that most of this code has been copied
//  directly from the U3_Simple example. This is because, 
//  in most cirumstances, the LabJack related code is the same
//  as it would be without a GUI
//
//  support@labjack.com
//  July 13, 2010
//	Revised December 29, 2010
//----------------------------------------------------------------------
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Timers;

// Import the UD .NET wrapper object.  The dll referenced is installed by the
// LabJackUD installer.
using LabJack.LabJackUD; 

namespace UE9_SimpleWindowed
{
	/// <summary>
	/// The application's basic form
	/// </summary>
	public class TimedWindow : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label versionDisplay;
		private System.Windows.Forms.Label errorLabel;
		private System.Windows.Forms.Label errorDisplay;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label ain2Label;
		private System.Windows.Forms.Label ain2Display;
		private System.Windows.Forms.Label ain3Label;
		private System.Windows.Forms.Label ain3Display;
		private System.Windows.Forms.Label fio1Label;
		private System.Windows.Forms.Label fio1Display;
		private System.Windows.Forms.Button goStopButton;
		
		// our UE9 variable
		private UE9 ue9;

		// The timer
		System.Timers.Timer updateTimer;
		private const int TIMER_INTERVAL = 1000; // milliseconds between reads

		public TimedWindow()
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
			this.goStopButton = new System.Windows.Forms.Button();
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
			// goStopButton
			// 
			this.goStopButton.Location = new System.Drawing.Point(8, 160);
			this.goStopButton.Name = "goStopButton";
			this.goStopButton.Size = new System.Drawing.Size(280, 24);
			this.goStopButton.TabIndex = 14;
			this.goStopButton.Text = "Go";
			this.goStopButton.Click += new System.EventHandler(this.goStopButton_Click);
			// 
			// errorLabel
			// 
			this.errorLabel.Location = new System.Drawing.Point(8, 104);
			this.errorLabel.Name = "errorLabel";
			this.errorLabel.Size = new System.Drawing.Size(136, 16);
			this.errorLabel.TabIndex = 15;
			this.errorLabel.Text = "Error:";
			// 
			// errorDisplay
			// 
			this.errorDisplay.Location = new System.Drawing.Point(152, 104);
			this.errorDisplay.Name = "errorDisplay";
			this.errorDisplay.Size = new System.Drawing.Size(136, 48);
			this.errorDisplay.TabIndex = 16;
			// 
			// TimedWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 198);
			this.Controls.Add(this.errorDisplay);
			this.Controls.Add(this.errorLabel);
			this.Controls.Add(this.goStopButton);
			this.Controls.Add(this.fio1Display);
			this.Controls.Add(this.fio1Label);
			this.Controls.Add(this.ain3Display);
			this.Controls.Add(this.ain3Label);
			this.Controls.Add(this.ain2Display);
			this.Controls.Add(this.ain2Label);
			this.Controls.Add(this.versionDisplay);
			this.Controls.Add(this.versionLabel);
			this.Name = "TimedWindow";
			this.Text = "TimedWindow";
			this.Load += new System.EventHandler(this.TimedWindow_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new TimedWindow());
		}

		/// <summary>
		/// Opens the LabJack, gets the UD driver version, and 
		/// configures the device.
		/// </summary>
		/// <param name="sender">The object that called this event</param>
		/// <param name="e">Event details</param>
		private void TimedWindow_Load(object sender, System.EventArgs e)
		{
			double dblDriverVersion;
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;

			// dummy variables to satisfy certian method signatures
			double dummyDouble = 0;
			int dummyInt = 0;

			// Create the event timer but do not start it yet
			updateTimer = new System.Timers.Timer();
			updateTimer.Elapsed += new ElapsedEventHandler( TimerEvent );
			updateTimer.Interval = TIMER_INTERVAL;

			// Disable the start button while the device is loading
			goStopButton.Enabled = false;
			Update();

			//Read and display the UD version.
			dblDriverVersion = LJUD.GetDriverVersion();
			versionDisplay.Text = String.Format("{0:0.000}",dblDriverVersion);

			// Open and configure UE9
			try 
			{
				//Open the device
				ue9 = new UE9(LJUD.CONNECTION.USB, "0", true);

				//Configure for 16-bit analog input measurements.
				LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 16, 0, 0);

				//Configure the analog input range on channels 2 and 3 for bipolar 5v.
				LJUD.AddRequest(ue9.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 2, (double) LJUD.RANGES.BIP5V, 0, 0);
				LJUD.AddRequest(ue9.ljhandle,  LJUD.IO.PUT_AIN_RANGE, (LJUD.CHANNEL) 3, (double) LJUD.RANGES.BIP5V, 0, 0);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}

			try
			{
				//Execute the requests.
				LJUD.GoOne (ue9.ljhandle);

				//Get all the results.  The input measurement results are stored.  All other
				//results are for configuration or output requests so we are just checking
				//whether there was an error. The rest of the results are in the below loop.
				LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}

			bool isFinished = false;
			while(!isFinished)
			{
				try {LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
				catch (LabJackUDException exc) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(exc.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
					{
						ShowErrorMessage(exc);
						return;
					}
				}
			}

			// Enable the start button
			goStopButton.Enabled = true;
		}

		/// <summary>
		/// Starts/stops the software timer that will update the GUI.
		/// </summary>
		/// <param name="sender">The object that executed this method</param>
		/// <param name="e">Event parameters</param>
		private void goStopButton_Click(object sender, System.EventArgs e)
		{
			// Stop the timer if it is enabled
			if (updateTimer.Enabled)
			{
				updateTimer.Stop();
				goStopButton.Text = "Start";
			}
			// Start the timer if it is not enabled
			else
			{
				updateTimer.Start();
				goStopButton.Text = "Stop";
			}
		}
		
		/// <summary>
		/// performs read on LabJack device
		/// </summary>
		/// <param name="source">The object that called the event</param>
		/// <param name="e">Event details</param>
		public void TimerEvent( object source, ElapsedEventArgs e)
		{
			LJUD.IO ioType=0;
			LJUD.CHANNEL channel=0;
			double dblValue=0;
			double Value2=0,Value3=0;
			double ValueDIBit=0;

			// dummy variables to satisfy certian method signatures
			double dummyDouble = 0;
			int dummyInt = 0;

			try
			{
				//Now we add requests to write and read I/O.  These requests
				//will be processed repeatedly by go/get statements in every
				//iteration of the while loop below.

				//Request AIN2 and AIN3.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0);
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_AIN, 3, 0, 0, 0);

				//Read digital input FIO1.
				LJUD.AddRequest (ue9.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 1, 0, 0, 0);
			}
			catch (LabJackUDException exc) 
			{
				ShowErrorMessage(exc);
				return;
			}

			try
			{
				//Execute the requests.
				LJUD.GoOne (ue9.ljhandle);

				//Get all the results.  The input measurement results are stored.  All other
				//results are for configuration or output requests so we are just checking
				//whether there was an error. The rest of the results are in the below loop.
				LJUD.GetFirstResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);
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

				}
				try {LJUD.GetNextResult(ue9.ljhandle, ref ioType, ref channel, ref dblValue, ref dummyInt, ref dummyDouble);}
				catch (LabJackUDException exc) 
				{
					// If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
					if(exc.LJUDError == UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE)
						isFinished = true;
					else
					{
						ShowErrorMessage(exc);
						return;
					}
				}
			}

			// Display results
			ain2Display.Text = String.Format("{0:0.###}",Value2);
			ain3Display.Text = String.Format("{0:0.###}",Value3);
			fio1Display.Text = String.Format("{0:0.###}",ValueDIBit);
		}
	}
}
