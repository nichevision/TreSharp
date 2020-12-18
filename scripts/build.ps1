Get-Command dotnet -ErrorAction Stop

dotnet build .\TreSharp\TreSharp.csproj -c Release /p:Platform=x86
dotnet build .\TreSharp\TreSharp.csproj -c Release /p:Platform=x64
dotnet build .\TreSharp\TreSharp.csproj -c Release /p:Platform=AnyCPU

nuget pack ./nuget/TreSharp.nuspec -Version $(git describe --tags)
