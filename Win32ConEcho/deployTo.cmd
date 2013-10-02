@pushd %~dp0

@set outdir=bin\Release

@echo pushd outdir=%outdir%
@pushd %outdir%

@for %%F in (xecho.exe) do (
    xcopy /i /y %%F "%~1"
    @echo popping to deploy directory "%~1"
    @pushd "%~1"
    ngen install %%F
    @echo popd deploy directory "%~1"
    @popd
)
@echo popd %outdir%
@popd

@popd
