@setlocal enabledelayedexpansion
@pushd %~dp0

@set outdir=bin\Release
@if not exist "%outdir%" mkdir "%outdir%"
@echo Building xecho.exe
@echo using: cscu /out:"%outdir%\xecho.exe" *.cs
@call cscu /out:"%outdir%\xecho.exe" *.cs
@echo finished build of xecho.exe

@popd
@endlocal