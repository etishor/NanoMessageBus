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

echo Merging
mkdir output\bin

REM Contract/Interface
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus\bin\%TARGET_CONFIG%\NanoMessageBus.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Core\bin\%TARGET_CONFIG%\NanoMessageBus.Core.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Endpoints\bin\%TARGET_CONFIG%\NanoMessageBus.Endpoints.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Logging\bin\%TARGET_CONFIG%\NanoMessageBus.Logging.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Serialization\bin\%TARGET_CONFIG%\NanoMessageBus.Serialization.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.SubscriptionStorage\bin\%TARGET_CONFIG%\NanoMessageBus.SubscriptionStorage.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Transports\bin\%TARGET_CONFIG%\NanoMessageBus.Transports.dll"
REM Implementation
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.DefaultImplementation\bin\%TARGET_CONFIG%\NanoMessageBus.DefaultImplementation.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Core.DefaultImplementation\bin\%TARGET_CONFIG%\NanoMessageBus.Core.DefaultImplementation.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Endpoints.Msmq\bin\%TARGET_CONFIG%\NanoMessageBus.Endpoints.Msmq.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Logging.Console\bin\%TARGET_CONFIG%\NanoMessageBus.Logging.Console.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Serialization.DefaultImplementation\bin\%TARGET_CONFIG%\NanoMessageBus.Serialization.DefaultImplementation.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.SubscriptionStorage.SqlEngine\bin\%TARGET_CONFIG%\NanoMessageBus.SubscriptionStorage.SqlEngine.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Transports.DefaultImplementation\bin\%TARGET_CONFIG%\NanoMessageBus.Transports.DefaultImplementation.dll"
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /xmldocs /targetplatform:%ILMERGE_VERSION% /out:output/bin/NanoMessageBus.dll %FILES_TO_MERGE%

REM - Serialization -
mkdir output\bin\serialization

REM Json
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Serialization.Json\bin\%TARGET_CONFIG%\NanoMessageBus.Serialization.Json.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Serialization.Json\bin\%TARGET_CONFIG%\Newtonsoft.Json.dll"
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /internalize /xmldocs /targetplatform:%ILMERGE_VERSION% /out:output/bin/serialization/NanoMessageBus.Serialization.Json.dll %FILES_TO_MERGE%

REM - Logging -
mkdir output\bin\logging

REM log4net
copy "src\proj\NanoMessageBus.Logging.Log4Net\bin\%TARGET_CONFIG%\NanoMessageBus.Logging.Log4Net*.*" "output\bin\logging"
copy "src\proj\NanoMessageBus.Logging.Log4Net\bin\%TARGET_CONFIG%\log4Net.*" "output\bin\logging"

REM NLog
copy "src\proj\NanoMessageBus.Logging.NLog\bin\%TARGET_CONFIG%\NanoMessageBus.Logging.NLog*.*" "output\bin\logging"
copy "src\proj\NanoMessageBus.Logging.NLog\bin\%TARGET_CONFIG%\NLog.*" "output\bin\logging"

echo Copying
mkdir output\doc
copy "doc\license.txt" "output\doc\NanoMessageBus License.txt"
copy "lib\Log4net\log4net.license.txt" "output\doc\log4net License.txt"
copy "lib\Newtonsoft.Json\readme.txt" "output\doc\Json.NET License.txt"
copy "lib\NLog\license.txt" "output\doc\NLog License.txt"

echo Cleaning
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean

echo Done