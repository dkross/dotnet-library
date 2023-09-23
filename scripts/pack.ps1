$projectsToPack = @(
  "Core/DKrOSS.Core.csproj"
)

$solutionDir = "${PSScriptRoot}/.."
Push-Location "${solutionDir}"

if(Test-Path ./packages)
{
    Remove-Item -Force -Recurse ./packages
}

foreach ($project in $projectsToPack) {
  Write-Output "===== Packing ${project} ====="
  dotnet pack "./src/${project}" -c Release -o ./packages
}

Pop-Location
