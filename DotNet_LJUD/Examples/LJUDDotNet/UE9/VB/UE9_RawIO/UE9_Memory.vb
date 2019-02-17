'---------------------------------------------------------------------------
'
'  UE9_Memory.cs
' 
'   Simple demonstration of using RAW_OUT and RAW_IN.  Sends a 0x70,0x70
'   which is a do nothing Comm command that simply responds with an echo.
'	Records the time for 1000 iterations and divides by 1000, to determine
'   the basic communication time of the LabJack.
'
'	For more in depth examples of low-level function calls, see the
'	Linux downloads.
'
'  support@labjack.com
'  June 22, 2009
'----------------------------------------------------------------------
'

Imports System

' Import the UD .NET wrapper object.  The dll referenced is installed by the
' LabJackUD installer.
Imports LabJack.LabJackUD
Module Module1

    Sub Main()
        Dim time As Long
        time = 0
        Dim numIterations As Long
        numIterations = 1000
        Dim ue9 As UE9

        Dim numBytesToWrite As Double
        Dim numBytesToRead As Double
        Dim writeArray(2) As Byte
        writeArray(0) = 112
        writeArray(1) = 112
        Dim readArray(2) As Byte

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabjackUDException
            showErrorMessage(ex)
        End Try

        Try
            time = Environment.TickCount

            For i As Integer = 0 To numIterations - 1
                numBytesToWrite = 2
                numBytesToRead = 2

                'Raw Out
                LJUD.eGet(ue9.ljhandle, LJUD.IO.RAW_OUT, 0, numBytesToWrite, writeArray)

                'Raw In
                LJUD.eGet(ue9.ljhandle, LJUD.IO.RAW_IN, 0, numBytesToRead, readArray)
            Next i
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try

        ' Display the results
        time = Environment.TickCount - time

        Console.Out.WriteLine("Milleseconds per iteration = {0:0.###}", time / numIterations)

        Console.ReadLine() ' Pause for user
    End Sub
    Private Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

End Module
