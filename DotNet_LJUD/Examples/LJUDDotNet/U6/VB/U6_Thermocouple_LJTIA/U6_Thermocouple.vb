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
    Friend WithEvents analogFourLabel As System.Windows.Forms.Label
    Friend WithEvents internalSensorLabel As System.Windows.Forms.Label
    Friend WithEvents thermocoupleSensorLabel As System.Windows.Forms.Label
    Friend WithEvents analogFourDisplay As System.Windows.Forms.Label
    Friend WithEvents internalSensorDisplay As System.Windows.Forms.Label
    Friend WithEvents thermocoupleSensorDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.analogFourLabel = New System.Windows.Forms.Label
        Me.internalSensorLabel = New System.Windows.Forms.Label
        Me.thermocoupleSensorLabel = New System.Windows.Forms.Label
        Me.analogFourDisplay = New System.Windows.Forms.Label
        Me.internalSensorDisplay = New System.Windows.Forms.Label
        Me.thermocoupleSensorDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'analogFourLabel
        '
        Me.analogFourLabel.Location = New System.Drawing.Point(8, 8)
        Me.analogFourLabel.Name = "analogFourLabel"
        Me.analogFourLabel.Size = New System.Drawing.Size(144, 16)
        Me.analogFourLabel.TabIndex = 0
        Me.analogFourLabel.Text = "Analog 2:"
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
        'analogFourDisplay
        '
        Me.analogFourDisplay.Location = New System.Drawing.Point(160, 8)
        Me.analogFourDisplay.Name = "analogFourDisplay"
        Me.analogFourDisplay.Size = New System.Drawing.Size(112, 16)
        Me.analogFourDisplay.TabIndex = 3
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
        Me.Controls.Add(Me.analogFourDisplay)
        Me.Controls.Add(Me.thermocoupleSensorLabel)
        Me.Controls.Add(Me.internalSensorLabel)
        Me.Controls.Add(Me.analogFourLabel)
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
        ioType = 0
        Dim channel As LJUD.CHANNEL
        channel = 0
        Dim dblValue As Double
        dblValue = 0
        Dim valueAIN As Double
        valueAIN = 0
        Dim u6 As U6
        Dim cjTempK As Double
        Dim pTCTempK As Double

        ' Variables to satisfy certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0

        Dim time As Long
        time = 0
        Dim tempChannel As LJUD.CHANNEL
        tempChannel = 2  'Channel which the TC/LJTIA is on (AIN2).

        Dim tcVolts As Double
        tcVolts = 0
        Dim tcType As LJUD.THERMOCOUPLETYPE
        tcType = LJUD.THERMOCOUPLETYPE.K
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

        'Offset calibration:  The nominal voltage offset of the LJTick is
        '0.4 volts.  For improved accuracy, though, you should measure the
        'overall system offset.  We know that if the end of the TC is at the
        'same temperature as the cold junction, the voltage should be zero.
        'Put the end of the TC near the LJTIA to make sure they are at the same
        'temperature, and note the voltage measured by FIO4.  This is the actual
        'offset that can be entered below.
        Dim offsetVoltage As Double
        offsetVoltage = 0.4

        Try
            'Open the first found LabJack U6 via USB.
            u6 = New U6(LJUD.CONNECTION.USB, "0", True)
        Catch ex As LabJackUDException
            showErrorMessage(ex)

        End Try

        Dim keyPressed As Boolean
        keyPressed = False
        ioType = 0
        channel = 0
        time = 0
        tcVolts = 0
        cjTempK = 0
        pTCTempK = 0

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
                    cjTempK = dblValue
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

        'Display Voltage Reading
        analogFourDisplay.Text = valueAIN.ToString("0.######")

        'Display the internal temperature sensor reading.  This example uses
        'that value for cold junction compensation.
        internalSensorDisplay.Text = cjTempK.ToString("0.0 deg K")

        'To get the thermocouple voltage we subtract the offset from the AIN
        'voltage and divide by the LJTIA gain.
        tcVolts = (valueAIN - offsetVoltage) / 51

        'Convert TC voltage to temperature.
        LJUD.TCVoltsToTemp(tcType, tcVolts, cjTempK, pTCTempK)

        'Display Temperature
        thermocoupleSensorDisplay.Text = pTCTempK.ToString("0.0 deg K")
    End Sub
End Class