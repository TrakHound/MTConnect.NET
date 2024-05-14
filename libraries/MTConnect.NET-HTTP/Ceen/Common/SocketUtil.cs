using System.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace Ceen
{
    /// <summary>
    /// Helper class for socket operations
    /// </summary>
    internal static class SocketUtil
    {
        /// <summary>
        /// Creates a socket and binds it to the given address
        /// </summary>
        /// <param name="addr">The address to bind to</param>
        /// <param name="backlog">The backlog setting</param>
        /// <returns>A bound socket</returns>
        public static Socket CreateAndBindSocket(EndPoint addr, int backlog)
        {
            Socket socket;
			if (addr is IPEndPoint)
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

#if NET5_0_OR_GREATER
			else if (addr is UnixDomainSocketEndPoint)
				socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
#endif

            else
                throw new Exception($"Cannot bind to an endpoint of type: {addr.GetType()}");

            BindSocket(socket, addr, backlog);
            return socket;
        }

        /// <summary>
        /// Binds a socket to an endpoint
        /// </summary>
        /// <param name="socket">The socket to bind</param>
        /// <param name="addr">The endpoint to bind to</param>
        /// <param name="backlog">The backlog number to set</param>
        public static void BindSocket(Socket socket, EndPoint addr, int backlog)
        {
			try 
            {
                socket.Bind(addr);
            }
            catch
            {
#if NET5_0_OR_GREATER
                if (addr is UnixDomainSocketEndPoint udep)
                {
                    var path = udep.ToString();
                    var fi = new FileInfo(path);
                    if (fi.Exists && fi.Length <= 0)
                    {
                        var failed = false;
                        try
                        {
                            using(var ts = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP))
                            {
                                var t = ts.ConnectAsync(addr);
                                t.Wait(TimeSpan.FromSeconds(1));                                
                            }                            
                        }
                        catch
                        {
                            failed = true;
                        }

                        // If something is using the address, stop now
                        if (!failed)
                            throw;

                        // Try to unlink the file
                        try { File.Delete(path); }
                        catch {}

                        // And retry the bind
                        socket.Bind(addr);
                    }
                    else
                    {
                        // Only fix if the file exists
                        throw;
                    }
                }
                else
                {
                    // Only fix unix domain sockets
                    throw;
                }
#endif
            }

            socket.Listen(backlog);
        }
    }
}