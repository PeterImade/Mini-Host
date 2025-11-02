using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Docker
{
    public static class DockerfileTemplates
    {
        public static string GetTemplateFor(string runtime)
        {
            return runtime.ToLower() switch
            {
                "node" => NodeTemplate(),
                "python" => PythonTemplate(),
                "dotnet" => DotnetTemplate(),
                "go" => GoTemplate(),
                _ => throw new Exception($"No Dockerfile template for runtime '{runtime}'")
            };
        }

        private static string NodeTemplate() => @"
        FROM node:20
        WORKDIR /app
        COPY . .
        RUN npm install --omit=dev
        EXPOSE 3000
        CMD [""npm"", ""start""]";

        private static string PythonTemplate() => @"
        FROM python:3.11
        WORKDIR /app
        COPY . .
        RUN pip install --no-cache-dir -r requirements.txt
        EXPOSE 8000
        CMD [""python"", ""app.py""]";

        private static string DotnetTemplate() => @"
        FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
        WORKDIR /app
        FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
        WORKDIR /src
        COPY . .
        RUN dotnet publish -c Release -o /app/publish
        FROM base AS final
        WORKDIR /app
        COPY --from=build /app/publish .
        EXPOSE 8080
        ENTRYPOINT [""dotnet"", ""App.dll""]";

        private static string GoTemplate() => @"
        FROM golang:1.22 AS build
        WORKDIR /src
        COPY . .
        RUN go build -o app
        FROM debian:bullseye-slim
        WORKDIR /app
        COPY --from=build /src/app .
        EXPOSE 8080
        CMD [""./app""]";
    }
}
