@echo pushd %~dp0
@pushd %~dp0

@set outdir=bin\Release
@if not exist "%outdir%" mkdir "%outdir%"
cscu /out:"%outdir%\xecho.exe" *.cs

@echo popd %~dp0
@popd
