'---------------------------------------------------------------------------
'
'  U3_Asynch.vb
' 
'	Demonstrates asynchronous communication using a loopback from
'	FIO4 to FIO5 on a U3 rev 1.30.  On earlier hardware revisions
'  use SDA and SCL.
'
'  support@labjack.com
'  June 12, 2009
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
    Friend WithEvents transmissionGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents origValueTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents origValueTwoLabel As System.Windows.Forms.Label
    Friend WithEvents origValueOneDisplay As System.Windows.Forms.Label
    Friend WithEvents origValueOneLabel As System.Windows.Forms.Label
    Friend WithEvents numBytesTransmittedDisplay As System.Windows.Forms.Label
    Friend WithEvents numBytesTransmittedLabel As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents newValueTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents newValueTwoLabel As System.Windows.Forms.Label
    Friend WithEvents newValueOneDisplay As System.Windows.Forms.Label
    Friend WithEvents newValueOneLabel As System.Windows.Forms.Label
    Friend WithEvents numBytesRecievedDisplay As System.Windows.Forms.Label
    Friend WithEvents numBytesRecievedLabel As System.Windows.Forms.Label
    Friend WithEvents runButton As System.Windows.Forms.Button
    Friend WithEvents notesLabel As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.transmissionGroupBox = New System.Windows.Forms.GroupBox
        Me.origValueTwoDisplay = New System.Windows.Forms.Label
        Me.origValueTwoLabel = New System.Windows.Forms.Label
        Me.origValueOneDisplay = New System.Windows.Forms.Label
        Me.origValueOneLabel = New System.Windows.Forms.Label
        Me.numBytesTransmittedDisplay = New System.Windows.Forms.Label
        Me.numBytesTransmittedLabel = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.newValueTwoDisplay = New System.Windows.Forms.Label
        Me.newValueTwoLabel = New System.Windows.Forms.Label
        Me.newValueOneDisplay = New System.Windows.Forms.Label
        Me.newValueOneLabel = New System.Windows.Forms.Label
        Me.numBytesRecievedDisplay = New System.Windows.Forms.Label
        Me.numBytesRecievedLabel = New System.Windows.Forms.Label
        Me.runButton = New System.Windows.Forms.Button
        Me.notesLabel = New System.Windows.Forms.Label
        Me.transmissionGroupBox.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'transmissionGroupBox
        '
        Me.transmissionGroupBox.Controls.Add(Me.origValueTwoDisplay)
        Me.transmissionGroupBox.Controls.Add(Me.origValueTwoLabel)
        Me.transmissionGroupBox.Controls.Add(Me.origValueOneDisplay)
        Me.transmissionGroupBox.Controls.Add(Me.origValueOneLabel)
        Me.transmissionGroupBox.Controls.Add(Me.numBytesTransmittedDisplay)
        Me.transmissionGroupBox.Controls.Add(Me.numBytesTransmittedLabel)
        Me.transmissionGroupBox.Location = New System.Drawing.Point(8, 8)
        Me.transmissionGroupBox.Name = "transmissionGroupBox"
        Me.transmissionGroupBox.Size = New System.Drawing.Size(280, 96)
        Me.transmissionGroupBox.TabIndex = 0
        Me.transmissionGroupBox.TabStop = False
        Me.transmissionGroupBox.Text = "Values transmitted"
        '
        'origValueTwoDisplay
        '
        Me.origValueTwoDisplay.Location = New System.Drawing.Point(152, 64)
        Me.origValueTwoDisplay.Name = "origValueTwoDisplay"
        Me.origValueTwoDisplay.Size = New System.Drawing.Size(120, 16)
        Me.origValueTwoDisplay.TabIndex = 11
        '
        'origValueTwoLabel
        '
        Me.origValueTwoLabel.Location = New System.Drawing.Point(8, 64)
        Me.origValueTwoLabel.Name = "origValueTwoLabel"
        Me.origValueTwoLabel.Size = New System.Drawing.Size(136, 16)
        Me.origValueTwoLabel.TabIndex = 10
        Me.origValueTwoLabel.Text = "Value 2 Transmitted"
        '
        'origValueOneDisplay
        '
        Me.origValueOneDisplay.Location = New System.Drawing.Point(152, 40)
        Me.origValueOneDisplay.Name = "origValueOneDisplay"
        Me.origValueOneDisplay.Size = New System.Drawing.Size(120, 16)
        Me.origValueOneDisplay.TabIndex = 9
        '
        'origValueOneLabel
        '
        Me.origValueOneLabel.Location = New System.Drawing.Point(8, 40)
        Me.origValueOneLabel.Name = "origValueOneLabel"
        Me.origValueOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.origValueOneLabel.TabIndex = 8
        Me.origValueOneLabel.Text = "Value 1 Transmitted"
        '
        'numBytesTransmittedDisplay
        '
        Me.numBytesTransmittedDisplay.Location = New System.Drawing.Point(152, 16)
        Me.numBytesTransmittedDisplay.Name = "numBytesTransmittedDisplay"
        Me.numBytesTransmittedDisplay.Size = New System.Drawing.Size(120, 16)
        Me.numBytesTransmittedDisplay.TabIndex = 7
        '
        'numBytesTransmittedLabel
        '
        Me.numBytesTransmittedLabel.Location = New System.Drawing.Point(8, 16)
        Me.numBytesTransmittedLabel.Name = "numBytesTransmittedLabel"
        Me.numBytesTransmittedLabel.Size = New System.Drawing.Size(136, 16)
        Me.numBytesTransmittedLabel.TabIndex = 6
        Me.numBytesTransmittedLabel.Text = "Num Bytes Transmitted:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.newValueTwoDisplay)
        Me.GroupBox1.Controls.Add(Me.newValueTwoLabel)
        Me.GroupBox1.Controls.Add(Me.newValueOneDisplay)
        Me.GroupBox1.Controls.Add(Me.newValueOneLabel)
        Me.GroupBox1.Controls.Add(Me.numBytesRecievedDisplay)
        Me.GroupBox1.Controls.Add(Me.numBytesRecievedLabel)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 120)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(280, 96)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Values recieved"
        '
        'newValueTwoDisplay
        '
        Me.newValueTwoDisplay.Location = New System.Drawing.Point(152, 64)
        Me.newValueTwoDisplay.Name = "newValueTwoDisplay"
        Me.newValueTwoDisplay.Size = New System.Drawing.Size(120, 16)
        Me.newValueTwoDisplay.TabIndex = 11
        '
        'newValueTwoLabel
        '
        Me.newValueTwoLabel.Location = New System.Drawing.Point(8, 64)
        Me.newValueTwoLabel.Name = "newValueTwoLabel"
        Me.newValueTwoLabel.Size = New System.Drawing.Size(136, 16)
        Me.newValueTwoLabel.TabIndex = 10
        Me.newValueTwoLabel.Text = "Value 2 Recieved"
        '
        'newValueOneDisplay
        '
        Me.newValueOneDisplay.Location = New System.Drawing.Point(152, 40)
        Me.newValueOneDisplay.Name = "newValueOneDisplay"
        Me.newValueOneDisplay.Size = New System.Drawing.Size(120, 16)
        Me.newValueOneDisplay.TabIndex = 9
        '
        'newValueOneLabel
        '
        Me.newValueOneLabel.Location = New System.Drawing.Point(8, 40)
        Me.newValueOneLabel.Name = "newValueOneLabel"
        Me.newValueOneLabel.Size = New System.Drawing.Size(136, 16)
        Me.newValueOneLabel.TabIndex = 8
        Me.newValueOneLabel.Text = "Value 1 Recieved"
        '
        'numBytesRecievedDisplay
        '
        Me.numBytesRecievedDisplay.Location = New System.Drawing.Point(152, 16)
        Me.numBytesRecievedDisplay.Name = "numBytesRecievedDisplay"
        Me.numBytesRecievedDisplay.Size = New System.Drawing.Size(120, 16)
        Me.numBytesRecievedDisplay.TabIndex = 7
        '
        'numBytesRecievedLabel
        '
        Me.numBytesRecievedLabel.Location = New System.Drawing.Point(8, 16)
        Me.numBytesRecievedLabel.Name = "numBytesRecievedLabel"
        Me.numBytesRecievedLabel.Size = New System.Drawing.Size(136, 16)
        Me.numBytesRecievedLabel.TabIndex = 6
        Me.numBytesRecievedLabel.Text = "Num Bytes Recieved:"
        '
        'runButton
        '
        Me.runButton.Location = New System.Drawing.Point(8, 224)
        Me.runButton.Name = "runButton"
        Me.runButton.Size = New System.Drawing.Size(272, 24)
        Me.runButton.TabIndex = 13
        Me.runButton.Text = "Run"
        '
        'notesLabel
        '
        Me.notesLabel.Location = New System.Drawing.Point(8, 256)
        Me.notesLabel.Name = "notesLabel"
        Me.notesLabel.Size = New System.Drawing.Size(272, 32)
        Me.notesLabel.TabIndex = 14
        Me.notesLabel.Text = "If FIO4 is shorted to FIO5 the values transmitted and recieved should be the same" & _
        "."
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(296, 292)
        Me.Controls.Add(Me.notesLabel)
        Me.Controls.Add(Me.runButton)
        Me.Controls.Add(Me.transmissionGroupBox)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.Text = "U3_Asynch"
        Me.transmissionGroupBox.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
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
        'long lngGetNextIteration
        'LJUD.IO ioType=0, channel=0
        'double dblValue=0

        'double numI2CBytesToWrite
        Dim numBytes As Double
        Dim array(2) As Byte
        Dim u3 As U3

        Try
            'Open the LabJack.
            u3 = New U3(LJUD.CONNECTION.USB, "0", True) ' Connection through USB

            'Start by using the pin_configuration_reset IOType so that all
            'pin assignments are in the factory default condition.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)


            ' 1 MHz timer clock base.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_BASE, LJUD.TIMERCLOCKS.MHZ1_DIV, 0)

            ' Set clock divisor to 1, so timer clock is 1 MHz.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_CLOCK_DIVISOR, LJUD.TIMERCLOCKS.MHZ1_DIV, 0)

            ' Set timer/counter pin offset to 4. TX and RX appear after any timers and counters on U3
            ' hardware rev 1.30.  We have no timers or counters enabled, so TX=FIO4 and RX=FIO5.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 4, 0)

            ' Set data rate for 9600 bps communication.
            LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.ASYNCH_BAUDFACTOR, 204, 0)

            ' Enable UART.
            LJUD.ePut(u3.ljhandle, LJUD.IO.ASYNCH_COMMUNICATION, LJUD.CHANNEL.ASYNCH_ENABLE, 1, 0)

            ' Transmit 2 bytes.
            numBytes = 3
            array(0) = 14
            array(1) = 13
            numBytesTransmittedDisplay.Text = numBytes
            origValueOneDisplay.Text = array(0)
            origValueTwoDisplay.Text = array(1)
            LJUD.eGet(u3.ljhandle, LJUD.IO.ASYNCH_COMMUNICATION, LJUD.CHANNEL.ASYNCH_TX, numBytes, array)

            ' Read 2 bytes.
            numBytes = 9999  'Dummy values so we can see them change.
            array(0) = 111
            array(1) = 111
            LJUD.eGet(u3.ljhandle, LJUD.IO.ASYNCH_COMMUNICATION, LJUD.CHANNEL.ASYNCH_RX, numBytes, array)
            numBytesRecievedDisplay.Text = numBytes
            newValueOneDisplay.Text = array(0)
            newValueTwoDisplay.Text = array(1)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

    End Sub
End Class
