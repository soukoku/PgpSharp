using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PgpSharp.GnuPG
{
    /// <summary>
    /// A read-only file stream that will delete the underlying file when disposed.
    /// </summary>
    class TempFileStream : Stream
    {
        string _filePath;
        Stream _stream;
        public TempFileStream(string filePath)
        {
            _filePath = filePath;
            _stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        void DoCleanup()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DoCleanup();
            }
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}
