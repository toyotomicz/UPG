@mkdir .\bin
dotnet msbuild ./UPG_semestralka.sln -t:Rebuild -p:Configuration=Release
xcopy .\UPG_semestralka\bin\Release\net8.0-windows\*.* .\bin /S
