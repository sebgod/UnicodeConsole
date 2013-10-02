@pushd %~dp0

@set outdir=bin\Release
@if not exist "%outdir%" mkdir "%outdir%"
cscu /out:"%outdir%\xecho.exe" *.cs
@pushd %outdir%
ngen install xecho.exe
:: pop %outdir%
@popd

:: pop %~dp0
@popd
