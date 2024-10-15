using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
    /// <summary>
    /// A request handler that mirrors an external HTTP source,
    /// using a local cache.
    /// </summary>
    internal class FileMirrorHandler : FileHandler
    {
        /// <summary>
        /// Gets or sets the URL prefix being mirrored.
        /// </summary>
        public string UrlPrefix { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the mirror cache.
        /// </summary>
        public long MaxMirrorCacheSize { get; set; } = 1024 * 1024 * 1024 * 10L; // 10GiB

        /// <summary>
        /// Gets or sets the maximum count of the mirror cache.
        /// </summary>
        public long MaxMirrorCacheCount { get; set; } = 1024 * 50L; // 50k items

        /// <summary>
        /// Gets or sets the number of seconds to cache mirrored items.
        /// </summary>
        public TimeSpan MirrorCacheSeconds { get; set; } = TimeSpan.FromDays(7);

        /// <summary>
        /// Gets or sets the age of cached mirrored items found on startup.
        /// A positive number of seconds will make the mirrored items expire
        /// faster.
        /// </summary>
        public TimeSpan StartupMirrorCacheAgeSeconds { get; set; } = new TimeSpan(0);

        /// <summary>
        /// Gets or sets the maximum number of 404 paths to cache
        /// </summary>
        /// <value>The max404 cache count.</value>
        public long Max404CacheCount { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the number of seconds to cache a 404 response
        /// </summary>
        public TimeSpan Cache404Seconds { get; set; } = TimeSpan.FromHours(4);

        /// <summary>
        /// The lock object guarding the active transfer tables
        /// </summary>
        protected readonly object m_statuslock = new object();

        /// <summary>
        /// The LRU cache
        /// </summary>
        protected LRUCache<DateTime> m_filecache = null;

        /// <summary>
        /// The 404 cache
        /// </summary>
        protected LRUCache<DateTime> m_404cache = null;


        /// <summary>
        /// The size of files currently being transferred
        /// </summary>
        protected readonly Dictionary<string, long> m_activeTransferSizes = new Dictionary<string, long>();

        /// <summary>
        /// Trigger tasks for items currently being transferred
        /// </summary>
        protected readonly Dictionary<string, Task<long>> m_activeTransfers = new Dictionary<string, Task<long>>();


        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.Httpd.Handler.FileMirrorHandler"/> class.
        /// </summary>
        /// <param name="sourcefolder">The folder to serve files from.</param>
        public FileMirrorHandler(string sourcefolder)
            : base(sourcefolder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.Httpd.Handler.FileMirrorHandler"/> class.
        /// </summary>
        /// <param name="sourcefolder">The folder to server files from.</param>
        /// <param name="mimetypelookup">A mapping function to return the mime type for a given path.</param>
        public FileMirrorHandler(string sourcefolder, Func<IHttpRequestInternal, string, string> mimetypelookup)
            : base(sourcefolder, mimetypelookup)
        {
        }

        /// <summary>
        /// Creates a remote url from a remote request
        /// </summary>
        /// <param name="context">The request context.</param>
        protected virtual string BuildUrl(IHttpContext context)
        {
            return UrlPrefix.TrimEnd('/') + '/' + context.Request.Path.TrimStart('/');
        }

        /// <summary>
        /// Handles requests for transfers
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="context">The request context.</param>
        /// <param name="cancellationToken">The token indicating to stop handling.</param>
        public override async Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken)
        {
            if (!string.Equals(context.Request.Method, "GET", StringComparison.Ordinal) && !string.Equals(context.Request.Method, "HEAD", StringComparison.Ordinal))
            {
                if (PassThrough)
                    return false;

                throw new HttpException(HttpStatusCode.MethodNotAllowed);
            }

            // Basic validation and sanitization of the request
            string localpath;

            try
            {
                localpath = base.GetLocalPath(context);
                if (string.IsNullOrWhiteSpace(localpath))
                    return false;
            }
            catch
            {
                if (PassThrough)
                    return false;

                throw;
            }

            var mimetype = m_mimetypelookup(context.Request, localpath);
            if (mimetype == null)
            {
                if (PassThrough)
                    return false;

                throw new HttpException(HttpStatusCode.NotFound);
            }

            // Reject any directory access
            if (localpath.EndsWith("/"))
                throw new HttpException(Directory.Exists(localpath) ? HttpStatusCode.Forbidden : HttpStatusCode.NotFound);

            // Reject known 404 requests immediately
            if ((await m_404cache.TryGetUnlessAsync(localpath, (k, v) => Cache404Seconds.Ticks > 0 && (DateTime.Now - v) > Cache404Seconds)).Key)
            {
                if (PassThrough)
                    return false;

                throw new HttpException(Ceen.HttpStatusCode.NotFound);
            }

            // Check if the file is already downloaded in full
            if (!(await m_filecache.TryGetUnlessAsync(localpath, (k, v) => !File.Exists(localpath) || (MirrorCacheSeconds.Ticks > 0 && (DateTime.Now - v) > MirrorCacheSeconds))).Key)
            {
                // Not complete, check if we need to start a transfer
                TaskCompletionSource<long> tcs = null;
                Task<long> task;

                lock (m_statuslock)
                {
                    if (!m_activeTransfers.TryGetValue(localpath, out task))
                    {
                        m_activeTransferSizes[localpath] = -1;
                        m_activeTransfers[localpath] = task = (tcs = new TaskCompletionSource<long>()).Task;
                    }
                }

                if (File.Exists(localpath) && tcs != null)
                {
                    // We hit a race here where the file is downloaded and exists,
                    // but the active tables have been cleared just after we checked the filecache
                    if (m_filecache.TryGetValue(localpath, out var v))
                        return await base.HandleAsync(context, cancellationToken);
                }

                // Download if we are the first process requesting it
                var downloader = tcs == null ? null : Task.Run(() => DownloadRemoteFileAsync(context, localpath, tcs));

                // Then follow the download task
                await PiggyBackDownloadAsync(context, localpath, mimetype, task);
                return true;

            }

            return await base.HandleAsync(context, cancellationToken);
        }
        /// <summary>
        /// The predicate method for expiring a 404 entry
        /// </summary>
        /// <returns><c>true</c> if the item is expired, <c>false</c> otherwise.</returns>
        /// <param name="key">The path for the request.</param>
        /// <param name="value">The time the item was injected into the cache.</param>
        private bool Expire404Predicate(string key, DateTime value)
        {
            return Cache404Seconds.Ticks > 0 && (DateTime.Now - value) > Cache404Seconds;
        }

        /// <summary>
        /// Piggybacks a remote download and returns the stream as soon as data is ready
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="context">The request context.</param>
        /// <param name="localpath">The local path to use.</param>
        /// <param name="task">The initial task</param>
        private async Task PiggyBackDownloadAsync(IHttpContext context, string localpath, string mimetype, Task<long> task)
        {
            // Wait until some data is ready
            await task;

            // Grab the full length of the target file
            if (!m_activeTransferSizes.TryGetValue(localpath, out var fullsize))
                throw new HttpException(HttpStatusCode.BadGateway);

            // Write a minimal response with as much information as we have
            context.Response.ContentType = mimetype;
            context.Response.StatusCode = HttpStatusCode.OK;
            context.Response.SetExpires(CacheSeconds);
            if (fullsize > 0)
                context.Response.ContentLength = fullsize;

            // If this was a HEAD request, there is no more we need to do now
            if (string.Equals(context.Request.Method, "HEAD", StringComparison.Ordinal))
            {
                // Force the content-length header to be "wrong",
                // but allowed pr. RFC7230 3.3.2
                await context.Response.FlushHeadersAsync();
                return;
            }

            // Make sure we do not cache the response data
            if (fullsize <= 0)
                await context.Response.FlushHeadersAsync();

            var buf = new byte[8 * 1024];
            var written = 0L;
            // Since this is a transfer, we do not honor the processing timeout here
            //var ct = context.Request.TimeoutCancellationToken;

            // Start writing data to output
            using (var local = new FileStream(localpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var os = context.Response.GetResponseStream())
            {
                while (task != null)
                {
                    var bytesready = await task;
                    while (written < bytesready)
                    {
                        var chunck = (int)Math.Min(buf.Length, (bytesready - written));
                        var read = await local.ReadAsync(buf, 0, chunck);
                        if (read == 0)
                            break;

                        using(var ct = new CancellationTokenSource(ActivityTimeoutSeconds))
                            await os.WriteAsync(buf, 0, read, ct.Token);
                        written += read;
                    }

                    // Try until the transfer is complete
                    m_activeTransfers.TryGetValue(localpath, out task);
                }

                // In case we stopped before getting the full results
                int r;
                while ((r = (await local.ReadAsync(buf, 0, buf.Length))) != 0)
                    await os.WriteAsync(buf, 0, r);
            }
        }

        /// <summary>
        /// Attempts to get the size of a file
        /// </summary>
        /// <returns>The file size async.</returns>
        /// <param name="file">File.</param>
        /// <param name="lastQuery">The time the item was last queried</param>
        private static async Task<long> TryFileSizeAsync(string file, DateTime lastQuery)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    return new FileInfo(file).Length;
                }
                catch
                {
                    try { if (!File.Exists(file)) return 0; }
                    catch { }

                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }
            }

            return 0;
        }

        /// <summary>
        /// Helper method to make multiple attempts at deleting a file
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="file">The file to delete.</param>
        /// <param name="remove">A flag indicating if the file is removed</param>
        /// <param name="lastQuery">The time the item was last queried</param>
        private static async Task TryDeleteAsync(string file, DateTime lastQuery, bool remove)
        {
            if (remove)
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Delete(file);
                        return;
                    }
                    catch
                    {
                        try { if (!File.Exists(file)) return; }
                        catch { }

                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }

                }
            }
        }

        /// <summary>
        /// The result for getting the remote stream
        /// </summary>
        public struct RemoteStreamResult : IDisposable
        {
            /// <summary>
            /// The data stream
            /// </summary>
            public Stream Stream;
            /// <summary>
            /// The size of the result, or -1
            /// </summary>
            public long Size;
            /// <summary>
            /// The last-modified timestamp
            /// </summary>
            public DateTime LastModified;
            /// <summary>
            /// A list of items that should be disposed
            /// </summary>
            public IDisposable[] DisposableItems;

            /// <summary>
            /// Releases all resource used by the
            /// <see cref="T:Ceen.Httpd.Handler.FileMirrorHandler.RemoteStreamResult"/> object.
            /// </summary>
            /// <remarks>Call <see cref="Dispose"/> when you are finished using the
            /// <see cref="T:Ceen.Httpd.Handler.FileMirrorHandler.RemoteStreamResult"/>. The <see cref="Dispose"/>
            /// method leaves the <see cref="T:Ceen.Httpd.Handler.FileMirrorHandler.RemoteStreamResult"/> in an unusable
            /// state. After calling <see cref="Dispose"/>, you must release all references to the
            /// <see cref="T:Ceen.Httpd.Handler.FileMirrorHandler.RemoteStreamResult"/> so the garbage collector can
            /// reclaim the memory that the <see cref="T:Ceen.Httpd.Handler.FileMirrorHandler.RemoteStreamResult"/> was occupying.</remarks>
            public void Dispose()
            {
                if (DisposableItems != null)
                    foreach (var n in DisposableItems)
                        n.Dispose();
            }
        }

        /*
         * 
         * Use this method in an overridden class to mirror S3
         * 
         * 
        /// <summary>
        /// Gets the remote stream from AWS.
        /// </summary>
        /// <returns>The remote stream.</returns>
        /// <param name="context">The request context.</param>
        protected override async Task<RemoteStreamResult> GetRemoteStream(IHttpContext context)
        {
            AmazonS3Client client = null;
            GetObjectResponse resp = null;

            try
            {
                client = new AmazonS3Client(
                    new BasicAWSCredentials(AWSID, AWSKEY),
                    Amazon.RegionEndpoint.USEast1
                );

                resp = await client.GetObjectAsync(new GetObjectRequest() { 
                    BucketName = BUCKET, 
                    Key = context.Request.Path.TrimStart('/') 
                });

                return new RemoteStreamResult()
                {
                    Stream = resp.ResponseStream,
                    Size = resp.ContentLength,
                    LastModified = resp.LastModified,
                    DisposableItems = new IDisposable[] { client, resp }
                };
            }
            catch (Exception ex)
            {
                // Convert to normal NotFound
                if (ex is AmazonS3Exception s3ex && s3ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new HttpException(HttpStatusCode.NotFound);

                client?.Dispose();
                resp?.Dispose();
                throw;
            }
        }

        */

        /// <summary>
        /// Returns the remote stream, the expected length, and last-modified timestamp.
        /// This method can be overridden in a subclass to fetch from a non-http source
        /// </summary>
        /// <returns>The remote stream, length and last-modified values.</returns>
        /// <param name="context">The request context.</param>
        protected virtual async Task<RemoteStreamResult> GetRemoteStream(IHttpContext context)
        {
            HttpClient hc = null;
            HttpResponseMessage response = null;
            try
            {
                hc = new HttpClient();
                response = await hc.GetAsync(BuildUrl(context), HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                    throw new HttpException(response.StatusCode);

                var srclen = -1L;
                if (response.Headers.TryGetValues("Content-Length", out var headers))
                {
                    // If upstream sends a bad header, give up
                    if (!long.TryParse(headers?.FirstOrDefault(), out srclen))
                        throw new HttpException(Ceen.HttpStatusCode.BadGateway);
                }

                var lastmodified = new DateTime(0);
                if (response.Headers.TryGetValues("Last-Modified", out headers))
                {
                    DateTime.TryParseExact(headers?.FirstOrDefault(), "R", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeUniversal, out lastmodified);
                }

                return new RemoteStreamResult()
                {
                    Stream = await response.Content.ReadAsStreamAsync(),
                    Size = srclen,
                    LastModified = lastmodified,
                    DisposableItems = new IDisposable[] { hc, response }
                };
            }
            catch
            {
                hc?.Dispose();
                response?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Downloads the file from the remote URL, signalling the progress task along the way
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="context">The request context.</param>
        /// <param name="localpath">The path to store the file at.</param>
        private async Task DownloadRemoteFileAsync(IHttpContext context, string localpath, TaskCompletionSource<long> signal)
        {
            try
            {
                using (var resp = await GetRemoteStream(context))
                {
                    lock (m_statuslock)
                    {
                        // Signal all others that we now know the full source length
                        m_activeTransferSizes[localpath] = resp.Size;

                        // And ensure the target folder exists
                        if (!Directory.Exists(Path.GetDirectoryName(localpath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(localpath));
                    }

                    var pg = 0L;
                    using (var remote = resp.Stream)
                    using (var local = new FileStream(localpath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        // Note: Since we write to the actual file,
                        // we may interfere with active downloads of
                        // the file...

                        int r;
                        var buffer = new byte[8 * 1024];

                        // Read some data
                        while ((r = await remote.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            // Write it to the local file
                            await local.WriteAsync(buffer, 0, r);
                            await local.FlushAsync();

                            pg += r;

                            // Swap around and notify of the progress
                            var oldsignal = signal;
                            lock (m_statuslock)
                                m_activeTransfers[localpath] = (signal = new TaskCompletionSource<long>()).Task;
                            oldsignal.TrySetResult(pg);
                        }
                    }

                    // Update the file timestamp if possible
                    if (resp.LastModified.Ticks != 0)
                    {
                        try { File.SetLastWriteTime(localpath, resp.LastModified); }
                        catch { }
                    }

                    // Register that the file is now available as a normal file
                    await m_filecache.AddOrReplaceAsync(localpath, DateTime.Now);

                    // Now the cache returns the results, so we can unregister our callback
                    lock (m_statuslock)
                    {
                        m_activeTransferSizes.Remove(localpath);
                        m_activeTransfers.Remove(localpath);
                    }

                    // Prevent hanging followers
                    signal.TrySetResult(pg);
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpException he && he.StatusCode == HttpStatusCode.NotFound)
                    await m_404cache.AddOrReplaceAsync(localpath, DateTime.Now);

                lock (m_statuslock)
                {
                    m_activeTransferSizes.Remove(localpath);
                    m_activeTransfers.Remove(localpath);
                }

                signal.TrySetException(ex);
            }
        }

        /// <summary>
        /// Handles post configuration setup
        /// </summary>
        public override void AfterConfigure()
        {
            base.AfterConfigure();
            if (!Directory.Exists(SourceFolder))
                Directory.CreateDirectory(SourceFolder);

            m_filecache = new LRUCache<DateTime>(
                sizelimit: MaxMirrorCacheSize,
                countlimit: MaxMirrorCacheCount,
                expirationHandler: TryDeleteAsync,
                sizeHandler: TryFileSizeAsync
            );

            foreach (var f in Directory.EnumerateFiles(SourceFolder, "*", SearchOption.AllDirectories))
                m_filecache.AddOrReplaceAsync(f, DateTime.Now + StartupMirrorCacheAgeSeconds).Wait();

            m_404cache = new LRUCache<DateTime>(countlimit: Max404CacheCount);
        }
    }
}
