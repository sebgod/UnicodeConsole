@setlocal enabledelayedexpansion

@echo Building projects in %~dp0
@for %%D in (Win32ConEcho) do @(
    @echo Push project %%D
    @pushd %%D
    @call build %*
    @echo Leave project %%D
    @popd
)

@endlocal