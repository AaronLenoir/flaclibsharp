@echo off
call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat" x86_amd64
msbuild "C:\Users\Public\Documents\Programming\github\flaclibsharp\FlacLibSharp\FlacLibSharp.sln" /p:Configuration=Debug /p:Platform="Any CPU"