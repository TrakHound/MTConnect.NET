using System.Diagnostics;

namespace TrakHound.Builder
{
    internal static class CommandLine
    {
        /// <summary>
        /// Show the output of the commands as they run. When false, the output is kept and only shown if the command fails
        /// </summary>
        public static bool Verbose = false;


        public static bool Run(string cmd, string workingDirectory)
        {
            try
            {
                // Output is kept when not Verbose so that it can be shown if the command fails
                var output = new System.Collections.Concurrent.ConcurrentQueue<string>();

                using (var process = Start(cmd, workingDirectory, line =>
                {
                    if (Verbose) Console.WriteLine(line);
                    else output.Enqueue(line);
                }))
                {
                    // Show a loading animation while waiting (output lines would overwrite it in Verbose mode)
                    using (var spinner = Verbose ? null : Spinner.StartAtCursor())
                    {
                        process.WaitForExit();
                    }

                    var exitCode = process.ExitCode;

                    if (exitCode != 0 && !output.IsEmpty)
                    {
                        Console.WriteLine();
                        foreach (var line in output) Console.WriteLine(line);
                    }

                    return exitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Run the commands at the same time and return whether each succeeded (in the same order).
        /// Each command's output is buffered so the commands don't interleave, then shown (with the command's label) when Verbose or when it fails
        /// </summary>
        public static bool[] RunParallel(IReadOnlyList<(string Label, string Command, string WorkingDirectory)> commands)
        {
            var results = new bool[commands.Count];
            var outputs = new System.Collections.Concurrent.ConcurrentQueue<string>[commands.Count];
            var processes = new Process[commands.Count];

            try
            {
                for (var i = 0; i < commands.Count; i++)
                {
                    var output = outputs[i] = new System.Collections.Concurrent.ConcurrentQueue<string>();
                    try
                    {
                        processes[i] = Start(commands[i].Command, commands[i].WorkingDirectory, output.Enqueue);
                    }
                    catch (Exception ex)
                    {
                        output.Enqueue(ex.Message);
                    }
                }

                using (var spinner = Spinner.StartAtCursor())
                {
                    foreach (var process in processes) process?.WaitForExit();
                }

                for (var i = 0; i < commands.Count; i++)
                {
                    results[i] = processes[i] != null && processes[i].ExitCode == 0;

                    if ((Verbose || !results[i]) && !outputs[i].IsEmpty)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"[{commands[i].Label}]");
                        foreach (var line in outputs[i]) Console.WriteLine(line);
                    }
                }
            }
            finally
            {
                foreach (var process in processes) process?.Dispose();
            }

            return results;
        }

        // Start the command with its stdout and stderr lines passed to onLine
        private static Process Start(string cmd, string workingDirectory, Action<string> onLine)
        {
            var regex = "\"(.*?)\"|([^\\s]+)";
            var regexMatches = System.Text.RegularExpressions.Regex.Matches(cmd, regex);
            var commandParts = regexMatches.Select(o => o.Groups[0].Value).ToArray();
            if (commandParts.Length == 0) throw new ArgumentException("No command specified", nameof(cmd));

            // The executable may be quoted (ex. a path with spaces), which ProcessStartInfo doesn't expect
            var executable = commandParts[0].Trim('"');
            var arguments = string.Join(' ', commandParts.Skip(1));

            var startInfo = new ProcessStartInfo(executable, arguments);
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = workingDirectory;

            var process = new Process();
            process.StartInfo = startInfo;

            // Read both streams asynchronously so a full stderr buffer can't deadlock the child process
            DataReceivedEventHandler onData = (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) onLine(e.Data);
            };
            process.OutputDataReceived += onData;
            process.ErrorDataReceived += onData;

            try
            {
                process.Start();
            }
            catch
            {
                process.Dispose();
                throw;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

        public class Spinner : IDisposable
        {
            private static readonly char[] _frames = { '|', '/', '-', '\\' };

            private readonly object _lock = new object();
            private readonly ManualResetEventSlim _stopEvent = new ManualResetEventSlim(false);
            private readonly int _left;
            private readonly int _top;
            private readonly int _delay;
            private int _frame = 0;
            private bool _active;
            private Thread _thread;


            public Spinner(int left, int top, int delay = 100)
            {
                _left = left;
                _top = top;
                _delay = delay;
            }

            // Returns null when the console can't be animated (redirected output or not enough room on the line)
            public static Spinner StartAtCursor(int delay = 100)
            {
                try
                {
                    if (Console.IsOutputRedirected) return null;
                    if (Console.CursorLeft + 1 >= Console.BufferWidth) return null;

                    var spinner = new Spinner(Console.CursorLeft, Console.CursorTop, delay);
                    spinner.Start();
                    return spinner;
                }
                catch
                {
                    return null;
                }
            }

            public void Start()
            {
                lock (_lock)
                {
                    if (_active) return;
                    _active = true;
                }

                _stopEvent.Reset();
                _thread = new Thread(Spin) { IsBackground = true };
                _thread.Start();
            }

            public void Stop()
            {
                lock (_lock)
                {
                    if (!_active) return;
                    _active = false;
                }

                _stopEvent.Set();
                _thread?.Join();

                try
                {
                    // Clear the spinner and return the cursor so the caller's next write (ex. "DONE") lands in its place
                    Console.SetCursorPosition(_left, _top);
                    Console.Write(' ');
                    Console.SetCursorPosition(_left, _top);
                    Console.CursorVisible = true;
                }
                catch { }
            }

            private void Spin()
            {
                try { Console.CursorVisible = false; } catch { }

                do
                {
                    lock (_lock)
                    {
                        if (!_active) break;
                        Turn();
                    }
                }
                while (!_stopEvent.Wait(_delay));
            }

            private void Draw()
            {
                try
                {
                    Console.SetCursorPosition(_left, _top);
                    Console.Write(_frames[_frame]);
                }
                catch { }
            }

            private void Turn()
            {
                Draw();
                _frame = (_frame + 1) % _frames.Length;
            }

            public void Dispose()
            {
                Stop();
                _stopEvent.Dispose();
            }
        }
    }
}
