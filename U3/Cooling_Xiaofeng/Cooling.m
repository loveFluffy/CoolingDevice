clear;clc;
para=setPara;
Rn=para.R;

%%
%Start LabJack
ljasm = NET.addAssembly('LJUDDotNet'); %Make the UD .NET assembly visible in MATLAB
ljudObj = LabJack.LabJackUD.LJUD;

%��Ϊһ��ѭ��������¼��ȥ����10s���²���������ں�������
loopMax=para.sampleRate*para.bufferTime;
e_t=ones(loopMax, 1)*para.noDataValue;
time_t=ones(loopMax, 1)*-1;
tPointer=1;

%��ʼ
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
    
    %��DAC0��ѹ��Ϊ5V����������ͨ�����ڻ�����������ģ���¶������ѹ�仯-----������
    %ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 0, 5, 0);
    
    
    %����ˮ�ã�ˮ����ͣ���ƣ���ѹ->��ѹ����ʱ�����������������ģ���
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 1, 5, 0);
    ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 1, 0, 0);
    
    %���ˮ�ã�
    %1.�Ƿ�ȷʵ�Ѿ���������δ�������������������ֶ���������
    %2.ת���Ƿ���ȷ��������ȷ��������Warning��ʾ
    %3.�Ƿ���ң��ģʽ��-how��
    
    sV=zeros(1000,1);
    iCheck=1;
    
    startTime=GetSecs;
    while GetSecs-startTime<=60*3
        
        %��ȡAIN0�˿ڵ�ѹֵ���õ�������ǰ�¶ȵĵ�ѹֵ
        ljudObj.AddRequest(ljhandle, LabJack.LabJackUD.IO.GET_AIN, 0, 0, 0, 0);
        ljudObj.Go();
        [ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetFirstResult(ljhandle, 0, 0, 0, 0, 0);
        disp(num2str(dblValue));
        
        %��¼�²�͸��¶����ݵ�ʱ��
        time_t(tPointer)=GetSecs-startTime;
        e_t(tPointer)=dblValue*10-para.T_target;
        
        %���ݲο����ף�����õ�ˮ��ת�ٿ��Ƶ�ѹֵ
        [speedVoltage, Rn]=getSpeed(para, e_t, time_t, tPointer, Rn);
        sV(iCheck)=speedVoltage;
        iCheck=iCheck+1;
        
        tPointer=tPointer+1;
        if tPointer>loopMax
            tPointer=1;
        end
        
        %��������ĵ�ѹֵд��LJTick-DAC��DACA�˿ڣ������Ϊˮ��ת�ٿ���
        if speedVoltage>=0
            %���ã�����ת��
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
% Set DACB to 10 volts. ---ͨ�����ڻ�����������ģ���¶������ѹ�仯
% ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.TDAC_COMMUNICATION, LabJack.LabJackUD.CHANNEL.TDAC_UPDATE_DACB, 10, 0);




















