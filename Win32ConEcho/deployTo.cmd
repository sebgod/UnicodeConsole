@echo Deploying xecho.exe targets
@if "%~1" == "" @(
    @echo You must specify the deploy directory >&2
    @exit /b 1
)

@pushd %~dp0

@set outdir=bin\Release

@echo pushd outdir=%outdir%
@pushd %outdir%

@for %%F in (xecho.exe) do @(
    @echo xcopy /i /y %%F "%~1"
    xcopy /i /y %%F "%~1"
    @echo popping to deploy directory "%~1"
    @pushd "%~1"
    call elevate ngen install %%F
    @echo popd deploy directory "%~1"
    @popd
)
@echo popd %outdir%
@popd

@popd
