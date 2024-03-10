using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PgpSharp.Gpg;

/// <summary>
/// Wraps a process with redirected input/outputs.
/// </summary>
class RedirectedProcess : IDisposable
{
    public RedirectedProcess(string exeFile, string args)
    {
        _errors = new StringBuilder();
        _output = new StringBuilder();

        _process = new Process();
        _process.StartInfo.FileName = exeFile;
        _process.StartInfo.Arguments = args;
        _process.StartInfo.UseShellExecute = false;
        _process.StartInfo.CreateNoWindow = true;
        _process.StartInfo.RedirectStandardInput = true;
        _process.StartInfo.RedirectStandardOutput = true;
        _process.StartInfo.RedirectStandardError = true;

        _process.OutputDataReceived += (s, e) =>
        {
            _output.AppendLine(e.Data);
        };
        _process.ErrorDataReceived += (s, e) =>
        {
            _errors.AppendLine(e.Data);
        };
    }

    Process? _process;
    readonly StringBuilder _errors;
    readonly StringBuilder _output;

    public int ExitCode { get; private set; }
    public string Error => _errors.ToString();
    public string Output => _output.ToString();

    public TextWriter Input
    {
        get
        {
            CheckDisposed();
            return _process!.StandardInput;
        }
    }

    public bool Start()
    {
        CheckDisposed();
        if (_process!.Start())
        {
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            return true;
        }
        return false;
    }

    public void WaitForExit() { WaitForExit(-1); }
    public void WaitForExit(int timeout)
    {
        CheckDisposed();
        _process!.WaitForExit(timeout);
        ExitCode = _process.ExitCode;

        if (timeout > 0)
        {
            // wait again for redirected outputs to finish
            _process.WaitForExit();
        }
        _process.CancelErrorRead();
        _process.CancelOutputRead();
    }

    void CheckDisposed()
    {
        if (_process == null) { throw new ObjectDisposedException(typeof(RedirectedProcess).Name); }
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