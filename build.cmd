@ECHO OFF
SET FRAMEWORK_PATH=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%FRAMEWORK_PATH%;

:target_config
SET TARGET_CONFIG=Release
IF x==%1x GOTO framework_version
SET TARGET_CONFIG=%1

:framework_version
SET FRAMEWORK_VERSION=v4.0
SET ILMERGE_VERSION=v4,%FRAMEWORK_PATH%
IF x==%2x GOTO build
SET FRAMEWORK_VERSION=%2
SET ILMERGE_VERSION=%3

:build
if exist output ( rmdir /s /q output )
mkdir output

echo Compiling / Target: %FRAMEWORK_VERSION% / Config: %TARGET_CONFIG%
msbuild /nologo /verbosity:quiet src\NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean
msbuild /nologo /verbosity:quiet src\NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /property:TargetFrameworkVersion=%FRAMEWORK_VERSION%

echo Testing
REM TODO

echo Merging
mkdir output\bin

echo Marging Primary Assembly
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\NanoMessageBus*.dll"
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /xmldocs /wildcards /targetplatform:%ILMERGE_VERSION% /out:output/bin/NanoMessageBus.dll %FILES_TO_MERGE%

echo Rereferencing Merged Assembly
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /p:ILMerged=true /p:TargetFrameworkVersion=%FRAMEWORK_VERSION%

echo Merging SubscriptionStorage.Raven
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.SubscriptionStorage.Raven/bin/%TARGET_CONFIG%/NanoMessageBus.*.dll"
echo NanoMessageBus.*>exclude.txt
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /wildcards /internalize:"exclude.txt" /targetplatform:%ILMERGE_VERSION% /out:output/bin/NanoMessageBus.SubscriptionStorage.Raven.dll %FILES_TO_MERGE%
del exclude.txt

echo Copying
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\Autofac*.*" "output\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\Newtonsoft.Json*.*" "output\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\protobuf-net.*" "output\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\log4net.*" "output\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\NLog.*" "output\bin"
copy "src\proj\NanoMessageBus.SubscriptionStorage.SqlStorage\bin\%TARGET_CONFIG%\*.sql" "output\bin\"

mkdir output\doc
copy "doc\license.txt" "output\doc\NanoMessageBus License.txt"
copy "lib\Autofac\Autofac License.txt" "output\doc\Autofac License.txt"
copy "lib\Newtonsoft.Json\readme.txt" "output\doc\Json.NET License.txt"
copy "lib\protobuf-net\License.txt" "output\doc\protobuf-net License.txt"
copy "lib\Log4net\log4net.license.txt" "output\doc\log4net License.txt"
copy "lib\NLog\license.txt" "output\doc\NLog License.txt"

echo Cleaning
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean

echo Done