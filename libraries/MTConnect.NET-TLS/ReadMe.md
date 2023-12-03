# MTConnect.NET-TLS
Classes to handle security cerificates for use with other MTConnect.NET libraries and applications.

## Download
<table>
    <tbody>
        <tr>
            <td>Nuget</td>
            <td><img src="https://img.shields.io/nuget/dt/MTConnect.NET-TLS?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/MTConnect.NET-TLS">https://www.nuget.org/packages/MTConnect.NET-TLS</a></td>
        </tr>
    </tbody>
</table>

## Self Signed Certificate (PowerShell)
```
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```