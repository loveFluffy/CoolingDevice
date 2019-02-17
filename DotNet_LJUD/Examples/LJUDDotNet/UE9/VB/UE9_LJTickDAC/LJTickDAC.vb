'---------------------------------------------------------------------------
'
'  UE9_LJTickDAC.cs
' 
'	Demonstrates using the add/go/get method to efficiently write and read
'	virtually all analog and digital I/O on the LabJack UE9.
'	Records the time for 1000 iterations and divides by 1000, to allow
'	verification of the basic command/response communication times of the
'	LabJack UE9 as documented in Section 3.1 of the U3 User's Guide.
'
'  support@labjack.com
'  November 10, 2009
'----------------------------------------------------------------------
'

'Simple VB.NET example uses the LabJackUD driver to communicate with a U3.
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
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents InputA As System.Windows.Forms.TextBox
    Friend WithEvents InputB As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.InputA = New System.Windows.Forms.TextBox
        Me.InputB = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(16, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(48, 16)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "DAC A:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(16, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "DAC B:"
        '
        'InputA
        '
        Me.InputA.Location = New System.Drawing.Point(112, 8)
        Me.InputA.Name = "InputA"
        Me.InputA.Size = New System.Drawing.Size(104, 20)
        Me.InputA.TabIndex = 2
        Me.InputA.Text = ""
        '
        'InputB
        '
        Me.InputB.Location = New System.Drawing.Point(112, 32)
        Me.InputB.Name = "InputB"
        Me.InputB.Size = New System.Drawing.Size(104, 20)
        Me.InputB.TabIndex = 3
        Me.InputB.Text = ""
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(16, 72)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(200, 24)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Go"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(232, 102)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.InputB)
        Me.Controls.Add(Me.InputA)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form1"
        Me.Text = "LJTickDAC"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '	long lngGetNextIteration
        '	LJUD.IO ioType=0, channel=0
        '	double dblValue=0

        Dim i As Long
        i = 0
        Dim pinNum As Double
        pinNum = 0 ' Specifies the pins (FIO0/FIO1) for the LJTickDAC
        Dim achrUserMem(64) As Integer
        Dim adblCalMem(4) As Double
        Dim serialNumber As Double
        serialNumber = 0
        Dim device As UE9

        ' Dummy variables to satisfy certain method signatures
        Dim dummyDouble As Double
        dummyDouble = 0

        'Open the LabJack.
        Try
            device = New UE9(LJUD.CONNECTION.USB, "0", True) ' Connection through USB
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Try
            'Specify where the LJTick-DAC is plugged in.
            'This is just setting a parameter in the driver, and not actually talking
            'to the hardware, and thus executes very fast.
            LJUD.ePut(device.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TDAC_SCL_PIN_NUM, pinNum, 0)

            'Set DACA to 1.2 volts.  If the driver has not previously talked to an LJTDAC
            'on the specified pins, it will first retrieve and store the cal constants.  The
            'low-level I2C command can only update 1 DAC channel at a time, so there
            'is no advantage to doing two updates within a single add-go-get block.
            LJUD.ePut(device.ljhandle, LJUD.IO.TDAC_COMMUNICATION, LJUD.CHANNEL.TDAC_UPDATE_DACA, CDbl(InputA.Text), 0)

            'Set DACB to 2.3 volts.
            LJUD.ePut(device.ljhandle, LJUD.IO.TDAC_COMMUNICATION, LJUD.CHANNEL.TDAC_UPDATE_DACB, CDbl(InputB.Text), 0)

            'Now for more advanced operations.

            'If at this point you removed that LJTDAC and plugged a different one
            'into the same pins, the driver would not know and would use the wrong
            'cal constants on future updates.  If we do a cal constant read,
            'the driver will store the constants from the new read.
            LJUD.eGet(device.ljhandle, LJUD.IO.TDAC_COMMUNICATION, LJUD.CHANNEL.TDAC_READ_CAL_CONSTANTS, dummyDouble, adblCalMem)
            Console.Out.WriteLine("DACA Slope = {0:0.0} bits/volt", adblCalMem(0))
            Console.Out.WriteLine("DACA Offset = {0:0.0} bits", adblCalMem(1))
            Console.Out.WriteLine("DACB Slope = {0:0.0} bits/volt", adblCalMem(2))
            Console.Out.WriteLine("DACB Offset = {0:0.0} bits", adblCalMem(3))

            'Read the serial number.
            LJUD.eGet(device.ljhandle, LJUD.IO.TDAC_COMMUNICATION, LJUD.CHANNEL.TDAC_SERIAL_NUMBER, serialNumber, 0)
            Console.Out.WriteLine("LJTDAC Serial Number = {0:0}", serialNumber)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

    End Sub
End Class
