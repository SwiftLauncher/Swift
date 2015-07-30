# Settings
$project = "Swift"
$revisionNumberPrefix = ""

# Set version number
$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\$project\bin\Release\$project.exe").GetName().Version
$versionStr = "{0}.{1}.{2}$revisionNumberPrefix{3}" -f ($version.Major,$version.Minor,$version.Build,$version.Revision)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\Deployment\$project.nuspec) 
$content = $content -replace '\$version\$',$versionStr

# Create NuGet package
$content | Out-File $root\Deployment\$project.compiled.nuspec

& $root\Deployment\NuGet.exe pack $root\Deployment\$project.compiled.nuspec
Remove-Item $root\Deployment\$project.compiled.nuspec

# Releasify
& $root\Deployment\Squirrel.exe --releasify $root\Deployment\Swift.$versionstr.nupkg