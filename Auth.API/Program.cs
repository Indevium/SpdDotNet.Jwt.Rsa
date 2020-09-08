﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpdDotNet.Jwt.Rsa.Lib;

 namespace SpdDotNet.Jwt.Rsa
{
    internal class Program
    {
        private static async Task<int> Main()
        {
            try
            {
                var host = new HostBuilder()
                    .UseEnvironment(Defines.Environment)
                    .ConfigureLogging((context, loggerBuilder) =>
                    {
                        loggerBuilder.ClearProviders()
                            .SetMinimumLevel(Defines.LogLevel)
                            .AddConsole();
                    })
                    .ConfigureWebHost(webHostBuilder =>
                    {
                        webHostBuilder.ConfigureKestrel((context, options) =>
                            {
                                options.AddServerHeader = Defines.AddKestrelHeaders;
                                options.ListenAnyIP(Defines.AuthApiPort);
                            })
                            .UseKestrel()
                            .UseStartup<Startup>();
                        ;
                    })
                    .Build();

                await host.StartAsync();

                await host.WaitForShutdownAsync();

                host.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                return 1;
            }

            return 0;
        }
    }
}
