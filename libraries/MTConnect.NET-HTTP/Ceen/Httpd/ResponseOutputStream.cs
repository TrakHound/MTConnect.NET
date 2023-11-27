using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Ceen.Common;

namespace Ceen.Httpd
{
	/// <summary>
	/// Wrapper class for providing access to the output,
	/// while allowing auto-setting and flushing of the headers
	/// </summary>
	internal class ResponseOutputStream : Stream
	{
		/// <summary>
		/// The stream into which data should be written
		/// </summary>
		private Stream m_parent;

		/// <summary>
		/// The response object that this stream is attached to
		/// </summary>
		private HttpResponse m_response;

		/// <summary>
		/// The optionally buffered content
		/// </summary>
		private MemoryStream m_buffer = null;

		/// <summary>
		/// A value indicating if a write should simply be sent to the underlying stream
		/// </summary>
		private bool m_passThrough = false;

		/// <summary>
		/// The number of bytes written
		/// </summary>
		private long m_written = 0;

		/// <summary>
		/// Flag for keeping track of the disposed state
		/// </summary>
		private bool m_isDisposed = false;

		/// <summary>
		/// The maximum number of bytes to buffer before sending data without a Content-Length header
		/// </summary>
		private const int MAX_RESPONSE_BUFFER = 5 * 1024 * 1024;

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.ResponseOutputStream"/> class.
		/// </summary>
		/// <param name="parent">The stream where data is written to.</param>
		/// <param name="response">The response instance that this stream is attached to.</param>
		public ResponseOutputStream(Stream parent, HttpResponse response)
		{
			m_parent = parent;
			m_response = response;
		}

		/// <summary>
		/// Sets the Content-Length header and writes data to the stream.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="forceLength">If set to <c>true</c> overwrite the Content-Length header.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		internal async Task SetLengthAndFlushAsync(bool forceLength, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!m_response.HasSentHeaders)
			{
				if (m_response.ContentLength < 0 || forceLength)
					m_response.ContentLength = m_buffer == null ? 0 : m_buffer.Length;
				await m_response.FlushHeadersAsync();
			}

			if (m_buffer != null)
			{
				m_buffer.Position = 0;
				await m_buffer.CopyToAsync(m_parent, 8 * 1024, cancellationToken);
				m_buffer = null;
			}
		}

		/// <summary>
		/// Clears the internal buffer
		/// </summary>
		internal void Clear()
		{
			if (m_buffer != null)
			{
				m_written -= m_buffer.Length;
				m_buffer = null;
			}
		}

		#region implemented abstract members of Stream

		/// <summary>
		/// Flush the contents to the underlying stream.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		public override async Task FlushAsync(CancellationToken cancellationToken)
		{
			await SetLengthAndFlushAsync(false, cancellationToken);
			await m_parent.FlushAsync(cancellationToken);
		}

		/// <summary>
		/// Flush this instance.
		/// </summary>
		public override void Flush()
		{
			SyncAwaiter.WaitSync(() => FlushAsync(CancellationToken.None));
		}

		/// <summary>
		/// Seek the specified offset and origin.
		/// </summary>
		/// <param name="offset">Offset.</param>
		/// <param name="origin">Origin.</param>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Sets the length.
		/// </summary>
		/// <param name="value">Value.</param>
		public override void SetLength(long value)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Read data from the stream into the buffer, given offset and count.
		/// </summary>
		/// <param name="buffer">The buffer to read into.</param>
		/// <param name="offset">The offset into the buffer where data is written.</param>
		/// <param name="count">The number of bytes to read.</param>
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Writes data from the stream into the buffer, given offset and count.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="buffer">The buffer to write into.</param>
		/// <param name="offset">The offset into the buffer where data is read from.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (m_isDisposed)
				throw new ObjectDisposedException("ResponseOutputStream");

			if (!m_passThrough)
			{
				if (m_response.HasSentHeaders || m_response.ContentLength >= 0 || m_buffer?.Length + count > MAX_RESPONSE_BUFFER)
				{
					m_passThrough = true;
				}
				else
				{
					if (m_buffer == null)
						m_buffer = new MemoryStream();

					await m_buffer.WriteAsync(buffer, offset, count, cancellationToken);
					m_written += count;
					return;
				}

				// If we get here, we dump what we have
				await SetLengthAndFlushAsync(false, cancellationToken);
			}

			await m_parent.WriteAsync(buffer, offset, count, cancellationToken);
			m_written += count;
		}

		/// <summary>
		/// Writes data from the stream into the buffer, given offset and count.
		/// </summary>
		/// <param name="buffer">The buffer to write into.</param>
		/// <param name="offset">The offset into the buffer where data is read from.</param>
		/// <param name="count">The number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
            SyncAwaiter.WaitSync(() => WriteAsync(buffer, offset, count));
		}

		/// <summary>
		/// Gets a value indicating whether this instance can be read.
		/// </summary>
		/// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
		public override bool CanRead => false;

		/// <summary>
		/// Gets a value indicating whether this instance can seek.
		/// </summary>
		/// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
		public override bool CanSeek => false;

		/// <summary>
		/// Gets a value indicating whether this instance can be written.
		/// </summary>
		/// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
		public override bool CanWrite => m_parent.CanWrite;

		/// <summary>
		/// Gets the length of the stream.
		/// </summary>
		/// <value>The length.</value>
		public override long Length => m_written;

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <returns>The position.</returns>
		public override long Position
		{
			get => m_written;
			set => throw new NotImplementedException();
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
            if (disposing)
			{
                Flush();
                m_isDisposed = true;
            }
		}

		/// <summary>
		/// Asyncronously dispose resources held by the stream
		/// </summary>
		/// <returns>An awaitable task</returns>
		protected virtual async ValueTask DisposeAsyncCore()
		{
			if (!m_isDisposed)
			{
				await FlushAsync();
				m_isDisposed = true;
			}
		}		


		/// <summary>
		/// Unbuffers the contents of this stream and puts it into the target stream
		/// </summary>
		/// <param name="target">The stream to unbuffer to</param>
		public void Unbuffer(Stream target) 
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			if (m_passThrough)
				throw new InvalidOperationException("Cannot unbuffer when the stream is no longer buffering");
			
			// Copy the current buffer reference
			var buf = m_buffer;

			// Mark this instance as not buffering
			Clear();

			// Copy to the target, which may start a new buffer on this instance
			if (buf != null)
				buf.CopyTo(target);
		}

		#endregion
	}
}