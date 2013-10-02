@pushd %~dp0

@set outdir=bin\Release
@pushd bin\Release
xcopy /i /y *.exe "%~1"
@popd

@popd
