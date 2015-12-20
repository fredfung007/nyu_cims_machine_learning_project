function result = Deep_Boost
clear;
x=textread('testinon.txt');
y=x(:,35);
x=x(:,1:34);

%sample size
m=351;
%iteration
T=10;
%hypothesis size
N=23868;
%dimension
d=34;


lambda=0;
beta=0;

D=zeros(T,m);
alpha=zeros(T,N);
S=ones(T);
temp=zeros(T);
epsilon=zeros(T,N);
tempd=zeros(N);
eta=zeros(T);
h=zeros(N,m);
for i=1:m
    for j=1:N
        h(j,i)=computeh(j,i);
    end
end


D(1,:)=1/m;

for t = 1 : T - 1
    
    for j= 1:N
        epsilon(t,j)=computeepsilon(t,j);
        if (alpha(t,j)~=0)
            tempd(j)=(epsilon(t,j)-0.5)+sign(alpha(t,j))*LAMBDA(j)*m/(2*S(t));
        elseif (abs(epsilon(t,j))<=(LAMBDA(j)*m)/(2*S(t)))
            tempd(j)=0;
        else
            tempd(j)=(epsilon(t,j)-0.5)-sign(epsilon(t,j)-0.5)*((LAMBDA(j)*m)/(2*S(t)));
        end
    end
    %compute k
    max=abs(tempd(1));
    k=1;
    for j=2:N
        if(max<abs(tempd(j)))
            max=abs(tempd(j));
            k=j;
        end
    end
    temp(t)=epsilon(t,k);
    %update eta
    if (abs((1-temp(t))*exp(alpha(t,k))-temp(t)*exp(-alpha(t,k)))<=(LAMBDA(k)*m)/(S(t)))
        eta(t)=-alpha(t,k);
    elseif((1-temp(t))*exp(alpha(t,k))-temp(t)*exp(-alpha(t,k))>(LAMBDA(k)*m)/(S(t)))
        eta(t)=log(-(LAMBDA(k)*m)/(2*S(t)*temp(t)) + sqrt(((LAMBDA(k)*m)/(2*S(t)*temp(t)))^2 + (1-temp(t))/temp(t)));
    else
        eta(t)=log((LAMBDA(k)*m)/(2*S(t)*temp(t)) + sqrt(((LAMBDA(k)*m)/(2*S(t)*temp(t)))^2 + (1-temp(t))/temp(t)));
    end
    %update alpha
    alpha(t+1,:)=alpha(t,:);
    alpha(t+1,k)=alpha(t,k)+eta(t);
    %update S
    for i=1:m
        S(t+1)=S(t+1)-exp(1-y(i,:)*f(t,i));
    end
    %update D
    for i=1:m
        D(t+1,i)=-exp(1-y(i,:)*f(t,i))/(S(t+1));
    end
end
    function result = f (t,i)
        result=0;
        for loop = 1:N
            result=result+alpha(t+1,j)*h(j,i);
        end
    end
    function result = computeepsilon(t,j)
        expectation=0;
        for loop=1:m
            expectation = expectation+D(t,loop)*y(loop)*h(j,loop);
        end
        result=0.5*(1-expectation);
    end
    function result = computeh (j,i)
        if(j<=N/2)
            sgn=1;
            dimension=floor((j-1)/m)+1;
            threshould= mod(j-1,m)+1;
        else
            j=j-N/2;
            sgn=-1;
            dimension=floor((j-1)/m)+1;
            threshould= mod(j-1,m)+1;
        end
        
        if (i==threshould)
            result = 1;
        else
            result=sgn*sign(x(i,dimension)-x(threshould,dimension));
        end
    end
    function result = LAMBDA ( j )
        result=lambda*sqrt((2 * log(2*m*d))/m)+beta;
    end
result = alpha(T,:);
end
