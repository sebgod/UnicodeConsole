@setlocal enabledelayedexpansion

@echo Deploying projects in %~dp0
@for %%D in (Win32ConEcho) do @(
    @echo Push project %%D
    @pushd %%D
    @call deployTo %*
    @echo Leave project %%D
    @popd
)

@endlocal