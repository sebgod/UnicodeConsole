@for %%D in (Win32ConEcho) do (
    @echo pushd %~dp0%%D
    @pushd %%D
    @call test %*
    @echo popd %~dp0%%D
    @popd
)