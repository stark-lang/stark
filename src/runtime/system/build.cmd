@echo off
cd %~dp0
dotnet ..\..\..\ext\stark-roslyn\artifacts\bin\starkc\Debug\netcoreapp2.1\starkc.dll -nostdlib -target:library -unsafe -langversion:8.0 -recurse:*.sk -runtimemetadataversion:v1.0.0 -out:system.sklib