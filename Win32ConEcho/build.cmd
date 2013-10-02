pushd %~dp0
mkdir bin
cscu /out:bin\xecho.exe *.cs
pushd bin
ngen install xecho.exe
popd

popd
