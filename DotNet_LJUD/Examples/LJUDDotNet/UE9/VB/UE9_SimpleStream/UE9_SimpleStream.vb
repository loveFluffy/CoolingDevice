'---------------------------------------------------------------------------
'
'  UE9_SimpleStream.vb
' 
'  2-channel stream on the UE9.
'
'  support@labjack.com
'  June 15, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a UE9.
Option Explicit On 

Imports System.Threading.Thread
Imports System.Runtime.InteropServices

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class UE9_SimpleStream
    Inherits System.Windows.Forms.Form
    Dim running As Boolean

    Dim i As Long = 0
    Dim ioType As LJUD.IO = 0
    Dim channel As LJUD.CHANNEL = 0
    Dim dblValue As Double = 0
    Dim dblCommBacklog As Double = 0
    Dim dblUDBacklog As Double = 0
    Dim delayms As Integer = 1000
    Const NUM_SCANS As Double = 2000
    Dim numScansRequested As Double
    Dim adblData(4000) As Double
    Dim ue9 As UE9
    Const SCAN_RATE As Long = 1000
    Const READ_INTERVAL As Long = 500  'millisecond interval to read stream data
    Dim iteration As Integer = 0

    'Set the number of scans to read.  We will request twice the number
    'we expect, to make sure we get everything that is available.  So
    'with scanRate=1000 and readInterval=500, we expect 500 scans (1000
    'samples) per read, and will request up to 1000 scans (2000 samples)
    'per read.  Since the data packets between host and UE9 are 16 samples
    'for Ethernet and 64 samples for USB, we would expect 496 or 504 scans
    'per read over Ethernet and 480 or 512 scans per read over USB.  If
    'you run this program and see that your are consistantly reading more
    'scans than that, it means VB is too slow and not calling the read
    'function at the specified interval.
    Const NUM_SCANS_REQUESTED As Long = 2 * SCAN_RATE * (READ_INTERVAL / 1000)
    Const NUM_SAMPLES_REQUESTED As Long = 2 * NUM_SCANS_REQUESTED

    ' Dummy variables to satisfy certain method signatures
    Dim dummyDouble As Double = 0
    Dim dummyDoubleArray As Double = (0)
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Dim dummyInt As Integer = 0

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
    Friend WithEvents firstScanLabel As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents iterationDisplay As System.Windows.Forms.Label
    Friend WithEvents firstScanDisplay As System.Windows.Forms.Label
    Friend WithEvents scanRateLabel As System.Windows.Forms.Label
    Friend WithEvents scanRateDisplay As System.Windows.Forms.Label
    Friend WithEvents sampleRateLabel As System.Windows.Forms.Label
    Friend WithEvents sampleDisplay As System.Windows.Forms.Label
    Friend WithEvents commBacklogLabel As System.Windows.Forms.Label
    Friend WithEvents commBacklogDisplay As System.Windows.Forms.Label
    Friend WithEvents udBacklogDisplay As System.Windows.Forms.Label
    Friend WithEvents udBacklogLabel As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.iterationLabel = New System.Windows.Forms.Label
        Me.firstScanLabel = New System.Windows.Forms.Label
        Me.commBacklogLabel = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.iterationDisplay = New System.Windows.Forms.Label
        Me.firstScanDisplay = New System.Windows.Forms.Label
        Me.commBacklogDisplay = New System.Windows.Forms.Label
        Me.scanRateLabel = New System.Windows.Forms.Label
        Me.scanRateDisplay = New System.Windows.Forms.Label
        Me.sampleRateLabel = New System.Windows.Forms.Label
        Me.sampleDisplay = New System.Windows.Forms.Label
        Me.udBacklogDisplay = New System.Windows.Forms.Label
        Me.udBacklogLabel = New System.Windows.Forms.Label
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'iterationLabel
        '
        Me.iterationLabel.Location = New System.Drawing.Point(8, 8)
        Me.iterationLabel.Name = "iterationLabel"
        Me.iterationLabel.Size = New System.Drawing.Size(160, 16)
        Me.iterationLabel.TabIndex = 3
        Me.iterationLabel.Text = "Iteration:"
        '
        'firstScanLabel
        '
        Me.firstScanLabel.Location = New System.Drawing.Point(8, 30)
        Me.firstScanLabel.Name = "firstScanLabel"
        Me.firstScanLabel.Size = New System.Drawing.Size(160, 16)
        Me.firstScanLabel.TabIndex = 5
        Me.firstScanLabel.Text = "Last Scan:"
        '
        'commBacklogLabel
        '
        Me.commBacklogLabel.Location = New System.Drawing.Point(8, 72)
        Me.commBacklogLabel.Name = "commBacklogLabel"
        Me.commBacklogLabel.Size = New System.Drawing.Size(160, 16)
        Me.commBacklogLabel.TabIndex = 6
        Me.commBacklogLabel.Text = "Comm Backlog:"
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(24, 184)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(232, 24)
        Me.goButton.TabIndex = 7
        Me.goButton.Text = "Go!"
        '
        'iterationDisplay
        '
        Me.iterationDisplay.Location = New System.Drawing.Point(168, 8)
        Me.iterationDisplay.Name = "iterationDisplay"
        Me.iterationDisplay.Size = New System.Drawing.Size(112, 16)
        Me.iterationDisplay.TabIndex = 11
        '
        'firstScanDisplay
        '
        Me.firstScanDisplay.Location = New System.Drawing.Point(168, 30)
        Me.firstScanDisplay.Name = "firstScanDisplay"
        Me.firstScanDisplay.Size = New System.Drawing.Size(112, 34)
        Me.firstScanDisplay.TabIndex = 13
        '
        'commBacklogDisplay
        '
        Me.commBacklogDisplay.Location = New System.Drawing.Point(168, 72)
        Me.commBacklogDisplay.Name = "commBacklogDisplay"
        Me.commBacklogDisplay.Size = New System.Drawing.Size(112, 16)
        Me.commBacklogDisplay.TabIndex = 14
        '
        'scanRateLabel
        '
        Me.scanRateLabel.Location = New System.Drawing.Point(8, 120)
        Me.scanRateLabel.Name = "scanRateLabel"
        Me.scanRateLabel.Size = New System.Drawing.Size(152, 16)
        Me.scanRateLabel.TabIndex = 15
        Me.scanRateLabel.Text = "Actual Scan Rate:"
        '
        'scanRateDisplay
        '
        Me.scanRateDisplay.Location = New System.Drawing.Point(160, 120)
        Me.scanRateDisplay.Name = "scanRateDisplay"
        Me.scanRateDisplay.Size = New System.Drawing.Size(120, 16)
        Me.scanRateDisplay.TabIndex = 16
        '
        'sampleRateLabel
        '
        Me.sampleRateLabel.Location = New System.Drawing.Point(8, 144)
        Me.sampleRateLabel.Name = "sampleRateLabel"
        Me.sampleRateLabel.Size = New System.Drawing.Size(152, 16)
        Me.sampleRateLabel.TabIndex = 17
        Me.sampleRateLabel.Text = "Actual Sample Rate:"
        '
        'sampleDisplay
        '
        Me.sampleDisplay.Location = New System.Drawing.Point(160, 144)
        Me.sampleDisplay.Name = "sampleDisplay"
        Me.sampleDisplay.Size = New System.Drawing.Size(120, 16)
        Me.sampleDisplay.TabIndex = 18
        '
        'udBacklogDisplay
        '
        Me.udBacklogDisplay.Location = New System.Drawing.Point(168, 96)
        Me.udBacklogDisplay.Name = "udBacklogDisplay"
        Me.udBacklogDisplay.Size = New System.Drawing.Size(112, 16)
        Me.udBacklogDisplay.TabIndex = 20
        '
        'udBacklogLabel
        '
        Me.udBacklogLabel.Location = New System.Drawing.Point(8, 96)
        Me.udBacklogLabel.Name = "udBacklogLabel"
        Me.udBacklogLabel.Size = New System.Drawing.Size(160, 16)
        Me.udBacklogLabel.TabIndex = 19
        Me.udBacklogLabel.Text = "UD Backlog:"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'UE9_SimpleStream
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 228)
        Me.Controls.Add(Me.udBacklogDisplay)
        Me.Controls.Add(Me.udBacklogLabel)
        Me.Controls.Add(Me.sampleDisplay)
        Me.Controls.Add(Me.sampleRateLabel)
        Me.Controls.Add(Me.scanRateDisplay)
        Me.Controls.Add(Me.scanRateLabel)
        Me.Controls.Add(Me.commBacklogDisplay)
        Me.Controls.Add(Me.firstScanDisplay)
        Me.Controls.Add(Me.iterationDisplay)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.commBacklogLabel)
        Me.Controls.Add(Me.firstScanLabel)
        Me.Controls.Add(Me.iterationLabel)
        Me.Name = "UE9_SimpleStream"
        Me.Text = "UE9_SimpleStream"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub FormClosing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If running Then StopStream()
    End Sub

    Sub stopStream()
        Try
            'Stop the stream
            LJUD.eGet(ue9.ljhandle, LJUD.IO.STOP_STREAM, 0, dummyDouble, dummyDoubleArray)
        Catch ex As Exception
            showErrorMessage(ex)
        End Try

        goButton.Text = "Start Again"
        running = False
        Timer1.Enabled = False
    End Sub

    Private Sub goButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        If Not running Then
            goButton.Text = "Stop"

            ' Open UE9
            Try
                ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

            Try
                'Configure the stream:
                'Configure all analog inputs for 12-bit resolution
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, 12, 0, 0)

                'Configure the analog input range on channel 0 for bipolar +-5 volts.
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 0, LJUD.RANGES.BIP5V, 0, 0)
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_AIN_RANGE, 1, LJUD.RANGES.BIP5V, 0, 0)

                'Set the scan rate.
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_SCAN_FREQUENCY, SCAN_RATE, 0, 0)

                'Give the driver a 5 second buffer (scanRate * 2 channels * 5 seconds).
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_BUFFER_SIZE, SCAN_RATE * 2 * 5, 0, 0)

                'Configure reads to retrieve whatever data is available without waiting (wait mode LJUD.STREAMWAITMODES.NONE).
                'See comments below to change this program to use LJUD.STREAMWAITMODES.SLEEP mode.
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.STREAM_WAIT_MODE, LJUD.STREAMWAITMODES.NONE, 0, 0)

                'Define the scan list as AIN0 then FIOEIO.
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.CLEAR_STREAM_CHANNELS, 0, 0, 0, 0)
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 0, 0, 0, 0)
                LJUD.AddRequest(ue9.ljhandle, LJUD.IO.ADD_STREAM_CHANNEL, 1, 0, 0, 0)

                'Execute the list of requests.
                LJUD.GoOne(ue9.ljhandle)

            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

            ' Get results until there is no more data available for error checking
            Dim isFinished As Boolean
            isFinished = False
            While Not isFinished
                Try
                    LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dummyDouble, dummyInt, dummyDouble)
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
            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

            Timer1.Interval = READ_INTERVAL
            Timer1.Enabled = True

            'The actual scan rate is dependent on how the desired scan rate divides into
            'the LabJack clock.  The actual scan rate is returned in the value parameter
            'from the start stream command.
            scanRateDisplay.Text = dblValue.ToString("0.###")
            sampleDisplay.Text = (2 * dblValue).ToString("0.###")

            'Toggle the button
            running = True

        Else
            stopStream()
        End If
    End Sub

    Private Sub UE9_SimpleStream_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Enabled = False
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim i As Long
        Dim dblValue As Double
        Dim lngError As Long

        iteration = iteration + 1

        iterationDisplay.Text = Str(iteration)

        Dim adblData(NUM_SAMPLES_REQUESTED) As Double

        'Initialize the array so it will be easy to tell if the values have changed
        For i = 1 To NUM_SAMPLES_REQUESTED
            adblData(i) = 9999
        Next i

        Try
            'Read the data. 
            numScansRequested = NUM_SCANS
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_STREAM_DATA, LJUD.CHANNEL.ALL_CHANNELS, numScansRequested, adblData)
            firstScanDisplay.Text = Str(adblData(0)) + ", " + Str(adblData(1))
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_COMM, dblValue, 0)
            commBacklogDisplay.Text = Str(dblValue)
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.STREAM_BACKLOG_UD, dblValue, 0)
            udBacklogDisplay.Text = Str(dblValue)
        Catch ex As Exception
            Timer1.Enabled = False
            showErrorMessage(ex)
        End Try
    End Sub
End Class
