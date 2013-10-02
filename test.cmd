@setlocal enabledelayedexpansion

@echo Testing projects in %~dp0
@for %%D in (Win32ConEcho) do @(
    @echo Push project %%D
    @pushd %%D
    @call test %*
    @echo Leave project %%D
    @popd
)

@endlocal