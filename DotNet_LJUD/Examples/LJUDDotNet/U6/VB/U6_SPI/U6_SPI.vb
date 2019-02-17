'---------------------------------------------------------------------------
'
'  U6_SPI.vb
' 
'	Demonstrates SPI communication.
'
'	You can short MOSI to MISO for testing.
'
'	MOSI    FIO2
'	MISO    FIO3
'	CLK     FIO0
'	CS      FIO1
'
'	If you short MISO to MOSI, then you will read back the same bytes that you write.  If you short
'	MISO to GND, then you will read back zeros.  If you short MISO to VS or leave it
'	unconnected, you will read back 255s.
'
'  support@labjack.com
'  June 5, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a U6.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U6_SPI
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
    Friend WithEvents dataZeroInLabel As System.Windows.Forms.Label
    Friend WithEvents dataOneInLabel As System.Windows.Forms.Label
    Friend WithEvents dataTwoInLabel As System.Windows.Forms.Label
    Friend WithEvents dataThreeInLabel As System.Windows.Forms.Label
    Friend WithEvents dataThreeOutLabel As System.Windows.Forms.Label
    Friend WithEvents dataTwoOutLabel As System.Windows.Forms.Label
    Friend WithEvents dataOneOutLabel As System.Windows.Forms.Label
    Friend WithEvents dataZeroOutLabel As System.Windows.Forms.Label
    Friend WithEvents dataZeroInDisplay As System.Windows.Forms.Label
    Friend WithEvents dataOneInDisplay As System.Windows.Forms.Label
    Friend WithEvents dataTwoInDisplay As System.Windows.Forms.Label
    Friend WithEvents dataThreeInDisplay As System.Windows.Forms.Label
    Friend WithEvents dataZeroOutDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents dataOneOutDisplay As System.Windows.Forms.Label
    Friend WithEvents dataTwoOutDisplay As System.Windows.Forms.Label
    Friend WithEvents dataThreeOutDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.dataZeroInLabel = New System.Windows.Forms.Label
        Me.dataOneInLabel = New System.Windows.Forms.Label
        Me.dataTwoInLabel = New System.Windows.Forms.Label
        Me.dataThreeInLabel = New System.Windows.Forms.Label
        Me.dataThreeOutLabel = New System.Windows.Forms.Label
        Me.dataTwoOutLabel = New System.Windows.Forms.Label
        Me.dataOneOutLabel = New System.Windows.Forms.Label
        Me.dataZeroOutLabel = New System.Windows.Forms.Label
        Me.dataZeroInDisplay = New System.Windows.Forms.Label
        Me.dataOneInDisplay = New System.Windows.Forms.Label
        Me.dataTwoInDisplay = New System.Windows.Forms.Label
        Me.dataThreeInDisplay = New System.Windows.Forms.Label
        Me.dataZeroOutDisplay = New System.Windows.Forms.Label
        Me.dataOneOutDisplay = New System.Windows.Forms.Label
        Me.dataTwoOutDisplay = New System.Windows.Forms.Label
        Me.dataThreeOutDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 248)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(416, 16)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "If you short MISO to MOSI, then you will read back the same bytes that you write." & _
        ""
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(8, 272)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(376, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "If you short MISO to VS or leave it unconnected, you will read back 255s."
        '
        'dataZeroInLabel
        '
        Me.dataZeroInLabel.Location = New System.Drawing.Point(8, 8)
        Me.dataZeroInLabel.Name = "dataZeroInLabel"
        Me.dataZeroInLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataZeroInLabel.TabIndex = 2
        Me.dataZeroInLabel.Text = "dataArray(0) in:"
        '
        'dataOneInLabel
        '
        Me.dataOneInLabel.Location = New System.Drawing.Point(8, 32)
        Me.dataOneInLabel.Name = "dataOneInLabel"
        Me.dataOneInLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataOneInLabel.TabIndex = 3
        Me.dataOneInLabel.Text = "dataArray(1) in:"
        '
        'dataTwoInLabel
        '
        Me.dataTwoInLabel.Location = New System.Drawing.Point(8, 56)
        Me.dataTwoInLabel.Name = "dataTwoInLabel"
        Me.dataTwoInLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataTwoInLabel.TabIndex = 4
        Me.dataTwoInLabel.Text = "dataArray(2) in:"
        '
        'dataThreeInLabel
        '
        Me.dataThreeInLabel.Location = New System.Drawing.Point(8, 80)
        Me.dataThreeInLabel.Name = "dataThreeInLabel"
        Me.dataThreeInLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataThreeInLabel.TabIndex = 5
        Me.dataThreeInLabel.Text = "dataArray(3) in:"
        '
        'dataThreeOutLabel
        '
        Me.dataThreeOutLabel.Location = New System.Drawing.Point(8, 176)
        Me.dataThreeOutLabel.Name = "dataThreeOutLabel"
        Me.dataThreeOutLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataThreeOutLabel.TabIndex = 9
        Me.dataThreeOutLabel.Text = "dataArray(3) out:"
        '
        'dataTwoOutLabel
        '
        Me.dataTwoOutLabel.Location = New System.Drawing.Point(8, 152)
        Me.dataTwoOutLabel.Name = "dataTwoOutLabel"
        Me.dataTwoOutLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataTwoOutLabel.TabIndex = 8
        Me.dataTwoOutLabel.Text = "dataArray(2) out:"
        '
        'dataOneOutLabel
        '
        Me.dataOneOutLabel.Location = New System.Drawing.Point(8, 128)
        Me.dataOneOutLabel.Name = "dataOneOutLabel"
        Me.dataOneOutLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataOneOutLabel.TabIndex = 7
        Me.dataOneOutLabel.Text = "dataArray(1) out:"
        '
        'dataZeroOutLabel
        '
        Me.dataZeroOutLabel.Location = New System.Drawing.Point(8, 104)
        Me.dataZeroOutLabel.Name = "dataZeroOutLabel"
        Me.dataZeroOutLabel.Size = New System.Drawing.Size(176, 16)
        Me.dataZeroOutLabel.TabIndex = 6
        Me.dataZeroOutLabel.Text = "dataArray(0) out:"
        '
        'dataZeroInDisplay
        '
        Me.dataZeroInDisplay.Location = New System.Drawing.Point(296, 8)
        Me.dataZeroInDisplay.Name = "dataZeroInDisplay"
        Me.dataZeroInDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataZeroInDisplay.TabIndex = 10
        '
        'dataOneInDisplay
        '
        Me.dataOneInDisplay.Location = New System.Drawing.Point(296, 32)
        Me.dataOneInDisplay.Name = "dataOneInDisplay"
        Me.dataOneInDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataOneInDisplay.TabIndex = 11
        '
        'dataTwoInDisplay
        '
        Me.dataTwoInDisplay.Location = New System.Drawing.Point(296, 56)
        Me.dataTwoInDisplay.Name = "dataTwoInDisplay"
        Me.dataTwoInDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataTwoInDisplay.TabIndex = 12
        '
        'dataThreeInDisplay
        '
        Me.dataThreeInDisplay.Location = New System.Drawing.Point(296, 80)
        Me.dataThreeInDisplay.Name = "dataThreeInDisplay"
        Me.dataThreeInDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataThreeInDisplay.TabIndex = 13
        '
        'dataZeroOutDisplay
        '
        Me.dataZeroOutDisplay.Location = New System.Drawing.Point(296, 104)
        Me.dataZeroOutDisplay.Name = "dataZeroOutDisplay"
        Me.dataZeroOutDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataZeroOutDisplay.TabIndex = 14
        '
        'dataOneOutDisplay
        '
        Me.dataOneOutDisplay.Location = New System.Drawing.Point(296, 128)
        Me.dataOneOutDisplay.Name = "dataOneOutDisplay"
        Me.dataOneOutDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataOneOutDisplay.TabIndex = 15
        '
        'dataTwoOutDisplay
        '
        Me.dataTwoOutDisplay.Location = New System.Drawing.Point(296, 152)
        Me.dataTwoOutDisplay.Name = "dataTwoOutDisplay"
        Me.dataTwoOutDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataTwoOutDisplay.TabIndex = 16
        '
        'dataThreeOutDisplay
        '
        Me.dataThreeOutDisplay.Location = New System.Drawing.Point(296, 176)
        Me.dataThreeOutDisplay.Name = "dataThreeOutDisplay"
        Me.dataThreeOutDisplay.Size = New System.Drawing.Size(152, 16)
        Me.dataThreeOutDisplay.TabIndex = 17
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(64, 208)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(288, 24)
        Me.goButton.TabIndex = 18
        Me.goButton.Text = "Go!"
        '
        'U6_SPI
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(448, 292)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.dataThreeOutDisplay)
        Me.Controls.Add(Me.dataTwoOutDisplay)
        Me.Controls.Add(Me.dataOneOutDisplay)
        Me.Controls.Add(Me.dataZeroOutDisplay)
        Me.Controls.Add(Me.dataThreeInDisplay)
        Me.Controls.Add(Me.dataTwoInDisplay)
        Me.Controls.Add(Me.dataOneInDisplay)
        Me.Controls.Add(Me.dataZeroInDisplay)
        Me.Controls.Add(Me.dataThreeOutLabel)
        Me.Controls.Add(Me.dataTwoOutLabel)
        Me.Controls.Add(Me.dataOneOutLabel)
        Me.Controls.Add(Me.dataZeroOutLabel)
        Me.Controls.Add(Me.dataThreeInLabel)
        Me.Controls.Add(Me.dataTwoInLabel)
        Me.Controls.Add(Me.dataOneInLabel)
        Me.Controls.Add(Me.dataZeroInLabel)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Name = "U6_SPI"
        Me.Text = "U6_SPI"
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
        Dim numSPIBytesToTransfer As Double
        Dim dataArray(50) As Byte

        ' Variables to satsify certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0
        Dim u6 As U6

        Try
            'Open the LabJack U6.
            u6 = New U6(LJUD.CONNECTION.USB, "0", True)

            'First, configure the SPI communication.

            'Enable automatic chip-select control.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_AUTO_CS, 1, 0, 0)

            'Do not disable automatic digital i/o direction configuration.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_DISABLE_DIR_CONFIG, 0, 0, 0)

            'Mode A:  CPHA=1, CPOL=1.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MODE, 0, 0, 0)

            '125kHz clock.
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLOCK_FACTOR, 0, 0, 0)

            'MOSI is FIO2
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MOSI_PIN_NUM, 2, 0, 0)

            'MISO is FIO3
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_MISO_PIN_NUM, 3, 0, 0)

            'CLK is FIO0
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CLK_PIN_NUM, 0, 0, 0)

            'CS is FIO1
            LJUD.AddRequest(u6.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.SPI_CS_PIN_NUM, 1, 0, 0)


            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(u6.ljhandle)

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(u6.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Dim finished As Boolean
        finished = False
        While Not finished
            Try
                LJUD.GetNextResult(u6.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If ex.LJUDError = UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE Then
                    finished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        'This example transfers 4 test bytes.
        numSPIBytesToTransfer = 4
        dataArray(0) = 170
        dataArray(1) = 240
        dataArray(2) = 170
        dataArray(3) = 240
        dataZeroInDisplay.Text = dataArray(0).ToString("0.#")
        dataOneInDisplay.Text = dataArray(1).ToString("0.#")
        dataTwoInDisplay.Text = dataArray(2).ToString("0.#")
        dataThreeInDisplay.Text = dataArray(3).ToString("0.#")

        'Transfer the data.  The write and read is done at the same time.
        Try
            LJUD.eGet(u6.ljhandle, LJUD.IO.SPI_COMMUNICATION, 0, numSPIBytesToTransfer, dataArray)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        'Display the read data.
        dataZeroOutDisplay.Text = dataArray(0).ToString("0.#")
        dataOneOutDisplay.Text = dataArray(1).ToString("0.#")
        dataTwoOutDisplay.Text = dataArray(2).ToString("0.#")
        dataThreeOutDisplay.Text = dataArray(3).ToString("0.#")

    End Sub
End Class
