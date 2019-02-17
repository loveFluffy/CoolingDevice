'---------------------------------------------------------------------------
'
'  UE9_AllIO.vb
' 
'	Demonstrates using the add/go/get method to efficiently write and read
'	virtually all analog and digital I/O on the LabJack UE9.
'	Records the time for 1000 iterations and divides by 1000, to allow
'	verification of the basic command/response communication times of the
'	LabJack UE9 as documented in Section 3.1 of the UE9 User's Guide.
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

Public Class Form1
    Inherits System.Windows.Forms.Form
    Dim running As Boolean

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
    Friend WithEvents iterationLabel As System.Windows.Forms.Label
    Friend WithEvents iterationDisplay As System.Windows.Forms.Label
    Friend WithEvents numberReadLabel As System.Windows.Forms.Label
    Friend WithEvents numberReadDisplay As System.Windows.Forms.Label
    Friend WithEvents scanLabel As System.Windows.Forms.Label
    Friend WithEvents scanDisplay As System.Windows.Forms.Label
    Friend WithEvents commBacklogLabel As System.Windows.Forms.Label
    Friend WithEvents backlogDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.iterationLabel = New System.Windows.Forms.Label
        Me.iterationDisplay = New System.Windows.Forms.Label
        Me.numberReadLabel = New System.Windows.Forms.Label
        Me.numberReadDisplay = New System.Windows.Forms.Label
        Me.scanLabel = New System.Windows.Forms.Label
        Me.scanDisplay = New System.Windows.Forms.Label
        Me.commBacklogLabel = New System.Windows.Forms.Label
        Me.backlogDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'iterationLabel
        '
        Me.iterationLabel.Location = New System.Drawing.Point(0, 8)
        Me.iterationLabel.Name = "iterationLabel"
        Me.iterationLabel.Size = New System.Drawing.Size(120, 16)
        Me.iterationLabel.TabIndex = 0
        Me.iterationLabel.Text = "Iteration:"
        '
        'iterationDisplay
        '
        Me.iterationDisplay.Location = New System.Drawing.Point(128, 8)
        Me.iterationDisplay.Name = "iterationDisplay"
        Me.iterationDisplay.Size = New System.Drawing.Size(144, 16)
        Me.iterationDisplay.TabIndex = 1
        '
        'numberReadLabel
        '
        Me.numberReadLabel.Location = New System.Drawing.Point(0, 32)
        Me.numberReadLabel.Name = "numberReadLabel"
        Me.numberReadLabel.Size = New System.Drawing.Size(120, 16)
        Me.numberReadLabel.TabIndex = 2
        Me.numberReadLabel.Text = "Number read:"
        '
        'numberReadDisplay
        '
        Me.numberReadDisplay.Location = New System.Drawing.Point(128, 32)
        Me.numberReadDisplay.Name = "numberReadDisplay"
        Me.numberReadDisplay.Size = New System.Drawing.Size(144, 16)
        Me.numberReadDisplay.TabIndex = 3
        '
        'scanLabel
        '
        Me.scanLabel.Location = New System.Drawing.Point(0, 56)
        Me.scanLabel.Name = "scanLabel"
        Me.scanLabel.Size = New System.Drawing.Size(120, 16)
        Me.scanLabel.TabIndex = 4
        Me.scanLabel.Text = "First scan:"
        '
        'scanDisplay
        '
        Me.scanDisplay.Location = New System.Drawing.Point(128, 56)
        Me.scanDisplay.Name = "scanDisplay"
        Me.scanDisplay.Size = New System.Drawing.Size(144, 64)
        Me.scanDisplay.TabIndex = 5
        '
        'commBacklogLabel
        '
        Me.commBacklogLabel.Location = New System.Drawing.Point(0, 128)
        Me.commBacklogLabel.Name = "commBacklogLabel"
        Me.commBacklogLabel.Size = New System.Drawing.Size(120, 16)
        Me.commBacklogLabel.TabIndex = 6
        Me.commBacklogLabel.Text = "Comm Backlog:"
        '
        'backlogDisplay
        '
        Me.backlogDisplay.Location = New System.Drawing.Point(120, 128)
        Me.backlogDisplay.Name = "backlogDisplay"
        Me.backlogDisplay.Size = New System.Drawing.Size(152, 16)
        Me.backlogDisplay.TabIndex = 7
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(16, 152)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(256, 24)
        Me.goButton.TabIndex = 8
        Me.goButton.Text = "Run"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 180)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.backlogDisplay)
        Me.Controls.Add(Me.commBacklogLabel)
        Me.Controls.Add(Me.scanDisplay)
        Me.Controls.Add(Me.scanLabel)
        Me.Controls.Add(Me.numberReadDisplay)
        Me.Controls.Add(Me.numberReadLabel)
        Me.Controls.Add(Me.iterationDisplay)
        Me.Controls.Add(Me.iterationLabel)
        Me.Name = "Form1"
        Me.Text = "ExternalTriggerStream"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub runButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        If Not running Then
            goButton.Text = "Stop"
            Dim trd As Threading.Thread
            trd = New Threading.Thread(AddressOf run)
            trd.IsBackground = True
            running = True
            trd.Start()
        Else
            goButton.Text = "Start Again"
            running = False
        End If
    End Sub

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub run()
        Dim i As Long
        i = 0
        Dim ioType As LJUD.IO
        ioType = 0
        Dim channel As LJUD.CHANNEL
        channel = 0
        Dim dblValue, dblCommBacklog As Double
        dblValue = 0

        ' Dummy variables to satisfy certain method signatures
        Dim dummyDouble As Double
        dummyDouble = 0
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDoubleArray(1) As Double
        dummyDoubleArray(0) = 0.0

        'The actual scan rate is determined by the external clock, but we need
        'an idea of how fast the scan rate will be so that we can make
        'the buffers big enough.  Also, the driver needs to have an idea of the
        'expected scan rate to help it decide how big of packets to transfer.
        Dim scanRate As Double
        scanRate = 1000
        Dim delayms As Integer
        delayms = 1000
        Dim numScans As Double
        numScans = 2000
        Dim numScansRequested As Double
        Dim adblData(12000) As Double
        Dim ue9 As UE9

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Try
            'Make sure the UE9 is not streaming.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, 0, dummyDouble, 0)
        Catch ex As LabJackUDException
            ' If the error indicates that the stream could not be stopped it is because the stream has not started yet and can be ignored 
            If Not ex.LJUDError = LJUD.LJUDERROR.UNABLE_TO_STOP_STREAM Then
                showErrorMessage(ex)
            End If
        End Try

        Try
            'Disable all timers and counters to put everything in a known initial state.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)


            'First we will configure Timer0 as system timer low and configure Timer1 to
            'output a 1000 Hz square wave.

            'Use the fixed 750kHz timer clock source.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.KHZ750, 0, 0)

            'Set the divisor to 3 so the actual timer clock is 250 kHz.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, 3, 0, 0)

            'Enable 2 timers.  They will use FIO0-FIO1.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0)

            'Configure Timer0 as system timer low.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.SYSTIMERLOW, 0, 0)

            'Configure Timer1 as frequency output.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, LJUD.TIMERMODE.FREQOUT, 0, 0)

            'Set the frequency output on Timer1 to 1000 Hz (250000/(2*125) = 1000).
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_VALUE, 1, 125, 0, 0)

            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try


        'Get all the results just to check for errors.
        Dim isFinished As Boolean
        isFinished = False
        Try
            LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

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
            'Configure the stream:
            'Configure resolution for all analog inputs.  Since the test external clock
            'is at 1000 Hz, and we are scanning 6 channels, we will have a
            'sample rate of 6000 samples/second.  That means the maximum resolution
            'we could use is 13-bit.  We will use 12-bit in this example.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 12, 0, 0)
            'Configure the analog input range on channel 0 for bipolar +-5 volts.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 0, LJUD.RANGES.BIP5V, 0, 0)
            'Configure the analog input range on channel 1 for bipolar +-5 volts.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 1, LJUD.RANGES.BIP5V, 0, 0)
            'Give the driver a 5 second buffer (scanRate * 6 channels * 5 seconds).
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_BUFFER_SIZE, scanRate * 6 * 5, 0, 0)
            'Configure reads to retrieve whatever data is available without waiting (wait mode LJ_swNONE).
            'See comments below to change this program to use LJ_swSLEEP mode.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_WAIT_MODE, LJUD.STREAMWAITMODES.NONE, 0, 0)
            'Configure for external triggering.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_EXTERNAL_TRIGGER, 1, 0, 0)
            'Define the scan list.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.CLEAR_STREAM_CHANNELS, 0, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 0, 0, 0, 0) 'AIN0
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 1, 0, 0, 0) 'AIN1
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 193, 0, 0, 0) 'EIO_FIO
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 194, 0, 0, 0) 'MIO_CIO
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 200, 0, 0, 0) 'Timer0 LSW
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 224, 0, 0, 0) 'Timer0 MSW

            'Execute the list of requests.
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
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

        Try
            'Start the stream.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.START_STREAM, 0, dblValue, 0)

            'Read data
            While running
                'Since we are using wait mode LJUD.STREAMWAITMODES.NONE, we will wait a little, then
                'read however much data is available.  Thus this delay will control how
                'fast the program loops and how much data is read each loop.  An
                'alternative common method is to use wait mode LJUD.STREAMWAITMODES.SLEEP where the
                'stream read waits for a certain number of scans.  In such a case
                'you would not have a delay here, since the stream read will actually
                'control how fast the program loops.
                '
                'To change this program to use sleep mode,
                '	-change numScans to the actual number of scans desired per read,
                '	-change wait mode addrequest value to LJ_swSLEEP,
                '	-comment out the following Sleep command.

                Threading.Thread.Sleep(delayms) 'Remove if using LJUD.STREAMWAITMODES.SLEEP

                'init array so we can easily tell if it has changed
                For k As Integer = 0 To numScans * 2 - 1
                    adblData(k) = 9999.0
                Next k

                'Read the data.  We will request twice the number we expect, to
                'make sure we get everything that is available.
                'Note that the array we pass must be sized to hold enough SAMPLES, and
                'the Value we pass specifies the number of SCANS to read.
                numScansRequested = numScans
                LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_STREAM_DATA, LJUD.CHANNEL.ALL_CHANNELS, numScansRequested, adblData)
                'This displays the number of scans that were actually read.
                iterationDisplay.Text = i
                numberReadDisplay.Text = numScansRequested
                'This displays just the first scan.
                scanDisplay.Text = adblData(0).ToString("0.###") + ", " + adblData(1).ToString("0.###") + ", " + adblData(2).ToString("0.###") + ", " + adblData(3).ToString("0.###") + ", " + adblData(4).ToString("0.###") + ", " + adblData(5).ToString("0.###")
                'Retrieve the current Comm backlog.  The UD driver retrieves stream data from
                'the UE9 in the background, but if the computer is too slow for some reason
                'the driver might not be able to read the data as fast as the UE9 is
                'acquiring it, and thus there will be data left over in the UE9 buffer.
                LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_COMM, dblCommBacklog, 0)
                backlogDisplay.Text = dblCommBacklog.ToString("0.###")
                i = i + 1
            End While


            'Stop the stream
            LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, 0, dummyDouble, 0)

            'Disable the timers.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

    End Sub
End Class
