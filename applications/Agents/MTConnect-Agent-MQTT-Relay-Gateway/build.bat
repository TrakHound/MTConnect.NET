dotnet build -c:Release -r:win-x86 --no-self-contained
dotnet build -c:Release -r:win-x64 --no-self-contained
"c:\Program Files (x86)\Inno Setup 6\iscc" installer.iss
