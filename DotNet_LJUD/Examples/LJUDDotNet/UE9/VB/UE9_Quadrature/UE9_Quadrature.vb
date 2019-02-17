'---------------------------------------------------------------------------
'
'  UE9_Memory.cs
' 
'   Enables quadrature input on FIO0/FIO1 and reads the current count
'	every half second, until a key is pressed.  To make something happen
'	without an actual quadrature signal, connect a couple wires to
'	ground and tap them on FIO0 and FIO1.
'
'  support@labjack.com
'  June 22, 2009
'----------------------------------------------------------------------
'

Imports System
Imports System.Runtime.InteropServices

' Import the UD .NET wrapper object.  The dll referenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD

Module Module1

    Private Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

    Sub Main()
        Dim lngGetNextIteration As Long
        Dim ioType As LJUD.IO
        ioType = 0
        Dim channel As LJUD.CHANNEL
        channel = 0
        Dim dblValue As Double
        dblValue = 0
        Dim ue9 As UE9
        Dim response As String


        ' Dummy variables to complete certain method signatures
        Dim dummyInt As Integer
        dummyInt = 0
        Dim dummyDouble As Double
        Dim running As Boolean
        running = True
        dummyDouble = 0

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabjackUDException
            showErrorMessage(ex)
        End Try

        Try
            'Disable all timers and counters to put everything in a known initial state.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0)
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)


            'First enable the quadrature input.

            'Enable 2 timers for phases A and B.  They will use FIO0 and FIO1.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 2, 0, 0)

            'Configure Timer0 as quadrature.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 0, LJUD.TIMERMODE.QUAD, 0, 0)

            'Configure Timer1 as quadrature.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_TIMER_MODE, 1, LJUD.TIMERMODE.QUAD, 0, 0)

            'Execute the requests on a single LabJack.  The driver will use a
            'single low-level TimerCounter command to handle all the requests above.
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabjackUDException
            showErrorMessage(ex)
        End Try

        'Get all the results just to check for errors.
        Try
            LJUD.GetFirstResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        Dim isFinished As Boolean
        isFinished = False
        While Not isFinished
            Try
                LJUD.GetNextResult(ue9.ljhandle, ioType, channel, dblValue, dummyInt, dummyDouble)
            Catch ex As LabJackUDException
                ' If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                If (ex.LJUDError = ue9.LJUDERROR.NO_MORE_DATA_AVAILABLE) Then
                    isFinished = True
                Else
                    showErrorMessage(ex)
                End If
            End Try
        End While

        Try
            While running 'Program will run until any key is hit

                'Wait 500 milliseconds
                Threading.Thread.Sleep(500)

                'Request a read from Timer0.  Timer0 and Timer1 both return the same
                'quadrature value.
                LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_TIMER, 0, dblValue, 0)
                Console.Out.WriteLine("Quad Counter = {0:0.0}", dblValue)
                Console.Out.WriteLine("press enter to go again or q and enter to quit")
                response = Console.ReadLine() ' Pause for user
                If response.Equals("q") Then
                    running = False
                End If

            End While


            'Disable the timers and the FIO lines will return to digital I/O.
            LJUD.AddRequest(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0)
            LJUD.GoOne(ue9.ljhandle)
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

    End Sub

End Module
