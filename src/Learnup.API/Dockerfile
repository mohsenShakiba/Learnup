FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

ENV NUGET_PACKAGES=/root/.nuget/packages

# copy all cs project files to respective directory\
COPY src/Learnup.API/*.csproj src/Learnup.API/
COPY src/Learnup.Application/*.csproj src/Learnup.Application/
COPY src/Learnup.Domain/*.csproj src/Learnup.Domain/
COPY src/Learnup.Infrastructure/*.csproj src/Learnup.Infrastructure/
COPY Learnup.sln Learnup.sln 

# configure nuget
RUN dotnet nuget add source https://baget.itsaze.com/repository/nuget-hosted/ --name nexus-hosted --username admin --password M1shakiba@ --store-password-in-clear-text

# restore with shared cache
RUN --mount=type=cache,id=nuget-packages,target=/root/.nuget/packages dotnet restore

RUN dotnet restore

# copy everything else
COPY . .
RUN --mount=type=cache,id=nuget-packages,target=/root/.nuget/packages dotnet publish -c Release -o out  ./src/Learnup.API/Learnup.API.csproj

FROM mcr.microsoft.com/dotnet/aspnet:10.0
ENV TZ=Asia/Tehran
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Learnup.API.dll"]
