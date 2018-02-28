# .NET-ReportingServices-Extensions

To be able to build this solution you have to create a NuGet-package with [**NuGet Package Explorer**](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer). Do the following:
1. Create the directory **C:\Data\NuGet-packages**
2. Install **Microsoft SQL Server 2017 Reporting Services** on some machine and copy the following files to some local directory:
   - C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin\Microsoft.ReportingServices.Authorization.dll
   - C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin\Microsoft.ReportingServices.Diagnostics.dll
   - C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer\bin\Microsoft.ReportingServices.Interfaces.dll
3. Open ****NuGet Package Explorer** and create a new package
4. Copy the following to the metadata source:

        <?xml version="1.0" encoding="utf-8"?>
        <package xmlns="http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd">
            <metadata>
                <id>Microsoft.ReportingServices</id>
                <version>14.0.0</version>
                <authors>Microsoft</authors>
                <owners>Microsoft</owners>
                <projectUrl>https://msdn.microsoft.com/en-us/library/mt451936.aspx/</projectUrl>
                <requireLicenseAcceptance>false</requireLicenseAcceptance>
                <description>Assemblies for Microsoft Reporting-Services.</description>
                <tags>Microsoft.ReportingServices.Authorization, Microsoft.ReportingServices.Diagnostics, Microsoft.ReportingServices.Interfaces</tags>
                <dependencies>
                    <group targetFramework=".NETFramework4.5" />
                </dependencies>
            </metadata>
        </package>

5. Add a **lib** folder
6. In the **lib** folder add a **net45** folder
7. In the **net45** folder add the existing files:
   - **Microsoft.ReportingServices.Authorization.dll**
   - **Microsoft.ReportingServices.Diagnostics.dll**
   - **Microsoft.ReportingServices.Interfaces.dll**
8. Save the file as **C:\Data\NuGet-packages\Microsoft.ReportingServices.14.0.0.nupkg**