'---------------------------------------------------------------------------
'
'  U3_EFunctions.vb
' 
'  Demonstrates the UD E-functions with the LabJack U3.  For timer/counter
'  testing, connect FIO4 to FIO5 and FIO6.
'
'  support@labjack.com
'  June 12, 2009
'----------------------------------------------------------------------
'

'Simple VB.NET example uses the LabJackUD driver to communicate with a U3.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U3_EFunctions
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents ainThreeLabel As System.Windows.Forms.Label
    Friend WithEvents dacZeroLabel As System.Windows.Forms.Label
    Friend WithEvents fioFourLabel As System.Windows.Forms.Label
    Friend WithEvents fioSevenLabel As System.Windows.Forms.Label
    Friend WithEvents timerOneLabel As System.Windows.Forms.Label
    Friend WithEvents counterOneLabel As System.Windows.Forms.Label
    Friend WithEvents highClicksOneLabel As System.Windows.Forms.Label
    Friend WithEvents lowClicksOneLabel As System.Windows.Forms.Label
    Friend WithEvents dutyCycleOneLabel As System.Windows.Forms.Label
    Friend WithEvents dutyCycleOneDisplay As System.Windows.Forms.Label
    Friend WithEvents lowClicksOneDisplay As System.Windows.Forms.Label
    Friend WithEvents highClicksOneDisplay As System.Windows.Forms.Label
    Friend WithEvents counterOneDisplay As System.Windows.Forms.Label
    Friend WithEvents timerOneDisplay As System.Windows.Forms.Label
    Friend WithEvents fioSevenDisplay As System.Windows.Forms.Label
    Friend WithEvents fioFourDisplay As System.Windows.Forms.Label
    Friend WithEvents dacZeroDisplay As System.Windows.Forms.Label
    Friend WithEvents ainThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.ainThreeLabel = New System.Windows.Forms.Label
        Me.dacZeroLabel = New System.Windows.Forms.Label
        Me.fioFourLabel = New System.Windows.Forms.Label
        Me.fioSevenLabel = New System.Windows.Forms.Label
        Me.timerOneLabel = New System.Windows.Forms.Label
        Me.counterOneLabel = New System.Windows.Forms.Label
        Me.highClicksOneLabel = New System.Windows.Forms.Label
        Me.lowClicksOneLabel = New System.Windows.Forms.Label
        Me.dutyCycleOneLabel = New System.Windows.Forms.Label
        Me.dutyCycleOneDisplay = New System.Windows.Forms.Label
        Me.lowClicksOneDisplay = New System.Windows.Forms.Label
        Me.highClicksOneDisplay = New System.Windows.Forms.Label
        Me.counterOneDisplay = New System.Windows.Forms.Label
        Me.timerOneDisplay = New System.Windows.Forms.Label
        Me.fioSevenDisplay = New System.Windows.Forms.Label
        Me.fioFourDisplay = New System.Windows.Forms.Label
        Me.dacZeroDisplay = New System.Windows.Forms.Label
        Me.ainThreeDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ainThreeLabel
        '
        Me.ainThreeLabel.Location = New System.Drawing.Point(8, 8)
        Me.ainThreeLabel.Name = "ainThreeLabel"
        Me.ainThreeLabel.Size = New System.Drawing.Size(136, 16)
        Me.ainThreeLabel.TabIndex = 0
        Me.ainThreeLabel.Text = "AIN3: "
        '
        'dacZeroLabel
        '
        Me.dacZeroLabel.Location = New System.Drawing.Point(8, 32)
        Me.dacZeroLabel.Name = "dacZeroLabel"
        Me.dacZeroLabel.Size = New System.Drawing.Size(136, 16)
        Me.dacZeroLabel.TabIndex = 1
        Me.dacZeroLabel.Text = "DAC0: "
        '
        'fioFourLabel
        '
        Me.fioFourLabel.Location = New System.Drawing.Point(8, 56)
        Me.fioFourLabel.Name = "fioFourLabel"
        Me.fioFourLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioFourLabel.TabIndex = 2
        Me.fioFourLabel.Text = "FIO4: "
        '
        'fioSevenLabel
        '
        Me.fioSevenLabel.Location = New System.Drawing.Point(8, 80)
        Me.fioSevenLabel.Name = "fioSevenLabel"
        Me.fioSevenLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioSevenLabel.TabIndex = 3
        Me.fioSevenLabel.Text = "FIO7:"
        '
        'timerOneLabel
        '
        Me.timerOneLabel.Location = New System.Drawing.Point(8, 104)
        Me.timerOneLabel.Name = "timerOneLabel"
        Me.timerOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.timerOneLabel.TabIndex = 4
        Me.timerOneLabel.Text = "Timer 1:"
        '
        'counterOneLabel
        '
        Me.counterOneLabel.Location = New System.Drawing.Point(8, 128)
        Me.counterOneLabel.Name = "counterOneLabel"
        Me.counterOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.counterOneLabel.TabIndex = 5
        Me.counterOneLabel.Text = "Counter 1:"
        '
        'highClicksOneLabel
        '
        Me.highClicksOneLabel.Location = New System.Drawing.Point(8, 152)
        Me.highClicksOneLabel.Name = "highClicksOneLabel"
        Me.highClicksOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.highClicksOneLabel.TabIndex = 6
        Me.highClicksOneLabel.Text = "High clicks Timer 1:"
        '
        'lowClicksOneLabel
        '
        Me.lowClicksOneLabel.Location = New System.Drawing.Point(8, 176)
        Me.lowClicksOneLabel.Name = "lowClicksOneLabel"
        Me.lowClicksOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.lowClicksOneLabel.TabIndex = 7
        Me.lowClicksOneLabel.Text = "Low clicks Timer 1:"
        '
        'dutyCycleOneLabel
        '
        Me.dutyCycleOneLabel.Location = New System.Drawing.Point(8, 200)
        Me.dutyCycleOneLabel.Name = "dutyCycleOneLabel"
        Me.dutyCycleOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.dutyCycleOneLabel.TabIndex = 8
        Me.dutyCycleOneLabel.Text = "Duty cycle Timer 1:"
        '
        'dutyCycleOneDisplay
        '
        Me.dutyCycleOneDisplay.Location = New System.Drawing.Point(136, 200)
        Me.dutyCycleOneDisplay.Name = "dutyCycleOneDisplay"
        Me.dutyCycleOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.dutyCycleOneDisplay.TabIndex = 17
        '
        'lowClicksOneDisplay
        '
        Me.lowClicksOneDisplay.Location = New System.Drawing.Point(136, 176)
        Me.lowClicksOneDisplay.Name = "lowClicksOneDisplay"
        Me.lowClicksOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.lowClicksOneDisplay.TabIndex = 16
        '
        'highClicksOneDisplay
        '
        Me.highClicksOneDisplay.Location = New System.Drawing.Point(136, 152)
        Me.highClicksOneDisplay.Name = "highClicksOneDisplay"
        Me.highClicksOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.highClicksOneDisplay.TabIndex = 15
        '
        'counterOneDisplay
        '
        Me.counterOneDisplay.Location = New System.Drawing.Point(136, 128)
        Me.counterOneDisplay.Name = "counterOneDisplay"
        Me.counterOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.counterOneDisplay.TabIndex = 14
        '
        'timerOneDisplay
        '
        Me.timerOneDisplay.Location = New System.Drawing.Point(136, 104)
        Me.timerOneDisplay.Name = "timerOneDisplay"
        Me.timerOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.timerOneDisplay.TabIndex = 13
        '
        'fioSevenDisplay
        '
        Me.fioSevenDisplay.Location = New System.Drawing.Point(136, 80)
        Me.fioSevenDisplay.Name = "fioSevenDisplay"
        Me.fioSevenDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioSevenDisplay.TabIndex = 12
        '
        'fioFourDisplay
        '
        Me.fioFourDisplay.Location = New System.Drawing.Point(136, 56)
        Me.fioFourDisplay.Name = "fioFourDisplay"
        Me.fioFourDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioFourDisplay.TabIndex = 11
        '
        'dacZeroDisplay
        '
        Me.dacZeroDisplay.Location = New System.Drawing.Point(136, 32)
        Me.dacZeroDisplay.Name = "dacZeroDisplay"
        Me.dacZeroDisplay.Size = New System.Drawing.Size(136, 16)
        Me.dacZeroDisplay.TabIndex = 10
        '
        'ainThreeDisplay
        '
        Me.ainThreeDisplay.Location = New System.Drawing.Point(136, 8)
        Me.ainThreeDisplay.Name = "ainThreeDisplay"
        Me.ainThreeDisplay.Size = New System.Drawing.Size(136, 16)
        Me.ainThreeDisplay.TabIndex = 9
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 224)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(264, 24)
        Me.goButton.TabIndex = 18
        Me.goButton.Text = "Go"
        '
        'U3_EFunctions
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 264)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.dutyCycleOneDisplay)
        Me.Controls.Add(Me.lowClicksOneDisplay)
        Me.Controls.Add(Me.highClicksOneDisplay)
        Me.Controls.Add(Me.counterOneDisplay)
        Me.Controls.Add(Me.timerOneDisplay)
        Me.Controls.Add(Me.fioSevenDisplay)
        Me.Controls.Add(Me.fioFourDisplay)
        Me.Controls.Add(Me.dacZeroDisplay)
        Me.Controls.Add(Me.ainThreeDisplay)
        Me.Controls.Add(Me.dutyCycleOneLabel)
        Me.Controls.Add(Me.lowClicksOneLabel)
        Me.Controls.Add(Me.highClicksOneLabel)
        Me.Controls.Add(Me.counterOneLabel)
        Me.Controls.Add(Me.timerOneLabel)
        Me.Controls.Add(Me.fioSevenLabel)
        Me.Controls.Add(Me.fioFourLabel)
        Me.Controls.Add(Me.dacZeroLabel)
        Me.Controls.Add(Me.ainThreeLabel)
        Me.Name = "U3_EFunctions"
        Me.Text = "U3_EFunctions"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub goButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        Dim dblValue As Double
        dblValue = 0
        Dim IntegerValue As Integer
        IntegerValue = 0

        Dim binary As Integer
        Dim aEnableTimers(2) As Integer
        Dim aEnableCounters(2) As Integer
        Dim tcpInOffset As Integer
        Dim timerClockDivisor As Integer
        Dim timerClockBaseIndex As LJUD.TIMERCLOCKS
        Dim aTimerModes(2) As Integer
        Dim adblTimerValues(2) As Double
        Dim aReadTimers(2) As Integer
        Dim aUpdateResetTimers(2) As Integer
        Dim aReadCounters(2) As Integer
        Dim aResetCounters(2) As Integer
        Dim adblCounterValues() As Double
        adblCounterValues = New Double() {0.0, 0.0}
        Dim intValue As Integer
        Dim highTime, lowTime, dutyCycle As Double

        'Open the first found LabJack U3.
        Try

            Dim u3 As U3

            u3 = New U3(LJUD.CONNECTION.USB, "0", True) ' Connection through USB

            'Start by using the pin_configuration_reset IOType so that all
            'pin assignments are in the factory default condition.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)

            'Take a single-ended measurement from AIN3.
            binary = 0
            LJUD.eAIN(u3.ljhandle, 3, 31, dblValue, -1, -1, -1, binary)
            ainThreeDisplay.Text = dblValue

            'Set DAC0 to 3.0 volts.
            dblValue = 3
            binary = 0
            LJUD.eDAC(u3.ljhandle, 0, dblValue, binary, 0, 0)
            dacZeroDisplay.Text = dblValue

            'Read state of FIO4.
            LJUD.eDI(u3.ljhandle, 4, intValue)
            fioFourDisplay.Text = intValue

            'Set the state of FIO7.
            intValue = 1
            LJUD.eDO(u3.ljhandle, 7, intValue)
            fioSevenDisplay.Text = intValue

            'Timers and Counters example.
            'First, a call to eTCConfig.  Fill the arrays with the desired values, then make the call.
            aEnableTimers(0) = 1 'Enable Timer0 (uses FIO4).
            aEnableTimers(1) = 1 'Enable Timer1 (uses FIO5).
            aEnableCounters(0) = 0 'Disable Counter0.
            aEnableCounters(1) = 1 'Enable Counter1 (uses FIO6).
            tcpInOffset = 4
            timerClockBaseIndex = LJUD.TIMERCLOCKS.MHZ48_DIV
            'timerClockBaseIndex = LJUD.TIMERCLOCKS.MHZ24_DIV  'Use this line instead for hardware rev 1.20.
            timerClockDivisor = 48
            'timerClockDivisor = 24  'Use this line instead for hardware rev 1.20.
            aTimerModes(0) = LJUD.TIMERMODE.PWM8 'Timer0 is 8-bit PWM output.  Frequency is 1M/256 = 3906 Hz.
            aTimerModes(1) = LJUD.TIMERMODE.DUTYCYCLE 'Timer1 is duty cyle input.
            adblTimerValues(0) = 16384 'Set PWM8 duty-cycle to 75%.
            adblTimerValues(1) = 0
            LJUD.eTCConfig(u3.ljhandle, aEnableTimers, aEnableCounters, tcpInOffset, timerClockBaseIndex, timerClockDivisor, aTimerModes, adblTimerValues, 0, 0)

            Threading.Thread.Sleep(1000) 'Wait 1 second.

            'Now, a call to eTCValues.
            aReadTimers(0) = 0 'Don't read Timer0 (output timer).
            aReadTimers(1) = 1 'Read Timer1
            aUpdateResetTimers(0) = 1 'Update Timer0
            aUpdateResetTimers(1) = 1 'Reset Timer1
            aReadCounters(0) = 0
            aReadCounters(1) = 1 'Read Counter1
            aResetCounters(0) = 0
            aResetCounters(1) = 1 'Reset Counter1.
            adblTimerValues(0) = 32768 'Change Timer0 duty-cycle to 50%.
            adblTimerValues(1) = 0
            LJUD.eTCValues(u3.ljhandle, aReadTimers, aUpdateResetTimers, aReadCounters, aResetCounters, adblTimerValues, adblCounterValues, 0, 0)
            timerOneDisplay.Text = adblTimerValues(1)
            counterOneDisplay.Text = adblCounterValues(1)

            'Convert Timer1 value to duty-cycle percentage.
            'High time is LSW
            highTime = ((adblTimerValues(1)) Mod (65536))
            'Low time is MSW
            lowTime = ((adblTimerValues(1)) / (65536))
            'Calculate the duty cycle percentage.
            dutyCycle = 100
            highClicksOneDisplay.Text = highTime
            lowClicksOneDisplay.Text = lowTime
            dutyCycleOneDisplay.Text = dutyCycle


            'Disable all timers and counters.
            aEnableTimers(0) = 0
            aEnableTimers(1) = 0
            aEnableCounters(0) = 0
            aEnableCounters(1) = 0
            LJUD.eTCConfig(u3.ljhandle, aEnableTimers, aEnableCounters, 4, timerClockBaseIndex, timerClockDivisor, aTimerModes, adblTimerValues, 0, 0)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try
    End Sub

End Class
