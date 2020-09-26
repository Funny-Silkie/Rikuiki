dotnet publish Rikuiki_Pitfall.csproj -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true
dotnet publish Rikuiki_Pitfall.csproj -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true
dotnet publish Rikuiki_Pitfall.csproj -c Release --self-contained true -r osx-x64 -p:PublishSingleFile=true