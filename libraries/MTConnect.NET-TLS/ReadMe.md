## Self Signed Certificate (PowerShell)
```
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```