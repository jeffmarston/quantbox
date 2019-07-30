@setlocal enableextensions

sc delete "Eze Black Box Simulator"
sc create "Eze Black Box Simulator" binpath= "%~dp0server\Eze.Quantbox.exe"
sc start "Eze Black Box Simulator"

@set /p DUMMY=Hit ENTER to continue...