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
    Friend WithEvents iterationTimeLabel As System.Windows.Forms.Label
    Friend WithEvents digitalInputLabel As System.Windows.Forms.Label
    Friend WithEvents iterationTimeDisplay As System.Windows.Forms.Label
    Friend WithEvents digitalInputDisplay As System.Windows.Forms.Label
    Friend WithEvents ainGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents ainListBox As System.Windows.Forms.ListBox
    Friend WithEvents runButton As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.iterationTimeLabel = New System.Windows.Forms.Label
        Me.digitalInputLabel = New System.Windows.Forms.Label
        Me.iterationTimeDisplay = New System.Windows.Forms.Label
        Me.digitalInputDisplay = New System.Windows.Forms.Label
        Me.ainGroupBox = New System.Windows.Forms.GroupBox
        Me.ainListBox = New System.Windows.Forms.ListBox
        Me.runButton = New System.Windows.Forms.Button
        Me.ainGroupBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'iterationTimeLabel
        '
        Me.iterationTimeLabel.Location = New System.Drawing.Point(8, 16)
        Me.iterationTimeLabel.Name = "iterationTimeLabel"
        Me.iterationTimeLabel.Size = New System.Drawing.Size(168, 16)
        Me.iterationTimeLabel.TabIndex = 0
        Me.iterationTimeLabel.Text = "Millesecounds per iteration: "
        '
        'digitalInputLabel
        '
        Me.digitalInputLabel.Location = New System.Drawing.Point(8, 48)
        Me.digitalInputLabel.Name = "digitalInputLabel"
        Me.digitalInputLabel.Size = New System.Drawing.Size(168, 16)
        Me.digitalInputLabel.TabIndex = 1
        Me.digitalInputLabel.Text = "Digital input:"
        '
        'iterationTimeDisplay
        '
        Me.iterationTimeDisplay.Location = New System.Drawing.Point(176, 16)
        Me.iterationTimeDisplay.Name = "iterationTimeDisplay"
        Me.iterationTimeDisplay.Size = New System.Drawing.Size(168, 16)
        Me.iterationTimeDisplay.TabIndex = 2
        '
        'digitalInputDisplay
        '
        Me.digitalInputDisplay.Location = New System.Drawing.Point(176, 48)
        Me.digitalInputDisplay.Name = "digitalInputDisplay"
        Me.digitalInputDisplay.Size = New System.Drawing.Size(168, 16)
        Me.digitalInputDisplay.TabIndex = 3
        '
        'ainGroupBox
        '
        Me.ainGroupBox.Controls.Add(Me.ainListBox)
        Me.ainGroupBox.Location = New System.Drawing.Point(8, 72)
        Me.ainGroupBox.Name = "ainGroupBox"
        Me.ainGroupBox.Size = New System.Drawing.Size(336, 184)
        Me.ainGroupBox.TabIndex = 4
        Me.ainGroupBox.TabStop = False
        Me.ainGroupBox.Text = "AIN readings from last iteration"
        '
        'ainListBox
        '
        Me.ainListBox.Location = New System.Drawing.Point(16, 16)
        Me.ainListBox.Name = "ainListBox"
        Me.ainListBox.Size = New System.Drawing.Size(320, 160)
        Me.ainListBox.TabIndex = 0
        '
        'runButton
        '
        Me.runButton.Location = New System.Drawing.Point(80, 264)
        Me.runButton.Name = "runButton"
        Me.runButton.Size = New System.Drawing.Size(192, 24)
        Me.runButton.TabIndex = 5
        Me.runButton.Text = "Run"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(352, 292)
        Me.Controls.Add(Me.runButton)
        Me.Controls.Add(Me.ainGroupBox)
        Me.Controls.Add(Me.digitalInputDisplay)
        Me.Controls.Add(Me.iterationTimeDisplay)
        Me.Controls.Add(Me.digitalInputLabel)
        Me.Controls.Add(Me.iterationTimeLabel)
        Me.Name = "Form1"
        Me.Text = "U9_AllIO"
        Me.ainGroupBox.ResumeLayout(False)
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

        ' Clear the ain values
        ainListBox.Items.Clear()

        ' Variables to pass to and from the driver object
        Dim ainResolution As Long
        ainResolution = 12
        Dim IOType As UE9.IO
        IOType = 0
        Dim channel As UE9.CHANNEL
        channel = 0
        Dim val As Double
        val = 0
        Dim dblVal As Double
        dblVal = 0
        Dim intVal As Integer
        intVal = 0
        Dim ue9 As UE9

        ' General variable settings
        Dim numIterations As Long
        numIterations = 1000
        Dim numChannels As Long
        numChannels = 6

        ' Variables to store results
        Dim ValueDIPort As Double
        ValueDIPort = 0
        Dim ValueAIN(16) As Double

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Try
            ' Set DAC0 to 2.5 volts
            LJUD.AddRequest(ue9.ljhandle, ue9.IO.PUT_DAC, 0, 2.5, 0, 0)

            ' Set DAC1 to 3.5 volts
            LJUD.AddRequest(ue9.ljhandle, ue9.IO.PUT_DAC, 1, 3.5, 0, 0)

            'Write all digital I/O.  Doing a bunch of bit instructions, rather than
            'the following port instruction, should not make a noticable difference
            'in the overall execution time.
            LJUD.AddRequest(ue9.ljhandle, ue9.IO.PUT_DIGITAL_PORT, 0, 0, 23, 0)

            'Configure the desired resolution.  Note that depending on resolution and
            'number of analog inputs, numIterations might need to be reduced from the
            'default above so the program does not take too long to execute.
            LJUD.AddRequest(ue9.ljhandle, ue9.IO.PUT_CONFIG, ue9.CHANNEL.AIN_RESOLUTION, ainResolution, 0, 0)

            'Add analog input requests.
            For j As Integer = 0 To numChannels - 1
                LJUD.AddRequest(ue9.ljhandle, ue9.IO.GET_AIN, j, 0, 0, 0)
            Next j

            'Request a read of all digital I/O.  Doing a bunch of bit instructions,
            'rather than the following port instruction, should not make a noticable
            'difference in the overall execution time.
            LJUD.AddRequest(ue9.ljhandle, ue9.IO.GET_DIGITAL_PORT, 0, 0, 23, 0)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        ' Get the current tick count for a reference frame
        Dim time As Double
        time = System.Environment.TickCount

        For i As Integer = 0 To numIterations - 1

            Try
                'Execute the requests.
                LJUD.GoOne(ue9.ljhandle)

                'Get all the results.  The input measurement results are stored.  All other
                'results are for configuration or output requests so we are just checking
                'whether there was an error.
                LJUD.GetFirstResult(ue9.ljhandle, IOType, channel, val, intVal, dblVal)

            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try


            ' Get results until there is no more data available
            Dim isFinished As Boolean
            isFinished = False
            While Not isFinished
                Select Case IOType

                    Case LJUD.IO.GET_AIN
                        ValueAIN(channel) = val

                    Case LJUD.IO.GET_DIGITAL_PORT
                        ValueDIPort = val

                End Select

                Try
                    LJUD.GetNextResult(ue9.ljhandle, IOType, channel, val, intVal, dblVal)

                Catch ex As LabJackUDException

                    ' If we get an error, report it.  If there is no more data available we are done
                    If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                        isFinished = True
                    Else
                        showErrorMessage(ex)
                    End If
                End Try
            End While
        Next i

        ' Determine how long it took to preform the above tasks and display the millisecounds per iteration along with the readings
        time = System.Environment.TickCount - time

        iterationTimeDisplay.Text = (time / numIterations)

        digitalInputDisplay.Text = ValueDIPort

        For j As Integer = 0 To numChannels - 1
            ainListBox.Items.Add(ValueAIN(j).ToString("0.###"))
        Next j

    End Sub
End Class
