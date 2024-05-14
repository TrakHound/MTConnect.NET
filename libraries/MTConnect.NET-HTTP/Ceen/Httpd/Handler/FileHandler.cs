using System.Threading;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Ceen.Httpd.Handler
{
    /// <summary>
    /// Basic implementation of a file-serving module
    /// </summary>
    internal class FileHandler : IHttpModuleWithSetup
    {
        /// <summary>
        /// Helper method to allow syntax similar to the Linq Any() call with async methods
        /// </summary>
        /// <returns><c>true</c> if an item was found, <c>false</c> otherwise.</returns>
        /// <param name="sequence">The items to look in.</param>
        /// <param name="predicate">The async predicate function.</param>
        /// <typeparam name="T">The data type parameter.</typeparam>
        private static async Task<bool> Any<T>(IEnumerable<T> sequence, Func<T, Task<bool>> predicate)
        {
            foreach (var n in sequence)
                if (await predicate(n))
                    return true;

            return false;
        }

        /// <summary>
        /// Helper method to allow syntax similar to the Linq FirstOrDefault() call with async methods
        /// </summary>
        /// <returns>The result or default(<typeparamref name="T"/>).</returns>
        /// <param name="sequence">The items to look in.</param>
        /// <param name="predicate">The async predicate function.</param>
        /// <typeparam name="T">The data type parameter.</typeparam>
        private static async Task<T> FirstOrDefault<T>(IEnumerable<T> sequence, Func<T, Task<bool>> predicate)
        {
            foreach (var n in sequence)
                if (await predicate(n))
                    return n;

            return default(T);
        }

        /// <summary>
        /// Interface for interacting with a virtual filesystem
        /// </summary>
        public interface IVirtualFileSystem
        {
            /// <summary>
            /// Returns a value determining if the given file exists
            /// </summary>
            /// <returns><c>true</c>, if the file exists, <c>false</c> otherwise.</returns>
            /// <param name="path">The full path to examine.</param>
            Task<bool> FileExistsAsync(string path);

            /// <summary>
            /// Returns a value determining if the given folder exists
            /// </summary>
            /// <returns><c>true</c>, if the folder exists, <c>false</c> otherwise.</returns>
            /// <param name="path">The full path to examine.</param>
            Task<bool> FolderExistsAsync(string path);

            /// <summary>
            /// Returns the last time the file was written
            /// </summary>
            /// <returns>The last file write time in UTC.</returns>
            /// <param name="path">The full path to examine.</param>
            Task<DateTime> GetLastFileWriteTimeUtcAsync(string path);

            /// <summary>
            /// Opens the given file for reading
            /// </summary>
            /// <returns>The stream.</returns>
            /// <param name="path">The full path of the file to open.</param>
            Task<Stream> OpenReadAsync(string path);

            /// <summary>
            /// Returns the path used for MIME type checking
            /// </summary>
            /// <returns>The path used for MIME type checking.</returns>
            /// <param name="path">The path to change</param>
            Task<string> GetMimeTypePathAsync(string path);
        }

        /// <summary>
        /// Implementation of a virtual filesystem that maps to the local filesystem
        /// </summary>
        public class FileSystem : IVirtualFileSystem
        {
            /// <inheritdoc />
            public Task<bool> FolderExistsAsync(string path) => Task.FromResult(Directory.Exists(path));
            /// <inheritdoc />
            public Task<bool> FileExistsAsync(string path) => Task.FromResult(File.Exists(path));
            /// <inheritdoc />
            public Task<DateTime> GetLastFileWriteTimeUtcAsync(string path) => Task.FromResult(File.GetLastWriteTimeUtc(path));
            /// <inheritdoc />
            public Task<Stream> OpenReadAsync(string path) => Task.FromResult((Stream)File.OpenRead(path));
            /// <inheritdoc />
            public Task<string> GetMimeTypePathAsync(string path) => Task.FromResult(path);
        }

        /// <summary>
        /// Simple composite virtual filesystem that picks the first match, if any
        /// </summary>
        public class CompositeFileSystem : IVirtualFileSystem
        {
            /// <summary>
            /// The list of virtual filesystems
            /// </summary>
            private readonly IVirtualFileSystem[] m_vfsList;
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.FileHandler.CompositeFileSystem"/> class.
            /// </summary>
            /// <param name="virtualFileSystems">The virtual file systems in prefered order.</param>
            public CompositeFileSystem(params IVirtualFileSystem[] virtualFileSystems)
            {
                if (virtualFileSystems == null || virtualFileSystems.Any(x => x == null))
                    throw new ArgumentNullException(nameof(virtualFileSystems));
                m_vfsList = virtualFileSystems;
            }

            /// <summary>
            /// Gets the first virtual filesystem where the file exists, or <c>null</c>
            /// </summary>
            /// <returns>The first match, or <c>null</c>.</returns>
            /// <param name="path">The path to match.</param>
            private Task<IVirtualFileSystem> MatchFileAsync(string path) => FirstOrDefault(m_vfsList, x => x.FileExistsAsync(path));

            /// <summary>
            /// Gets the first virtual filesystem where the directory exists, or <c>null</c>
            /// </summary>
            /// <returns>The first match, or <c>null</c>.</returns>
            /// <param name="path">The path to match.</param>
            private Task<IVirtualFileSystem> MatchFolderAsync(string path) => FirstOrDefault(m_vfsList, x => x.FolderExistsAsync(path));

            /// <inheritdoc />
            public async Task<bool> FileExistsAsync(string path) => await MatchFileAsync(path).ContinueWith(x => x.Result.FileExistsAsync(path)).ConfigureAwait(false) != null;
            /// <inheritdoc />
            public async Task<bool> FolderExistsAsync(string path) => await MatchFolderAsync(path).ConfigureAwait(false) != null;
            /// <inheritdoc />
            public async Task<DateTime> GetLastFileWriteTimeUtcAsync(string path) => await (await MatchFileAsync(path).ConfigureAwait(false)).GetLastFileWriteTimeUtcAsync(path).ConfigureAwait(false);
            /// <inheritdoc />
            public async Task<Stream> OpenReadAsync(string path) => await (await MatchFileAsync(path).ConfigureAwait(false)).OpenReadAsync(path).ConfigureAwait(false);
            /// <inheritdoc />
            public async Task<string> GetMimeTypePathAsync(string path) => await (await MatchFileAsync(path).ConfigureAwait(false)).GetMimeTypePathAsync(path).ConfigureAwait(false);
        }

        /// <summary>
        /// A virtual filesystem that works on remapped paths
        /// </summary>
        public class RemappedVirtualFileSystem : IVirtualFileSystem
        {
            /// <summary>
            /// The remap function
            /// </summary>
            private readonly Func<string, string> m_pathRemap;

            /// <summary>
            /// The method used to perform mime-type remapping
            /// </summary>
            private readonly Func<string, Task<string>> m_mimeRemap;

            /// <summary>
            /// The method used to check for file existence
            /// </summary>
            private readonly Func<string, Task<bool>> m_fileExists;

            /// <summary>
            /// The method used to check for folder existince
            /// </summary>
            private readonly Func<string, Task<bool>> m_folderExists;

            /// <summary>
            /// The function used to get the last write time
            /// </summary>
            private readonly Func<string, Task<DateTime>> m_lastWriteTime;

            /// <summary>
            /// The method used to get the stream with contents
            /// </summary>
            private readonly Func<string, Task<Stream>> m_open;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.FileHandler.RemappedVirtualFileSystem"/> class.
            /// </summary>
            /// <param name="pathRemap">The optional function used to remap paths.</param>
            /// <param name="fileExists">The optional function used to check for a file</param>
            /// <param name="folderExists">The optional function used to check for a folder</param>
            /// <param name="lastWriteTime">The optional function used to get a files last write time</param>
            /// <param name="openFile">The optional method used to get the file contents</param>
            /// <param name="mimeRemap">The optional method used to get the mime path</param>
            public RemappedVirtualFileSystem(Func<string, string> pathRemap = null, Func<string, Task<Stream>> openFile = null, Func<string, Task<bool>> fileExists = null, Func<string, Task<bool>> folderExists = null, Func<string, Task<DateTime>> lastWriteTime = null, Func<string, Task<string>> mimeRemap = null)
            {
                m_pathRemap = pathRemap ?? (x => x);
                m_mimeRemap = mimeRemap ?? (x => Task.FromResult(x));
                m_fileExists = fileExists ?? (x => Task.FromResult(File.Exists(x)));
                m_folderExists = folderExists ?? (x => Task.FromResult(Directory.Exists(x)));
                m_lastWriteTime = lastWriteTime ?? (x => Task.FromResult(File.GetLastWriteTimeUtc(x)));
                m_open = openFile ?? (x => Task.FromResult((Stream)File.OpenRead(x)));
            }

            /// <inheritdoc />
            public Task<bool> FileExistsAsync(string path) => m_fileExists(m_pathRemap(path));
            /// <inheritdoc />
            public Task<bool> FolderExistsAsync(string path) => m_folderExists(m_pathRemap(path));
            /// <inheritdoc />
            public Task<DateTime> GetLastFileWriteTimeUtcAsync(string path) => m_lastWriteTime(m_pathRemap(path));
            /// <inheritdoc />
            public Task<Stream> OpenReadAsync(string path) => m_open(m_pathRemap(path));
            /// <inheritdoc />
            public Task<string> GetMimeTypePathAsync(string path) => m_mimeRemap(m_pathRemap(path));
        }

        /// <summary>
        /// The folder where files are served from
        /// </summary>
        public string SourceFolder { get; set; }
        /// <summary>
        /// Cached copy of the directory separator as a string
        /// </summary>
        protected static readonly string DIRSEP = Path.DirectorySeparatorChar.ToString();
        /// <summary>
        /// Parser to match Range requests
        /// </summary>
        protected static readonly Regex RANGE_MATCHER = new Regex("bytes=(?<start>\\d*)-(?<end>\\d*)");
        /// <summary>
        /// Chars that are not allowed in the path
        /// </summary>
        protected static readonly string[] FORBIDDENCHARS = { "\\", "..", ":" };
        /// <summary>
        /// Function that maps a request to a mime type
        /// </summary>
        protected Func<IHttpRequestInternal, string, string> m_mimetypelookup;
        /// <summary>
        /// List of allowed index documents
        /// </summary>
        public string[] IndexDocuments { get; set; } = new string[] { "index.html", "index.htm" };
        /// <summary>
        /// List of allowed index file extensions
        /// </summary>
        public string[] AutoProbeExtensions { get; set; } = new string[] { ".html", ".htm" };
        /// <summary>
        /// The current etag cache
        /// </summary>
        protected readonly Dictionary<string, KeyValuePair<string, long>> m_etagCache = new Dictionary<string, KeyValuePair<string, long>>();
        /// <summary>
        /// The lock used to guard the ETag cache
        /// </summary>
        protected readonly AsyncLock m_etagLock = new AsyncLock();
        /// <summary>
        /// The regular expression used to extract etags from the request
        /// </summary>
        protected readonly Regex ETAG_RE = new Regex(@"\s*(?<isWeak>W\\)?""(?<etag>\w+)""\s*,?");
        /// <summary>
        /// The etag salt in byte-array format
        /// </summary>
        protected byte[] m_etagsalt = null;

        /// <summary>
        /// Gets or sets the etag hashing algorithm.
        /// </summary>
        public string EtagAlgorithm { get; set; } = "MD5";
        /// <summary>
        /// An optional etag salt
        /// </summary>
        public string EtagSalt { get; set; } = null;
        /// <summary>
        /// Gets or sets the path prefix
        /// </summary>
        public string PathPrefix { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating if this module is simply acting as a rewrite filter,
        /// that is it converts /test to /test.html or /test/index.html if possible.
        /// Using a rewrite filter can simplify other filters as you can write a *.html filter,
        /// and avoid other triggers activating on non *.html files
        /// </summary>
        public bool RedirectOnly { get; set; } = false;

        /// <summary>
        /// Enable the ETag header output, which returns an MD5 value for each.
        /// Setting this to less than zero will disable ETag output.
        /// Setting this to zero will emit ETag output, but not cache the results,
        /// causing an etag to be computed for each request.
        /// </summary>
        public int ETagCacheSize { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the number of seconds the browser is allowed to cache the response.
        /// </summary>
        public TimeSpan CacheSeconds { get; set; } = TimeSpan.FromDays(1);

        /// <summary>
        /// By default, the file handler module with report 404 - Not Found,
        /// if the item could not be processed, along with other appropriate
        /// error messages. If this property is set to <c>true</c> the module
        /// will instead ignore the request, allowing other modules to handle
        /// the request
        /// </summary>
        public bool PassThrough { get; set; } = false;

        /// <summary>
        /// The number of seconds to wait when writing a response chunk to the client
        /// </summary>
        public TimeSpan ActivityTimeoutSeconds { get; set; } = TimeSpan.FromSeconds(5);
        /// <summary>
        /// The size of chunks read from the VFS and sent to the client during transfers 
        /// </summary>
        public long TransferChunkSize { get; set; } = 8 * 1024;

        /// <summary>
        /// The virtual filesystem used for all requests
        /// </summary>
        protected readonly IVirtualFileSystem m_vfs;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.FileHandler"/> class.
        /// </summary>
        /// <param name="sourcefolder">The folder to server files from.</param>
        public FileHandler(string sourcefolder)
            : this(sourcefolder, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.Httpd.Handler.FileHandler"/> class.
        /// </summary>
        /// <param name="sourcefolder">The folder to server files from.</param>
        /// <param name="mimetypelookup">A mapping function to return the mime type for a given path.</param>
        public FileHandler(string sourcefolder, Func<IHttpRequestInternal, string, string> mimetypelookup = null)
            : this(new FileSystem(), mimetypelookup)
        {
            SourceFolder = sourcefolder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.FileHandler"/> class.
        /// </summary>
        /// <param name="vfs">The virtual filesystem to use.</param>
        /// <param name="mimetypelookup">Mimetypelookup.</param>
        public FileHandler(IVirtualFileSystem vfs, Func<IHttpRequestInternal, string, string> mimetypelookup = null)
        {
            m_vfs = vfs ?? throw new ArgumentNullException(nameof(vfs));
            m_mimetypelookup = mimetypelookup ?? DefaultMimeTypes;
        }

        /// <summary>
        /// An overrideable method to hook in logic before
        /// flushing the headers and sending content, allows
        /// an overriding class to alter the response
        /// </summary>
        /// <param name="context">The request context.</param>
        /// <param name="sourcedata">The file with the source data</param>
        public virtual Task BeforeResponseAsync(IHttpContext context, Stream sourcedata)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Workaround for <see name="System.Security.Cryptography.HashAlgorithm.Create" /> not being correctly supported in .Net core
        /// </summary>
        /// <param name="id">The algorithm name</param>
        /// <returns>The hash algorithm instance</returns>
        private static System.Security.Cryptography.HashAlgorithm CreateFromId(string id)
        {
            return System.Security.Cryptography.CryptoConfig.CreateFromName(id) as System.Security.Cryptography.HashAlgorithm;
        }

        /// <summary>
        /// Computes the ETag value for a given resources
        /// </summary>
        /// <returns>The ETag value.</returns>
        /// <param name="sourcedata">The source stream to compute the ETag for.</param>
        public virtual async Task<string> ComputeETag(Stream sourcedata)
        {
            var buffer = new byte[8 * 1024];
            using (var hasher = string.IsNullOrWhiteSpace(EtagAlgorithm) ? System.Security.Cryptography.MD5.Create() : CreateFromId(EtagAlgorithm))
            {
                if (m_etagsalt != null)
                    hasher.TransformBlock(m_etagsalt, 0, m_etagsalt.Length, m_etagsalt, 0);

                int r = 0;
                while ((r = await sourcedata.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    hasher.TransformBlock(buffer, 0, r, buffer, 0);
                hasher.TransformFinalBlock(buffer, 0, 0);
                return Convert.ToBase64String(hasher.Hash).TrimEnd('=');
            }
        }

        /// <summary>
        /// Helper method to report the given range as invalid
        /// </summary>
        /// <returns><c>true</c></returns>
        /// <param name="context">The request context.</param>
        /// <param name="bytecount">The byte count to report</param>
        private bool SetInvalidRangeHeader(IHttpContext context, long bytecount)
        {
            context.Response.StatusCode = HttpStatusCode.RangeNotSatisfiable;
            context.Response.Headers["Content-Range"] = "bytes */" + bytecount;
            return true;
        }

        /// <summary>
        /// Helper method to report 304 - Not modified to the client
        /// </summary>
        /// <returns><c>true</c></returns>
        /// <param name="context">The request context.</param>
        /// <param name="etag">The resource ETag, if any.</param>
        private bool SetNotModified(IHttpContext context, string etag)
        {
            context.Response.StatusCode = HttpStatusCode.NotModified;
            context.Response.ContentLength = 0;
            context.Response.SetExpires(CacheSeconds);
            if (!string.IsNullOrWhiteSpace(etag))
                context.Response.Headers["ETag"] = $"\"{etag}\"";
            return true;
        }

        /// <summary>
        /// Extracts and validates the local path from the remote request
        /// </summary>
        /// <returns>The local path.</returns>
        /// <param name="context">The request context.</param>
        protected virtual string GetLocalPath(IHttpContext context)
        {
            var pathrequest = Uri.UnescapeDataString(context.Request.Path);

            foreach (var c in FORBIDDENCHARS)
                if (pathrequest.Contains(c))
                    throw new HttpException(HttpStatusCode.BadRequest);

            if (!pathrequest.StartsWith(PathPrefix, StringComparison.Ordinal))
                return null;

            pathrequest = pathrequest.Substring(PathPrefix.Length);


            var path = pathrequest.Replace("/", DIRSEP);
            while (path.StartsWith(DIRSEP, StringComparison.Ordinal))
                path = path.Substring(1);

            path = Path.Combine(SourceFolder, path);
            if (!path.StartsWith(SourceFolder, StringComparison.Ordinal))
                throw new HttpException(HttpStatusCode.BadRequest);

            return path;
        }

        /// <summary>
        /// Performs internal redirects on paths with missing trailings slashes
        /// and handles redirects to index files
        /// </summary>
        /// <returns><c>true</c>, if redirect was issued, <c>false</c> otherwise.</returns>
        /// <param name="path">The local path to use.</param>
        /// <param name="context">The request context.</param>
        protected virtual async Task<bool> AutoRedirect(string path, IHttpContext context)
        {
            if (!await m_vfs.FileExistsAsync(path))
            {
                if (string.IsNullOrWhiteSpace(Path.GetExtension(path)) && !path.EndsWith("/", StringComparison.Ordinal) && !path.EndsWith(".", StringComparison.Ordinal))
                {
                    var ix = await FirstOrDefault(AutoProbeExtensions, p => m_vfs.FileExistsAsync(path + p));
                    if (!string.IsNullOrWhiteSpace(ix))
                    {
                        context.Response.InternalRedirect(context.Request.Path + ix);
                        return true;
                    }
                }

                if (await m_vfs.FolderExistsAsync(path))
                {
                    if (!context.Request.Path.EndsWith("/", StringComparison.Ordinal))
                    {
                        if (!await Any(IndexDocuments, p => m_vfs.FileExistsAsync(Path.Combine(path, p))))
                        {
                            if (PassThrough)
                                return false;

                            throw new HttpException(HttpStatusCode.NotFound);
                        }

                        context.Response.Redirect(context.Request.Path + "/");
                        return true;
                    }

                    var ix = await FirstOrDefault(IndexDocuments, p => m_vfs.FileExistsAsync(Path.Combine(path, p)));
                    if (!string.IsNullOrWhiteSpace(ix))
                    {
                        context.Response.InternalRedirect(context.Request.Path + ix);
                        return true;
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// Serves the request
        /// </summary>
        /// <returns>An awaitable task.</returns>
        /// <param name="path">The local path to a file to send.</param>
        /// <param name="mimetype">The mime type to report.</param>
        /// <param name="context">The request context.</param>
        protected virtual async Task<bool> ServeRequest(string path, string mimetype, IHttpContext context)
        {
            var permissionissue = false;

            try
            {
                string etag = null;
                string etagkey = ETagCacheSize < 0 ? null : (await m_vfs.GetLastFileWriteTimeUtcAsync(path)).Ticks + path;
                string[] clientetags = new string[0];

                if (etagkey != null)
                {
                    KeyValuePair<string, long> etagcacheddata;
                    using (await m_etagLock.LockAsync())
                        m_etagCache.TryGetValue(etagkey, out etagcacheddata);

                    etag = etagcacheddata.Key;

                    var ce = ETAG_RE.Matches(context.Request.Headers["If-None-Match"] ?? string.Empty);
                    if (ce.Count > 0)
                    {
                        clientetags = new string[ce.Count];
                        for (var i = 0; i < clientetags.Length; i++)
                        {
                            clientetags[i] = ce[i].Groups["etag"].Value;
                            if (etag != null && string.Equals(clientetags[i], etag, StringComparison.OrdinalIgnoreCase))
                                return SetNotModified(context, etag);
                        }
                    }
                }

                permissionissue = true;
                using (var fs = await m_vfs.OpenReadAsync(path))
                {
                    permissionissue = false;
                    var startoffset = 0L;
                    var bytecount = fs.Length;
                    var endoffset = bytecount - 1;

                    var rangerequest = context.Request.Headers["Range"];
                    if (!string.IsNullOrWhiteSpace(rangerequest))
                    {
                        var m = RANGE_MATCHER.Match(rangerequest);
                        if (!m.Success || m.Length != rangerequest.Length)
                            return SetInvalidRangeHeader(context, bytecount);

                        if (m.Groups["start"].Length != 0)
                            if (!long.TryParse(m.Groups["start"].Value, out startoffset))
                                return SetInvalidRangeHeader(context, bytecount);

                        if (m.Groups["end"].Length != 0)
                            if (!long.TryParse(m.Groups["end"].Value, out endoffset))
                                return SetInvalidRangeHeader(context, bytecount);

                        if (m.Groups["start"].Length == 0 && m.Groups["end"].Length == 0)
                            return SetInvalidRangeHeader(context, bytecount);

                        if (m.Groups["start"].Length == 0 && m.Groups["end"].Length != 0)
                        {
                            startoffset = bytecount - endoffset;
                            endoffset = bytecount - 1;
                        }

                        if (endoffset > bytecount - 1)
                            endoffset = bytecount - 1;

                        if (endoffset < startoffset)
                            return SetInvalidRangeHeader(context, bytecount);
                    }

                    if (etagkey != null && etag == null)
                    {
                        fs.Position = 0;
                        etag = await ComputeETag(fs);

                        if (ETagCacheSize > 0)
                        {
                            using (await m_etagLock.LockAsync())
                            {
                                m_etagCache[etagkey] = new KeyValuePair<string, long>(etag, DateTime.UtcNow.Ticks);

                                if (m_etagCache.Count > ETagCacheSize)
                                {
                                    // Don't repeatedly remove items,
                                    // but batch up the removal,
                                    // as the sorting takes some time
                                    var removecount = Math.Max(1, m_etagCache.Count / 3);

                                    foreach (var key in m_etagCache.OrderBy(x => x.Value.Value).Select(x => x.Key).Take(removecount).ToArray())
                                        m_etagCache.Remove(key);
                                }
                            }
                        }
                    }

                    if (etag != null && clientetags != null && clientetags.Any(x => string.Equals(x, etag, StringComparison.Ordinal)))
                        return SetNotModified(context, etag);

                    var lastmodified = await m_vfs.GetLastFileWriteTimeUtcAsync(path);
                    context.Response.ContentType = mimetype;
                    context.Response.StatusCode = HttpStatusCode.OK;
                    context.Response.AddHeader("Last-Modified", lastmodified.ToString("R", CultureInfo.InvariantCulture));
                    context.Response.AddHeader("Accept-Ranges", "bytes");
                    
                    // If the VFS or something else handles cache headers, do not overwrite them here
                    if (!context.Response.Headers.ContainsKey("Cache-Control") && !context.Response.Headers.ContainsKey("Expires"))
                        context.Response.SetExpires(CacheSeconds);

                    DateTime modifiedsincedate;
                    DateTime.TryParseExact(context.Request.Headers["If-Modified-Since"], CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out modifiedsincedate);

                    if (modifiedsincedate == lastmodified)
                    {
                        return SetNotModified(context, etag);
                    }
                    else
                    {
                        if (etag != null)
                            context.Response.Headers["ETag"] = $"\"{etag}\"";

                        context.Response.ContentLength = endoffset - startoffset + 1;
                        if (context.Response.ContentLength != bytecount)
                        {
                            context.Response.StatusCode = HttpStatusCode.PartialContent;
                            context.Response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", startoffset, endoffset, bytecount));
                        }
                    }

                    await BeforeResponseAsync(context, fs);

                    if (context.Response.StatusCode == HttpStatusCode.NotModified)
                        return true;

                    if (string.Equals(context.Request.Method, "HEAD", StringComparison.Ordinal))
                    {
                        if (context.Response.ContentLength != 0)
                        {
                            context.Response.KeepAlive = false;
                            await context.Response.FlushHeadersAsync();
                        }
                        return true;
                    }

                    fs.Position = startoffset;
                    var remain = context.Response.ContentLength;
                    var buf = new byte[TransferChunkSize];
                    
                    // Since this is a transfer, we do not honor the processing timeout here
                    //var ct = context.Request.TimeoutCancellationToken;

                    using (var os = context.Response.GetResponseStream())
                    {
                        while (remain > 0)
                        {
                            var r = await fs.ReadAsync(buf, 0, (int)Math.Min(buf.Length, remain));
                            using(var ct = new CancellationTokenSource(ActivityTimeoutSeconds))
                                await os.WriteAsync(buf, 0, r, ct.Token);
                            remain -= r;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                await context.LogMessageAsync(LogLevel.Error, $"Failed to process file: {path}", ex);

                // If this happens when we try to open the file, report as permission problem
                if (permissionissue)
                    throw new HttpException(HttpStatusCode.Forbidden);

                // Something else has happened
                throw new HttpException(HttpStatusCode.InternalServerError);
            }

            return true;
        }

        #region IHttpModule implementation
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <returns>The awaitable task.</returns>
        /// <param name="context">The http context.</param>
        public virtual async Task<bool> HandleAsync(IHttpContext context)
        {
            if (!string.Equals(context.Request.Method, "GET", StringComparison.Ordinal) && !string.Equals(context.Request.Method, "HEAD", StringComparison.Ordinal))
            {
                if (PassThrough)
                    return false;
                throw new HttpException(HttpStatusCode.MethodNotAllowed);
            }

            string path;
            try
            {
                path = GetLocalPath(context);
                if (string.IsNullOrWhiteSpace(path))
                    return false;
            }
            catch
            {
                if (PassThrough)
                    return false;

                throw;
            }

            if (await AutoRedirect(path, context))
                return true;

            if (!await m_vfs.FileExistsAsync(path))
            {
                if (PassThrough)
                    return false;

                throw new HttpException(HttpStatusCode.NotFound);
            }

            // If this is just a rewrite handler, stop now as we did not handle it
            if (RedirectOnly)
                return false;

            var mimetype = m_mimetypelookup(context.Request, path);
            if (mimetype == null)
            {
                if (PassThrough)
                    return false;

                throw new HttpException(HttpStatusCode.NotFound);
            }

            return await ServeRequest(path, mimetype, context);
        }
        #endregion

        /// <summary>
        /// Returns the default mime type for a request
        /// </summary>
        /// <returns>The mime type.</returns>
        /// <param name="request">The request.</param>
        /// <param name="mappedpath">The mapped filepath.</param>
        public static string DefaultMimeTypes(IHttpRequestInternal request, string mappedpath)
        {
            return DefaultMimeTypes(mappedpath);
        }

        /// <summary>
        /// Returns the default mime type for a path
        /// </summary>
        /// <returns>The mime type.</returns>
        /// <param name="mappedpath">The mapped file path.</param>
        public static string DefaultMimeTypes(string mappedpath)
        {
            var ext = Path.GetExtension(mappedpath).ToLowerInvariant();

            switch (ext)
            {
                case ".txt":
                    return "text/plain";
                case ".htm":
                case ".html":
                    return "text/html; charset=utf-8";
                case ".jpg":
                case ".jpeg":
                    return "image/jpg";
                case ".bmp":
                    return "image/bmp";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".ico":
                    return "image/vnd.microsoft.icon";
                case ".css":
                    return "text/css";
                case ".gz":
                case ".gzip":
                    return "application/x-gzip";
                case ".zip":
                    return "application/x-zip";
                case ".tar":
                    return "application/x-tar";
                case ".pdf":
                    return "application/pdf";
                case ".rtf":
                    return "application/rtf";
                case ".js":
                    return "application/javascript";
                case ".au":
                    return "audio/basic";
                case ".snd":
                    return "audio/basic";
                case ".es":
                    return "audio/echospeech";
                case ".mp3":
                    return "audio/mpeg";
                case ".mp2":
                    return "audio/mpeg";
                case ".mid":
                    return "audio/midi";
                case ".wav":
                    return "audio/x-wav";
                case ".avi":
                    return "video/avi";
                case ".htc":
                    return "text/x-component";
                case ".map":
                    return "application/json";
                case ".hbs":
                    return "application/x-handlebars-template";
                case ".woff":
                case ".woff2":
                    return "application/font-woff";
                case ".ttf":
                    return "application/font-ttf";
                case ".eot":
                    return "application/vnd.ms-fontobject";
                case ".otf":
                    return "application/font-otf";
                case ".svg":
                    return "application/svg+xml";
                case ".xml":
                    return "application/xml";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Handles post-configuration setup
        /// </summary>
        public virtual void AfterConfigure()
        {
            if (!string.IsNullOrWhiteSpace(EtagSalt))
                m_etagsalt = System.Text.Encoding.UTF8.GetBytes(EtagSalt);

            SourceFolder = Path.GetFullPath(SourceFolder);

            IndexDocuments = IndexDocuments ?? new string[0];
            AutoProbeExtensions =
                (AutoProbeExtensions ?? new string[0])
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.StartsWith(".", StringComparison.Ordinal) ? x : "." + x)
                    .ToArray();
        }
    }
}

