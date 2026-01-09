using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace Win11Tools
{
    public partial class MainWindow : Window
    {
        private Process? _currentProcess;

        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version;

            TitleText.Text = $"Windows 11 Troubleshooting Tools v{version!.Major}.{version.Minor}.{version.Build}";
        }

        private async void BtnSfc_Click(object sender, RoutedEventArgs e)
            => await RunToolAsync("sfc.exe", "/scannow", "SFC");

        private async void BtnDism_Click(object sender, RoutedEventArgs e)
            => await RunToolAsync("dism.exe", "/online /cleanup-image /restorehealth", "DISM");

        private async void BtnChk_Click(object sender, RoutedEventArgs e)
            => await RunToolAsync("chkdsk.exe", "C: /scan", "CHKDSK");

        private async Task RunToolAsync(string fileName, string args, string title)
        {
            try
            {
                SetUiRunning(true);
                OutputBox.Clear();
                AppendLine($"> {fileName} {args}");
                AppendLine("");

                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                _currentProcess = new Process
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };

                _currentProcess.OutputDataReceived += (_, ev) =>
                {
                    if (ev.Data != null) Dispatcher.Invoke(() => AppendLine(ev.Data));
                };

                _currentProcess.ErrorDataReceived += (_, ev) =>
                {
                    if (ev.Data != null) Dispatcher.Invoke(() => AppendLine(ev.Data));
                };

                StatusText.Text = $"Running {title}...";
                _currentProcess.Start();
                _currentProcess.BeginOutputReadLine();
                _currentProcess.BeginErrorReadLine();

                await _currentProcess.WaitForExitAsync();

                AppendLine("");
                AppendLine($"Exit code: {_currentProcess.ExitCode}");
                StatusText.Text = $"Finished {title}. (Exit code {_currentProcess.ExitCode})";
            }
            catch (Exception ex)
            {
                AppendLine("");
                AppendLine("ERROR: " + ex.Message);
                StatusText.Text = "Error.";
            }
            finally
            {
                _currentProcess?.Dispose();
                _currentProcess = null;
                SetUiRunning(false);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentProcess != null && !_currentProcess.HasExited)
                {
                    _currentProcess.Kill(entireProcessTree: true);
                    AppendLine("");
                    AppendLine("Cancelled by user.");
                    StatusText.Text = "Cancelled.";
                }
            }
            catch (Exception ex)
            {
                AppendLine("Cancel failed: " + ex.Message);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            OutputBox.Clear();
            StatusText.Text = "Ready.";
        }

        private void SetUiRunning(bool running)
        {
            BtnSfc.IsEnabled = !running;
            BtnDism.IsEnabled = !running;
            BtnChk.IsEnabled = !running;
            BtnCancel.IsEnabled = running;
        }

        private void AppendLine(string line)
        {
            OutputBox.AppendText(line + Environment.NewLine);
            OutputBox.ScrollToEnd();
        }
    }
}
