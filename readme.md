# Build project
dotnet publish -f:net9.0-android -c Release -o ./publish /p:AndroidPackageFormat=apk

# Migration
dotnet ef migrations add AddChangeSet --project SimpleEventAccountingMobile --framework net9.0-windows10.0.19041.0