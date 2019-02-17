'Simple VB.NET example uses the LabJackUD driver to communicate with a UE9.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll referenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD


Public Class Form1
    Inherits System.Windows.Forms.Form

    Sub ErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        Dim lngError As Integer
        Dim lngHandle As Integer
        Dim dblDriverVersion As Double
        Dim strError(255) As Char
        Dim lngErrorSum As Integer
        Dim dblSerialNumber As Double
        Dim dblAIN0 As Double
        Dim dblAIN1 As Double
        Dim dblFIO0 As Double
        Dim dblCount0 As Double
        Dim lngIOType As Integer
        Dim lngChannel As Integer
        Dim dblValue As Double

        'Retrieve the LabJackUD driver version.
        dblDriverVersion = LJUD.GetDriverVersion()
        txtDriverVersion.Text = Str(dblDriverVersion)
        textDNDriverVersion.Text = LJUD.LJUDDOTNET_VERSION

        'Open the first found LabJack UE9 over USB.
        Try

            LJUD.OpenLabJack(LJUD.DEVICE.UE9, LJUD.CONNECTION.USB, "0", 1, lngHandle)
        Catch x As LabJackUDException
            ErrorMessage(x)
            lngError = x.LJUDError
        End Try

        txtHandle.Text = Str(lngHandle)
        txtOpenErrorNumber.Text = Str(lngError)
        'Convert the error code to a string.
        LJUD.ErrorToString(lngError, strError)
        txtOpenErrorString.Text = strError


        Try

            'First, examples using eGet():
            'Read and display the serial number.
            LJUD.eGet(lngHandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.SERIAL_NUMBER, dblSerialNumber, 0)
            txtSerialNumber.Text = Str(dblSerialNumber)
            'Configure FIO0 and FIO1 as analog, all else as digital.  That means
            'we pass starting channel 0, a value of 3 (binary ...0011), and
            'a number of bits of 16.
            LJUD.eGet(lngHandle, LJUD.IO.PUT_ANALOG_ENABLE_PORT, 0, 3, 16)
            'Get a reading from AIN1.
            LJUD.eGet(lngHandle, LJUD.IO.GET_AIN, 1, dblAIN1, 0)
            'Set the state of FIO0
            LJUD.ePut(lngHandle, LJUD.IO.PUT_DIGITAL_BIT, 0, 1, 0)
            'Set DAC0 to 2.5 volts.
            LJUD.eGet(lngHandle, LJUD.IO.PUT_DAC, 0, 2.5, 0)

            txtAIN1.Text = Format(dblAIN1, "#.#####")
            txtFIO0.Text = Str(dblFIO0)


            'Now, examples using AddRequest/GoOne/GetResult.  This method can
            'have a speed advantage, as multiple requests can be handled in
            'a single low level command:
            'Request a reading from AIN0 and AIN1.
            LJUD.AddRequest(lngHandle, LJUD.IO.GET_AIN, 0, 0, 0, 0)
            LJUD.AddRequest(lngHandle, LJUD.IO.GET_AIN, 1, 0, 0, 0)
            'Request the state of FIO0.
            LJUD.AddRequest(lngHandle, LJUD.IO.GET_DIGITAL_BIT, 0, 0, 0, 0)
            'Request to set DAC0 to 2.5 volts.
            LJUD.AddRequest(lngHandle, LJUD.IO.PUT_DAC, 0, 2.5, 0, 0)
            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level Feedback command to handle all the requests above.
            LJUD.GoOne(lngHandle)
            'Get the analog input voltage readings.
            LJUD.GetResult(lngHandle, LJUD.IO.GET_AIN, 0, dblAIN0)
            LJUD.GetResult(lngHandle, LJUD.IO.GET_AIN, 1, dblAIN1)
            'Get the state of FIO6.
            LJUD.GetResult(lngHandle, LJUD.IO.GET_DIGITAL_BIT, 0, dblFIO0)
            'Get the DAC request result just to check for an errorcode.
            LJUD.GetResult(lngHandle, LJUD.IO.PUT_DAC, 0, 0)
            

            'The requests can be executed repeatedly, as the request list remains
            'in effect until AddRequest is called again:
            LJUD.GoOne(lngHandle)
            'Get the analog input voltage readings again.
            LJUD.GetResult(lngHandle, LJUD.IO.GET_AIN, 0, dblAIN0)
            LJUD.GetResult(lngHandle, LJUD.IO.GET_AIN, 1, dblAIN1)

            'Offset the timer/counters by 2 so they start appearing on FIO2.
            LJUD.AddRequest(lngHandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 2, 0, 0)
            'Enable Counter0.  It will use FIO2.
            LJUD.AddRequest(lngHandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 1, 0, 0)
            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(lngHandle)

            'Now we will do all the GetResults just to check for errors.
            LJUD.GetResult(lngHandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0)

            'Wait half a second, then read Counter1.  Should get roughly 977 counts.
            Sleep(500)
            LJUD.eGet(lngHandle, LJUD.IO.GET_COUNTER, 0, dblCount0, 0)
            txtCount1.Text = Str(dblCount0)

            'Disable the counter.
            LJUD.AddRequest(lngHandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
            LJUD.GoOne(lngHandle)
        Catch x As LabJackUDException
            ErrorMessage(x)
        End Try



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
    Friend WithEvents txtSerialNumber As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents textDNDriverVersion As System.Windows.Forms.TextBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents txtAIN1 As System.Windows.Forms.TextBox
    Friend WithEvents txtOpenErrorString As System.Windows.Forms.TextBox
    Friend WithEvents txtOpenErrorNumber As System.Windows.Forms.TextBox
    Friend WithEvents txtHandle As System.Windows.Forms.TextBox
    Friend WithEvents txtDriverVersion As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtCount1 As System.Windows.Forms.TextBox
    Friend WithEvents txtFIO6 As System.Windows.Forms.TextBox
    Friend WithEvents txtFIO0 As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtSerialNumber = New System.Windows.Forms.TextBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.textDNDriverVersion = New System.Windows.Forms.TextBox
        Me.btnClose = New System.Windows.Forms.Button
        Me.txtCount1 = New System.Windows.Forms.TextBox
        Me.txtFIO0 = New System.Windows.Forms.TextBox
        Me.txtAIN1 = New System.Windows.Forms.TextBox
        Me.txtOpenErrorString = New System.Windows.Forms.TextBox
        Me.txtOpenErrorNumber = New System.Windows.Forms.TextBox
        Me.txtHandle = New System.Windows.Forms.TextBox
        Me.txtDriverVersion = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'txtSerialNumber
        '
        Me.txtSerialNumber.Location = New System.Drawing.Point(12, 184)
        Me.txtSerialNumber.Name = "txtSerialNumber"
        Me.txtSerialNumber.TabIndex = 42
        Me.txtSerialNumber.Text = ""
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(12, 168)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(112, 16)
        Me.Label10.TabIndex = 41
        Me.Label10.Text = "Serial Number"
        '
        'Label9
        '
        Me.Label9.Location = New System.Drawing.Point(132, 16)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(128, 16)
        Me.Label9.TabIndex = 40
        Me.Label9.Text = "VSDotNetLJUD Version"
        '
        'textDNDriverVersion
        '
        Me.textDNDriverVersion.Location = New System.Drawing.Point(132, 32)
        Me.textDNDriverVersion.Name = "textDNDriverVersion"
        Me.textDNDriverVersion.TabIndex = 39
        Me.textDNDriverVersion.Text = ""
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(324, 232)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.TabIndex = 23
        Me.btnClose.Text = "Close"
        '
        'txtCount1
        '
        Me.txtCount1.Location = New System.Drawing.Point(12, 304)
        Me.txtCount1.Name = "txtCount1"
        Me.txtCount1.Size = New System.Drawing.Size(80, 20)
        Me.txtCount1.TabIndex = 37
        Me.txtCount1.Text = ""
        '
        'txtFIO0
        '
        Me.txtFIO0.Location = New System.Drawing.Point(12, 264)
        Me.txtFIO0.Name = "txtFIO0"
        Me.txtFIO0.TabIndex = 43
        Me.txtFIO0.Text = ""
        '
        'txtAIN1
        '
        Me.txtAIN1.Location = New System.Drawing.Point(12, 224)
        Me.txtAIN1.Name = "txtAIN1"
        Me.txtAIN1.Size = New System.Drawing.Size(80, 20)
        Me.txtAIN1.TabIndex = 35
        Me.txtAIN1.Text = ""
        '
        'txtOpenErrorString
        '
        Me.txtOpenErrorString.Location = New System.Drawing.Point(156, 144)
        Me.txtOpenErrorString.Name = "txtOpenErrorString"
        Me.txtOpenErrorString.Size = New System.Drawing.Size(264, 20)
        Me.txtOpenErrorString.TabIndex = 34
        Me.txtOpenErrorString.Text = ""
        '
        'txtOpenErrorNumber
        '
        Me.txtOpenErrorNumber.Location = New System.Drawing.Point(12, 144)
        Me.txtOpenErrorNumber.Name = "txtOpenErrorNumber"
        Me.txtOpenErrorNumber.TabIndex = 33
        Me.txtOpenErrorNumber.Text = ""
        '
        'txtHandle
        '
        Me.txtHandle.Location = New System.Drawing.Point(12, 104)
        Me.txtHandle.Name = "txtHandle"
        Me.txtHandle.TabIndex = 32
        Me.txtHandle.Text = ""
        '
        'txtDriverVersion
        '
        Me.txtDriverVersion.Location = New System.Drawing.Point(12, 32)
        Me.txtDriverVersion.Name = "txtDriverVersion"
        Me.txtDriverVersion.TabIndex = 31
        Me.txtDriverVersion.Text = ""
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(12, 288)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(56, 16)
        Me.Label7.TabIndex = 30
        Me.Label7.Text = "Count0"
        '
        'Label6
        '
        Me.Label6.Location = New System.Drawing.Point(12, 248)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(48, 16)
        Me.Label6.TabIndex = 29
        Me.Label6.Text = "FIO0"
        '
        'Label5
        '
        Me.Label5.Location = New System.Drawing.Point(12, 208)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 16)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "AIN1"
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(156, 128)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(100, 16)
        Me.Label4.TabIndex = 27
        Me.Label4.Text = "Open Error String"
        '
        'Label3
        '
        Me.Label3.Location = New System.Drawing.Point(12, 128)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(112, 16)
        Me.Label3.TabIndex = 26
        Me.Label3.Text = "Open Error Number"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(12, 88)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 16)
        Me.Label2.TabIndex = 25
        Me.Label2.Text = "Handle"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(12, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 16)
        Me.Label1.TabIndex = 24
        Me.Label1.Text = "UD Driver Version"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(432, 357)
        Me.Controls.Add(Me.txtSerialNumber)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.textDNDriverVersion)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.txtCount1)
        Me.Controls.Add(Me.txtFIO0)
        Me.Controls.Add(Me.txtAIN1)
        Me.Controls.Add(Me.txtOpenErrorString)
        Me.Controls.Add(Me.txtOpenErrorNumber)
        Me.Controls.Add(Me.txtHandle)
        Me.Controls.Add(Me.txtDriverVersion)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form1"
        Me.Text = "Simple UE9 VB.NET Example"
        Me.ResumeLayout(False)

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Label8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class
