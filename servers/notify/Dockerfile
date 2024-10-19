FROM ubuntu

RUN apt-get update -yqq && apt-get upgrade -yqq
RUN apt-get install -yqq dotnet-sdk-8.0
RUN apt-get install -yqq aspnetcore-runtime-8.0

ADD ./notify /opt/notify
ADD ./shared /opt/shared
WORKDIR /opt/notify
RUN dotnet restore

EXPOSE 5002

CMD ["dotnet", "run"]
