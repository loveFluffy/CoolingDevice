'---------------------------------------------------------------------------
'
'  UE9_TimerCounter.vb
' 
'  Demonstrates a few of different timer/counter features.
'
'  support@labjack.com
'  June 16, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a U3.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U3_TimerCounter
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
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents counterLabel As System.Windows.Forms.Label
    Friend WithEvents counterDisplay As System.Windows.Forms.Label
    Friend WithEvents period32Label As System.Windows.Forms.Label
    Friend WithEvents period32Display As System.Windows.Forms.Label
    Friend WithEvents period16Label As System.Windows.Forms.Label
    Friend WithEvents period16Display As System.Windows.Forms.Label
    Friend WithEvents highClicksLabel As System.Windows.Forms.Label
    Friend WithEvents highClicksDisplay As System.Windows.Forms.Label
    Friend WithEvents lowClicksLabel As System.Windows.Forms.Label
    Friend WithEvents lowClicksDisplay As System.Windows.Forms.Label
    Friend WithEvents dutyCycleLabel As System.Windows.Forms.Label
    Friend WithEvents dutyCycleDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.goButton = New System.Windows.Forms.Button
        Me.counterLabel = New System.Windows.Forms.Label
        Me.counterDisplay = New System.Windows.Forms.Label
        Me.period32Label = New System.Windows.Forms.Label
        Me.period32Display = New System.Windows.Forms.Label
        Me.period16Label = New System.Windows.Forms.Label
        Me.period16Display = New System.Windows.Forms.Label
        Me.highClicksLabel = New System.Windows.Forms.Label
        Me.highClicksDisplay = New System.Windows.Forms.Label
        Me.lowClicksLabel = New System.Windows.Forms.Label
        Me.lowClicksDisplay = New System.Windows.Forms.Label
        Me.dutyCycleLabel = New System.Windows.Forms.Label
        Me.dutyCycleDisplay = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 168)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(240, 24)
        Me.goButton.TabIndex = 4
        Me.goButton.Text = "GO!"
        '
        'counterLabel
        '
        Me.counterLabel.Location = New System.Drawing.Point(0, 8)
        Me.counterLabel.Name = "counterLabel"
        Me.counterLabel.Size = New System.Drawing.Size(112, 16)
        Me.counterLabel.TabIndex = 5
        Me.counterLabel.Text = "Counter:"
        '
        'counterDisplay
        '
        Me.counterDisplay.Location = New System.Drawing.Point(120, 8)
        Me.counterDisplay.Name = "counterDisplay"
        Me.counterDisplay.Size = New System.Drawing.Size(136, 16)
        Me.counterDisplay.TabIndex = 6
        '
        'period32Label
        '
        Me.period32Label.Location = New System.Drawing.Point(0, 32)
        Me.period32Label.Name = "period32Label"
        Me.period32Label.Size = New System.Drawing.Size(112, 16)
        Me.period32Label.TabIndex = 7
        Me.period32Label.Text = "Period32:"
        '
        'period32Display
        '
        Me.period32Display.Location = New System.Drawing.Point(120, 32)
        Me.period32Display.Name = "period32Display"
        Me.period32Display.Size = New System.Drawing.Size(136, 16)
        Me.period32Display.TabIndex = 8
        '
        'period16Label
        '
        Me.period16Label.Location = New System.Drawing.Point(0, 56)
        Me.period16Label.Name = "period16Label"
        Me.period16Label.Size = New System.Drawing.Size(112, 16)
        Me.period16Label.TabIndex = 9
        Me.period16Label.Text = "Period16:"
        '
        'period16Display
        '
        Me.period16Display.Location = New System.Drawing.Point(120, 56)
        Me.period16Display.Name = "period16Display"
        Me.period16Display.Size = New System.Drawing.Size(136, 16)
        Me.period16Display.TabIndex = 10
        '
        'highClicksLabel
        '
        Me.highClicksLabel.Location = New System.Drawing.Point(0, 80)
        Me.highClicksLabel.Name = "highClicksLabel"
        Me.highClicksLabel.Size = New System.Drawing.Size(112, 16)
        Me.highClicksLabel.TabIndex = 11
        Me.highClicksLabel.Text = "High Clicks:"
        '
        'highClicksDisplay
        '
        Me.highClicksDisplay.Location = New System.Drawing.Point(120, 80)
        Me.highClicksDisplay.Name = "highClicksDisplay"
        Me.highClicksDisplay.Size = New System.Drawing.Size(136, 16)
        Me.highClicksDisplay.TabIndex = 12
        '
        'lowClicksLabel
        '
        Me.lowClicksLabel.Location = New System.Drawing.Point(0, 104)
        Me.lowClicksLabel.Name = "lowClicksLabel"
        Me.lowClicksLabel.Size = New System.Drawing.Size(112, 16)
        Me.lowClicksLabel.TabIndex = 13
        Me.lowClicksLabel.Text = "Low Clicks:"
        '
        'lowClicksDisplay
        '
        Me.lowClicksDisplay.Location = New System.Drawing.Point(120, 104)
        Me.lowClicksDisplay.Name = "lowClicksDisplay"
        Me.lowClicksDisplay.Size = New System.Drawing.Size(136, 16)
        Me.lowClicksDisplay.TabIndex = 14
        '
        'dutyCycleLabel
        '
        Me.dutyCycleLabel.Location = New System.Drawing.Point(0, 128)
        Me.dutyCycleLabel.Name = "dutyCycleLabel"
        Me.dutyCycleLabel.Size = New System.Drawing.Size(112, 16)
        Me.dutyCycleLabel.TabIndex = 15
        Me.dutyCycleLabel.Text = "Duty Cycle:"
        '
        'dutyCycleDisplay
        '
        Me.dutyCycleDisplay.Location = New System.Drawing.Point(120, 128)
        Me.dutyCycleDisplay.Name = "dutyCycleDisplay"
        Me.dutyCycleDisplay.Size = New System.Drawing.Size(136, 16)
        Me.dutyCycleDisplay.TabIndex = 16
        '
        'U3_TimerCounter
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(264, 204)
        Me.Controls.Add(Me.dutyCycleDisplay)
        Me.Controls.Add(Me.dutyCycleLabel)
        Me.Controls.Add(Me.lowClicksDisplay)
        Me.Controls.Add(Me.lowClicksLabel)
        Me.Controls.Add(Me.highClicksDisplay)
        Me.Controls.Add(Me.highClicksLabel)
        Me.Controls.Add(Me.period16Display)
        Me.Controls.Add(Me.period16Label)
        Me.Controls.Add(Me.period32Display)
        Me.Controls.Add(Me.period32Label)
        Me.Controls.Add(Me.counterDisplay)
        Me.Controls.Add(Me.counterLabel)
        Me.Controls.Add(Me.goButton)
        Me.Name = "U3_TimerCounter"
        Me.Text = "U3_TimerCounter"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub goButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        Dim ioType As LJUD.IO
        ioType = 0
        Dim channel As LJUD.CHANNEL
        channel = 0
        Dim dblValue As Double
        dblValue = 0
        Dim highTime, lowTime As Double
        Dim period16, period32 As Double
        period16 = -1
        period32 = -1

        ' Variables that satisfy a method signature
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0
        Dim ue9 As UE9

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        'Disable all timers and counters to put everything in a known initial state.
        'Disable the timer and counter, and the FIO lines will return to digital I/O.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0)
        LJUD.GoOne(ue9.ljhandle)


        'First we will output a square wave and count the number of pulses for about 1 second.
        'Connect a jumper on the UE9 from FIO0 (PWM output) to
        'FIO1 (Counter0 input).

        'Use the fixed 750kHz timer clock source.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.KHZ750, 0, 0)

        'Set the divisor to 3 so the actual timer clock is 250kHz.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0)

        'Enable 1 timer.  It will use FIO0.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 1, 0, 0)

        'Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.PWM8, 0, 0)

        'Set the PWM duty cycle to 50%.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0)

        'Enable Counter0.  It will use FIO1 since 1 timer is enabled.
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0, 0)


        'Execute the requests on a single LabJack.  The driver will use a
        'single low-level TimerCounter command to handle all the requests above.
        LJUD.GoOne(ue9.ljhandle)


        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Dim isFinished As Boolean
        isFinished = False
        While Not isFinished
            Try
                LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If

            End Try
        End While


        Try

            'Wait 1 second.
            Threading.Thread.Sleep(1000)

            'Request a read from the counter.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_COUNTER, 0, dblValue, 0)

            'This should read roughly 977 counts.
            counterDisplay.Text = dblValue

            'Disable the timer and counter, and the FIO lines will return to digital I/O.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)

            'Output a square wave and measure the period.
            'Connect a jumper on the UE9 from FIO0 (PWM8 output) to
            'FIO1 (RISINGEDGES32 input) and FIO2 (RISINGEDGES16).

            'Use the fixed 750kHz timer clock source.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.KHZ750, 0, 0)

            'Set the divisor to 3 so the actual timer clock is 250kHz.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0)

            'Enable 3 timers.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 3, 0, 0)

            'Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.PWM8, 0, 0)

            'Set the PWM duty cycle to 50%.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0)

            'Configure Timer1 as 32-bit period measurement.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, LJUD.TIMERMODE.RISINGEDGES32, 0, 0)

            'Configure Timer2 as 16-bit period measurement.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 2, LJUD.TIMERMODE.RISINGEDGES16, 0, 0)


            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(ue9.ljhandle)

        Catch ex As LabJackUDException
            ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
            If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                isFinished = True
            Else
                showErrorMessage(ex)
            End If
        End Try

        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        isFinished = False
        While Not isFinished
            Try
                LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException

                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        'Wait 1 second.
        Threading.Thread.Sleep(1000)

        'Now read the period measurements from the 2 timers.  We
        'will use the Add/Go/Get method so that both
        'reads are done in a single low-level call.

        'Request a read from Timer1
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.GET_TIMER, 1, 0, 0, 0)

        'Request a read from Timer2
        LJUD.AddRequest(ue9.ljhandle, LJUD.IO.GET_TIMER, 2, 0, 0, 0)

        'Execute the requests on a single LabJack.  The driver will use a
        'single low-level TimerCounter command to handle all the requests above.
        LJUD.GoOne(ue9.ljhandle)

        'Get the results of the two read requests.
        LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        isFinished = False
        While Not isFinished
            If ioType = LJUD.IO.GET_TIMER Then
                Select Case (channel)
                    Case 1
                        period32 = dblValue
                    Case 2
                        period16 = dblValue

                End Select
            End If

            Try
                LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' After encountering a 
                If (ex.LJUDError > ue9.LJUDERROR.MIN_GROUP_ERROR) Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try

        End While

        Try

            'Both period measurements should read about 256.  The timer
            'clock was set to 250 kHz, so each tick equals 4 microseconds, so
            '256 ticks means a period of 1024 microseconds which is a frequency
            'of 977 Hz.
            period32Display.Text = period32
            period16Display.Text = period16

            'Disable the timer and counter, and the FIO lines will return to digital I/O.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)

            'Now we will output a 25% duty-cycle PWM output on Timer0 (FIO0) and measure
            'the duty cycle on Timer1 FIO1.  Requires Control firmware V1.21 or higher.

            'Use the fixed 750kHz timer clock source.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.KHZ750, 0, 0)

            'Set the divisor to 3 so the actual timer clock is 250kHz.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0)

            'Enable 2 timers.  They will use FIO0 and FIO1.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0)

            'Configure Timer0 as 8-bit PWM.  Frequency will be 250k/256 = 977 Hz.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.PWM8, 0, 0)

            'Set the PWM duty cycle to 25%.  The passed value is the low time.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 49152, 0, 0)

            'Configure Timer1 as duty cycle measurement.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, LJUD.TIMERMODE.DUTYCYCLE, 0, 0)


            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabJackUDException

            ' After encountering a 
            If (ex.LJUDError > ue9.LJUDERROR.MIN_GROUP_ERROR) Then
                isFinished = True
            Else
                showErrorMessage(ex)
            End If
        End Try

        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        isFinished = False
        While Not isFinished

            Try
                LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try

        End While

        'Wait a little so we are sure a duty cycle measurement has occured.
        Threading.Thread.Sleep(100)

        Try
            'Request a read from Timer1.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_TIMER, 1, dblValue, 0)

            'High time is LSW
            highTime = ((dblValue) Mod (65536))
            'Low time is MSW
            lowTime = ((dblValue) / (65536))


            highClicksDisplay.Text = highTime
            lowClicksDisplay.Text = lowTime
            dutyCycleDisplay.Text = (100 * highTime / (highTime + lowTime))


            'Disable the timers, and the FIO lines will return to digital I/O.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)


            'The PWM output sets FIO0 to output, so we do a read here to set
            'FIO0 to input.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 0, dblValue, 0)
        Catch ex As LabJackUDException

            ' After encountering a 
            If (ex.LJUDError > ue9.LJUDERROR.MIN_GROUP_ERROR) Then
                isFinished = True
            Else
                showErrorMessage(ex)
            End If
        End Try

    End Sub
End Class
