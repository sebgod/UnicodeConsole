@echo Testing deployed xecho
@if "%~1" == "" @(
    @echo You must specify the deploy directory >&2
    @exit /b 1
)
@set xecho="%~1\xecho"
@%xecho% "Test without newline: "
@%xecho% -x -n [\e1;32mOK\e0m]
@%xecho% -x Test with embedded newline\n
@%xecho% -n now adding a newline
@%xecho% -x -n "with escaped \" quotes \" "
@%xecho% -x -n "Testing \eredmcolor\em-\eMagenta_names\e"
@%xecho% -x -n "Colors can be esaped using \\e(^<Name^>//;)[_ m] or the traditional ANSI codes"
@%xecho% -x -n "Test beep: \a"
@%xecho% -x -n "Test \e3minverse colours\e_"