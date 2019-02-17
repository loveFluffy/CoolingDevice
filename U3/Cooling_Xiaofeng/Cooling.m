clear;clc;
para=setPara;
Rn=para.R;

%%
%Start LabJack
ljasm = NET.addAssembly('LJUDDotNet'); %Make the UD .NET assembly visible in MATLAB
ljudObj = LabJack.LabJackUD.LJUD;

%作为一个循环链表，记录过去至少10s的温差情况，用于后续计算
loopMax=para.sampleRate*para.bufferTime;
e_t=ones(loopMax, 1)*para.noDataValue;
time_t=ones(loopMax, 1)*-1;
tPointer=1;

%开始
try
    %Read and display the UD version.
    disp(['UD Driver Version = ' num2str(ljudObj.GetDriverVersion())])

    %Open the first found LabJack U3.
    [ljerror, ljhandle] = ljudObj.OpenLabJack(LabJack.LabJackUD.DEVICE.U3, LabJack.LabJackUD.CONNECTION.USB, '0', true, 0);
    
    
    %Start by using the pin_configuration_reset IOType so that all
    %pin assignments are in the factory default condition.
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PIN_CONFIGURATION_RESET, 0, 0, 0);
    
    
    %LJTick-DAC is connected to FIO4 (SCL) and FIO5 (SDA).
    %Make sure FIO4 and FIO5 are set to digital I/O.
    %Starting line is 4, settings are 0 (b00, where 0 = digital) and updating 2 lines.
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_ANALOG_ENABLE_PORT, 4, 0, 2);
    
    % Specify where the LJTick-DAC is plugged in.
    % SCL is 4 (FIO4) and SDA will automatically be 5 (FIO5).
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_CONFIG, LabJack.LabJackUD.CHANNEL.TDAC_SCL_PIN_NUM, 4, 0);
    
    %将DAC0电压设为5V，这样可以通过调节滑动变阻器，模拟温度输入电压变化-----测试用
    %ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 0, 5, 0);
    
    
    %启动水泵：水泵启停控制，高压->低压跳变时启动（好像是这样的）。
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 1, 5, 0);
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 1, 0, 0);
    
    %检查水泵：
    %1.是否确实已经启动，若未启动，给出警报请求手动进行启动
    %2.转向是否正确，若不正确，纠正并Warning提示
    %3.是否处于遥控模式？-how？
    
    sV=zeros(1000,1);
    iCheck=1;
    
    startTime=GetSecs;
    while GetSecs-startTime<=60*3
        
        %读取AIN0端口电压值，得到表征当前温度的电压值
        ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.GET_AIN, 0, 0, 0, 0);
        ljudObj.Go();
        [ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetFirstResult(ljhandle, 0, 0, 0, 0, 0);
        disp(num2str(dblValue));
        
        %记录温差和该温度数据的时间
        time_t(tPointer)=GetSecs-startTime;
        e_t(tPointer)=dblValue*10-para.T_target;
        
        %根据参考文献，计算得到水泵转速控制电压值
        [speedVoltage, Rn]=getSpeed(para, e_t, time_t, tPointer, Rn);
        sV(iCheck)=speedVoltage;
        iCheck=iCheck+1;
        
        tPointer=tPointer+1;
        if tPointer>loopMax
            tPointer=1;
        end
        
        %将计算出的电压值写入LJTick-DAC的DACA端口，输出作为水泵转速控制
        if speedVoltage>=0
            %启泵，设置转速
            ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.TDAC_COMMUNICATION, LabJack.LabJackUD.CHANNEL.TDAC_UPDATE_DACA, speedVoltage, 0);
            disp(['Output Voltage: ' num2str(speedVoltage) 'V.']);
        else
            disp('Keep output.');
        end
        
        WaitSecs(1/para.sampleRate);
        
    end
    
    
    catch e
    showErrorMessage(e);
end



%%
% Set DACB to 10 volts. ---通过调节滑动变阻器，模拟温度输入电压变化
% ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.TDAC_COMMUNICATION, LabJack.LabJackUD.CHANNEL.TDAC_UPDATE_DACB, 10, 0);




















