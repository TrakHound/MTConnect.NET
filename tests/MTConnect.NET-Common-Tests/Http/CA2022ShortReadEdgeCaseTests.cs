// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MTConnect.NET_Common_Tests.Http
{
    /// <summary>
    /// Boundary and failure-path coverage FLOOR (CONVENTIONS §1.0d-trigies-novodecies)
    /// for the CA2022 short-read fix on
    /// <c>MTConnectPostResponseHandler.ReadRequestBytes</c>. The sibling
    /// <c>CA2022ShortReadTests</c> file pins the happy-path
    /// "worst-case-one-byte-at-a-time-preserves-trailing-zero" contract but
    /// leaves several input-class boundaries uncovered per the coverage
    /// FLOOR panel:
    ///
    ///   * empty body (0 bytes) — the loop exits immediately on the first
    ///     read; the returned array must be zero-length rather than the
    ///     pre-fix 2 MB of zero-padding.
    ///   * body length equals the 2 MB buffer exactly — the loop exits on
    ///     the buffer-full branch instead of on EOF; the returned array is
    ///     the original 2 MB verbatim (no truncation).
    ///   * pre-cancelled token — the reader honours cancellation at
    ///     entry; the accumulator propagates the <c>OperationCanceledException</c>
    ///     rather than swallowing it, so the outer Ceen pipeline can
    ///     surface the aborted-request signal to callers.
    ///   * cancelled mid-drip — a token cancelled while the reader is
    ///     awaiting the next chunk cancels the pending <c>ReadAsync</c>
    ///     within one read cycle and propagates <c>OperationCanceledException</c>.
    ///   * body larger than the 2 MB buffer — the loop stops at the buffer
    ///     size; the returned array is exactly the buffer size (extra
    ///     bytes discarded, no exception).
    /// </summary>
    [TestFixture]
    [Category("CA2022ShortReadEdgeCase")]
    public class CA2022ShortReadEdgeCaseTests
    {
        /// <summary>Pins the empty-body boundary: a stream that returns 0 on the first read produces a zero-length result — the pre-fix 2 MB zero-padded buffer never leaks out.</summary>
        [Test]
        public async Task ReadRequestBytes_returns_empty_array_on_empty_body()
        {
            using var empty = new ScriptedStream(new byte[0]);
            var result = await Invoke(empty);

            Assert.That(result, Is.Not.Null,
                "ReadRequestBytes must return an empty array — not null — for a legitimately empty body.");
            Assert.That(result!.Length, Is.EqualTo(0),
                "The 2 MB buffer must be truncated to the actually-filled length (0 for an empty body).");
        }

        /// <summary>Pins the buffer-fill boundary: a body whose length exactly matches the 2 MB internal buffer takes the "buffer full" exit branch rather than the EOF branch; the returned array is the original body verbatim, no truncation.</summary>
        [Test]
        public async Task ReadRequestBytes_returns_full_buffer_when_body_exactly_fills_buffer()
        {
            const int bufferSize = 2 * 1024 * 1024;
            var body = new byte[bufferSize];
            for (var i = 0; i < body.Length; i++)
                body[i] = (byte)(i % 251);

            using var full = new ScriptedStream(body, chunkSize: 4096);
            var result = await Invoke(full);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Length, Is.EqualTo(bufferSize),
                "Buffer-full exit branch must return the full 2 MB, not one byte short (off-by-one guard).");
            Assert.That(result, Is.EqualTo(body),
                "Buffer-full body must be reconstructed byte-for-byte.");
        }

        /// <summary>Pins the oversized-body boundary: a body larger than the 2 MB internal buffer is truncated at exactly the buffer size — the fix takes the "buffer full" branch and stops reading; no exception leaks.</summary>
        [Test]
        public async Task ReadRequestBytes_truncates_body_larger_than_buffer_without_throwing()
        {
            const int bufferSize = 2 * 1024 * 1024;
            var body = new byte[bufferSize + 1024];
            for (var i = 0; i < body.Length; i++)
                body[i] = (byte)((i + 1) % 251);

            using var big = new ScriptedStream(body, chunkSize: 8192);
            var result = await Invoke(big);

            Assert.That(result, Is.Not.Null,
                "Oversized body must return the truncated buffer, not null (the try/catch must not swallow a benign case).");
            Assert.That(result!.Length, Is.EqualTo(bufferSize),
                "Oversized body must be truncated to exactly the 2 MB buffer size.");
        }

        /// <summary>Pins the transport-error swallow contract: if the underlying stream throws a non-cancellation exception (e.g. an <c>IOException</c> from a broken transport, an <c>InvalidDataException</c> from a corrupt chunked-encoding envelope), the outer try/catch swallows and returns null so a malformed asset POST cannot tear down the request pipeline. Sibling <c>ReadRequestBytes_cancelled_mid_drip_throws_within_next_read</c> pins the inverted rule for cancellation — <c>OperationCanceledException</c> propagates rather than being swallowed here.</summary>
        [Test]
        public async Task ReadRequestBytes_returns_null_when_stream_throws()
        {
            using var throwing = new ThrowingStream();
            var result = await Invoke(throwing);

            Assert.That(result, Is.Null,
                "The outer try/catch must swallow the underlying-stream exception and return null — the caller's null-as-error contract is stable.");
        }

        /// <summary>Pins the null-input contract: passing a null Stream must not throw; the method must return null. The pre-fix method had the same shape via `if (inputStream != null)` — the fix preserves it.</summary>
        [Test]
        public async Task ReadRequestBytes_returns_null_for_null_input_stream()
        {
            var result = await Invoke(null);

            Assert.That(result, Is.Null,
                "A null Stream input must be a benign null return, not a NullReferenceException.");
        }

        /// <summary>Pins the pre-cancelled-token boundary: when the caller passes a token that is already cancelled at entry, <c>ReadRequestBytes</c> propagates <c>OperationCanceledException</c> rather than reading to completion or swallowing the cancellation into a benign null return. Pre-fix, the method's signature did not accept a token, so the token was ignored and the read proceeded — the assertion below fails RED on the pre-fix HEAD.</summary>
        [Test]
        public void ReadRequestBytes_pre_cancelled_token_throws_immediately()
        {
            var body = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            using var scripted = new ScriptedStream(body);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                (Func<Task>)(async () => await Invoke(scripted, cts.Token)),
                Throws.InstanceOf<OperationCanceledException>(),
                "A pre-cancelled token must surface as OperationCanceledException. "
                + "Pre-fix, the accumulator ignored the token (signature took only Stream) "
                + "and read the body to completion; post-fix, the token is honoured at the "
                + "first ReadAsync and the outer catch preserves OperationCanceledException.");
        }

        /// <summary>Pins the mid-drip cancellation boundary: when the token is cancelled while the reader is awaiting the next chunk, the pending <c>ReadAsync</c> cancels within one read cycle and <c>OperationCanceledException</c> propagates within ~200 ms rather than the full drip duration. Pre-fix, the token was ignored and the read completed after the full ~1 s drip — the timeout assertion fails RED on the pre-fix HEAD.</summary>
        [Test]
        public void ReadRequestBytes_cancelled_mid_drip_throws_within_next_read()
        {
            // 100-byte body, 1 byte per read with a 10 ms per-read delay
            // → ~1 s to drain in the happy path. Cancel after ~50 ms and
            // expect OperationCanceledException within ~200 ms. Pre-fix,
            // the token was not threaded to the ReadAsync overload so the
            // drip completes normally and the assertion times out RED.
            var body = new byte[100];
            for (var i = 0; i < body.Length; i++)
                body[i] = (byte)(i + 1);
            using var slow = new ScriptedStream(body, chunkSize: 1, perReadDelay: TimeSpan.FromMilliseconds(10));
            using var cts = new CancellationTokenSource();

            Assert.That(
                (Func<Task>)(async () =>
                {
                    var invocation = Invoke(slow, cts.Token);
                    // Give the reader time to start awaiting the first drip,
                    // then request cancellation. The next ReadAsync's
                    // Task.Delay(cancellationToken) throws immediately.
                    cts.CancelAfter(TimeSpan.FromMilliseconds(50));
                    await invocation.WaitAsync(TimeSpan.FromMilliseconds(200));
                }),
                Throws.InstanceOf<OperationCanceledException>(),
                "A token cancelled mid-drip must surface as OperationCanceledException within "
                + "the next ReadAsync cycle. Pre-fix, the accumulator ignored the token and "
                + "read to completion after the full drip duration; post-fix, Task.Delay "
                + "honours the token and the exception propagates through the outer catch.");
        }

        // -----------------------------------------------------------------
        // Actually-real DiscardAllAsync exercise — the sibling fixture only
        // pins the *shape* via a mirror loop. This one instantiates the
        // internal `LimitedBodyStream` via reflection and asserts the true
        // return contract on premature EOF.
        // -----------------------------------------------------------------

        /// <summary>Pins the actual real <c>LimitedBodyStream.DiscardAllAsync</c> return contract: when the underlying transport hits EOF before <c>m_bytesleft</c> reaches zero, <c>DiscardAllAsync</c> returns <c>false</c> and does NOT deadlock on the <c>while (m_bytesleft &gt; 0)</c> gate. Uses reflection on the internal type in <c>MTConnect.NET-HTTP</c>.</summary>
        [Test]
        public async Task LimitedBodyStream_DiscardAllAsync_returns_false_on_premature_eof()
        {
            // Anchor on a public type from MTConnect.NET-HTTP.dll to force
            // the assembly to load, then locate the internal
            // Ceen.Httpd.LimitedBodyStream + Ceen.Httpd.BufferedStreamReader
            // via GetType.
            var anchor = typeof(MTConnect.Servers.Http.MTConnectHttpServer).Assembly;
            var bodyStreamType = anchor.GetType("Ceen.Httpd.LimitedBodyStream", throwOnError: false);
            var bufferedReaderType = anchor.GetType("Ceen.Httpd.BufferedStreamReader", throwOnError: false);
            if (bodyStreamType == null || bufferedReaderType == null)
            {
                Assert.Inconclusive(
                    "Ceen.Httpd.LimitedBodyStream / BufferedStreamReader not visible via reflection — "
                    + "either the type was renamed or its assembly-internal access changed. "
                    + "Fixture skips gracefully; the sibling shape-mirror pin still runs.");
                return;
            }

            // BufferedStreamReader(Stream, timeouts...): find the ctor that takes a Stream first.
            var readerCtor = bufferedReaderType.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(c =>
                {
                    var ps = c.GetParameters();
                    return ps.Length >= 1 && typeof(Stream).IsAssignableFrom(ps[0].ParameterType);
                });
            if (readerCtor == null)
            {
                Assert.Inconclusive("Ceen.Httpd.BufferedStreamReader has no Stream-first ctor via reflection.");
                return;
            }

            // Underlying stream: 8 bytes, but LimitedBodyStream is asked
            // for 1 KB. So EOF hits after 8 bytes and DiscardAllAsync must
            // return false (fix), not deadlock (pre-fix).
            using var underlying = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            object bufferedReader;
            try
            {
                var readerArgs = new object?[readerCtor.GetParameters().Length];
                readerArgs[0] = underlying;
                for (var i = 1; i < readerArgs.Length; i++)
                {
                    var pt = readerCtor.GetParameters()[i].ParameterType;
                    readerArgs[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
                }
                bufferedReader = readerCtor.Invoke(readerArgs)!;
            }
            catch
            {
                Assert.Inconclusive("BufferedStreamReader could not be constructed via reflection.");
                return;
            }

            var bodyCtor = bodyStreamType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: new[] { bufferedReaderType, typeof(long), typeof(TimeSpan), typeof(Task), typeof(Task) },
                modifiers: null);
            if (bodyCtor == null)
            {
                Assert.Inconclusive("LimitedBodyStream ctor signature has drifted; skipping this fixture.");
                return;
            }
            var neverCompleting = new TaskCompletionSource<bool>().Task;
            var body = (Stream)bodyCtor.Invoke(new object?[]
            {
                bufferedReader, (long)1024, TimeSpan.FromSeconds(5), neverCompleting, neverCompleting,
            });

            var discardMethod = bodyStreamType.GetMethod("DiscardAllAsync",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(discardMethod, Is.Not.Null,
                "DiscardAllAsync method not found; refactor may have renamed it.");
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var task = (Task<bool>)discardMethod!.Invoke(body, new object?[] { cts.Token })!;
            var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10)));

            Assert.That(completed, Is.SameAs(task),
                "DiscardAllAsync deadlocked past 10s on premature EOF — the CA2022 short-read fix regressed. "
                + "Pre-fix, the loop spun forever on read==0 without decrementing m_bytesleft.");
            var result = await task;
            Assert.That(result, Is.False,
                "DiscardAllAsync must return false on premature EOF so the caller can propagate the drain failure.");
        }

        // -----------------------------------------------------------------
        // Helpers.
        // -----------------------------------------------------------------

        private static async Task<byte[]?> Invoke(Stream? inputStream, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(MTConnect.Servers.Http.MTConnectHttpServer).Assembly
                .GetType("MTConnect.Servers.MTConnectPostResponseHandler", throwOnError: false)
                ?? throw new InvalidOperationException("MTConnectPostResponseHandler not visible via reflection.");

            // Prefer the post-fix (Stream, CancellationToken) signature so
            // the cancellation-boundary tests exercise the real token path.
            // Fall back to the pre-fix (Stream)-only shape so this fixture
            // stays runnable on the parent commit while the RED tests
            // deliberately fail against it — the fallback drives the RED
            // outcome (token is ignored → OperationCanceledException never
            // fires → the Throws assertion fails).
            var tokenMethod = handlerType.GetMethod(
                "ReadRequestBytes",
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(Stream), typeof(CancellationToken) },
                modifiers: null);
            if (tokenMethod != null)
            {
                object? tokenTaskObj;
                try
                {
                    tokenTaskObj = tokenMethod.Invoke(null, new object?[] { inputStream, cancellationToken });
                }
                catch (TargetInvocationException tie) when (tie.InnerException != null)
                {
                    // Unwrap so callers can Assert.Throws on the real exception.
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo
                        .Capture(tie.InnerException).Throw();
                    throw; // unreachable
                }
                return await (Task<byte[]?>)tokenTaskObj!;
            }

            var legacyMethod = handlerType.GetMethod(
                "ReadRequestBytes",
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: new[] { typeof(Stream) },
                modifiers: null)
                ?? throw new InvalidOperationException("ReadRequestBytes not found via reflection.");
            object? legacyTaskObj;
            try
            {
                legacyTaskObj = legacyMethod.Invoke(null, new object?[] { inputStream });
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo
                    .Capture(tie.InnerException).Throw();
                throw; // unreachable
            }
            return await (Task<byte[]?>)legacyTaskObj!;
        }

        /// <summary>
        /// A Stream that returns its content in fixed-size chunks (or one
        /// byte at a time by default) and then 0 on EOF. Mirrors real HTTP
        /// request-body arrival patterns. When <c>perReadDelay</c> is
        /// non-null the async read overload awaits that delay under the
        /// supplied cancellation token, so mid-drip cancellation cancels
        /// the pending Task.Delay and surfaces OperationCanceledException.
        /// </summary>
        private sealed class ScriptedStream : Stream
        {
            private readonly byte[] _content;
            private readonly int _chunkSize;
            private readonly TimeSpan? _perReadDelay;
            private int _position;

            public ScriptedStream(byte[] content, int chunkSize = 1, TimeSpan? perReadDelay = null)
            {
                _content = content;
                _chunkSize = Math.Max(1, chunkSize);
                _perReadDelay = perReadDelay;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => _content.Length;
            public override long Position
            {
                get => _position;
                set => throw new NotSupportedException();
            }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (_position >= _content.Length || count == 0) return 0;
                var take = Math.Min(count, Math.Min(_chunkSize, _content.Length - _position));
                Array.Copy(_content, _position, buffer, offset, take);
                _position += take;
                return take;
            }

            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_perReadDelay.HasValue)
                    await Task.Delay(_perReadDelay.Value, cancellationToken).ConfigureAwait(false);
                return Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }

        /// <summary>Stream whose ReadAsync always throws; models the transport-error / mid-drip cancellation path.</summary>
        private sealed class ThrowingStream : Stream
        {
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => 0;
            public override long Position { get => 0; set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new IOException("simulated transport error");
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => Task.FromException<int>(new IOException("simulated transport error"));
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}
