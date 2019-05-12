using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Process = System.Diagnostics.Process;

namespace Kungshol.Services.PowerLinux.Controllers
{
    public class UpsStatusService
    {
        private readonly ILogger _logger;

        public UpsStatusService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<UpsStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            var properties = new List<string>();

            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        FileName = "upsc",
                        RedirectStandardError = true
                    };

                    process.StartInfo.ArgumentList.Add("eaton@localhost");

                    _logger.Information("Starting process {Process} {Args}",
                        process.StartInfo.FileName,
                        process.StartInfo.ArgumentList);

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrWhiteSpace(args?.Data))
                        {
                            _logger.Debug("Output line: {Line}", args.Data);
                            properties.Add(args.Data);
                        }
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (!string.IsNullOrWhiteSpace(args?.Data))
                        {
                            _logger.Error("Error output line: {Line}", args.Data);
                            properties.Add(args.Data);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();

                    int exitCode = process.ExitCode;

                    if (exitCode != 0)
                    {
                        return UpsStatus.Invalid($"Exit code {exitCode}");
                    }

                    if (properties.Count == 0)
                    {
                        _logger.Warning("Could not get any line output from process");
                    }
                }
            }
            catch (Exception ex)
            {
                return UpsStatus.Invalid(ex.ToString());
            }

            return UpsStatus.TryCreate(properties, _logger);
        }
    }
}