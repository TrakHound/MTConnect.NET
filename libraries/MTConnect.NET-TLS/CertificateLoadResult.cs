// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect.Tls
{
    public struct CertificateLoadResult
    {
        public bool Success { get; set; }

        public X509Certificate2 Certificate { get; set; }

        public Exception Exception { get; set; }


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
