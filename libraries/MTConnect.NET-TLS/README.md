![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/master/img/logo.png) 

# MTConnect.NET-TLS
MTConnect.NET-TLS is an extension library to MTConnect.NET that provides TLS certificate loading and configuration for use with the HTTP and MQTT transport layers.

For TLS configuration within the HTTP server module or the MQTT relay module, see [Modules](https://www.trakhound.com/docs/mtconnect-net/modules).

## Nuget
<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Package Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>MTConnect.NET-TLS</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-TLS?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-TLS">https://www.nuget.org/packages/MTConnect.NET-TLS</a></td>
        </tr>
    </tbody>
</table>

## Overview
This library defines the `TlsConfiguration` class and its companion types (`PfxCertificateConfiguration`, `PemCertificateConfiguration`, `CertificateLoadResult`) that are consumed by the HTTP server (`MTConnectHttpServer`), the MQTT client (`MTConnectMqttClient`), and the MQTT relay/broker Agent modules to set up TLS-secured connections. Both PFX (PKCS#12) and PEM certificate formats are supported.

> Loading a PEM certificate requires .NET 5 or later. On .NET Framework and .NET Standard 2.0, a `pem` configuration does not load a certificate (an error result is returned), so use `pfx` on those frameworks.

The `TlsConfiguration.GetCertificate()` method loads the certificate from the configured source and returns a `CertificateLoadResult`:
- When the certificate was loaded, `Success` is `true` and `Certificate` contains the loaded `X509Certificate2`.
- When loading failed, `Success` is `false` and `Exception` contains the exception that prevented loading.
- When no certificate is configured (neither `pfx` nor `pem` is set, or the `certificatePath` is empty), `Success` is `false` and both `Certificate` and `Exception` are null.

The `TlsConfiguration.GetCertificateAuthority()` method loads the certificate set in `pem.certificateAuthority` and returns a `CertificateLoadResult` in the same way.

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
    * `certificateAuthority` - Path to the `.pem` or `.crt` file containing the trusted CA certificate. Only read by the MQTT client and the MQTT relay module.
    * `certificatePath` - Path to the `.pem` or `.crt` file containing the server or client certificate.
    * `privateKeyPath` - Path to the `.key` file containing the private key.
    * `privateKeyPassword` - Password protecting the private key (omit if the key is unencrypted).

* `verifyClientCertificate` - Only read by the MQTT client (`MTConnectMqttClient`) and the MQTT relay module. When `true`, an untrusted certificate presented by the MQTT broker is rejected. When `false` (the default), untrusted certificates are allowed. It is not read by the HTTP server or the MQTT broker module, so it does not make a server require a client certificate.

* `omitCAValidation` - Only read by the MQTT client (`MTConnectMqttClient`) and the MQTT relay module. When `true`, the `certificateAuthority` certificate is not added to the list of client certificates that is sent to the MQTT broker. On .NET 5 or later, the certificate presented by the broker is still validated against the `certificateAuthority` certificate when one is configured.

## Generating a Self-Signed Certificate (PowerShell)
```powershell
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```

## Related Libraries
- [MTConnect.NET-HTTP](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-HTTP) — HTTP client and server transport
- [MTConnect.NET-MQTT](https://github.com/TrakHound/MTConnect.NET/tree/master/libraries/MTConnect.NET-MQTT) — MQTT client transport

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/MTConnect.NET/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/MTConnect.NET/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This library and its source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.