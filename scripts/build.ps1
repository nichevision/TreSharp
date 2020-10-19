$VSWhere = Get-Command vswhere.exe -ErrorAction Ignore
if (!$VSWhere)
{
    $VSWhere = "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"
    if (!(Test-Path $VSWhere))
    {
        throw "vswhere.exe command is required in order to find latest MSBuild. You must install Visual Studio 2017 or newer or add the command to your PATH."
    }
}

$MsBuildCommand = &$VSWhere -latest -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe"
if (!$MsBuildCommand) {
    throw "msbuild is required."
}

&$MsBuildCommand TreSharp.sln /p:Configuration=Release /t:Rebuild /p:Platform=x86 /verbosity:quiet
&$MsBuildCommand TreSharp.sln /p:Configuration=Release /t:Rebuild /p:Platform=x64 /verbosity:quiet

nuget pack ./nuget/TreSharp.nuspec /version $(git describe --tags)
