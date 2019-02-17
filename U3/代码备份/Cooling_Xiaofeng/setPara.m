function para=setPara()
%set parameters
%2017.03.02
%By Xiaofeng Xu

%Can be changed
para.T_target=33; %目标温度（摄氏度）
para.sampleRate=10; %10 times per second
para.bufferTime=15; %save 15 seconds' tempture data for canculation
para.noDataValue=100; %this means empty(100 Centigrades), for initialization.
para.stableVary=0.1; %+-0.1

%Need to test
para.Kx=1;
para.R=1;
para.Kd=1;
para.Ki=1;


%Can NOT be changed.
para.V_max=10; %水泵最大转速电压值（伏特）


% V_t=; %水泵当前转速控制电压（0~10伏特）













end
