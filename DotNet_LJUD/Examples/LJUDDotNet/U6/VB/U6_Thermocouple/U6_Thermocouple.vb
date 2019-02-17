'---------------------------------------------------------------------------
'
'  U6_Thermocouple.vb
' 
'  Demonstrates using the TCVoltsToTemp to yield temperature readings for
'	a thermocouple connected to a U6 via a LJTick-InAmp
'
'  Setup:
'  TC plus to INA+.
'	TC minus to INA-.
'	10k resistor from INA- to GND on LJTIA (a short is also acceptable in most cases).
'	LJTIA connected to AIN2/AIN33 block on U6.
'	LJTIA offset set to 0.4 volts.
'	LJTIA channel A gain set to 51.
'
'  For the best accuracy, we recommend doing a quick offset calibration as
'	described in the comments below, and using an external cold-junction
'	temperature sensor rather than the internal U6 temp sensor.  The internal
'	temp sensor is often sufficient, but an external sensor, such as 
'	the LM34CAZ (national.com) or the EI-1034 (labjack.com), placed near
'	the LJTIA, can provide better CJC, particularly when the U6 itself
'	is subjected to varying temperatures.
'
'  support@labjack.com
'  June 15, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a U6.
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
    Friend WithEvents internalSensorLabel As System.Windows.Forms.Label
    Friend WithEvents thermocoupleSensorLabel As System.Windows.Forms.Label
    Friend WithEvents internalSensorDisplay As System.Windows.Forms.Label
    Friend WithEvents thermocoupleSensorDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents analogLabel As System.Windows.Forms.Label
    Friend WithEvents analogDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.analogLabel = New System.Windows.Forms.Label
        Me.internalSensorLabel = New System.Windows.Forms.Label
        Me.thermocoupleSensorLabel = New System.Windows.Forms.Label
        Me.analogDisplay = New System.Windows.Forms.Label
        Me.internalSensorDisplay = New System.Windows.Forms.Label
        Me.thermocoupleSensorDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'analogLabel
        '
        Me.analogLabel.Location = New System.Drawing.Point(8, 8)
        Me.analogLabel.Name = "analogLabel"
        Me.analogLabel.Size = New System.Drawing.Size(144, 16)
        Me.analogLabel.TabIndex = 0
        Me.analogLabel.Text = "Analog 0:"
        '
        'internalSensorLabel
        '
        Me.internalSensorLabel.Location = New System.Drawing.Point(8, 32)
        Me.internalSensorLabel.Name = "internalSensorLabel"
        Me.internalSensorLabel.Size = New System.Drawing.Size(144, 16)
        Me.internalSensorLabel.TabIndex = 1
        Me.internalSensorLabel.Text = "U6 internal sensor:"
        '
        'thermocoupleSensorLabel
        '
        Me.thermocoupleSensorLabel.Location = New System.Drawing.Point(8, 56)
        Me.thermocoupleSensorLabel.Name = "thermocoupleSensorLabel"
        Me.thermocoupleSensorLabel.Size = New System.Drawing.Size(144, 16)
        Me.thermocoupleSensorLabel.TabIndex = 2
        Me.thermocoupleSensorLabel.Text = "Thermocouple sensor:"
        '
        'analogDisplay
        '
        Me.analogDisplay.Location = New System.Drawing.Point(160, 8)
        Me.analogDisplay.Name = "analogDisplay"
        Me.analogDisplay.Size = New System.Drawing.Size(112, 16)
        Me.analogDisplay.TabIndex = 3
        '
        'internalSensorDisplay
        '
        Me.internalSensorDisplay.Location = New System.Drawing.Point(160, 32)
        Me.internalSensorDisplay.Name = "internalSensorDisplay"
        Me.internalSensorDisplay.Size = New System.Drawing.Size(104, 16)
        Me.internalSensorDisplay.TabIndex = 4
        '
        'thermocoupleSensorDisplay
        '
        Me.thermocoupleSensorDisplay.Location = New System.Drawing.Point(160, 56)
        Me.thermocoupleSensorDisplay.Name = "thermocoupleSensorDisplay"
        Me.thermocoupleSensorDisplay.Size = New System.Drawing.Size(104, 16)
        Me.thermocoupleSensorDisplay.TabIndex = 5
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 88)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(256, 24)
        Me.goButton.TabIndex = 6
        Me.goButton.Text = "Go!"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 116)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.thermocoupleSensorDisplay)
        Me.Controls.Add(Me.internalSensorDisplay)
        Me.Controls.Add(Me.analogDisplay)
        Me.Controls.Add(Me.thermocoupleSensorLabel)
        Me.Controls.Add(Me.internalSensorLabel)
        Me.Controls.Add(Me.analogLabel)
        Me.Name = "Form1"
        Me.Text = "Form1"
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
        Dim channel As LJUD.CHANNEL
        Dim dblValue As Double
        Dim valueAIN As Double
        Dim u6 As U6
        Dim cjTempK As Double
        Dim pTCTempK As Double
        Dim keyPressed As Boolean
        Dim ainResolution As Double
        Dim time As Long
        Dim tempChannel As LJUD.CHANNEL
        Dim tcVolts As Double
        Dim tcType As LJUD.THERMOCOUPLETYPE
        Dim dblInternal As Double
        Dim range As Double

        ' Variables to satisfy certain method signatures
        Dim dummyInt As Integer
        Dim dummyDouble As Double

        tcType = LJUD.THERMOCOUPLETYPE.K
        tcVolts = 0
        tempChannel = 0  'Channel which the TC/LJTIA is on (AIN0).
        time = 0
        dummyDouble = 0
        dummyInt = 0
        valueAIN = 0
        dblValue = 0
        channel = 0
        ioType = 0
        keyPressed = False
        ioType = 0
        channel = 0
        time = 0
        tcVolts = 0
        cjTempK = 0
        pTCTempK = 0
        ainResolution = 18
        dblInternal = 0
        range = LJUD.RANGES.BIPP01V

        'Set the temperature sensor to a k type thermocouple
        'Possible Thermocouple types are:
        'B = 6001
        'E = 6002
        'J = 6003
        'K = 6004
        'N = 6005
        'R = 6006
        'S = 6007
        'T = 6008

        Try
            'Open the first found LabJack U6 via USB.
            u6 = New U6(LJUD.CONNECTION.USB, "0", True)

            'Configure the desired resolution
            LJUD.eGet(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.AIN_RESOLUTION, ainResolution, 0)

            ' Set the range on the analog input channel to +/- 0.1 volts  (x100 gain)
            LJUD.eGet(u6.ljhandle, LJUD.IO.PUT_AIN_RANGE, tempChannel, range, 0)

            'Add analog input requests.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.GET_AIN, tempChannel, 0, 0, 0)

            'Add request for internal temperature reading -- Internal temp sensor uses 
            'analog input channel 14.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.GET_AIN, 14, 0, 0, 0)

            'Execute all requests on the labjack u6.ljhandle.
            LJUD.GoOne(u6.ljhandle)

            'Get all the results.  The first result should be the voltage reading of the 
            'temperature channel.
            LJUD.GetFirstResult(u6.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As Exception
            showErrorMessage(ex)
        End Try

        'Get the rest of the results.  There should only be one more on the request 
        'queue.
        Dim finished As Boolean
        finished = False
        While Not finished
            If ioType = LJUD.IO.GET_AIN Then

                If channel = tempChannel Then
                    valueAIN = dblValue
                End If

                If channel = 14 Then
                    dblInternal = dblValue
                End If
            End If


            Try
                LJUD.GetNextResult(u6.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException

                If (ex.LJUDError = LJUD.LJUDERROR.NO_DATA_AVAILABLE) Then
                    finished = True
                ElseIf (ex.LJUDError > LJUD.LJUDERROR.MIN_GROUP_ERROR) Then
                    finished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        'The cold junction is the screw-terminal block where the thermocouple
        'is connected.  As discussed in the U6 User's Guide, add 2.5 degrees C
        'to the internal temp sensor reading.  If using the CB37 rather than
        'the built-in screw terminals, just add 1.0 degrees C.
        cjTempK = dblInternal + 2.5

        'Display Voltage Reading
        analogDisplay.Text = valueAIN.ToString("0.######")

        'Display the internal temperature sensor reading.  This example uses
        'that value for cold junction compensation.
        internalSensorDisplay.Text = dblInternal.ToString("0.0 deg K")

        'Convert TC voltage to temperature.
        LJUD.TCVoltsToTemp(tcType, tcVolts, cjTempK, pTCTempK)

        'Display Temperature
        thermocoupleSensorDisplay.Text = pTCTempK.ToString("0.0 deg K")
    End Sub
End Class