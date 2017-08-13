call sc create "App Domain Learning Project" binPath="C:\Users\josep\Documents\Code\.net-plugin-demo\Host.App\bin\Debug\Host.App.exe" DisplayName= "App Domain Learning Project"
call sc description "App Domain Learning Project" "This service is a project to learn how app domains work in .NET."
call sc config "App Domain Learning Project" obj= "LocalSystem"

REM call eventcreate /ID 25 /L APPLICATION /SO Plugin-Demo /T INFORMATION /D ".NET Plugin Demo Application"
