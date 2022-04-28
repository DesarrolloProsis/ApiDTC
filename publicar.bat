@echo off

cd /d %~dp0

set /p pagina=Nombre de la pagina:

setlocal EnableDelayedExpansion 

set "condition="

if exist C:\inetpub\wwwroot\%pagina%\ (

    %windir%\system32\inetsrv\appcmd stop site /site.name:%pagina%
    set "condition=y"
    goto Fin
    
) else (
    echo El Nombre de la pagina no existe

    CHOICE /C SN /M "Presiona S para continuar y N para cancelar"
    if errorlevel 2 goto cancelar
    goto makedire
)

:makedire

mkdir C:\inetpub\wwwroot\%pagina%\
echo directorio %pagina% creado
goto Fin


:cancelar
echo Cancelado
pause
goto :EOF

:Fin

copy %cd%\ApiDTC\bin\Debug\netcoreapp2.1\publish\* C:\inetpub\wwwroot\%pagina%\

FOR /d %%i IN (%cd%\ApiDTC\bin\Debug\netcoreapp2.1\publish\*) DO move "%%i" C:\inetpub\wwwroot\%pagina%\%%~ni

echo Publicado con exito

if defined condition (

    %windir%\system32\inetsrv\appcmd start site /site.name:%pagina%
    
)

pause
goto :EOF