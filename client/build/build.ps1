# 1. Nettoyer les builds précédents
Write-Host "Cleaning previous builds..."
dotnet clean

# 2. Construire le projet en mode Release
Write-Host "Building the project in Release mode..."
dotnet build -c Release

# 3. Publier le projet pour Windows (64 bits)
Write-Host "Publishing the project for Windows..."
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./Publish

Write-Host "Build and publish completed!"
