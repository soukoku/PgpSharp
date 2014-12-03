using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

namespace PgpSharp
{
    /// <summary>
    /// Wraps a process with redirected input/outputs.
    /// </summary>
    class RedirectedProcess : IDisposable
    {
        public RedirectedProcess(string exeFile, string args)
        {
            _psi = new ProcessStartInfo(exeFile, args);
            _psi.UseShellExecute = false;
            _psi.CreateNoWindow = true;
            _psi.RedirectStandardInput = true;
            _psi.RedirectStandardOutput = true;
            _psi.RedirectStandardError = true;

            _errors = new StringBuilder();
            _output = new StringBuilder();
        }

        ProcessStartInfo _psi;
        Process _process;
        StringBuilder _errors;
        StringBuilder _output;

        public int ExitCode { get; private set; }
        public string Error { get { return _errors.ToString(); } }
        public string Output { get { return _output.ToString(); } }

        public bool Start()
        {
            if (_process == null)
            {
                _process = new Process { StartInfo = _psi };
                _process.OutputDataReceived += (s, e) =>
                {
                    _output.Append(e.Data);
                };
                _process.ErrorDataReceived += (s, e) =>
                {
                    _errors.Append(e.Data);
                };
                if (_process.Start())
                {
                    _process.BeginOutputReadLine();
                    _process.BeginErrorReadLine();
                    return true;
                }
            }
            return false;
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
            ExitCode = _process.ExitCode;
        }

        public void PushInput(SecureString secureString)
        {
            IOUtility.PushSecret(_process.StandardInput, secureString);
        }

        public void PushInput(string text)
        {
            _process.StandardInput.Write(text);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }
        }

        #endregion
    }
}
