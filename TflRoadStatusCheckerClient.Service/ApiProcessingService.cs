using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TflRoadStatusCheckerClient.Contract;
using TflRoadStatusCheckerClient.Contract.Models;

namespace TflRoadStatusCheckerClient.Service
{
    public sealed class ApiProcessingService : IHostedService, IDisposable
    {
        private readonly CommandLineArgs _commandLineArgs;
        private readonly IHostApplicationLifetime _host;
        private readonly ILogger _logger;
        private readonly ITflRoadStatusCheckerAPIService<TflRoadStatusCheckerResponse> _service;
        private CancellationTokenSource _cts;
        private int? _exitCode;

        public ApiProcessingService(IHostApplicationLifetime host, ITflRoadStatusCheckerAPIService<TflRoadStatusCheckerResponse> service
            , CommandLineArgs commandLineArgs, ILogger<ApiProcessingService> logger
            )
        {
            _service = service;
            _commandLineArgs = commandLineArgs;
            _logger = logger;
            _host = host;
        }

        public void Dispose()
        {
            _cts.Cancel();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_commandLineArgs == null)
            {
                _logger.LogInformation($"{_commandLineArgs.Args} is not a valid commandline argument");
                _exitCode = 1;
                _host.StopApplication();
            }
            _host.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                        var response = await _service.GetRoadStatusAsync(_commandLineArgs.Args, _cts.Token);
                        if (response.isValidRoad)
                        {
                            var responseUserMessage = $"The status of the {_commandLineArgs.Args} is as follows{Environment.NewLine}      Road Status is {response.StatusSeverity}{Environment.NewLine}      Road Status Description is {response.StatusSeverityDescription}";
                            Console.WriteLine(responseUserMessage);
                            _logger.LogInformation(responseUserMessage);
                            _exitCode = 0;
                        }
                        else
                        {
                            _logger.LogWarning($"{response.Message}");
                            var responseUserErrorMessage = $"{_commandLineArgs.Args} is not a valid road";
                            Console.WriteLine(responseUserErrorMessage);
                            _logger.LogInformation(responseUserErrorMessage);
                            _exitCode = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"{_commandLineArgs.Args} is not a valid road");
                        _logger.LogError(ex, "Unhandled exception!");
                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _host.StopApplication();
                    }
                });
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cts.Cancel();
            }
            finally
            {
                _logger.LogDebug($"Exiting with return code: {_exitCode}");

                Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            }
            return Task.CompletedTask;
        }
    }
}