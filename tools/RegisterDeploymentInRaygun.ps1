[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string] $raygunAuthToken,
    [Parameter(Mandatory=$True)]
    [string] $raygunApiKey,
    [Parameter(Mandatory=$True)]
    [string] $version,
    [Parameter()]
    [string] $ownerName,
    [Parameter()]
    [string] $emailAddress,
    [Parameter()]
    [string] $scmIdentifier,
    [Parameter()]
    [string] $releaseNotes
)

Write-Debug "Adding deployment in Raygun"
$url = "https://app.raygun.io/deployments?authToken=" + $raygunAuthToken

$command = ConvertTo-Json @{
    apiKey = $raygunApiKey
    version = $version
    ownerName = $ownerName
    emailAddress = $emailAddress
    comment = $releaseNotes
    scmIdentifier = $scmIdentifier
}

$bytes = [System.Text.Encoding]::ASCII.GetBytes($command)
$web = [System.Net.WebRequest]::Create($url)
$web.Method = "POST"
$web.ContentLength = $bytes.Length
$web.ContentType = "application/json"
$stream = $web.GetRequestStream()
$stream.Write($bytes,0,$bytes.Length)
$stream.close()

$response = [System.Net.HttpWebResponse]$web.GetResponse()
if($response.StatusCode -eq [System.Net.HttpStatusCode]::OK) {
    Write-Debug "Added deployment in Raygun"
} else {
    Write-Host "Error received when adding deployment in Raygun: " $response.StatusCode " - " $response.StatusDescription
}
