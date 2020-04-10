Write-Output "---Creating Artifacts---"

# Build web application
Set-Location .\KenticoInspector.Web
dotnet publish KenticoInspector.Web.csproj /p:PublishDir=..\publish -c Release -r win-x64 --self-contained true

# Copy compiled front-end to publish folder
Set-Location .\Client
mkdir "..\..\publish\Client\dist"
Copy-Item ".\dist\*" -Recurse -Destination "..\..\publish\Client\dist\"

# Go back to root
Set-Location ..\..\

# Create archive & push to Appveyor
7z a "Kentico Inspector $Env:SEMVER_VERSION.zip" .\publish\*
appveyor PushArtifact "Kentico Inspector $Env:SEMVER_VERSION.zip"