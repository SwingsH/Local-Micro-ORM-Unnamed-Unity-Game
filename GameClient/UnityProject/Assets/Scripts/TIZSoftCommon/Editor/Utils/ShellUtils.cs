using System;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace TIZSoft.Utils
{
    /// <summary>
    /// Shell 工具，用來呼叫執行 batch, bash 等 shell script 或 command。
    /// </summary>
    public static class ShellUtils
    {
        public static string Execute(string filename, string command, Action<ProcessStartInfo> setupAction = null)
        {
            return ExecuteShellImpl(filename, command, setupAction);
        }

        public static string ExecuteShell(string command, Action<ProcessStartInfo> setupAction = null)
        {
#if UNITY_EDITOR_OSX
            return ExecuteMacShell(command, setupAction);
#elif UNITY_EDITOR_WIN
            return ExecuteWindowsShell(command, setupAction);
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Executes a shell command synchronously using current platform shell.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static string ExecuteShellScriptFile(string command, Action<ProcessStartInfo> setupAction = null)
        {
#if UNITY_EDITOR_OSX
            return ExecuteMacShellScriptFile(command, setupAction);
#elif UNITY_EDITOR_WIN
            return ExecuteWindowsShellScriptFile(command, setupAction);
#else
			throw new NotSupportedException();
#endif
        }

        public static string ExecuteWindowsShell(string command, Action<ProcessStartInfo> setupAction = null)
        {
            return ExecuteShellImpl("powershell", command, setupAction);
        }

        /// <summary>
        /// Executes a Windows shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static string ExecuteWindowsShellScriptFile(string command, Action<ProcessStartInfo> setupAction = null)
        {
            return ExecuteShellImpl("powershell", "\"& " + command + "\"", setupAction);
        }
        
        public static string ExecuteMacShell(string command, Action<ProcessStartInfo> setupAction = null)
        {
            return ExecuteShellImpl("sh", command, setupAction);
        }

        /// <summary>
        /// Executes a Mac shell command synchronously.
        /// </summary>
        /// <param name="command">string command</param>
        /// <returns>string, as output of the command.</returns>
        public static string ExecuteMacShellScriptFile(string command, Action<ProcessStartInfo> setupAction = null)
        {
            return ExecuteShellImpl("open", command, setupAction);
        }

        static string ExecuteShellImpl(string filename, string command, Action<ProcessStartInfo> setupAction = null)
        {
            try
            {
                Debug.Log("Running command (" + filename + "): " + command);

                // create the ProcessStartInfo using "sh" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var procStartInfo = new ProcessStartInfo(filename, command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.RedirectStandardError = true;
                procStartInfo.UseShellExecute = false;

                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                if (setupAction != null)
                {
                    setupAction(procStartInfo);
                }

                // Now we create a process, assign its ProcessStartInfo and start it
                using (var proc = new Process())
                {
                    var result = string.Empty;
                    var error = string.Empty;

                    proc.StartInfo = procStartInfo;
                    proc.EnableRaisingEvents = true;
                    proc.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            error += args.Data;
                        }
                    };
                    proc.OutputDataReceived += (sender, args) =>
                     {
                         if (args.Data != null)
                         {
                             result += args.Data;
                         }
                     };
                    
                    proc.Start();
                    proc.BeginErrorReadLine();
                    proc.BeginOutputReadLine();

                    // Get the output into a string
                    while (!proc.HasExited)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Running", command, 0F))
                        {
                            proc.Kill();
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    
                    var exitCode = proc.ExitCode;

                    // Display the command output.
                    Debug.Log(string.Format("ExitCode: {0}\nResult: {1}\nError: {2}", exitCode, result, error));
                    return result;
                }
            }
            catch (Exception e)
            {
                // Log the exception
                Debug.Log("Got exception: " + e);
            }

            return string.Empty;
        }
    }
}