using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Docker
{
    public static class DockerIgnoreTemplates
    {
        public static string GetTemplateFor(string runtime)
        {

            switch (runtime.ToLowerInvariant())
            {
                case "node":
                    return @"
                        node_modules
                        npm-debug.log
                        yarn-error.log
                        dist
                        .git
                        .env
                        .DS_Store
                        ";

                case "python":
                    return @"
                        __pycache__
                        *.pyc
                        *.pyo
                        .git
                        .env
                        .venv
                        .DS_Store
                        ";

                case "dotnet":
                    return @"
                        bin
                        obj
                        .git
                        .env
                        .vscode
                        .DS_Store
                        ";

                case "go":
                    return @"
                        bin
                        vendor
                        .git
                        .env
                        .DS_Store
                        ";

                default:
                    return @"
                        .git
                        node_modules
                        bin
                        obj
                        dist
                        .env
                        .DS_Store
                    ";
            }
        }   
    }

}
