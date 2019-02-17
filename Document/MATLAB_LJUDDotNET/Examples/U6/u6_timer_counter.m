%
% Basic U6 example does a PWM output and a counter input features using
% MATLAB, .NET and the UD driver.
%
% support@labjack.com
%

clc %Clear the MATLAB command window
clear %Clear the MATLAB variables

ljasm = NET.addAssembly('LJUDDotNet'); %Make the UD .NET assembly visible in MATLAB
ljudObj = LabJack.LabJackUD.LJUD;

try
    %Read and display the UD version.
    disp(['UD Driver Version = ' num2str(ljudObj.GetDriverVersion())])

    %Open the first found LabJack U6.
    [ljerror, ljhandle] = ljudObj.OpenLabJack(LabJack.LabJackUD.DEVICE.U6, LabJack.LabJackUD.CONNECTION.USB, '0', true, 0);
    
    %First requests to configure the timer and counter.  These will be
    %done with and add/go/get block.
    
    %Set the timer/counter pin offset to 0, which will put the first
    %timer/counter on FIO0.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.TIMER_COUNTER_PIN_OFFSET, 0, 0, 0);
    
    %Use the 48 MHz timer clock base with divider (LJ_tc48MHZ_DIV).  Since we are using clock with divisor
    %support, Counter0 is not available.
    LJ_tc48MHZ_DIV = ljudObj.StringToConstant('LJ_tc48MHZ_DIV');
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.TIMER_CLOCK_BASE, LJ_tc48MHZ_DIV, 0, 0);
    
    %Set the divisor to 48 so the actual timer clock is 1 MHz.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.TIMER_CLOCK_DIVISOR, 48, 0, 0);
    
    %Enable 1 timer.  It will use FIO0.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.NUMBER_TIMERS_ENABLED, 1, 0, 0);
    
    %Make sure Counter0 is disabled.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_COUNTER_ENABLE, 0, 0, 0, 0);
    
    %Enable Counter1.  It will use FIO1 since 1 timer is enabled.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_COUNTER_ENABLE, 1, 1, 0, 0);
    
    %Configure Timer0 as 8-bit PWM (LJ_tmPWM8).  Frequency will be 1M/256 = 3906 Hz.
    LJ_tmPWM8 = ljudObj.StringToConstant('LJ_tmPWM8');
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_TIMER_MODE, 0, LJ_tmPWM8, 0, 0);
    
    %Set the PWM duty cycle to 50%.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_TIMER_VALUE, 0, 32768, 0, 0);
    
    %Execute the requests.
    ljudObj.GoOne(ljhandle);
    
    %Get all the results just to check for errors.
    [ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetFirstResult(ljhandle, 0, 0, 0, 0, 0);
    
    finished = false;
    while finished == false
        try
            [ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetNextResult(ljhandle, 0, 0, 0, 0, 0);
        catch e
            if(isa(e, 'NET.NetException'))
                eNet = e.ExceptionObject;
                if(isa(eNet, 'LabJack.LabJackUD.LabJackUDException'))
                    %If we get an error, report it.  If the error is NO_MORE_DATA_AVAILABLE we are done
                    if(eNet.LJUDError == LabJack.LabJackUD.LJUDERROR.NO_MORE_DATA_AVAILABLE)
                        finished = true;
                    end
                end
            end
            %Report non NO_MORE_DATA_AVAILABLE error.
            if(finished == false)
                throw(e)
            end
        end
    end
    
    %Wait 1 second.
    pause(1);
    
    %Request a read from the counter.
    [ljerror, dblValue] = ljudObj.eGet(ljhandle, LabJack.LabJackUD.IO.GET_COUNTER, 1, 0, 0);
    
    %This should read roughly 4k counts if FIO0 is shorted to FIO1.
    disp(['Counter 1 = ' num2str(dblValue)]);
    
    %Wait 1 second.
    pause(1);
    
    %Request a read from the counter.
    [ljerror, dblValue] = ljudObj.eGet(ljhandle, LabJack.LabJackUD.IO.GET_COUNTER, 1, 0, 0);
    
    %This should read about 3906 counts more than the previous read.
    disp(['Counter 1 = ' num2str(dblValue)]);
    
    %Disable the timer and counter, and the FIO lines will return to
    %digital I/O. Also setting timer clock base to default 48 Mhz.
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.NUMBER_TIMERS_ENABLED, 0, 0, 0);
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_COUNTER_ENABLE, 1, 0, 0, 0);
    LJ_tc48MHZ = ljudObj.StringToConstant('LJ_tc48MHZ');
    ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.TIMER_CLOCK_BASE, LJ_tc48MHZ, 0, 0);
    ljudObj.GoOne(ljhandle);
    
    %The PWM output sets FIO0 to output, so we do a read here to set
    %it to input.
    ljudObj.eGet(ljhandle, LabJack.LabJackUD.IO.GET_DIGITAL_BIT, 0, 0, 0);
catch e
    showErrorMessage(e)
end