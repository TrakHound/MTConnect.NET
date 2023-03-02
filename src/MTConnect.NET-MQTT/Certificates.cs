// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using OpenSSL.X509Certificate2Provider;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace MTConnect
{
    public static class Certificates
    {
        public static X509Certificate2 FromPemFile(string cerificatePath, string privateKeyPath)
        {
            if (!string.IsNullOrEmpty(cerificatePath) && !string.IsNullOrEmpty(privateKeyPath))
            {
                try
                {
                    var certificateText = File.ReadAllText(cerificatePath);
                    var privateKeyText = File.ReadAllText(privateKeyPath);

                    var provider = new CertificateFromFileProvider(certificateText, privateKeyText);
                    return provider.Certificate;
                }
                catch { }
            }

            return null;
        }
    }
}
