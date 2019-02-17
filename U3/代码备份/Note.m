%给DAC0-1赋值电压值(约可以赋值为对地电压值为0.02~4.9V)
%handle,固定变量,channel number,voltage value,0
ljudObj.ePut(ljhandle, LabJack.LabJackUD.IO.PUT_DAC, 0, 3.5, 0);


%从AIN0-3接入信号，读信号的电压值
ljudObj.Go();
[ljerror, ioType, channel, dblValue, dummyInt, dummyDbl] = ljudObj.GetFirstResult(ljhandle, 0, 0, 0, 0, 0);
disp(num2str(dblValue));

