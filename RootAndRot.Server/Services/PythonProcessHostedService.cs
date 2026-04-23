using System.Diagnostics;

namespace RootAndRot.Server.Services
{
    public class PythonProcessHostedService : IHostedService, IDisposable
    {
        private Process? _proc;
        private readonly ILogger<PythonProcessHostedService> _logger;
        private readonly string _scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts", "receive.py");
        private readonly string _pythonExe = "python"; // or full path to python.exe

        public PythonProcessHostedService(ILogger<PythonProcessHostedService> logger) => _logger = logger;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartProcess();
            return Task.CompletedTask;
        }

        private void StartProcess()
        {
            var psi = new ProcessStartInfo
            {
                FileName = _pythonExe,
                Arguments = $"\"{_scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(_scriptPath)
            };

            _proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _proc.OutputDataReceived += (s, e) => { if (e.Data != null) _logger.LogInformation(e.Data); };
            _proc.ErrorDataReceived += (s, e) => { if (e.Data != null) _logger.LogError(e.Data); };
            _proc.Exited += (s, e) =>
            {
                _logger.LogWarning("Python process exited with code {code}", _proc?.ExitCode);
                // restart after short delay
                Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => StartProcess());
            };

            _proc.Start();
            _proc.BeginOutputReadLine();
            _proc.BeginErrorReadLine();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_proc != null && !_proc.HasExited)
                {
                    _proc.Kill(true);
                    _proc.WaitForExit(2000);
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Error stopping python process"); }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _proc?.Dispose();
        }
    }
}
