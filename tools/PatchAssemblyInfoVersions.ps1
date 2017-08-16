[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$assemblyInfoFile,
    [Parameter(Mandatory=$True)]
    [string]$assemblyVersion,
    [Parameter(Mandatory=$True)]
    [string]$assemblyFileVersion,
    [Parameter(Mandatory=$True)]
    [string]$assemblyInformationalVersion
)

$oldVersion = "AssemblyVersion\(""[^""]+""\)"
$newVersion = "AssemblyVersion(""$assemblyVersion"")"
$oldFileVersion = "AssemblyFileVersion\(""[^""]+""\)"
$newFileVersion = "AssemblyFileVersion(""$assemblyFileVersion"")"
$oldInformationalVersion = "AssemblyInformationalVersion\(""[^""]+""\)"
$newInformationalVersion = "AssemblyInformationalVersion(""$assemblyInformationalVersion"")"

(Get-Content $assemblyInfoFile) | Foreach-Object {
    $_ -replace $oldVersion , $newVersion `
       -replace $oldFileVersion, $newFileVersion `
       -replace $oldInformationalVersion, $newInformationalVersion
} | Set-Content $assemblyInfoFile
