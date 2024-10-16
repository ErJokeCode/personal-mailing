FROM ubuntu

RUN apt-get update -yqq && apt-get upgrade -yqq
RUN apt-get install -yqq dotnet-sdk-8.0
RUN apt-get install -yqq aspnetcore-runtime-8.0

ADD . /opt/user
WORKDIR /opt/user
RUN dotnet restore

CMD ["dotnet", "run"]
