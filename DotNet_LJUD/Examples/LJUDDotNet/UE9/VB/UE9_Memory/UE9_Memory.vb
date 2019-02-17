'---------------------------------------------------------------------------
'
'  UE9_Memory.cs
' 
'	Demonstrates writing and reading to the user and cal memory on the UE9.
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
        Dim achrUserMem(1024) As Double
        Dim adblCalMem(128) As Double
        Dim ue9 As UE9

        ' Dummy variables that will be used to satisfy certain method signatures but have no other purpose
        Dim dummyDouble As Double
        dummyDouble = 0

        'Make a long parameter which holds the address of the data arrays.  We do this
        'so the compiler does not generate a warning in the eGet call that passes
        'the data.  Note that the x1 parameter  in eGet (and AddRequest) is fairly
        'generic, in that sometimes it could just be a write parameter, and sometimes
        'it has the address of an array. 

        'Seed the random number function.
        Dim rand As Random
        rand = New Random(Environment.TickCount)

        ' Open UE9
        Try
            ue9 = New UE9(LJUD.CONNECTION.USB, "0", True)
            'ue9 = new UE9(LJUD.CONNECTION.ETHERNET, "192.168.1.50", true) ' Connection through ethernet
        Catch ex As LabjackUDException
            showErrorMessage(ex)
        End Try

        'First a user memory example.  We will read the memory, update a few elements,
        'and write the memory.  The entire memory area is read and written each time.
        'The user memory is just stored as bytes, so almost any information can be
        'put in there such as integers, doubles, or strings.

        Try
            'Read the user memory.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.USER_MEM, dummyDouble, achrUserMem)
            'Display the first 4 elements.
            Console.Out.WriteLine("Read User Mem (0-3) = {0:0}, {1:0}, {2:0}, {3:0}", achrUserMem(0), achrUserMem(1), achrUserMem(2), achrUserMem(3))
            'Create 4 new pseudo-random numbers to write.  We will update the first
            '4 elements of user memory, but the rest will be unchanged.
            For i As Integer = 0 To 3
                achrUserMem(i) = rand.Next(100)
            Next i
            Console.Out.WriteLine("Write User Mem (0-3) = {0:0}, {1:0}, {2:0}, {3:0}", achrUserMem(0), achrUserMem(1), achrUserMem(2), achrUserMem(3))
            LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.USER_MEM, 0, achrUserMem)
            'Re-read the user memory.
            LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.USER_MEM, dummyDouble, achrUserMem)
            'Display the first 4 elements.
            Console.Out.WriteLine("Read User Mem (0-3) = {0:0}, {1:0}, {2:0}, {3:0}", achrUserMem(0), achrUserMem(1), achrUserMem(2), achrUserMem(3))
        Catch ex As LabJackUDException
            showErrorMessage(ex)
        End Try



        'Now a cal constants example.  The calibration memory is passed as doubles.
        'The memory area consists of 8 blocks (0-7) of 16 doubles each, for a total
        'of 128 elements.  As of this writing, block 7 is not used, so we will
        'use the last 4 elements of block 7 for testing, which is elements 124-127.
        'We will read the constants, update a few elements, and write the constants.  
        'The entire memory area is read and written each time.

        'This cal example is commented out by default, as writing and reading
        'the cal area is an advanced operation.
        'Read the cal constants.
        LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, dummyDouble, adblCalMem)
        'Display the last 4 elements.
        Console.Out.WriteLine("Read Cal Constants (124-127) = {0:0}, {1:0}, {2:0}, {3:0}", adblCalMem(124), adblCalMem(125), adblCalMem(126), adblCalMem(127))
        'Create 4 new pseudo-random numbers to write.  We will update the last
        '4 cal constants, but the rest will be unchanged.
        For i As Integer = 124 To 127
            adblCalMem(i) = (100 * (rand.Next(100) / 100)) - 50
        Next i
        Console.Out.WriteLine("Write  Cal Constants (124-127) = {0:0}, {1:0}, {2:0}, {3:0}", adblCalMem(124), adblCalMem(125), adblCalMem(126), adblCalMem(127))
        'The special value (0x4C6C) must be put in to write the cal constants.
        LJUD.ePut(ue9.ljhandle, LJUD.IO.PUT_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, 19564, adblCalMem)
        'Re-read the cal constants.
        LJUD.eGet(ue9.ljhandle, LJUD.IO.GET_CONFIG, LJUD.CHANNEL.CAL_CONSTANTS, dummyDouble, adblCalMem)
        'Display the first 4 elements.
        Console.Out.WriteLine("Read  Cal Constants (124-127) = {0:0}, {1:0}, {2:0}, {3:0}", adblCalMem(124), adblCalMem(125), adblCalMem(126), adblCalMem(127))

        Console.ReadLine() ' Pause for user
    End Sub

    Private Sub showErrorMessage(ByVal err As LabJackUDException)
        MsgBox("Function returned LabJackUD Error #" & _
            Str$(err.LJUDError) & _
            "  " & _
            err.ToString)
    End Sub

End Module
