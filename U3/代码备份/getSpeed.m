function [speedVoltage, Rn]=getSpeed(para, e_t, time_t, tPointer, Rn)
%Reference Paper:
%"Fabrication of an inexpensive, implantable cooling device for reversible
%deactivation in animals ranging from rodents to primates"(Page 3549-3550)
%
%
%

et=e_t(tPointer);
time0=time_t(tPointer);

%State 1
if et>1
    speedVoltage=para.V_max;
    return;
end

%stable or not
isStable=false;
middleTerm=(para.Ki)*inteFunc(e_t, time_t, time0-5, time0);
leftRight=inteFunc(e_t, time_t, time0-10, time0-5);
leftTerm=(para.Ki)*leftRight-para.stableVary./10;
rightTerm=(para.Ki)*leftRight+para.stableVary./10;
if leftTerm<middleTerm && middleTerm<rightTerm && abs(middleTerm)<para.stableVary
    isStable=true;
end

%State 2
if et<=1 && ~isStable
    %First Term
    Rn=Rn+middleTerm;
    
    %Second Term(part)
    derValue=derFunc(e_t, time_t, time0);
    
    speedVoltage=Rn+para.Kx*log((et+para.Kd*derValue)./para.T_target);
    if speedVoltage>para.V_max
        speedVoltage=para.V_max;
    end
    if speedVoltage<0
        speedVoltage=0;
    end
    
    return;
end

%State 3
if isStable
    %This means don't change the speed.
    speedVoltage=-1;
    
    return;
end


end



function inteValue=inteFunc(e_t, time_t, timeStart, timeEnd)
%integral function

if timeStart<0
    timeStart=0;
end

try
%tmpLogicAll: [timeStart, timeEnd];
tmpLogicAll=time_t>0 & time_t>=timeStart & time_t<=timeEnd;
etData=e_t(tmpLogicAll);
timeAll=time_t(tmpLogicAll);

[timeAll, tmpOrder]=sort(timeAll);
etData=etData(tmpOrder);

time1=timeAll(1:end-1);
etData=etData(1:end-1);
time2=timeAll(2:end);

tmp=etData.*(time2-time1);
inteValue=sum(tmp);

catch e
    showErrorMessage(e);
    keyboard;
end

end





%%

function derivativeValue=derFunc(e_t, time_t, time0)
%Canculate derivative of error

time_1=time0-1;
time_2=time0-2;
if time_1<0
    time_1=0;
end
if time_2<0
    time_2=0;
end

tmpLogic1=time_t>0 & time_t>=time_1 & time_t<time0;
tmpLogic2=time_t>0 & time_t>=time_2 & time_t<time_1;

derivativeValue=mean(e_t(tmpLogic1))-mean(e_t(tmpLogic2));


end












