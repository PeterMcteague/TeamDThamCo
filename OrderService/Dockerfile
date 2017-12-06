FROM gcr.io/google-appengine/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "OrderService.dll"]