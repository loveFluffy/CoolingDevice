%��DAC0-1��ֵ��ѹֵ(Լ���Ը�ֵΪ�Եص�ѹֵΪ0.02~4.9V)
%handle,�̶�����,channel number,voltage value,0
ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 0, 3.5, 0);


%��AIN0-3�����źţ����źŵĵ�ѹֵ
ljudObj.Go();
[ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetFirstResult(ljhandle, 0, 0, 0, 0, 0);
disp(num2str(dblValue));

