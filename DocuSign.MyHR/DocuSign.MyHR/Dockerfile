#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src
COPY DocuSign.MyHR/*.sln ./
COPY ["DocuSign.MyHR/DocuSign.MyHR/DocuSign.MyHR.csproj", "DocuSign.MyHR/"]
COPY ["DocuSign.MyHR/DocuSign.MyHR.UnitTests/DocuSign.MyHR.UnitTests.csproj", "DocuSign.MyHR.UnitTests/"]
RUN dotnet restore "DocuSign.MyHR.UnitTests/DocuSign.MyHR.UnitTests.csproj"
RUN dotnet restore "DocuSign.MyHR/DocuSign.MyHR.csproj"
#RUN dotnet restore
COPY . .
WORKDIR "/src/DocuSign.MyHR"
RUN dotnet build "DocuSign.MyHR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocuSign.MyHR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocuSign.MyHR.dll"]