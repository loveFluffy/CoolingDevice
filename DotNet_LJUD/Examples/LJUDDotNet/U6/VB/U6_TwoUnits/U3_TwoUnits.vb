'---------------------------------------------------------------------------
'
'  U6_TwoUnits.vb
' 
'	Simple example demonstrates communication with 2 U6s.
'   Local ids can be set from LJControlPannel
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

Public Class U6_TwoUnits
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
    Friend WithEvents unitThreeLabel As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents unitTwoAINOneLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINOneDisplay As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINTwoLabel As System.Windows.Forms.Label
    Friend WithEvents unitTwoAINTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINTwoLabel As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINOneDisplay As System.Windows.Forms.Label
    Friend WithEvents unitThreeAINOneLabel As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.driverLabel = New System.Windows.Forms.Label
        Me.driverDisplay = New System.Windows.Forms.Label
        Me.unitTwoLabel = New System.Windows.Forms.Label
        Me.unitTwoAINOneLabel = New System.Windows.Forms.Label
        Me.unitTwoAINOneDisplay = New System.Windows.Forms.Label
        Me.unitTwoAINTwoLabel = New System.Windows.Forms.Label
        Me.unitTwoAINTwoDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINTwoDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINTwoLabel = New System.Windows.Forms.Label
        Me.unitThreeAINOneDisplay = New System.Windows.Forms.Label
        Me.unitThreeAINOneLabel = New System.Windows.Forms.Label
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
        'unitTwoAINOneLabel
        '
        Me.unitTwoAINOneLabel.Location = New System.Drawing.Point(16, 56)
        Me.unitTwoAINOneLabel.Name = "unitTwoAINOneLabel"
        Me.unitTwoAINOneLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitTwoAINOneLabel.TabIndex = 3
        Me.unitTwoAINOneLabel.Text = "AIN1:"
        '
        'unitTwoAINOneDisplay
        '
        Me.unitTwoAINOneDisplay.Location = New System.Drawing.Point(128, 56)
        Me.unitTwoAINOneDisplay.Name = "unitTwoAINOneDisplay"
        Me.unitTwoAINOneDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitTwoAINOneDisplay.TabIndex = 4
        '
        'unitTwoAINTwoLabel
        '
        Me.unitTwoAINTwoLabel.Location = New System.Drawing.Point(16, 80)
        Me.unitTwoAINTwoLabel.Name = "unitTwoAINTwoLabel"
        Me.unitTwoAINTwoLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitTwoAINTwoLabel.TabIndex = 5
        Me.unitTwoAINTwoLabel.Text = "AIN2:"
        '
        'unitTwoAINTwoDisplay
        '
        Me.unitTwoAINTwoDisplay.Location = New System.Drawing.Point(128, 80)
        Me.unitTwoAINTwoDisplay.Name = "unitTwoAINTwoDisplay"
        Me.unitTwoAINTwoDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitTwoAINTwoDisplay.TabIndex = 6
        '
        'unitThreeAINTwoDisplay
        '
        Me.unitThreeAINTwoDisplay.Location = New System.Drawing.Point(128, 176)
        Me.unitThreeAINTwoDisplay.Name = "unitThreeAINTwoDisplay"
        Me.unitThreeAINTwoDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitThreeAINTwoDisplay.TabIndex = 13
        '
        'unitThreeAINTwoLabel
        '
        Me.unitThreeAINTwoLabel.Location = New System.Drawing.Point(16, 176)
        Me.unitThreeAINTwoLabel.Name = "unitThreeAINTwoLabel"
        Me.unitThreeAINTwoLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitThreeAINTwoLabel.TabIndex = 12
        Me.unitThreeAINTwoLabel.Text = "AIN2:"
        '
        'unitThreeAINOneDisplay
        '
        Me.unitThreeAINOneDisplay.Location = New System.Drawing.Point(128, 152)
        Me.unitThreeAINOneDisplay.Name = "unitThreeAINOneDisplay"
        Me.unitThreeAINOneDisplay.Size = New System.Drawing.Size(96, 16)
        Me.unitThreeAINOneDisplay.TabIndex = 11
        '
        'unitThreeAINOneLabel
        '
        Me.unitThreeAINOneLabel.Location = New System.Drawing.Point(16, 152)
        Me.unitThreeAINOneLabel.Name = "unitThreeAINOneLabel"
        Me.unitThreeAINOneLabel.Size = New System.Drawing.Size(104, 16)
        Me.unitThreeAINOneLabel.TabIndex = 10
        Me.unitThreeAINOneLabel.Text = "AIN1:"
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
        'U6_TwoUnits
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(224, 264)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.unitThreeAINTwoDisplay)
        Me.Controls.Add(Me.unitThreeAINTwoLabel)
        Me.Controls.Add(Me.unitThreeAINOneDisplay)
        Me.Controls.Add(Me.unitThreeAINOneLabel)
        Me.Controls.Add(Me.unitThreeLabel)
        Me.Controls.Add(Me.unitTwoAINTwoDisplay)
        Me.Controls.Add(Me.unitTwoAINTwoLabel)
        Me.Controls.Add(Me.unitTwoAINOneDisplay)
        Me.Controls.Add(Me.unitTwoAINOneLabel)
        Me.Controls.Add(Me.unitTwoLabel)
        Me.Controls.Add(Me.driverDisplay)
        Me.Controls.Add(Me.driverLabel)
        Me.Name = "U6_TwoUnits"
        Me.Text = "U6_TwoUnits"
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
        Dim Value12 As Double
        Value12 = 9999
        Dim Value22 As Double
        Value22 = 9999
        Dim Value13 As Double
        Value13 = 9999
        Dim Value23 As Double
        Value23 = 9999
        Dim unit2, unit3 As U6

        ' Variables to satisfy certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        dummyDouble = 0

        'Read and display the UD version.
        dblDriverVersion = LJUD.GetDriverVersion()
        driverDisplay.Text = dblDriverVersion


        'Open the U6 with local ID 2.
        Try
            'Open the U6's with thier local ids
            unit2 = New U6(LJUD.CONNECTION.USB, "2", False)
            unit3 = New U6(LJUD.CONNECTION.USB, "3", False)


            'First a configuration command.  These will be done with the ePut
            'function which combines the add/go/get into a single call.


            'The following commands will use the add-go-get method to group
            'multiple requests into a single low-level function.

            'Request a single-ended reading from AIN1.
            LJUD.AddRequest(unit2.ljhandle, LJUD.IO.GET_AIN, 1, 0, 0, 0)
            LJUD.AddRequest(unit3.ljhandle, LJUD.IO.GET_AIN, 1, 0, 0, 0)

            'Request a single-ended reading from AIN2.
            LJUD.AddRequest(unit2.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0)
            LJUD.AddRequest(unit3.ljhandle, LJUD.IO.GET_AIN, 2, 0, 0, 0)

        Catch ex As LabJackUDException
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
                If channel = 1 Then
                    Value12 = dblValue
                ElseIf channel = 2 Then
                    Value22 = dblValue
                End If
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
                If channel = 1 Then
                    Value13 = dblValue
                ElseIf channel = 2 Then
                    Value23 = dblValue
                End If
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

        unitTwoAINOneDisplay.Text = Value12.ToString("0.###")
        unitTwoAINTwoDisplay.Text = Value22.ToString("0.###")
        unitThreeAINOneDisplay.Text = Value13.ToString("0.###")
        unitThreeAINTwoDisplay.Text = Value23.ToString("0.###")

    End Sub

    Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

End Class
