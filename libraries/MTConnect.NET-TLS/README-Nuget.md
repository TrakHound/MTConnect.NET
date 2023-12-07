![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect.NET-TLS
Classes to handle security cerificates for use with other MTConnect.NET libraries and applications.

## Self Signed Certificate (PowerShell)
```
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```