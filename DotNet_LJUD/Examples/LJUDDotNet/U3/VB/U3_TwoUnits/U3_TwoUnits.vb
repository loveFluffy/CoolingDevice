'---------------------------------------------------------------------------
'
'  U3_TwoUnits.vb
' 
'	Simple example demonstrates communication with 2 U3s.
'   Local ids can be set from LJControlPannel
'
'  support@labjack.com
'  June 15, 2009
'----------------------------------------------------------------------
'


'VB.NET example uses the LabJackUD driver to communicate with a U3.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U3_TwoUnits
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
    Friend WithEvents driverLabel As System.Windows.Forms.Label
    Friend WithEvents driverDisplay As System.Windows.Forms.Label
    Friend WithEvents unitTwoLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINTwoLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINThreeLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINFourLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINFourDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINTwoLabel As System.Windows.Forms.Label
    Friend WithEvents unitThreeLabel As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINFourDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINFourLabel As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINThreeLabel As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents unitTwoAINThreeDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.driverLabel = New System.Windows.Forms.Label
        Me.driverDisplay = New System.Windows.Forms.Label
        Me.unitTwoLabel = New System.Windows.Forms.Label
        Me.unitTwoAINTwoLabel = New System.Windows.Forms.Label
        Me.unitTwoAINTwoDisplay = New System.Windows.Forms.Label
        Me.unitTwoAINThreeLabel = New System.Windows.Forms.Label
        Me.unitTwoAINThreeDisplay = New System.Windows.Forms.Label
        Me.unitTwoAINFourLabel = New System.Windows.Forms.Label
        Me.unitTwoAINFourDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINFourDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINFourLabel = New System.Windows.Forms.Label
        Me.unitThreeAINThreeDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINThreeLabel = New System.Windows.Forms.Label
        Me.unitThreeAINTwoDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINTwoLabel = New System.Windows.Forms.Label
        Me.unitThreeLabel = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'driverLabel
        '
        Me.driverLabel.Location = New System.Drawing.Point(8, 8)
        Me.driverLabel.Name = "driverLabel"
        Me.driverLabel.Size = New System.Drawing.Size(112, 16)
        Me.driverLabel.TabIndex = 0
        Me.driverLabel.Text = "UD Driver Version:"
        '
        'driverDisplay
        '
        Me.driverDisplay.Location = New System.Drawing.Point(128, 8)
        Me.driverDisplay.Name = "driverDisplay"
        Me.driverDisplay.Size = New System.Drawing.Size(96, 16)
        Me.driverDisplay.TabIndex = 1
        '
        'unitTwoLabel
        '
        Me.unitTwoLabel.Location = New System.Drawing.Point(8, 32)
        Me.unitTwoLabel.Name = "unitTwoLabel"
        Me.unitTwoLabel.Size = New System.Drawing.Size(112, 16)
        Me.unitTwoLabel.TabIndex = 2
        Me.unitTwoLabel.Text = "Unit 2:"
        '
        'unitTwoAINTwoLabel
        '
        Me.unitTwoAINTwoLabel.Location = New System.Drawing.Point(16, 56)
        Me.unitTwoAINTwoLabel.Name = "unitTwoAINTwoLabel"
        Me.unitTwoAINTwoLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitTwoAINTwoLabel.TabIndex = 3
        Me.unitTwoAINTwoLabel.Text = "AIN2:"
        '
        'unitTwoAINTwoDisplay
        '
        Me.unitTwoAINTwoDisplay.Location = New System.Drawing.Point(128, 56)
        Me.unitTwoAINTwoDisplay.Name = "unitTwoAINTwoDisplay"
        Me.unitTwoAINTwoDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitTwoAINTwoDisplay.TabIndex = 4
        '
        'unitTwoAINThreeLabel
        '
        Me.unitTwoAINThreeLabel.Location = New System.Drawing.Point(16, 80)
        Me.unitTwoAINThreeLabel.Name = "unitTwoAINThreeLabel"
        Me.unitTwoAINThreeLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitTwoAINThreeLabel.TabIndex = 5
        Me.unitTwoAINThreeLabel.Text = "AIN3:"
        '
        'unitTwoAINThreeDisplay
        '
        Me.unitTwoAINThreeDisplay.Location = New System.Drawing.Point(128, 80)
        Me.unitTwoAINThreeDisplay.Name = "unitTwoAINThreeDisplay"
        Me.unitTwoAINThreeDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitTwoAINThreeDisplay.TabIndex = 6
        '
        'unitTwoAINFourLabel
        '
        Me.unitTwoAINFourLabel.Location = New System.Drawing.Point(16, 104)
        Me.unitTwoAINFourLabel.Name = "unitTwoAINFourLabel"
        Me.unitTwoAINFourLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitTwoAINFourLabel.TabIndex = 7
        Me.unitTwoAINFourLabel.Text = "AIN4:"
        '
        'unitTwoAINFourDisplay
        '
        Me.unitTwoAINFourDisplay.Location = New System.Drawing.Point(128, 104)
        Me.unitTwoAINFourDisplay.Name = "unitTwoAINFourDisplay"
        Me.unitTwoAINFourDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitTwoAINFourDisplay.TabIndex = 8
        '
        'unitThreeAINFourDisplay
        '
        Me.unitThreeAINFourDisplay.Location = New System.Drawing.Point(128, 200)
        Me.unitThreeAINFourDisplay.Name = "unitThreeAINFourDisplay"
        Me.unitThreeAINFourDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitThreeAINFourDisplay.TabIndex = 15
        '
        'unitThreeAINFourLabel
        '
        Me.unitThreeAINFourLabel.Location = New System.Drawing.Point(16, 200)
        Me.unitThreeAINFourLabel.Name = "unitThreeAINFourLabel"
        Me.unitThreeAINFourLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitThreeAINFourLabel.TabIndex = 14
        Me.unitThreeAINFourLabel.Text = "AIN4:"
        '
        'unitThreeAINThreeDisplay
        '
        Me.unitThreeAINThreeDisplay.Location = New System.Drawing.Point(128, 176)
        Me.unitThreeAINThreeDisplay.Name = "unitThreeAINThreeDisplay"
        Me.unitThreeAINThreeDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitThreeAINThreeDisplay.TabIndex = 13
        '
        'unitThreeAINThreeLabel
        '
        Me.unitThreeAINThreeLabel.Location = New System.Drawing.Point(16, 176)
        Me.unitThreeAINThreeLabel.Name = "unitThreeAINThreeLabel"
        Me.unitThreeAINThreeLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitThreeAINThreeLabel.TabIndex = 12
        Me.unitThreeAINThreeLabel.Text = "AIN3:"
        '
        'unitThreeAINTwoDisplay
        '
        Me.unitThreeAINTwoDisplay.Location = New System.Drawing.Point(128, 152)
        Me.unitThreeAINTwoDisplay.Name = "unitThreeAINTwoDisplay"
        Me.unitThreeAINTwoDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitThreeAINTwoDisplay.TabIndex = 11
        '
        'unitThreeAINTwoLabel
        '
        Me.unitThreeAINTwoLabel.Location = New System.Drawing.Point(16, 152)
        Me.unitThreeAINTwoLabel.Name = "unitThreeAINTwoLabel"
        Me.unitThreeAINTwoLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitThreeAINTwoLabel.TabIndex = 10
        Me.unitThreeAINTwoLabel.Text = "AIN2:"
        '
        'unitThreeLabel
        '
        Me.unitThreeLabel.Location = New System.Drawing.Point(8, 128)
        Me.unitThreeLabel.Name = "unitThreeLabel"
        Me.unitThreeLabel.Size = New System.Drawing.Size(112, 16)
        Me.unitThreeLabel.TabIndex = 9
        Me.unitThreeLabel.Text = "Unit 3:"
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 224)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(208, 24)
        Me.goButton.TabIndex = 16
        Me.goButton.Text = "Go!"
        '
        'U3_TwoUnits
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(224, 264)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.unitThreeAINFourDisplay)
        Me.Controls.Add(Me.unitThreeAINFourLabel)
        Me.Controls.Add(Me.unitThreeAINThreeDisplay)
        Me.Controls.Add(Me.unitThreeAINThreeLabel)
        Me.Controls.Add(Me.unitThreeAINTwoDisplay)
        Me.Controls.Add(Me.unitThreeAINTwoLabel)
        Me.Controls.Add(Me.unitThreeLabel)
        Me.Controls.Add(Me.unitTwoAINFourDisplay)
        Me.Controls.Add(Me.unitTwoAINFourLabel)
        Me.Controls.Add(Me.unitTwoAINThreeDisplay)
        Me.Controls.Add(Me.unitTwoAINThreeLabel)
        Me.Controls.Add(Me.unitTwoAINTwoDisplay)
        Me.Controls.Add(Me.unitTwoAINTwoLabel)
        Me.Controls.Add(Me.unitTwoLabel)
        Me.Controls.Add(Me.driverDisplay)
        Me.Controls.Add(Me.driverLabel)
        Me.Name = "U3_TwoUnits"
        Me.Text = "U3_TwoUnits"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub goButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        Dim dblDriverVersion As Double
        Dim ioType As LJUD.IO
        ioType = 0
        Dim channel As LJUD.CHANNEL
        channel = 0
        Dim dblValue As Double
        dblValue = 0
        Dim Value22 As Double
        Value22 = 9999
        Dim Value32 As Double
        Value32 = 9999
        Dim Value42 As Double
        Value42 = 9999
        Dim Value23 As Double
        Value23 = 9999
        Dim Value33 As Double
        Value33 = 9999
        Dim Value43 As Double
        Value22 = 9999
        Dim unit2, unit3 As U3

        ' Variables to satisfy certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0

        'Read and display the UD version.
        dblDriverVersion = LJUD.GetDriverVersion()
        driverDisplay.Text = dblDriverVersion


        'Open the U3 with local ID 2.
        Try
            'Open the U3's with thier local ids
            unit2 = New U3(LJUD.CONNECTION.USB, "2", False)
            unit3 = New U3(LJUD.CONNECTION.USB, "3", False)

            'Start by using the pin_configuration_reset IOType so that all
            'pin assignments are in the factory default condition.
            LJUD.ePut(unit2.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)
            LJUD.ePut(unit3.ljhandle, LJUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0)


            'First a configuration command.  These will be done with the ePut
            'function which combines the add/go/get into a single call.

            'Configure FIO2-FIO4 as analog, all else as digital, on both devices.
            'That means we will start from channel 0 and update all 16 flexible bits.
            'We will pass a value of b0000000000011100 or d28.
            LJUD.ePut(unit2.ljhandle, LJUD.IO.PUT_ANALOG_ENABLE_PORT, 0, 28, 16)
            LJUD.ePut(unit3.ljhandle, LJUD.IO.PUT_ANALOG_ENABLE_PORT, 0, 28, 16)


            'The following commands will use the add-go-get method to group
            'multiple requests into a single low-level function.

            'Request a single-ended reading from AIN2.
            LJUD.AddRequest(unit2.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0)
            LJUD.AddRequest(unit3.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0)

            'Request a single-ended reading from AIN3.
            LJUD.AddRequest(unit2.ljhandle, LJUD.IO.GET_AIN, 3, 0, 0, 0)
            LJUD.AddRequest(unit3.ljhandle, LJUD.IO.GET_AIN, 3, 0, 0, 0)

            'Request a reading from AIN4 using the Special 0-3.6 range.
            LJUD.AddRequest(unit2.ljhandle, LJUD.IO.GET_AIN_DIFF, 4, 0, 32, 0)
            LJUD.AddRequest(unit3.ljhandle, LJUD.IO.GET_AIN_DIFF, 4, 0, 32, 0)

        Catch ex As LabjackUDException
            showErrorMessage(ex)
        End Try

            Try
                'Execute all requests on all open LabJacks.
                LJUD.Go()

                'Get all the results for unit 2.  The input measurement results are stored.
                LJUD.GetFirstResult(unit2.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                showErrorMessage(ex)
            End Try

        Dim unit2Finished As Boolean
        unit2Finished = False
        While Not unit2Finished
            If ioType = LJUD.IO.GET_AIN Then
                If channel = 2 Then
                    Value22 = dblValue
                ElseIf channel = 3 Then
                    Value32 = dblValue
                End If
            ElseIf ioType = LJUD.IO.GET_AIN_DIFF Then
                Value42 = dblValue
            End If

            Try
                LJUD.GetNextResult(unit2.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    unit2Finished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        'Get all the results for unit 3.  The input measurement results are stored.
        Try
            LJUD.GetFirstResult(unit3.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Dim unit3Finished As Boolean
        unit3Finished = False
        While Not unit3Finished
            If ioType = LJUD.IO.GET_AIN Then
                If channel = 2 Then
                    Value23 = dblValue
                ElseIf channel = 3 Then
                    Value33 = dblValue
                End If
            ElseIf ioType = LJUD.IO.GET_AIN_DIFF Then
                Value43 = dblValue
            End If
            Try
                LJUD.GetNextResult(unit3.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = UE9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    unit3Finished = True
                Else
                    showErrorMessage(ex)
                End If
			End Try

        End While

        unitTwoAINTwoDisplay.Text = Value22.ToString("0.###")
        unitTwoAINThreeDisplay.Text = Value32.ToString("0.###")
        unitTwoAINFourDisplay.Text = Value42.ToString("0.###")
        unitThreeAINTwoDisplay.Text = Value23.ToString("0.###")
        unitThreeAINThreeDisplay.Text = Value33.ToString("0.###")
        unitThreeAINFourDisplay.Text = Value43.ToString("0.###")

    End Sub

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

End Class
