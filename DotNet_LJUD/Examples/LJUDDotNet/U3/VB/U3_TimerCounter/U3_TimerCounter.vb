'---------------------------------------------------------------------------
'
'  U3_TimerCounter.vb
' 
'  Basic U3 example does a PWM output and a counter input, using AddGoGet method.
'
'  support@labjack.com
'  June 15, 2009
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
    Friend WithEvents origLabel As System.Windows.Forms.Label
    Friend WithEvents origDisplay As System.Windows.Forms.Label
    Friend WithEvents resultLabel As System.Windows.Forms.Label
    Friend WithEvents resultDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.origLabel = New System.Windows.Forms.Label
        Me.origDisplay = New System.Windows.Forms.Label
        Me.resultLabel = New System.Windows.Forms.Label
        Me.resultDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'origLabel
        '
        Me.origLabel.Location = New System.Drawing.Point(0, 8)
        Me.origLabel.Name = "origLabel"
        Me.origLabel.Size = New System.Drawing.Size(128, 16)
        Me.origLabel.TabIndex = 0
        Me.origLabel.Text = "Original Counter Value:"
        '
        'origDisplay
        '
        Me.origDisplay.Location = New System.Drawing.Point(136, 8)
        Me.origDisplay.Name = "origDisplay"
        Me.origDisplay.Size = New System.Drawing.Size(128, 16)
        Me.origDisplay.TabIndex = 1
        '
        'resultLabel
        '
        Me.resultLabel.Location = New System.Drawing.Point(0, 32)
        Me.resultLabel.Name = "resultLabel"
        Me.resultLabel.Size = New System.Drawing.Size(128, 16)
        Me.resultLabel.TabIndex = 2
        Me.resultLabel.Text = "Result Counter Value:"
        '
        'resultDisplay
        '
        Me.resultDisplay.Location = New System.Drawing.Point(136, 32)
        Me.resultDisplay.Name = "resultDisplay"
        Me.resultDisplay.Size = New System.Drawing.Size(128, 16)
        Me.resultDisplay.TabIndex = 3
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 56)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(240, 24)
        Me.goButton.TabIndex = 4
        Me.goButton.Text = "GO!"
        '
        'U3_TimerCounter
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(264, 84)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.resultDisplay)
        Me.Controls.Add(Me.resultLabel)
        Me.Controls.Add(Me.origDisplay)
        Me.Controls.Add(Me.origLabel)
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
        Dim u3 As U3

        ' Variables to satisfy certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0
        Dim dummyDoubleArray(1) As Double

        Try

            'Open the first found LabJack U3.
            u3 = New U3(LJUD.CONNECTION.USB, "0", True)

            'Start by using the pin_configuration_reset IOType so that all
            'pin assignments are in the factory default condition.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)


            'First requests to configure the timer and counter.  These will be
            'done with and add/go/get block.

            'Set the timer/counter pin offset to 4, which will put the first
            'timer/counter on FIO4.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 4, 0, 0)

            'Use the 48 MHz timer clock base with divider.  Since we are using clock with divisor
            'support, Counter0 is not available.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.MHZ48_DIV, 0, 0)
            'LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.MHZ24_DIV, 0, 0)  'Use this line instead for hardware rev 1.20.

            'Set the divisor to 48 so the actual timer clock is 1 MHz.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 48, 0, 0)
            'LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 24, 0, 0)  'Use this line instead for hardware rev 1.20.

            'Enable 1 timer.  It will use FIO4.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 1, 0, 0)

            'Configure Timer0 as 8-bit PWM.  Frequency will be 1M/256 = 3906 Hz.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.PWM8, 0, 0)

            'Set the PWM duty cycle to 50%.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0)

            'Enable Counter1.  It will use FIO5 since 1 timer is enabled.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 1, 0, 0)

            'Execute the requests.
            LJUD.GoOne(u3.ljhandle)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(u3.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try
        Dim finished As Boolean
        finished = False
        While Not finished
            Try
                LJUD.GetNextResult(u3.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    finished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        Try
            'Wait 1 second.
            Threading.Thread.Sleep(1000)

            'Request a read from the counter.
            LJUD.eGet(u3.ljhandle, LJUD.IO.GET_COUNTER, 1, dblValue, dummyDoubleArray)

            'This should read roughly 4k counts if FIO4 is shorted to FIO5.
            origDisplay.Text = dblValue.ToString("0.0")
            origDisplay.Refresh()

            'Wait 1 second.
            Threading.Thread.Sleep(1000)

            'Request a read from the counter.
            LJUD.eGet(u3.ljhandle, LJUD.IO.GET_COUNTER, 1, dblValue, dummyDoubleArray)

            'This should read about 3906 counts more than the previous read.
            resultDisplay.Text = dblValue.ToString("0.0")

            'Reset all pin assignments to factory default condition.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)

            'The PWM output sets FIO4 to output, so we do a read here to set
            'it to input.
            LJUD.eGet(u3.ljhandle, LJUD.IO.GET_DIGITAL_BIT, 4, dblValue, dummyDoubleArray)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try
    End Sub
End Class
