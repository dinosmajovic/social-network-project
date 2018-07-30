FROM microsoft/dotnet:sdk as builder  
 
RUN mkdir -p /root/src/app/aspnetcoreapp
WORKDIR /root/src/app/aspnetcoreapp
 
COPY /SocialNetwork.API/SocialNetwork.API.csproj . 

RUN dotnet restore SocialNetwork.API.csproj 

COPY . .
RUN dotnet publish -c release -o published

FROM microsoft/dotnet:2.1.2-aspnetcore-runtime

WORKDIR /app/  
COPY --from=builder /root/src/app/aspnetcoreapp/published .
COPY /SocialNetwork.API/appsettings.json .
COPY /SocialNetwork.API/appsettings.Development.json .
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000/tcp

CMD ["dotnet", "./SocialNetwork.API.dll"] 