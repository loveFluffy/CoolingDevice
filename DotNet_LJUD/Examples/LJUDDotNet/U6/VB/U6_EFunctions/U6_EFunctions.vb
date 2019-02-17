'---------------------------------------------------------------------------
'
'  U6_EFunctions.vb
' 
'  Demonstrates the UD E-functions with the LabJack U6.
'
'  support@labjack.com
'  June 12, 2009
'----------------------------------------------------------------------
'

'Simple VB.NET example uses the LabJackUD driver to communicate with a U6.
Option Explicit On 

Imports System.Threading.Thread

' Import the UD .NET wrapper object.  The dll ByReferenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Public Class U6_EFunctions
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
    Friend WithEvents ainThreeLabel As System.Windows.Forms.Label
    Friend WithEvents dacZeroLabel As System.Windows.Forms.Label
    Friend WithEvents fioFourLabel As System.Windows.Forms.Label
    Friend WithEvents dacZeroDisplay As System.Windows.Forms.Label
    Friend WithEvents ainThreeDisplay As System.Windows.Forms.Label
    Friend WithEvents goButton As System.Windows.Forms.Button
    Friend WithEvents fioTwoLabel As System.Windows.Forms.Label
    Friend WithEvents fioTwoDisplay As System.Windows.Forms.Label
    Friend WithEvents fioOneDisplay As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.ainThreeLabel = New System.Windows.Forms.Label
        Me.dacZeroLabel = New System.Windows.Forms.Label
        Me.fioFourLabel = New System.Windows.Forms.Label
        Me.fioTwoLabel = New System.Windows.Forms.Label
        Me.fioTwoDisplay = New System.Windows.Forms.Label
        Me.fioOneDisplay = New System.Windows.Forms.Label
        Me.dacZeroDisplay = New System.Windows.Forms.Label
        Me.ainThreeDisplay = New System.Windows.Forms.Label
        Me.goButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ainThreeLabel
        '
        Me.ainThreeLabel.Location = New System.Drawing.Point(8, 8)
        Me.ainThreeLabel.Name = "ainThreeLabel"
        Me.ainThreeLabel.Size = New System.Drawing.Size(136, 16)
        Me.ainThreeLabel.TabIndex = 0
        Me.ainThreeLabel.Text = "AIN3: "
        '
        'dacZeroLabel
        '
        Me.dacZeroLabel.Location = New System.Drawing.Point(8, 32)
        Me.dacZeroLabel.Name = "dacZeroLabel"
        Me.dacZeroLabel.Size = New System.Drawing.Size(136, 16)
        Me.dacZeroLabel.TabIndex = 1
        Me.dacZeroLabel.Text = "DAC0: "
        '
        'fioFourLabel
        '
        Me.fioFourLabel.Location = New System.Drawing.Point(8, 56)
        Me.fioFourLabel.Name = "fioFourLabel"
        Me.fioFourLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioFourLabel.TabIndex = 2
        Me.fioFourLabel.Text = "FIO1: "
        '
        'fioTwoLabel
        '
        Me.fioTwoLabel.Location = New System.Drawing.Point(8, 80)
        Me.fioTwoLabel.Name = "fioTwoLabel"
        Me.fioTwoLabel.Size = New System.Drawing.Size(136, 16)
        Me.fioTwoLabel.TabIndex = 3
        Me.fioTwoLabel.Text = "FIO2:"
        '
        'fioTwoDisplay
        '
        Me.fioTwoDisplay.Location = New System.Drawing.Point(136, 80)
        Me.fioTwoDisplay.Name = "fioTwoDisplay"
        Me.fioTwoDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioTwoDisplay.TabIndex = 12
        '
        'fioOneDisplay
        '
        Me.fioOneDisplay.Location = New System.Drawing.Point(136, 56)
        Me.fioOneDisplay.Name = "fioOneDisplay"
        Me.fioOneDisplay.Size = New System.Drawing.Size(136, 16)
        Me.fioOneDisplay.TabIndex = 11
        '
        'dacZeroDisplay
        '
        Me.dacZeroDisplay.Location = New System.Drawing.Point(136, 32)
        Me.dacZeroDisplay.Name = "dacZeroDisplay"
        Me.dacZeroDisplay.Size = New System.Drawing.Size(136, 16)
        Me.dacZeroDisplay.TabIndex = 10
        '
        'ainThreeDisplay
        '
        Me.ainThreeDisplay.Location = New System.Drawing.Point(136, 8)
        Me.ainThreeDisplay.Name = "ainThreeDisplay"
        Me.ainThreeDisplay.Size = New System.Drawing.Size(136, 16)
        Me.ainThreeDisplay.TabIndex = 9
        '
        'goButton
        '
        Me.goButton.Location = New System.Drawing.Point(8, 104)
        Me.goButton.Name = "goButton"
        Me.goButton.Size = New System.Drawing.Size(264, 24)
        Me.goButton.TabIndex = 18
        Me.goButton.Text = "Go"
        '
        'U6_EFunctions
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(284, 134)
        Me.Controls.Add(Me.goButton)
        Me.Controls.Add(Me.fioTwoDisplay)
        Me.Controls.Add(Me.fioOneDisplay)
        Me.Controls.Add(Me.dacZeroDisplay)
        Me.Controls.Add(Me.ainThreeDisplay)
        Me.Controls.Add(Me.fioTwoLabel)
        Me.Controls.Add(Me.fioFourLabel)
        Me.Controls.Add(Me.dacZeroLabel)
        Me.Controls.Add(Me.ainThreeLabel)
        Me.Name = "U6_EFunctions"
        Me.Text = "EFunctions Example"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Private Sub goButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles goButton.Click
        Dim dblValue As Double
        dblValue = 0
        Dim IntegerValue As Integer
        IntegerValue = 0

        Dim binary As Integer
        Dim aEnableTimers(2) As Integer
        Dim aEnableCounters(2) As Integer
        Dim tcpInOffset As Integer
        Dim timerClockDivisor As Integer
        Dim timerClockBaseIndex As LJUD.TIMERCLOCKS
        Dim aTimerModes(2) As Integer
        Dim adblTimerValues(2) As Double
        Dim aReadTimers(2) As Integer
        Dim aUpdateResetTimers(2) As Integer
        Dim aReadCounters(2) As Integer
        Dim aResetCounters(2) As Integer
        Dim adblCounterValues() As Double
        adblCounterValues = New Double() {0.0, 0.0}
        Dim intValue As Integer
        Dim highTime, lowTime, dutyCycle As Double

        'Open the first found LabJack U6.
        Try

            Dim u6 As U6

            u6 = New U6(LJUD.CONNECTION.USB, "0", True) ' Connection through USB

            'Take a single-ended measurement from AIN3.
            binary = 0
            LJUD.eAIN(u6.ljhandle, 3, 199, dblValue, -1, -1, -1, binary)
            ainThreeDisplay.Text = dblValue

            'Set DAC0 to 3.0 volts.
            dblValue = 3
            binary = 0
            LJUD.eDAC(u6.ljhandle, 0, dblValue, binary, 0, 0)
            dacZeroDisplay.Text = dblValue

            'Read state of FIO1.
            LJUD.eDI(u6.ljhandle, 1, intValue)
            fioOneDisplay.Text = intValue

            'Set the state of FIO2.
            intValue = 1
            LJUD.eDO(u6.ljhandle, 2, intValue)
            fioTwoDisplay.Text = intValue

        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try
    End Sub

    Private Sub timerOneDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub U6_EFunctions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
    Private Sub timerOneLabel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
End Class
