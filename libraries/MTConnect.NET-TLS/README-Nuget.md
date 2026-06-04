![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/mtconnect-net-03-md.png) 

# MTConnect.NET-TLS
MTConnect.NET-TLS is an extension library to MTConnect.NET that provides TLS certificate loading and configuration for use with the HTTP and MQTT transport layers.

For TLS configuration within the HTTP server module or the MQTT relay module, see [docs/modules](../../docs/modules/).

## Overview
This library defines the `TlsConfiguration` class and its companion types (`PfxCertificateConfiguration`, `PemCertificateConfiguration`, `CertificateLoadResult`) that are consumed by the HTTP server module and the MQTT relay/broker modules to set up TLS-secured connections. Both PFX (PKCS#12) and PEM certificate formats are supported.

The `TlsConfiguration.GetCertificate()` method loads the certificate from the configured source and returns a `CertificateLoadResult` that carries either the loaded `X509Certificate2` or the exception that prevented loading.

## Configuration

### PFX Certificate
```yaml
tls:
  pfx:
    certificatePath: c:\certs\mtconnect-testing.pfx
    certificatePassword: mtconnect
```

### PEM Certificate (with Certificate Authority)
```yaml
tls:
  pem:
    certificateAuthority: c:\certs\rootCA.crt
    certificatePath: c:\certs\mtconnect-testing.crt
    privateKeyPath: c:\certs\mtconnect-testing.key
    privateKeyPassword: mtconnect
```

### Configuration Properties

* `pfx` - PFX (PKCS#12) certificate settings. Takes precedence over `pem` when both are provided.
    * `certificatePath` - Path to the `.pfx` file containing the certificate and its private key.
    * `certificatePassword` - Password protecting the PFX file (omit if the file is not password-protected).

* `pem` - PEM certificate settings, used when `pfx` is not configured.
    * `certificateAuthority` - Path to the `.pem` or `.crt` file containing the trusted CA chain.
    * `certificatePath` - Path to the `.pem` or `.crt` file containing the server or client certificate.
    * `privateKeyPath` - Path to the `.key` file containing the private key.
    * `privateKeyPassword` - Password protecting the private key (omit if the key is unencrypted).

* `verifyClientCertificate` - When `true`, the endpoint requires and validates a client certificate.

* `omitCAValidation` - When `true`, certificate-authority chain validation is skipped (useful for self-signed certificates in development).

## Generating a Self-Signed Certificate (PowerShell)
```powershell
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```

## Related Libraries
- [MTConnect.NET-HTTP](../MTConnect.NET-HTTP/) — HTTP client and server transport
- [MTConnect.NET-MQTT](../MTConnect.NET-MQTT/) — MQTT broker and client transport

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This library and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.