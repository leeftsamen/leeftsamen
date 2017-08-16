# Settings
#---------------
$serviceName = "LeeftSamenBackgroundService"
$service = Get-WmiObject -Class Win32_Service -Filter "Name = '$serviceName'"
$servicePath = (resolve-path .)


# Uninstall
#---------------
if ($service -ne $null)
{
Write-Host "Stop and delete service"
$service | stop-service
$service.Delete() | out-null
}
else
{
Write-Host "Service not installed"
}


# Installation
#---------------
$installUtil = join-path $env:SystemRoot Microsoft.NET\Framework\v4.0.30319\installutil.exe
$serviceExe = join-path $servicePath LeeftSamen.BackgroundService.exe
$installUtilLog = join-path $servicePath InstallUtil.log

& $installUtil $serviceExe /logfile="$installUtilLog" | write-verbose

$service = Get-WmiObject -Class Win32_Service -Filter "Name = '$serviceName'"

$service | set-service -startuptype Automatic -passthru
Write-Host "Successfully installed service $($service.name)"