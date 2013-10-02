@pushd %~dp0

@pushd Win32ConEcho
@call build %*
@popd

@popd
