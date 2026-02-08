FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR app

COPY . .

RUN dotnet build PersonalAccount.sln
RUN dotnet test
RUN dotnet publish --output ./publish PersonalAccount.sln

WORKDIR publish
ENTRYPOINT ["dotnet", "PersonalAccount.Console.dll"]