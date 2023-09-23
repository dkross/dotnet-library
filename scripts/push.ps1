$solutionDir = "${PSScriptRoot}/.."
Push-Location "${solutionDir}"

foreach ($package in $(Get-ChildItem ./packages)) {
  Write-Output "===== Pushing ${package} ====="
  dotnet nuget push "${package}" --source github
}

Pop-Location
