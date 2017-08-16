[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$msDeployPath,
    [Parameter(Mandatory=$True)]
    [string]$package,
    [Parameter(Mandatory=$True)]
    [string]$computerName,
    [Parameter(Mandatory=$True)]
    [string]$userName,
    [Parameter(Mandatory=$True)]
    [string]$password,
    [Parameter(Mandatory=$True)]
    [string[]]$parameters
)

$command = "$msDeployPath\msdeploy.exe"

$arguments = @(
    "-verb:sync",
    "-source:package='$package'",
    "-dest:auto,computerName='$computerName',userName='$userName',password='$password',includeAcls=False",
    "-disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension"
)
foreach ($parameter in $parameters)
{
    $arguments += "-setParam:$parameter"
}

cmd.exe /c "`"$command`" $arguments"
