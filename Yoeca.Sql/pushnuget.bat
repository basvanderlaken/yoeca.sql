@ECHO OFF
"../packages/NuGet.CommandLine.4.3.0/tools/nuget.exe" pack "Yoeca.Sql.nuspec"
FOR /F %%I IN ('DIR *.nupkg /B /O:D') DO SET NewestFile=%%I
"../packages/NuGet.CommandLine.4.3.0/tools/nuget.exe" push %NewestFile% -Source https://www.nuget.org/api/v2/package -ApiKey b782d453-c09d-454a-9e5b-8f0e610a9902
pause
REM nuget pack Yoeca.Sql.nuspec -Prop Configuration=Release
REM nuget push *.nupkg 'b782d453-c09d-454a-9e5b-8f0e610a9902' -s rferee.yoeca.nl:8888
