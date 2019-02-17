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
    Friend WithEvents dacZeroDisplay As System.Windows.Forms.Label
    Friend WithEvents ainThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents fioTwoLabel As System.Windows.Forms.Label
    Friend WithEvents fioThreeLabel As System.Windows.Forms.Label
    Friend WithEvents fioThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents fioTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents timerTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents timerTwoLabel As System.Windows.Forms.Label
    Friend WithEvents timerThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents timerThreeLabel As System.Windows.Forms.Label
    Friend WithEvents counterZeroDisplay As System.Windows.Forms.Label
    Friend WithEvents counterZeroLabel As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.ainThreeLabel = New System.Windows.Forms.Label
        Me.dacZeroLabel = New System.Windows.Forms.Label
        Me.fioTwoLabel = New System.Windows.Forms.Label
        Me.fioThreeLabel = New System.Windows.Forms.Label
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
        Me.fioThreeDisplay = New System.Windows.Forms.Label
        Me.fioTwoDisplay = New System.Windows.Forms.Label
        Me.dacZeroDisplay = New System.Windows.Forms.Label
        Me.ainThreeDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.timerTwoDisplay = New System.Windows.Forms.Label
        Me.timerTwoLabel = New System.Windows.Forms.Label
        Me.timerThreeDisplay = New System.Windows.Forms.Label
        Me.timerThreeLabel = New System.Windows.Forms.Label
        Me.counterZeroDisplay = New System.Windows.Forms.Label
        Me.counterZeroLabel = New System.Windows.Forms.Label
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
        'fioTwoLabel
        '
        Me.fioTwoLabel.Location = New System.Drawing.Point(8, 56)
        Me.fioTwoLabel.Name = "fioTwoLabel"
        Me.fioTwoLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioTwoLabel.TabIndex = 2
        Me.fioTwoLabel.Text = "FIO2: "
        '
        'fioThreeLabel
        '
        Me.fioThreeLabel.Location = New System.Drawing.Point(8, 80)
        Me.fioThreeLabel.Name = "fioThreeLabel"
        Me.fioThreeLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioThreeLabel.TabIndex = 3
        Me.fioThreeLabel.Text = "FIO3:"
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
        Me.counterOneLabel.Location = New System.Drawing.Point(8, 200)
        Me.counterOneLabel.Name = "counterOneLabel"
        Me.counterOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.counterOneLabel.TabIndex = 5
        Me.counterOneLabel.Text = "Counter 1:"
        '
        'highClicksOneLabel
        '
        Me.highClicksOneLabel.Location = New System.Drawing.Point(8, 224)
        Me.highClicksOneLabel.Name = "highClicksOneLabel"
        Me.highClicksOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.highClicksOneLabel.TabIndex = 6
        Me.highClicksOneLabel.Text = "High clicks Timer 1:"
        '
        'lowClicksOneLabel
        '
        Me.lowClicksOneLabel.Location = New System.Drawing.Point(8, 248)
        Me.lowClicksOneLabel.Name = "lowClicksOneLabel"
        Me.lowClicksOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.lowClicksOneLabel.TabIndex = 7
        Me.lowClicksOneLabel.Text = "Low clicks Timer 1:"
        '
        'dutyCycleOneLabel
        '
        Me.dutyCycleOneLabel.Location = New System.Drawing.Point(8, 272)
        Me.dutyCycleOneLabel.Name = "dutyCycleOneLabel"
        Me.dutyCycleOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.dutyCycleOneLabel.TabIndex = 8
        Me.dutyCycleOneLabel.Text = "Duty cycle Timer 1:"
        '
        'dutyCycleOneDisplay
        '
        Me.dutyCycleOneDisplay.Location = New System.Drawing.Point(136, 272)
        Me.dutyCycleOneDisplay.Name = "dutyCycleOneDisplay"
        Me.dutyCycleOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.dutyCycleOneDisplay.TabIndex = 17
        '
        'lowClicksOneDisplay
        '
        Me.lowClicksOneDisplay.Location = New System.Drawing.Point(136, 248)
        Me.lowClicksOneDisplay.Name = "lowClicksOneDisplay"
        Me.lowClicksOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.lowClicksOneDisplay.TabIndex = 16
        '
        'highClicksOneDisplay
        '
        Me.highClicksOneDisplay.Location = New System.Drawing.Point(136, 224)
        Me.highClicksOneDisplay.Name = "highClicksOneDisplay"
        Me.highClicksOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.highClicksOneDisplay.TabIndex = 15
        '
        'counterOneDisplay
        '
        Me.counterOneDisplay.Location = New System.Drawing.Point(136, 200)
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
        'fioThreeDisplay
        '
        Me.fioThreeDisplay.Location = New System.Drawing.Point(136, 80)
        Me.fioThreeDisplay.Name = "fioThreeDisplay"
        Me.fioThreeDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioThreeDisplay.TabIndex = 12
        '
        'fioTwoDisplay
        '
        Me.fioTwoDisplay.Location = New System.Drawing.Point(136, 56)
        Me.fioTwoDisplay.Name = "fioTwoDisplay"
        Me.fioTwoDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioTwoDisplay.TabIndex = 11
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
        Me.goButton.Location = New System.Drawing.Point(8, 296)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(264, 24)
        Me.goButton.TabIndex = 18
        Me.goButton.Text = "Go"
        '
        'timerTwoDisplay
        '
        Me.timerTwoDisplay.Location = New System.Drawing.Point(136, 128)
        Me.timerTwoDisplay.Name = "timerTwoDisplay"
        Me.timerTwoDisplay.Size = New System.Drawing.Size(136, 16)
        Me.timerTwoDisplay.TabIndex = 20
        '
        'timerTwoLabel
        '
        Me.timerTwoLabel.Location = New System.Drawing.Point(8, 128)
        Me.timerTwoLabel.Name = "timerTwoLabel"
        Me.timerTwoLabel.Size = New System.Drawing.Size(136, 16)
        Me.timerTwoLabel.TabIndex = 19
        Me.timerTwoLabel.Text = "Timer 2:"
        '
        'timerThreeDisplay
        '
        Me.timerThreeDisplay.Location = New System.Drawing.Point(136, 152)
        Me.timerThreeDisplay.Name = "timerThreeDisplay"
        Me.timerThreeDisplay.Size = New System.Drawing.Size(136, 16)
        Me.timerThreeDisplay.TabIndex = 22
        '
        'timerThreeLabel
        '
        Me.timerThreeLabel.Location = New System.Drawing.Point(8, 152)
        Me.timerThreeLabel.Name = "timerThreeLabel"
        Me.timerThreeLabel.Size = New System.Drawing.Size(136, 16)
        Me.timerThreeLabel.TabIndex = 21
        Me.timerThreeLabel.Text = "Timer 3:"
        '
        'counterZeroDisplay
        '
        Me.counterZeroDisplay.Location = New System.Drawing.Point(136, 176)
        Me.counterZeroDisplay.Name = "counterZeroDisplay"
        Me.counterZeroDisplay.Size = New System.Drawing.Size(136, 16)
        Me.counterZeroDisplay.TabIndex = 24
        '
        'counterZeroLabel
        '
        Me.counterZeroLabel.Location = New System.Drawing.Point(8, 176)
        Me.counterZeroLabel.Name = "counterZeroLabel"
        Me.counterZeroLabel.Size = New System.Drawing.Size(136, 16)
        Me.counterZeroLabel.TabIndex = 23
        Me.counterZeroLabel.Text = "Counter 0:"
        '
        'U3_EFunctions
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 332)
        Me.Controls.Add(Me.counterZeroDisplay)
        Me.Controls.Add(Me.counterZeroLabel)
        Me.Controls.Add(Me.timerThreeDisplay)
        Me.Controls.Add(Me.timerThreeLabel)
        Me.Controls.Add(Me.timerTwoDisplay)
        Me.Controls.Add(Me.timerTwoLabel)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.dutyCycleOneDisplay)
        Me.Controls.Add(Me.lowClicksOneDisplay)
        Me.Controls.Add(Me.highClicksOneDisplay)
        Me.Controls.Add(Me.counterOneDisplay)
        Me.Controls.Add(Me.timerOneDisplay)
        Me.Controls.Add(Me.fioThreeDisplay)
        Me.Controls.Add(Me.fioTwoDisplay)
        Me.Controls.Add(Me.dacZeroDisplay)
        Me.Controls.Add(Me.ainThreeDisplay)
        Me.Controls.Add(Me.dutyCycleOneLabel)
        Me.Controls.Add(Me.lowClicksOneLabel)
        Me.Controls.Add(Me.highClicksOneLabel)
        Me.Controls.Add(Me.counterOneLabel)
        Me.Controls.Add(Me.timerOneLabel)
        Me.Controls.Add(Me.fioThreeLabel)
        Me.Controls.Add(Me.fioTwoLabel)
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
        Dim intValue As Integer
        intValue = 0
        Dim range As LJUD.RANGES
        Dim intResolution As Integer
        Dim intBinary As Integer
        Dim aIntEnableTimers(6) As Integer
        Dim aIntEnableCounters(2) As Integer
        Dim intTimerClockBaseIndex As Integer
        Dim intTimerClockDivisor As Integer
        Dim aIntTimerModes(6) As Integer
        Dim aDblTimerValues(6) As Double
        Dim aIntReadTimers(6) As Integer
        Dim aIntUpdateResetTimers(6) As Integer
        Dim aIntReadCounters(2) As Integer
        Dim aIntResetCounters(2) As Integer
        Dim aDblCounterValues(2) As Double
        aDblCounterValues(0) = 0
        aDblCounterValues(1) = 1
        Dim highTime, lowTime, dutyCycle As Double
        Dim ue9 As UE9

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Try
            'Take a measurement from AIN3.
            range = LJUD.RANGES.BIP5V
            Dim Resolution As Integer
            Resolution = 17
            Dim Binary As Integer
            Binary = 0
            LJUD.eAIN(ue9.ljhandle, 3, 0, dblValue, range, intResolution, 0, 0)
            ainThreeDisplay.Text = dblValue.ToString("0.###")

            'Set DAC0 to 3.0 volts.
            dblValue = 3
            intBinary = 0
            LJUD.eDAC(ue9.ljhandle, 0, dblValue, intBinary, 0, 0)
            dacZeroDisplay.Text = dblValue.ToString("0.###")

            'Read state of FIO2.
            LJUD.eDI(ue9.ljhandle, 2, intValue)
            fioTwoDisplay.Text = intValue.ToString("0.###")

            'Set the state of FIO3.
            Dim Value As Integer
            Value = 0
            LJUD.eDO(ue9.ljhandle, 3, intValue)
            fioThreeDisplay.Text = intValue.ToString("0.###")

            'Timers and Counters example.
            'First, a call to eTCConfig.  Fill the arrays with the desired values, then make the call.
            intTimerClockBaseIndex = LJUD.TIMERCLOCKS.KHZ750   'Choose 750 kHz base clock.
            Dim TimerClockDivisor As Integer
            TimerClockDivisor = 3
            aIntEnableTimers(0) = 1 'Enable Timer0 (uses FIO0).
            aIntEnableTimers(1) = 1 'Enable Timer1 (uses FIO1).
            aIntEnableTimers(2) = 1 'Enable Timer2 (uses FIO2).
            aIntEnableTimers(3) = 1 'Enable Timer3 (uses FIO3).
            aIntEnableTimers(4) = 0 'Disable Timer4.
            aIntEnableTimers(5) = 0 'Disable Timer5.
            aIntTimerModes(0) = LJUD.TIMERMODE.PWM8  'Timer0 is 8-bit PWM output.  Frequency is 250k/256 = 977 Hz.
            aIntTimerModes(1) = LJUD.TIMERMODE.DUTYCYCLE  'Timer1 is duty cyle input.
            aIntTimerModes(2) = LJUD.TIMERMODE.FIRMCOUNTER  'Timer2 is firmware counter input.
            aIntTimerModes(3) = LJUD.TIMERMODE.RISINGEDGES16  'Timer3 is 16-bit period measurement.
            aIntTimerModes(4) = 0 'Timer4 not enabled.
            aIntTimerModes(5) = 0 'Timer5 not enabled.
            aDblTimerValues(0) = 16384 'Set PWM8 duty-cycle to 75%.
            aDblTimerValues(1) = 0
            aDblTimerValues(2) = 0
            aDblTimerValues(3) = 0
            aDblTimerValues(4) = 0
            aDblTimerValues(5) = 0
            aIntEnableCounters(0) = 1 'Enable Counter0 (uses FIO4).
            aIntEnableCounters(1) = 1 'Enable Counter1 (uses FIO5).
            LJUD.eTCConfig(ue9.ljhandle, aIntEnableTimers, aIntEnableCounters, 0, intTimerClockBaseIndex, intTimerClockDivisor, aIntTimerModes, aDblTimerValues, 0, 0)
            Console.Out.WriteLine("Timers and Counters enabled.\n")

            Threading.Thread.Sleep(1000) 'Wait 1 second.

            'Now, a call to eTCValues.
            aIntReadTimers(0) = 0 'Don't read Timer0 (output timer).
            aIntReadTimers(1) = 1 'Read Timer1
            aIntReadTimers(2) = 1 'Read Timer2
            aIntReadTimers(3) = 1 'Read Timer3
            aIntReadTimers(4) = 0 'Timer4 not enabled.
            aIntReadTimers(5) = 0 'Timer5 not enabled.
            aIntUpdateResetTimers(0) = 1 'Update Timer0
            aIntUpdateResetTimers(1) = 1 'Reset Timer1
            aIntUpdateResetTimers(2) = 1 'Reset Timer2
            aIntUpdateResetTimers(3) = 1 'Reset Timer3
            aIntUpdateResetTimers(4) = 0 'Timer4 not enabled.
            aIntUpdateResetTimers(5) = 0 'Timer5 not enabled.
            aIntReadCounters(0) = 1 'Read Counter0
            aIntReadCounters(1) = 1 'Read Counter1
            aIntResetCounters(0) = 1 'Reset Counter0.
            aIntResetCounters(1) = 1 'Reset Counter1.
            aDblTimerValues(0) = 32768 'Change Timer0 duty-cycle to 50%.
            aDblTimerValues(1) = 0
            aDblTimerValues(2) = 0
            aDblTimerValues(3) = 0
            aDblTimerValues(4) = 0
            aDblTimerValues(5) = 0
            LJUD.eTCValues(ue9.ljhandle, aIntReadTimers, aIntUpdateResetTimers, aIntReadCounters, aIntResetCounters, aDblTimerValues, aDblCounterValues, 0, 0)
            timerOneDisplay.Text = aDblTimerValues(1).ToString("0.###")
            timerTwoDisplay.Text = aDblTimerValues(2).ToString("0.###")
            timerThreeDisplay.Text = aDblTimerValues(3).ToString("0.###")
            counterZeroDisplay.Text = aDblCounterValues(0).ToString("0.###")
            counterOneDisplay.Text = aDblCounterValues(1).ToString("0.###")

            'Convert Timer1 value to duty-cycle percentage.
            'High time is LSW
            highTime = ((aDblTimerValues(1)) Mod (65536))
            'Low time is MSW
            lowTime = ((aDblTimerValues(1)) / (65536))
            'Calculate the duty cycle percentage.
            dutyCycle = 100
            highClicksOneDisplay.Text = highTime.ToString("0.###")
            lowClicksOneDisplay.Text = lowTime.ToString("0.###")
            dutyCycleOneDisplay.Text = dutyCycle.ToString("0.###")


            'Disable all timers and counters.
            aIntEnableTimers(0) = 0
            aIntEnableTimers(1) = 0
            aIntEnableTimers(2) = 0
            aIntEnableTimers(3) = 0
            aIntEnableTimers(4) = 0
            aIntEnableTimers(5) = 0
            aIntEnableCounters(0) = 0
            aIntEnableCounters(1) = 0
            LJUD.eTCConfig(ue9.ljhandle, aIntEnableTimers, aIntEnableCounters, 0, intTimerClockBaseIndex, intTimerClockDivisor, aIntTimerModes, aDblTimerValues, 0, 0)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try
    End Sub

End Class
