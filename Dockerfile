FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /App
# Copy everything
COPY . ./

RUN ls
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN ls /App/StoreAPI
RUN dotnet publish /App/StoreAPI/StoreAPI.csproj -c Release -o /App/build


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN apt-get update -qq && apt-get -y install libgdiplus libc6-dev 
RUN apt-get update && apt-get install -y wget fontconfig


RUN mkdir -p /usr/share/fonts/truetype/poppins && \
    wget -O /usr/share/fonts/truetype/poppins/Poppins-Regular.ttf https://github.com/google/fonts/raw/main/ofl/poppins/Poppins-Regular.ttf && \
    wget -O /usr/share/fonts/truetype/poppins/Poppins-Bold.ttf https://github.com/google/fonts/raw/main/ofl/poppins/Poppins-Bold.ttf && \
    fc-cache -f -v
WORKDIR /App
COPY --from=build-env /App/build .
COPY ./StoreAPI/Templates ./Templates
RUN chmod 755 /App/Rotativa/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "StoreAPI.dll"]
