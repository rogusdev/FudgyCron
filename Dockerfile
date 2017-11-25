# https://docs.docker.com/engine/examples/dotnetcore/#create-a-dockerfile-for-an-aspnet-core-application
# https://hub.docker.com/r/microsoft/dotnet/
# https://hub.docker.com/r/microsoft/aspnetcore-build/
# https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images
# https://docs.microsoft.com/en-us/dotnet/core/deploying/index
# https://github.com/dotnet/dotnet-docker-samples/tree/master/aspnetapp
FROM microsoft/aspnetcore-build:2.0.3 AS build-env
WORKDIR /app

COPY *.sln .
COPY FudgyCron.Web/*.csproj FudgyCron.Web/
COPY FudgyCron.Tests/*.csproj FudgyCron.Tests/
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0.3
WORKDIR /app
COPY --from=build-env /app/FudgyCron.Web/out .
RUN touch .env

# https://www.ctl.io/developers/blog/post/dockerfile-entrypoint-vs-cmd/
# Heroku will not accept an empty array CMD! -- must pass empty string inside
# but it actually just straight up ignores ENTRYPOINT anyway!  grr
CMD ["dotnet", "FudgyCron.Web.dll"]
#CMD [""]
