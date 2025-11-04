using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Modules.Deployments.Infrastructure.Docker
{
    public static class DockerfileTemplates
    {
        public static string GetTemplateFor(string runtime, string repoPath)
        {
            return runtime.ToLower() switch
            {
                "node" => NodeTemplate(),
                "python" => PythonTemplate(),
                "dotnet" => DotnetTemplate(repoPath),
                "go" => GoTemplate(),
                _ => throw new Exception($"No Dockerfile template for runtime '{runtime}'")
            };
        }

        // ----------------- NODE -----------------
        private static string NodeTemplate() => @"
FROM node:20
WORKDIR /app
COPY . .
RUN npm install --omit=dev
EXPOSE 3000
CMD [""npm"", ""start""]";

        // ----------------- PYTHON -----------------
        private static string PythonTemplate() => @"
FROM python:3.11
WORKDIR /app
COPY . .
RUN pip install --no-cache-dir -r requirements.txt
EXPOSE 8000
CMD [""python"", ""app.py""]";

        // ----------------- GO -----------------
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

        // ----------------- DOTNET (Smart) -----------------
        private static string DotnetTemplate(string repoPath)
        {
            var hasSolution = Directory.GetFiles(repoPath, "*.sln", SearchOption.AllDirectories).Any();
            var csprojFiles = Directory.GetFiles(repoPath, "*.csproj", SearchOption.AllDirectories);

            if (csprojFiles.Length == 0)
                throw new Exception("No .csproj files found. Cannot generate Dockerfile.");

            // Detect entry project (prefer API/Web)
            var entryProject = csprojFiles
                .FirstOrDefault(f => f.Contains("API", StringComparison.OrdinalIgnoreCase) ||
                                     f.Contains("Web", StringComparison.OrdinalIgnoreCase))
                ?? csprojFiles.First();

            // Normalize relative path for Docker (Linux-friendly)
            var relativeEntryProject = Path.GetRelativePath(repoPath, entryProject)
                .Replace("\\", "/");

            var entryProjectName = Path.GetFileNameWithoutExtension(entryProject);
            var entryDll = $"{entryProjectName}.dll";

            // Build dynamic COPY lines for .csproj folders
            var csprojDirs = csprojFiles
                .Select(f => Path.GetDirectoryName(f)!)
                .Distinct()
                .Select(d => Path.GetRelativePath(repoPath, d).Replace("\\", "/"))
                .ToArray();

            var sb = new StringBuilder();
            foreach (var dir in csprojDirs)
                sb.AppendLine($"COPY {dir}/*.csproj {dir}/");

            if (hasSolution)
            {
                var solutionFile = Directory.GetFiles(repoPath, "*.sln", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .FirstOrDefault() ?? "app.sln";

                return @$"
# Auto-generated Dockerfile (solution-based)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY {solutionFile} ./
{sb}
RUN dotnet restore ""{solutionFile}""
COPY . .
RUN dotnet publish ""{relativeEntryProject}"" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT [""dotnet"", ""{entryDll}""]";
            }
            else
            {
                return @$"
# Auto-generated Dockerfile (project-based)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

{sb}
COPY . .
RUN dotnet restore ""{relativeEntryProject}""
RUN dotnet publish ""{relativeEntryProject}"" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT [""dotnet"", ""{entryDll}""]";
            }
        }

    }
}
