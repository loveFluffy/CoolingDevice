'---------------------------------------------------------------------------
'
'  U6_ei1050.vb
' 
'  Demonstrates talking to 1 or 2 EI-1050 probes.
'
'  support@labjack.com
'  June 12, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a U6.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U6_EI1050
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
    Friend WithEvents radioGroup As System.Windows.Forms.GroupBox
    Friend WithEvents radioOne As System.Windows.Forms.RadioButton
    Friend WithEvents probeALabel As System.Windows.Forms.Label
    Friend WithEvents tempProbeAKelvinsDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeACelciusDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeBFahrenheitDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeBCelciusDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeBKelvinsDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeBLabel As System.Windows.Forms.Label
    Friend WithEvents runButton As System.Windows.Forms.Button
    Friend WithEvents radioTwo As System.Windows.Forms.RadioButton
    Friend WithEvents tempProbeAFahrenheitDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeAHumidityDisplay As System.Windows.Forms.Label
    Friend WithEvents tempProbeBHumidityDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.radioGroup = New System.Windows.Forms.GroupBox
        Me.radioTwo = New System.Windows.Forms.RadioButton
        Me.radioOne = New System.Windows.Forms.RadioButton
        Me.probeALabel = New System.Windows.Forms.Label
        Me.tempProbeAKelvinsDisplay = New System.Windows.Forms.Label
        Me.tempProbeACelciusDisplay = New System.Windows.Forms.Label
        Me.tempProbeAFahrenheitDisplay = New System.Windows.Forms.Label
        Me.tempProbeBFahrenheitDisplay = New System.Windows.Forms.Label
        Me.tempProbeBCelciusDisplay = New System.Windows.Forms.Label
        Me.tempProbeBKelvinsDisplay = New System.Windows.Forms.Label
        Me.tempProbeBLabel = New System.Windows.Forms.Label
        Me.runButton = New System.Windows.Forms.Button
        Me.tempProbeAHumidityDisplay = New System.Windows.Forms.Label
        Me.tempProbeBHumidityDisplay = New System.Windows.Forms.Label
        Me.radioGroup.SuspendLayout()
        Me.SuspendLayout()
        '
        'radioGroup
        '
        Me.radioGroup.Controls.Add(Me.radioTwo)
        Me.radioGroup.Controls.Add(Me.radioOne)
        Me.radioGroup.Location = New System.Drawing.Point(0, 8)
        Me.radioGroup.Name = "radioGroup"
        Me.radioGroup.Size = New System.Drawing.Size(280, 64)
        Me.radioGroup.TabIndex = 0
        Me.radioGroup.TabStop = False
        Me.radioGroup.Text = "Number of EI1050s connected"
        '
        'radioTwo
        '
        Me.radioTwo.Location = New System.Drawing.Point(8, 40)
        Me.radioTwo.Name = "radioTwo"
        Me.radioTwo.Size = New System.Drawing.Size(256, 16)
        Me.radioTwo.TabIndex = 1
        Me.radioTwo.Text = "Two"
        '
        'radioOne
        '
        Me.radioOne.Checked = True
        Me.radioOne.Location = New System.Drawing.Point(8, 16)
        Me.radioOne.Name = "radioOne"
        Me.radioOne.Size = New System.Drawing.Size(256, 16)
        Me.radioOne.TabIndex = 0
        Me.radioOne.TabStop = True
        Me.radioOne.Text = "One"
        '
        'probeALabel
        '
        Me.probeALabel.Location = New System.Drawing.Point(8, 80)
        Me.probeALabel.Name = "probeALabel"
        Me.probeALabel.Size = New System.Drawing.Size(112, 16)
        Me.probeALabel.TabIndex = 1
        Me.probeALabel.Text = "Temp Probe A = "
        '
        'tempProbeAKelvinsDisplay
        '
        Me.tempProbeAKelvinsDisplay.Location = New System.Drawing.Point(136, 80)
        Me.tempProbeAKelvinsDisplay.Name = "tempProbeAKelvinsDisplay"
        Me.tempProbeAKelvinsDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeAKelvinsDisplay.TabIndex = 2
        '
        'tempProbeACelciusDisplay
        '
        Me.tempProbeACelciusDisplay.Location = New System.Drawing.Point(136, 104)
        Me.tempProbeACelciusDisplay.Name = "tempProbeACelciusDisplay"
        Me.tempProbeACelciusDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeACelciusDisplay.TabIndex = 3
        '
        'tempProbeAFahrenheitDisplay
        '
        Me.tempProbeAFahrenheitDisplay.Location = New System.Drawing.Point(136, 128)
        Me.tempProbeAFahrenheitDisplay.Name = "tempProbeAFahrenheitDisplay"
        Me.tempProbeAFahrenheitDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeAFahrenheitDisplay.TabIndex = 4
        '
        'tempProbeBFahrenheitDisplay
        '
        Me.tempProbeBFahrenheitDisplay.Location = New System.Drawing.Point(136, 232)
        Me.tempProbeBFahrenheitDisplay.Name = "tempProbeBFahrenheitDisplay"
        Me.tempProbeBFahrenheitDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeBFahrenheitDisplay.TabIndex = 8
        '
        'tempProbeBCelciusDisplay
        '
        Me.tempProbeBCelciusDisplay.Location = New System.Drawing.Point(136, 208)
        Me.tempProbeBCelciusDisplay.Name = "tempProbeBCelciusDisplay"
        Me.tempProbeBCelciusDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeBCelciusDisplay.TabIndex = 7
        '
        'tempProbeBKelvinsDisplay
        '
        Me.tempProbeBKelvinsDisplay.Location = New System.Drawing.Point(136, 184)
        Me.tempProbeBKelvinsDisplay.Name = "tempProbeBKelvinsDisplay"
        Me.tempProbeBKelvinsDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeBKelvinsDisplay.TabIndex = 6
        '
        'tempProbeBLabel
        '
        Me.tempProbeBLabel.Location = New System.Drawing.Point(8, 184)
        Me.tempProbeBLabel.Name = "tempProbeBLabel"
        Me.tempProbeBLabel.Size = New System.Drawing.Size(112, 16)
        Me.tempProbeBLabel.TabIndex = 5
        Me.tempProbeBLabel.Text = "Temp Probe B = "
        '
        'runButton
        '
        Me.runButton.Location = New System.Drawing.Point(40, 288)
        Me.runButton.Name = "runButton"
        Me.runButton.Size = New System.Drawing.Size(192, 24)
        Me.runButton.TabIndex = 9
        Me.runButton.Text = "Run!"
        '
        'tempProbeAHumidityDisplay
        '
        Me.tempProbeAHumidityDisplay.Location = New System.Drawing.Point(128, 152)
        Me.tempProbeAHumidityDisplay.Name = "tempProbeAHumidityDisplay"
        Me.tempProbeAHumidityDisplay.Size = New System.Drawing.Size(144, 16)
        Me.tempProbeAHumidityDisplay.TabIndex = 10
        '
        'tempProbeBHumidityDisplay
        '
        Me.tempProbeBHumidityDisplay.Location = New System.Drawing.Point(128, 256)
        Me.tempProbeBHumidityDisplay.Name = "tempProbeBHumidityDisplay"
        Me.tempProbeBHumidityDisplay.Size = New System.Drawing.Size(136, 16)
        Me.tempProbeBHumidityDisplay.TabIndex = 11
        '
        'U6_EI1050
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 324)
        Me.Controls.Add(Me.tempProbeBHumidityDisplay)
        Me.Controls.Add(Me.tempProbeAHumidityDisplay)
        Me.Controls.Add(Me.runButton)
        Me.Controls.Add(Me.tempProbeBFahrenheitDisplay)
        Me.Controls.Add(Me.tempProbeBCelciusDisplay)
        Me.Controls.Add(Me.tempProbeBKelvinsDisplay)
        Me.Controls.Add(Me.tempProbeBLabel)
        Me.Controls.Add(Me.tempProbeAFahrenheitDisplay)
        Me.Controls.Add(Me.tempProbeACelciusDisplay)
        Me.Controls.Add(Me.tempProbeAKelvinsDisplay)
        Me.Controls.Add(Me.probeALabel)
        Me.Controls.Add(Me.radioGroup)
        Me.Name = "U6_EI1050"
        Me.Text = "U6_EI1050"
        Me.radioGroup.ResumeLayout(False)
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

        Dim dblValue As Double
        dblValue = 0
        Dim U6 As U6

        'Open the first found LabJack U6.
        Try
            U6 = New U6(LJUD.CONNECTION.USB, "0", True)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Try

            'Set the Data line to FIO0
            LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SHT_DATA_CHANNEL, 0, 0)

            'Set the Clock line to FIO1
            LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SHT_CLOCK_CHANNEL, 1, 0)

            'Set FIO2 to output-high to provide power to the EI-1050. 
            LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 6, 2, 0)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        If radioOne.Checked Then
            'Code for single EI1050
            '	Connections for one probe:
            '	Red (Power)         FIO2
            '	Black (Ground)      GND
            '	Green (Data)        FIO0
            '	White (Clock)       FIO1
            '	Brown (Enable)      FIO2

            Try
                'Now, an add/go/get block to get the temp & humidity at the same time.
                'Request a temperature reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0)

                'Request a humidity reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0)

                'Execute the requests.  Will take about 0.5 seconds with a USB high-high
                'or Ethernet connection, and about 1.5 seconds with a normal USB connection.
                LJUD.GoOne(U6.ljhandle)

                'Get the temperature reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, dblValue)
                tempProbeAKelvinsDisplay.Text = dblValue.ToString() + " deg K"
                tempProbeACelciusDisplay.Text = (dblValue - 273.15).ToString() + " deg C"
                tempProbeAFahrenheitDisplay.Text = "" + (((dblValue - 273.15) * 1.8) + 32).ToString() + " deg F"

                'Get the humidity reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, dblValue)
                tempProbeAHumidityDisplay.Text = "Humidity: " + dblValue.ToString("0.##") + " %"

            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

            'End of single probe code.

        ElseIf radioTwo.Checked Then
            'Use this code if two EI-1050 probes are connected.
            '	Connections for both probes:
            '	Red (Power)         FIO2
            '	Black (Ground)      GND
            '	Green (Data)        FIO0
            '	White (Clock)       FIO1
            '
            '	Probe A:
            '	Brown (Enable)    FIO3
            '
            '	Probe B:
            '	Brown (Enable)    DAC0

            Try
                'Set FIO3 to output-low to disable probe A. 
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 3, 0, 0)

                'Set DAC0 to 0 volts to disable probe B.
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DAC, 0, 0.0, 0)

                'Set FIO3 to output-high to enable probe A. 
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 3, 1, 0)

                'Now, an add/go/get block to get the temp & humidity at the same time.
                'Request a temperature reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0)

                'Request a humidity reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0)

                'Execute the requests.  Will take about 0.5 seconds with a USB high-high
                'or Ethernet connection, and about 1.5 seconds with a normal USB connection.
                LJUD.GoOne(U6.ljhandle)

                'Get the temperature reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, dblValue)
                tempProbeAKelvinsDisplay.Text = dblValue.ToString() + " deg K"
                tempProbeACelciusDisplay.Text = (dblValue - 273.15).ToString() + " deg C"
                tempProbeAFahrenheitDisplay.Text = "" + (((dblValue - 273.15) * 1.8) + 32).ToString() + " deg F"

                'Get the humidity reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, dblValue)
                tempProbeAHumidityDisplay.Text = "Humidity: " + dblValue.ToString("0.##") + " percent"

                'Set FIO3 to output-low to disable probe A. 
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DIGITAL_BIT, 3, 0, 0)

                'Set DAC0 to 3.3 volts to enable probe B.
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DAC, 0, 3.3, 0)

                'Since the DACs on the U6 are slower than the communication speed,
                'we put a delay here to make sure the DAC has time to rise to 3.3 volts
                'before communicating with the EI-1050.
                Threading.Thread.Sleep(30)  'Wait 30 ms.

                'Now, an add/go/get block to get the temp & humidity at the same time.
                'Request a temperature reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, 0, 0, 0)

                'Request a humidity reading from the EI-1050.
                LJUD.AddRequest(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, 0, 0, 0)

                'Execute the requests.  Will take about 0.5 seconds with a USB high-high
                'or Ethernet connection, and about 1.5 seconds with a normal USB connection.
                LJUD.GoOne(U6.ljhandle)

                'Get the temperature reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_TEMP, dblValue)
                tempProbeBKelvinsDisplay.Text = dblValue.ToString() + " deg K"
                tempProbeBCelciusDisplay.Text = (dblValue - 273.15).ToString() + " deg C"
                tempProbeBFahrenheitDisplay.Text = "" + (((dblValue - 273.15) * 1.8) + 32).ToString() + " deg F"

                'Get the humidity reading.
                LJUD.GetResult(U6.ljhandle, LJUD.IO.SHT_GET_READING, LJUD.CHANNEL.SHT_RH, dblValue)
                tempProbeBHumidityDisplay.Text = "Humidity: " + dblValue.ToString("0.##") + " percent"

                'Set DAC0 to 0 volts to disable probe B.
                LJUD.ePut(U6.ljhandle, LJUD.IO.PUT_DAC, 0, 0.0, 0)

                'If we were going to loop and talk to probe A next, we would
                'want a delay here to make sure the DAC falls to 0 volts
                'before enabling probe A.
                Threading.Thread.Sleep(30)  'Wait 30 ms.
            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

            'End of dual probe code.
        End If
    End Sub
End Class
