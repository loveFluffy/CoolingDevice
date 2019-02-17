'---------------------------------------------------------------------------
'
'  I2C.cpp
'  
'	Demonstrates I2C communication using the LJTick-DAC, which is plugged
'  into FIO0/FIO1 for this example.  Writes/reads 4 bytes to the
'  user area (bytes 0-63) on the 24C01C EEPROM.  Reads the cal constants
'  and serial number from the EEPROM.  Updates both DACs on the LTC2617.
'
'  Note that this example is meant to demonstrate I2C communication with
'  the UD driver, not LJTick-DAC operation with the UD driver.  The UD
'  driver has high-level support for the LJTick-DAC, so the I2C
'  communication demonstrated here would not normally be used with the
'  LJTick-DAC when using the UD driver.
'
'	Tested with UD driver V2.74.
'  Tested with UE9 Comm firmware V1.43 and Control firmware V1.83.
'  Tested with U3 firmware V1.43.
'
'  support@labjack.com
'  Aug 22, 2009
'----------------------------------------------------------------------
'

'VB.NET example uses the LabJackUD driver to communicate with a U3.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U3_SPI
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
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.goButton = New System.Windows.Forms.Button
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.SuspendLayout()
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(40, 232)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(288, 24)
        Me.goButton.TabIndex = 18
        Me.goButton.Text = "Go!"
        '
        'ListBox1
        '
        Me.ListBox1.Location = New System.Drawing.Point(24, 8)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(320, 186)
        Me.ListBox1.TabIndex = 19
        '
        'U3_SPI
        '
        Me.AcceptButton = Me.goButton
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(368, 292)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.goButton)
        Me.Name = "U3_SPI"
        Me.Text = "U3_I2C"
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
        Dim lngError As Long
        Dim lngGetNextIteration As Long
        Dim lngIOType As Long
        lngIOType = 0
        Dim dblValue As Double
        dblValue = 0
        Dim ljhandle As Long
        ljhandle = 0
        Dim u3 As U3
        Dim numI2CBytesToWrite As Double
        Dim numI2CBytesToRead As Double
        Dim writeArray(128) As Byte
        Dim readArray(128) As Byte
        Dim i As Long
        i = 0
        Dim serialNumber As Long
        serialNumber = 0
        Dim slopeDACA As Double
        slopeDACA = 0
        Dim writeACKS As Double
        writeACKS = 0
        Dim lngChannel As Long
        Dim isFinished As Boolean
        Dim dblVal As Double
        Dim intVal As Integer
        Dim expectedACKS As Double
        Dim RandomClass As New Random
        Dim offsetDACA As Double
        Dim offsetDACB As Double
        Dim slopeDACB As Double

        'Open the LabJack.
        u3 = New U3(LJUD.CONNECTION.USB, "0", True) ' Connection through USB

        'Configure the I2C communication.
        'The address of the EEPROM on the LJTick-DAC is 0xA0.
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_ADDRESS_BYTE, 160, 0, 0)

        'SCL is FIO0
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_SCL_PIN_NUM, 0, 0, 0)

        'SDA is FIO1
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_SDA_PIN_NUM, 1, 0, 0)

        'See description of low-level I2C function.
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_OPTIONS, 0, 0, 0)

        'See description of low-level I2C function.  0 is max speed of about 130 kHz.
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_SPEED_ADJUST, 0, 0, 0)

        'Execute the requests on a single LabJack.
        LJUD.GoOne(u3.ljhandle)

        'Get all the results just to check for errors.
        LJUD.GetFirstResult(u3.ljhandle, lngIOType, lngChannel, dblValue, 0, 0)
        lngGetNextIteration = 0 'Used by the error handling function.

        While (Not isFinished)
            Try
                LJUD.GetNextResult(u3.ljhandle, lngIOType, lngChannel, dblVal, intVal, dblVal)
            Catch ex As LabJackUDException
                If ex.LJUDError = u3.LJUDERROR.NO_MORE_DATA_AVAILABLE Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        'Initial read of EEPROM bytes 0-3 in the user memory area.
        'We need a single I2C transmission that writes the address and then reads
        'the data.  That is, there needs to be an ack after writing the address,
        'not a stop condition.  To accomplish this, we use Add/Go/Get to combine
        'the write and read into a single low-level call.
        numI2CBytesToWrite = 1
        writeArray(0) = 0  'Memory address.  User area is 0-63.
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, numI2CBytesToWrite, writeArray, 0)

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, 0, 0, 0)

        numI2CBytesToRead = 4
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_READ, numI2CBytesToRead, readArray, 0)

        'Execute the requests.
        LJUD.GoOne(u3.ljhandle)

        'Get the result of the write just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, 0)

        'Get the write ACKs and compare to the expected value.  We expect bit 0 to be
        'the ACK of the last data byte progressing up to the ACK of the address
        'byte (data bytes only for Control firmware 1.43 and less).  So if n is the
        'number of data bytes, the ACKs value should be (2^(n+1))-1.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, writeACKS)
        expectedACKS = Math.Pow(2, numI2CBytesToWrite + 1) - 1
        If (writeACKS <> expectedACKS) Then
            MsgBox("Expected ACKs = " + Str(expectedACKS) + ", " + "Received ACKs = " + Str(writeACKS))
        End If

        'When the GoOne processed the read request, the read data was put into the readArray buffer that
        'we passed, so this GetResult is also just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_READ, 0)

        'Display the first 4 elements.
        ListBox1.Items.Add("Read User Mem (0-3) = " + Str(readArray(0)) + ", " + Str(readArray(1)) + ", " + Str(readArray(2)) + ", " + Str(readArray(3)))

        'Write EEPROM bytes 0-3 in the user memory area, using the page write technique.  Note
        'that page writes are limited to 16 bytes max, and must be aligned with the 16-byte
        'page intervals.  For instance, if you start writing at address 14, you can only write
        'two bytes because byte 16 is the start of a new page.
        numI2CBytesToWrite = 5
        writeArray(0) = 0  'Memory address.  User area is 0-63.

        'Create 4 new pseudo-random numbers to write.
        For i = 1 To 4
            writeArray(i) = 255 * (RandomClass.Next(0, 255) / 255)
        Next i

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, numI2CBytesToWrite, writeArray, 0)

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, 0, 0, 0)

        'Execute the requests.
        LJUD.GoOne(u3.ljhandle)

        'Get the result of the write just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, 0)

        'Get the write ACKs and compare to the expected value.  We expect bit 0 to be
        'the ACK of the last data byte progressing up to the ACK of the address
        'byte (data bytes only for Control firmware 1.43 and less).  So if n is the
        'number of data bytes, the ACKs value should be (2^(n+1))-1.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, writeACKS)
        expectedACKS = Math.Pow(2, numI2CBytesToWrite + 1) - 1
        If (writeACKS <> expectedACKS) Then
            MsgBox("Expected ACKs = " + Str(expectedACKS) + ", " + "Received ACKs = " + Str(writeACKS))
        End If

        'Delay to allow the EEPROM to complete the write cycle.  Datasheet says 1.5 ms max.
        Sleep(2)

        ListBox1.Items.Add("Write User Mem (0-3) = " + Str(writeArray(1)) + ", " + Str(writeArray(2)) + ", " + Str(writeArray(3)) + ", " + Str(writeArray(4)))

        'Final read of EEPROM bytes 0-3 in the user memory area.
        'We need a single I2C transmission that writes the address and then reads
        'the data.  That is, there needs to be an ack after writing the address,
        'not a stop condition.  To accomplish this, we use Add/Go/Get to combine
        'the write and read into a single low-level call.
        numI2CBytesToWrite = 1
        writeArray(0) = 0  'Memory address.  User area is 0-63.
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, numI2CBytesToWrite, writeArray, 0)

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, 0, 0, 0)

        numI2CBytesToRead = 4
        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_READ, numI2CBytesToRead, readArray, 0)

        'Execute the requests.
        LJUD.GoOne(u3.ljhandle)

        'Get the result of the write just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, 0)

        'Get the write ACKs and compare to the expected value.  We expect bit 0 to be
        'the ACK of the last data byte progressing up to the ACK of the address
        'byte (data bytes only for Control firmware 1.43 and less).  So if n is the
        'number of data bytes, the ACKs value should be (2^(n+1))-1.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, writeACKS)
        expectedACKS = Math.Pow(2, numI2CBytesToWrite + 1) - 1
        If (writeACKS <> expectedACKS) Then
            MsgBox("Expected ACKs = " + Str(expectedACKS) + ", " + "Received ACKs = " + Str(writeACKS))
        End If

        'When the GoOne processed the read request, the read data was put into the readArray buffer that
        'we passed, so this GetResult is also just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_READ, 0)
        If lngError <> 0 Then Err.Raise(lngError + 50000)

        'Display the first 4 elements.
        ListBox1.Items.Add("Read User Mem (0-3) = " + Str(readArray(0)) + ", " + Str(readArray(1)) + ", " + Str(readArray(2)) + ", " + Str(readArray(3)))

        'Update both DAC outputs.

        'Set the I2C address in the UD driver so that we not talk to the DAC chip.
        'The address of the DAC chip on the LJTick-DAC is 0x24.
        LJUD.ePut(u3.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.I2C_ADDRESS_BYTE, 36, 0)

        'Set DACA to 1.2 volts.
        numI2CBytesToWrite = 3
        writeArray(0) = 48  'Write and update DACA.
        writeArray(1) = ((1.2 * slopeDACA) + offsetDACA) / 256 'Upper byte of binary DAC value.
        writeArray(2) = ((1.2 * slopeDACA) + offsetDACA) Mod 256 'Lower byte of binary DAC value.

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, numI2CBytesToWrite, writeArray, 0)

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, 0, 0, 0)

        'Execute the requests.
        LJUD.GoOne(u3.ljhandle)

        'Get the result of the write just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, 0)

        'Get the write ACKs and compare to the expected value.  We expect bit 0 to be
        'the ACK of the last data byte progressing up to the ACK of the address
        'byte (data bytes only for Control firmware 1.43 and less).  So if n is the
        'number of data bytes, the ACKs value should be (2^(n+1))-1.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, writeACKS)
        expectedACKS = Math.Pow(2, numI2CBytesToWrite + 1) - 1
        If (writeACKS <> expectedACKS) Then
            MsgBox("Expected ACKs = " + Str(expectedACKS) + ", " + "Received ACKs = " + Str(writeACKS))
        End If

        ListBox1.Items.Add("DACA set to 1.2 volts")


        'Set DACB to 2.3 volts.
        numI2CBytesToWrite = 3
        writeArray(0) = 49  'Write and update DACB.
        writeArray(1) = ((2.3 * slopeDACB) + offsetDACB) / 256 'Upper byte of binary DAC value.
        writeArray(2) = ((2.3 * slopeDACB) + offsetDACB) Mod 256 'Lower byte of binary DAC value.

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, numI2CBytesToWrite, writeArray, 0)

        LJUD.AddRequest(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, 0, 0, 0)

        'Execute the requests.
        LJUD.GoOne(u3.ljhandle)

        'Get the result of the write just to check for an error.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_WRITE, 0)

        'Get the write ACKs and compare to the expected value.  We expect bit 0 to be
        'the ACK of the last data byte progressing up to the ACK of the address
        'byte (data bytes only for Control firmware 1.43 and less).  So if n is the
        'number of data bytes, the ACKs value should be (2^(n+1))-1.
        LJUD.GetResult(u3.ljhandle, LJUD.IO.I2C_COMMUNICATION, LJUD.CHANNEL.I2C_GET_ACKS, writeACKS)
        expectedACKS = Math.Pow(2, numI2CBytesToWrite + 1) - 1
        If (writeACKS <> expectedACKS) Then
            MsgBox("Expected ACKs = %.0f, Received ACKs = %0.f\n", expectedACKS, writeACKS)
        End If

        ListBox1.Items.Add("DACB set to 2.3 volts")

    End Sub

    Private Sub dataZeroInLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class
