@set xecho="%~dp0bin\xecho"
@%xecho% "Test without newline: "
@%xecho% -x -n [\e1;32mOK\e0m]
@%xecho% -x Test with embedded newline\n
@%xecho% -n now adding a newline
@%xecho% -x -n "with escaped \" quotes \" "
@%xecho% -x -n "Testing \eredmcolor\em-\eMagenta_names\e"
@%xecho% -x -n "Colors can be esaped using \\e(^<Name^>//;)[_ m] or the traditional ANSI codes"
