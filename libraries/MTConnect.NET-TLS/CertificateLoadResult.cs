// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect.Tls
{
    /// <summary>
    /// The outcome of attempting to load a TLS certificate, carrying either the loaded certificate or the failure.
    /// </summary>
    public struct CertificateLoadResult
    {
        /// <summary>
        /// Indicates whether the certificate was loaded successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The loaded certificate when <see cref="Success"/> is true; otherwise null.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// The exception that caused the load to fail when <see cref="Success"/> is false; otherwise null.
        /// </summary>
        public Exception Exception { get; set; }


        /// <summary>
        /// Creates a successful result wrapping the loaded certificate.
        /// </summary>
        /// <param name="certificate">The certificate that was loaded.</param>
        /// <returns>A successful result, or an unsuccessful result when <paramref name="certificate"/> is null.</returns>
        public static CertificateLoadResult Ok(X509Certificate2 certificate)
        {
            var result = new CertificateLoadResult();

            if (certificate != null)
            {
                result.Success = true;
                result.Certificate = certificate;
            }

            return result;
        }

        /// <summary>
        /// Creates an unsuccessful result wrapping the exception that caused the failure.
        /// </summary>
        /// <param name="exception">The exception describing the load failure.</param>
        /// <returns>An unsuccessful result.</returns>
        public static CertificateLoadResult Error(Exception exception)
        {
            var result = new CertificateLoadResult();
            result.Success = false;

            if (exception != null)
            {
                result.Exception = exception;
            }

            return result;
        }
    }
}
