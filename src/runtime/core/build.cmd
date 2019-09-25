@echo off
pushd %~dp0
dotnet ..\..\compiler\starkc\Bin\Debug\netcoreapp2.1\starkc.dll -nostdlib -target:library -unsafe -langversion:8.0 -recurse:*.sk -runtimemetadataversion:v1.0.0 -out:core.sklib
popd