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
if exist publish ( rmdir /s /q publish )
mkdir publish

echo Compiling / Target: %FRAMEWORK_VERSION% / Config: %TARGET_CONFIG%
msbuild /nologo /verbosity:quiet src\NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean
msbuild /nologo /verbosity:quiet src\NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /property:TargetFrameworkVersion=%FRAMEWORK_VERSION%

echo Testing
REM TODO

echo Merging
mkdir publish\bin
mkdir publish\plugins
mkdir publish\plugins\SubscriptionStorage
mkdir publish\plugins\Serialization

echo Marging Primary Assembly
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\NanoMessageBus*.dll"
(echo.|set /p =NanoMessageBus.*)>exclude.txt
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /internalize:"exclude.txt" /xmldocs /wildcards /targetplatform:%ILMERGE_VERSION% /out:publish/bin/NanoMessageBus.dll %FILES_TO_MERGE%

echo Rereferencing Merged Assembly
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /p:ILMerged=true /p:TargetFrameworkVersion=%FRAMEWORK_VERSION%

echo Merging SubscriptionStorage.Raven
mkdir publish\plugins\SubscriptionStorage\Raven\
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.SubscriptionStorage.Raven/bin/%TARGET_CONFIG%/NanoMessageBus.SubscriptionStorage.Raven.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.SubscriptionStorage.Raven.Wireup/bin/%TARGET_CONFIG%/NanoMessageBus.SubscriptionStorage.Raven.Wireup.dll"
echo NanoMessageBus.*>exclude.txt
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /wildcards /internalize:"exclude.txt" /targetplatform:%ILMERGE_VERSION% /out:publish/plugins/SubscriptionStorage/Raven/NanoMessageBus.SubscriptionStorage.Raven.dll %FILES_TO_MERGE%
del exclude.txt

echo Copying SubscriptionStorage.Raven
copy "src\proj\NanoMessageBus.SubscriptionStorage.Raven\bin\%TARGET_CONFIG%\AsyncCtpLibrary.dll" "publish\plugins\SubscriptionStorage\Raven\"
copy "src\proj\NanoMessageBus.SubscriptionStorage.Raven\bin\%TARGET_CONFIG%\MissingBitsFromClientProfile.dll" "publish\plugins\SubscriptionStorage\Raven\"
copy "src\proj\NanoMessageBus.SubscriptionStorage.Raven\bin\%TARGET_CONFIG%\Raven.Abstractions.dll" "publish\plugins\SubscriptionStorage\Raven\"
copy "src\proj\NanoMessageBus.SubscriptionStorage.Raven\bin\%TARGET_CONFIG%\Raven.Client.Lightweight.dll" "publish\plugins\SubscriptionStorage\Raven\"

echo Merging Serialization.Json
mkdir publish\plugins\Serialization\Json.Net\
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.Serialization.Json/bin/%TARGET_CONFIG%/NanoMessageBus.Serialization.Json.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.Serialization.Json.Wireup/bin/%TARGET_CONFIG%/NanoMessageBus.Serialization.Json.Wireup.dll"
echo NanoMessageBus.*>exclude.txt
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /wildcards /internalize:"exclude.txt" /targetplatform:%ILMERGE_VERSION% /out:publish/plugins/Serialization/Json.NET/NanoMessageBus.Serialization.Json.dll %FILES_TO_MERGE%
del exclude.txt

echo Merging Serialization.Protobuf
mkdir publish\plugins\Serialization\ProtocolBuffers.Net\
SET FILES_TO_MERGE=
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.Serialization.ProtocolBuffers/bin/%TARGET_CONFIG%/NanoMessageBus.Serialization.ProtocolBuffers.dll"
SET FILES_TO_MERGE=%FILES_TO_MERGE% "src/proj/NanoMessageBus.Serialization.ProtocolBuffers.Wireup/bin/%TARGET_CONFIG%/NanoMessageBus.Serialization.ProtocolBuffers.Wireup.dll"
echo NanoMessageBus.*>exclude.txt
bin\ILMerge\ILMerge.exe /keyfile:src/NanoMessageBus.snk /wildcards /internalize:"exclude.txt" /targetplatform:%ILMERGE_VERSION% /out:publish/plugins/Serialization/ProtocolBuffers.NET/NanoMessageBus.Serialization.ProtocolBuffers.dll %FILES_TO_MERGE%
del exclude.txt

echo Copying Dependencies
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\Autofac*.*" "publish\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\log4net.*" "publish\bin"
copy "src\proj\NanoMessageBus.Wireup.Autofac\bin\%TARGET_CONFIG%\NLog.*" "publish\bin"
copy "src\proj\NanoMessageBus.SubscriptionStorage.SqlStorage\bin\%TARGET_CONFIG%\*.sql" "publish\bin\"

echo Copying Docs
mkdir publish\doc
copy "doc\license.txt" "publish\doc\NanoMessageBus License.txt"
copy "lib\Autofac\Autofac License.txt" "publish\doc\Autofac License.txt"
copy "lib\Newtonsoft.Json\license.txt" "publish\doc\Json.NET License.txt"
copy "lib\protobuf-net\License.txt" "publish\doc\protobuf-net License.txt"
copy "lib\Log4net\log4net.license.txt" "publish\doc\log4net License.txt"
copy "lib\NLog\license.txt" "publish\doc\NLog License.txt"

echo Cleaning
msbuild /nologo /verbosity:quiet src/NanoMessageBus.sln /p:Configuration=%TARGET_CONFIG% /t:Clean

echo Done