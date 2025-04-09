FROM ubuntu:24.10

RUN apt-get update -yqq && apt-get upgrade -yqq
RUN apt-get install -yqq dotnet-sdk-9.0
RUN apt-get install -yqq aspnetcore-runtime-9.0
RUN apt-get install -yqq curl

WORKDIR /shared
COPY ./shared/Shared.csproj .
RUN dotnet restore
COPY ./shared .

WORKDIR /app
COPY ./chatter/Chatter.csproj .
RUN dotnet restore
COPY ./chatter .

EXPOSE 5040

CMD ["dotnet", "watch", "--no-hot-reload"]
