'---------------------------------------------------------------------------
'
'  U3_AllIO.vb
' 
'	Demonstrates using the add/go/get method to efficiently write and read
'	virtually all analog and digital I/O on the LabJack U3.
'	Records the time for 1000 iterations and divides by 1000, to allow
'	verification of the basic command/response communication times of the
'	LabJack U3 as documented in Section 3.1 of the U3 User's Guide.
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

Public Class Form1
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
    Friend WithEvents iterationTimeLabel As System.Windows.Forms.Label
    Friend WithEvents digitalInputLabel As System.Windows.Forms.Label
    Friend WithEvents iterationTimeDisplay As System.Windows.Forms.Label
    Friend WithEvents digitalInputDisplay As System.Windows.Forms.Label
    Friend WithEvents ainGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents ainListBox As System.Windows.Forms.ListBox
    Friend WithEvents runButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.iterationTimeLabel = New System.Windows.Forms.Label
        Me.digitalInputLabel = New System.Windows.Forms.Label
        Me.iterationTimeDisplay = New System.Windows.Forms.Label
        Me.digitalInputDisplay = New System.Windows.Forms.Label
        Me.ainGroupBox = New System.Windows.Forms.GroupBox
        Me.ainListBox = New System.Windows.Forms.ListBox
        Me.runButton = New System.Windows.Forms.Button
        Me.ainGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'iterationTimeLabel
        '
        Me.iterationTimeLabel.Location = New System.Drawing.Point(8, 16)
        Me.iterationTimeLabel.Name = "iterationTimeLabel"
        Me.iterationTimeLabel.Size = New System.Drawing.Size(168, 16)
        Me.iterationTimeLabel.TabIndex = 0
        Me.iterationTimeLabel.Text = "Millesecounds per iteration: "
        '
        'digitalInputLabel
        '
        Me.digitalInputLabel.Location = New System.Drawing.Point(8, 48)
        Me.digitalInputLabel.Name = "digitalInputLabel"
        Me.digitalInputLabel.Size = New System.Drawing.Size(168, 16)
        Me.digitalInputLabel.TabIndex = 1
        Me.digitalInputLabel.Text = "Digital input:"
        '
        'iterationTimeDisplay
        '
        Me.iterationTimeDisplay.Location = New System.Drawing.Point(176, 16)
        Me.iterationTimeDisplay.Name = "iterationTimeDisplay"
        Me.iterationTimeDisplay.Size = New System.Drawing.Size(168, 16)
        Me.iterationTimeDisplay.TabIndex = 2
        '
        'digitalInputDisplay
        '
        Me.digitalInputDisplay.Location = New System.Drawing.Point(176, 48)
        Me.digitalInputDisplay.Name = "digitalInputDisplay"
        Me.digitalInputDisplay.Size = New System.Drawing.Size(168, 16)
        Me.digitalInputDisplay.TabIndex = 3
        '
        'ainGroupBox
        '
        Me.ainGroupBox.Controls.Add(Me.ainListBox)
        Me.ainGroupBox.Location = New System.Drawing.Point(8, 72)
        Me.ainGroupBox.Name = "ainGroupBox"
        Me.ainGroupBox.Size = New System.Drawing.Size(336, 184)
        Me.ainGroupBox.TabIndex = 4
        Me.ainGroupBox.TabStop = False
        Me.ainGroupBox.Text = "AIN readings from last iteration"
        '
        'ainListBox
        '
        Me.ainListBox.Location = New System.Drawing.Point(16, 16)
        Me.ainListBox.Name = "ainListBox"
        Me.ainListBox.Size = New System.Drawing.Size(320, 160)
        Me.ainListBox.TabIndex = 0
        '
        'runButton
        '
        Me.runButton.Location = New System.Drawing.Point(80, 264)
        Me.runButton.Name = "runButton"
        Me.runButton.Size = New System.Drawing.Size(192, 24)
        Me.runButton.TabIndex = 5
        Me.runButton.Text = "Run"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(352, 292)
        Me.Controls.Add(Me.runButton)
        Me.Controls.Add(Me.ainGroupBox)
        Me.Controls.Add(Me.digitalInputDisplay)
        Me.Controls.Add(Me.iterationTimeDisplay)
        Me.Controls.Add(Me.digitalInputLabel)
        Me.Controls.Add(Me.iterationTimeLabel)
        Me.Name = "Form1"
        Me.Text = "U3_AllIO"
        Me.ainGroupBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub runButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles runButton.Click

        Dim ioType As LJUD.IO
        Dim channel As LJUD.CHANNEL
        Dim dblVal As Double
        Dim valueDIPort As Double
        Dim intVal As Integer
        Dim val As Double
        Dim ValueAIN(16) As Double
        Dim time As Long
        Dim numIterations As Long
        Dim numChannels As Integer
        Dim quickSample As Long
        Dim longSettling As Long
        Dim u3 As U3

        numIterations = 100
        numChannels = 16 'Number of AIN channels, 0-16.
        longSettling = 1 'Set to TRUE for extra AIN settling time.
        quickSample = 0 'Set to TRUE for quick AIN sampling.

        ' Clear out the ain readings
        ainListBox.Items.Clear()

        Try
            u3 = New U3(LJUD.CONNECTION.USB, "0", True) ' Connection through USB

            'Start by using the pin_configuration_reset IOType so that all
            'pin assignments are in the factory default condition.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)

            'Configure quickSample.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, quickSample, 0)

            'Configure longSettling.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_SETTLING_TIME, longSettling, 0)

            'Configure the necessary lines as analog.
            Dim v As Double
            v = Math.Pow(2, numChannels) - 1
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_ANALOG_ENABLE_PORT, 0, v, numChannels)

            'Now an Add/Go/Get block to configure the timers and counters.  These
            'are configured on EIO0-EIO3, so if more than 8 analog inputs are
            'enabled then the analog inputs use these lines.

            If numChannels <= 8 Then

                'Set the timer/counter pin offset to 8, which will put the first
                'timer/counter on EIO0.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 8, 0, 0)

                'Use the default clock source.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.MHZ48, 0, 0)

                'Enable 2 timers.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0)

                'Configure Timer0 as 8-bit PWM.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.PWM8, 0, 0)

                'Set the PWM duty cycle to 50%.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0)

                'Configure Timer1 as 8-bit PWM.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, LJUD.TIMERMODE.PWM8, 0, 0)

                'Set the PWM duty cycle to 50%.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 1, 32768, 0, 0)

                'Enable Counter0.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0, 0)

                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 1, 0, 0)

                'Execute the requests.
                LJUD.GoOne(u3.ljhandle)
            End If

            'Now add requests that will be processed every iteration of the loop.

            'Add analog input requests.
            For j As Integer = 0 To numChannels - 1
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_AIN, j, 0, 0, 0)
            Next j

            'Set DAC0 to 2.5 volts.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_DAC, 0, 2.5, 0, 0)

            'Read CIO digital lines.
            LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_DIGITAL_PORT, 16, 0, 4, 0)

            'Only do the timer/counter stuff if there are less than 8 analog inputs.
            If numChannels <= 8 Then
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_COUNTER, 0, 0, 0, 0)

                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_COUNTER, 1, 0, 0, 0)

                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_TIMER, 0, 0, 0, 0)

                LJUD.AddRequest(u3.ljhandle, LJUD.IO.GET_TIMER, 1, 0, 0, 0)

                'Set the PWM duty cycle to 50%.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0)

                'Set the PWM duty cycle to 50%.
                LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 1, 32768, 0, 0)
            End If

            time = Environment.TickCount

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        For i As Integer = 0 To numIterations - 1
            Try
                'Execute the requests.
                LJUD.GoOne(u3.ljhandle)

                'Get all the results.  The input measurement results are stored.  All other
                'results are for configuration or output requests so we are just checking
                'whether there was an error.
                LJUD.GetFirstResult(u3.ljhandle, ioType, channel, val, intVal, dblVal)

            Catch ex As LabJackUDException
                showErrorMessage(ex)

            End Try

            ' Get results until there is no more data available
            Dim isFinished As Boolean
            isFinished = False
            While Not isFinished
                Select Case ioType
                    Case LJUD.IO.GET_AIN
                        ValueAIN(channel) = val
                    Case LJUD.IO.GET_DIGITAL_PORT
                        valueDIPort = val
                End Select

                Try
                    LJUD.GetNextResult(u3.ljhandle, ioType, channel, val, intVal, dblVal)
                Catch ex As LabJackUDException
                    If ex.LJUDError = UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE Then
                        isFinished = True
                    Else
                        showErrorMessage(ex)
                    End If
                End Try
            End While

        Next i

        time = Environment.TickCount - time

        iterationTimeDisplay.Text = time / numIterations
        digitalInputDisplay.Text = valueDIPort
        For i As Integer = 0 To numChannels - 1
            ainListBox.Items.Add(ValueAIN(i))
        Next i
        runButton.Text = "Go again!"
    End Sub
End Class
