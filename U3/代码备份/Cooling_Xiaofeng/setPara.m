function para=setPara()
%set parameters
%2017.03.02
%By Xiaofeng Xu

%Can be changed
para.T_target=33; %Ŀ���¶ȣ����϶ȣ�
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
para.V_max=10; %ˮ�����ת�ٵ�ѹֵ�����أ�


% V_t=; %ˮ�õ�ǰת�ٿ��Ƶ�ѹ��0~10���أ�













end
