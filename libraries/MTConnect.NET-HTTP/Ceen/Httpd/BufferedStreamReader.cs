using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Ceen.Httpd
{
    /// <summary>
    /// Helper class that reads a stream,
    /// but also provides a buffer to allow peek/read-ahead
    /// It contains a special method to read data until the first 
    /// CR LF CR LF entry (end of HTTP header), and leaves the rest of
    /// the stream untouched.
    /// </summary>
    internal class BufferedStreamReader : Stream
	{
		/// <summary>
		/// The underlying network stream
		/// </summary>
		private Stream m_parent;
		/// <summary>
		/// The internal read-ahead buffer
		/// </summary>
		private byte[] m_buffer;
		/// <summary>
		/// The number of bytes in the internal buffer
		/// </summary>
		private int m_buffercount = 0;
		/// <summary>
		/// The offset in the internal buffer from where the data starts
		/// </summary>
		private int m_bufferoffset = 0;
		/// <summary>
		/// The CR ASCII value.
		/// </summary>
		private const byte CR = 13;
		/// <summary>
		/// The LF ASCII value.
		/// </summary>
		private const byte LF = 10;
		/// <summary>
		/// The number of bytes we are allowed to read
		/// </summary>
		private long m_remainingbytes = 0;
		/// <summary>
		/// The number of bytes read
		/// </summary>
		private long m_maxread = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.BufferedStreamReader"/> class.
		/// </summary>
		/// <param name="parent">The parent stream.</param>
		/// <param name="initialbuffersize">Intitial size of the read-ahead buffer.</param>
		public BufferedStreamReader(Stream parent, int initialbuffersize = 1024)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));
			
			m_parent = parent;
			m_buffer = new byte[initialbuffersize];
		}

		/// <summary>
		/// Resets the number of bytes read.
		/// </summary>
		/// <param name="maxsize">The maximum number of bytes to allow reading</param>
		internal void ResetReadLength(long maxsize)
		{
			m_maxread = m_remainingbytes = maxsize;
		}


		/// <summary>
		/// Reads all lines until an empty line (i.e. it reads until CR LF CR LF).
		/// Each read line, except the empty line is send to the linehandler function
		/// </summary>
		/// <returns>An awaitable task.</returns>
		/// <param name="maxlinesize">The maximum size of a single line.</param>
		/// <param name="maxheadersize">The maximum size of the headers.</param>
		/// <param name="idletimeout">The maximum time to remain idle.</param>
		/// <param name="linehandler">Callback method that processes each line.</param>
		/// <param name="timeouttask">Awaitable task for stopping the request.</param>
		/// <param name="stoptask">Awaitable task for stopping the request.</param>
		public async Task ReadHeaders(int maxlinesize, int maxheadersize, TimeSpan idletimeout, Action<string> linehandler, Task timeouttask, Task stoptask)
		{
			var buf = new byte[maxlinesize];
			var bufoffset = 0;
			var bufsize = 0;
			var totalread = 0;
			var lastlookoffset = 0;

			// Repeat the parsing until we time out or get an empty line
			while (true)
			{
				if (totalread == maxheadersize)
					throw new HttpException(HttpStatusCode.RequestHeaderFieldsTooLarge);

                Task<int> rtask;
                Task rt;

                using (var cs = new CancellationTokenSource(idletimeout))
                {
                    rtask = ReadAsync(buf, bufsize, Math.Min(buf.Length - bufsize, maxheadersize - totalread), cs.Token);
                    rt = await Task.WhenAny( stoptask, timeouttask, rtask);
                }

                // Timeout or stop has happened
				if (rt != rtask)
				{
					if (rt == stoptask)
						throw new TaskCanceledException();
					else if (totalread == 0)
						throw new EmptyStreamClosedException();
					else
						throw new HttpException(HttpStatusCode.RequestTimeout); 
				}

				var r = rtask.Result;
				totalread += r;
				bufsize += r;

				// Stream closed
				if (r == 0)
				{
					if (totalread == 0)
						throw new EmptyStreamClosedException();
					else
						throw new HttpException(HttpStatusCode.BadRequest);
				}

				// Look for first CR
				var ix = Array.IndexOf(buf, CR, lastlookoffset, bufsize - lastlookoffset);
				while (ix >= 0 && ix < bufsize - 1)
				{
					// Check if the next byte is LF
					if (buf[ix + 1] == LF)
					{
						var datalen = ix - bufoffset;

						// Check if the line is empty
						if (datalen == 0)
						{
							bufoffset = ix + 2;

							// If we have more data, "unread it"
							if (bufsize - bufoffset != 0)
								UnreadBytesIntoBuffer(buf, bufoffset, bufsize - bufoffset);

							return;
						}
						else
						{
							linehandler(System.Text.Encoding.ASCII.GetString(buf, bufoffset, datalen));
							bufoffset = ix + 2;
							lastlookoffset = bufoffset;
						}
					}
					else
					{
						// False hit, or char hit a split with blocks
						lastlookoffset = ix + 1;
					}

					// Get the next entry
					ix = Array.IndexOf(buf, CR, lastlookoffset, bufsize - lastlookoffset);
				}

				// Move trailing bytes to the start of the buffer to reduce space use
				// and make sure we do not search more than once for the CRLF
				bufsize = bufsize - bufoffset;
				if (bufoffset > 0 && bufsize > 0)
					Array.Copy(buf, bufoffset, buf, 0, bufsize);

				bufoffset = 0;
				lastlookoffset = Math.Max(0, bufsize - 1);
			}
		}

		/// <summary>
		/// Keeps reading from the stream until the supplied number of bytes are read
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="count">The number of bytes to read.</param>
		/// <param name="idletimeout">The time to wait while idle</param>
		/// <param name="timeouttask">The task that signals request timeout</param>
		/// <param name="stoptask">The task that signals stop for the server</param>
		internal async Task<byte[]> RepeatReadAsync(int count, TimeSpan idletimeout, Task timeouttask, Task stoptask)
		{
			var buf = new byte[count];
			await RepeatReadAsync(buf, 0, count, idletimeout, timeouttask, stoptask);
			return buf;
		}

		/// <summary>
		/// Gets the delimited sub stream.
		/// </summary>
		/// <returns>The delimited sub stream.</returns>
		/// <param name="delimiter">Delimiter.</param>
		/// <param name="idletime">The time to wait while idle</param>
		/// <param name="timeouttask">The task that signals request timeout</param>
		/// <param name="stoptask">The task that signals stop for the server</param>
		internal Stream GetDelimitedSubStream(byte[] delimiter, TimeSpan idletime, Task timeouttask, Task stoptask)
		{
			return new DelimitedSubStream(this, delimiter, idletime, timeouttask, stoptask);
		}

		/// <summary>
		/// Keeps reading from the stream until the supplied number of bytes are read.
		/// Throws an exception if the stream fails to provide the required bytes.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="buffer">The buffer to read into.</param>
		/// <param name="offset">The offset into the buffer where data is written.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <param name="idletimeout">The time to wait while idle</param>
		/// <param name="timeouttask">The task that signals request timeout</param>
		/// <param name="stoptask">The task that signals stop for the server</param>
		internal async Task RepeatReadAsync(byte[] buffer, int offset, int count, TimeSpan idletimeout, Task timeouttask, Task stoptask)
		{
			while (count > 0)
			{
                Task<int> rtask;
                Task rt;

                using (var cs = new CancellationTokenSource(idletimeout))
                {
                    rtask = ReadAsync(buffer, offset, count, cs.Token);
                    rt = await Task.WhenAny( stoptask, timeouttask, rtask);
                }

                // Timeout or stop has happened
				if (rt != rtask)
				{
					if (rt == stoptask)
						throw new TaskCanceledException();
					else
						throw new HttpException(HttpStatusCode.RequestTimeout); 
				}

				var r = rtask.Result;
				offset += r;
				count -= r;
				if (r == 0)
					throw new HttpException(HttpStatusCode.BadRequest);
			}
		}

		/// <summary>
		/// Reads the stream until a CRLF pair is encountered
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="maxcount">The maximum number of bytes to read before failing.</param>
		/// <param name="buffersize">The size of the read buffer.</param>
		/// <param name="idletimeout">The time to wait while idle</param>
		/// <param name="timeouttask">The task that signals request timeout</param>
		/// <param name="stoptask">The task that signals stop for the server</param>
		internal async Task<byte[]> ReadUntilCrlfAsync(int maxcount, int buffersize, TimeSpan idletimeout, Task timeouttask, Task stoptask)
		{
			var buf = new byte[buffersize];
			var offset = 0;

			// Avoid the stream if it all fits in a single package
			MemoryStream ms = null;

			while (maxcount > 0)
			{
                Task<int> rtask;
                Task rt;

                using (var cs = new CancellationTokenSource(idletimeout))
                {
                    rtask = ReadAsync(buf, offset, Math.Min(buf.Length - offset, maxcount), cs.Token);
                    rt = await Task.WhenAny( stoptask, timeouttask, rtask);
                }

                // Timeout or stop has happened
				if (rt != rtask)
				{
					if (rt == stoptask)
						throw new TaskCanceledException();
					else
						throw new HttpException(HttpStatusCode.RequestTimeout); 
				}

				var r = rtask.Result;
				offset += r;
				maxcount -= r;
				if (r == 0)
					throw new HttpException(HttpStatusCode.BadRequest);

				var ix = Array.IndexOf(buf, CR, 0, offset);
				if (ix >= 0 && ix < offset - 1)
				{
					if (ix != offset - 1)
						throw new HttpException(HttpStatusCode.BadRequest);
					
					if (ms == null)
					{
						Array.Resize(ref buf, ix);
						return buf;
					}
					else
					{
						await ms.WriteAsync(buf, 0, ix);
						return ms.ToArray();
					}
				}

				// Allocate a stream to pick up the chunks
				if (ms == null)
					ms = new MemoryStream();
				ms.Write(buf, 0, r - 1);
				buf[0] = buf[offset - 1];
				offset = 1;
			}

			throw new HttpException(HttpStatusCode.PayloadTooLarge);
		}

		/// <summary>
		/// Appends bytes to the internal buffer.
		/// </summary>
		/// <param name="data">The data to add.</param>
		/// <param name="offset">The data offset.</param>
		/// <param name="count">The number of bytes.</param>
		internal void UnreadBytesIntoBuffer(byte[] data, int offset, int count)
		{
			// If we have no buffered data, just add the new data to the buffer
			if (m_buffercount == 0)
			{
				// Make sure there is space
                if (m_buffer.Length < count)
                    Array.Resize(ref m_buffer, count);
				// Copy in the data
                Array.Copy(data, offset, m_buffer, 0, count);
				// Adjust the counters
				m_buffercount = count;
				m_bufferoffset = 0;
				m_remainingbytes += count;
            }
            // We need to pre-pend the data
            else
			{
				// If we can add the data before the current cursor, we do it
				if (m_bufferoffset < count)
				{
					Array.Copy(data, offset, m_buffer, m_bufferoffset - count, count);
					m_bufferoffset -= count;
					m_buffercount += count;
					m_remainingbytes += count;
				}
				else
				{
					// Make sure we have space for it
					if (m_buffer.Length < count + m_buffercount)
                        Array.Resize(ref m_buffer, m_buffer.Length + count);

					// Move the current data so the new data fits before it
					Array.Copy(m_buffer, m_bufferoffset, m_buffer, count, m_buffercount);
					// Then copy the new data behind it
					Array.Copy(data, offset, m_buffer, 0, count);
					m_bufferoffset = 0;
					m_buffercount += count;
                    m_remainingbytes += count;
                }				
			}
		}

		/// <summary>
		/// Reads data from the buffer
		/// </summary>
		/// <returns>The number of bytes read from the buffer.</returns>
		/// <param name="buffer">The target buffer.</param>
		/// <param name="offset">The target offset.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		private int ReadFromBuffer(byte[] buffer, int offset, int count)
		{
			if (m_buffercount > 0)
			{
				var size = Math.Min(count, m_buffercount);
				Array.Copy(m_buffer, m_bufferoffset, buffer, offset, size);
				m_buffercount -= size;
				m_bufferoffset = m_buffercount == 0 ? 0 : m_bufferoffset + size;

				return size;
			}

			return 0;
		}

		#region implemented abstract members of Stream
		/// <summary>
		/// Flush this instance.
		/// </summary>
		public override void Flush()
		{
			m_parent.Flush();
		}
        /// <summary>
        /// Seek the specified offset and origin.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="origin">Origin.</param>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        /// <summary>
        /// Sets the length.
        /// </summary>
        /// <param name="value">Value.</param>
        public override void SetLength(long value) => throw new NotImplementedException();

        /// <summary>
        /// Read data from the stream into the buffer asynchronously, given offset and count.
        /// </summary>
        /// <returns>The awaitable task.</returns>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The offset into the buffer where data is written.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			if (m_remainingbytes == 0)
				return 0;

			int res;
			var mread = (int)Math.Min(count, m_remainingbytes + 1);
			if (m_buffercount > 0)
				res = ReadFromBuffer(buffer, offset, mread);
			else
				res = await m_parent.ReadAsync(buffer, offset, mread, cancellationToken);
			
			m_remainingbytes -= res;
			if (m_remainingbytes < 0)
			{
				m_remainingbytes = 0;
				throw new HttpException(HttpStatusCode.PayloadTooLarge);
			}
			return res;
		}

		/// <summary>
		/// Read data from the stream into the buffer, given offset and count.
		/// </summary>
		/// <param name="buffer">The buffer to read into.</param>
		/// <param name="offset">The offset into the buffer where data is written.</param>
		/// <param name="count">The number of bytes to read.</param>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (m_remainingbytes == 0)
				return 0;
			
			int res;
			var mread = (int)Math.Min(count, m_remainingbytes + 1);
			if (m_buffercount > 0)
				res = ReadFromBuffer(buffer, offset, mread);
			else
				res = m_parent.Read(buffer, offset, mread);

			m_remainingbytes -= res;
			if (m_remainingbytes < 0)
			{
				m_remainingbytes = 0;
				throw new HttpException(HttpStatusCode.PayloadTooLarge);
			}
			return res;
		}
        /// <summary>
        /// Writes data from the stream into the buffer, given offset and count.
        /// </summary>
        /// <param name="buffer">The buffer to write into.</param>
        /// <param name="offset">The offset into the buffer where data is read from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether this instance can be read.
        /// </summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
        public override bool CanRead
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Gets a value indicating whether this instance can seek.
		/// </summary>
		/// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets a value indicating whether this instance can be written.
		/// </summary>
		/// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}
        /// <summary>
        /// Gets the length of the stream.
        /// </summary>
        /// <value>The length.</value>
        public override long Length => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <returns>The position.</returns>
        public override long Position 
        {
            get => m_maxread - m_remainingbytes;
            set => throw new NotImplementedException();
        }
        #endregion
    }
}

