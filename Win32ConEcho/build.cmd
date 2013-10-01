pushd %~dp0
mkdir bin
cscu /out:bin\xecho.exe *.cs
popd
