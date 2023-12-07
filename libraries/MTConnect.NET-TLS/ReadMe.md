![MTConnect.NET Logo](https://raw.githubusercontent.com/TrakHound/MTConnect.NET/dev/img/mtconnect-net-03-md.png) 

# MTConnect.NET-TLS
Classes to handle security cerificates for use with other MTConnect.NET libraries and applications.

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

## Self Signed Certificate (PowerShell)
```
New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "cert:\LocalMachine\My" -FriendlyName "MTConnect-Test" -Type "SSLServerAuthentication" -NotAfter (Get-Date).AddYears(1)
```