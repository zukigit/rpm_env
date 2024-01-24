@echo off

if "%3" == "" (
    set ERRMSG=Usage: %0 file_name file_delete_flag file_wait_time
    goto FAILURE
)

set filename=%~1
set file_path=%~dp1
set file_delete_flag=%~2
set file_wait_time=%~3

if not exist "%file_path%" (
    set ERRMSG=The path '%file_path%' does not exist
    goto FAILURE
)

if not exist "%filename%" (
    echo The file '%filename%' does not exist
    exit 0
)

set att=%~a1
if %att:~0,1%==d (
    set ERRMSG=The file '%file_name%' is not a regular file
    goto FAILURE
)

echo the file '%filename%' exists
if %file_delete_flag% equ 1 (
    echo Delete the file '%filename%'
    set ERRMSG=Can not delete the file '%filename%'
    del "%filename%"| find /V "" && goto FAILURE
)
exit 1

:FAILURE
echo %ERRMSG% 1>&2
exit 255
