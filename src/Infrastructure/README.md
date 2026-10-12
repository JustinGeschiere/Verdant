1. Open Package Manager Console (PMC)
2. Make sure the Default project (PMC) is set to "Infrastructure"
3. Make sure the Startup project (Solution) is set to "Api"

``` $Env:DOTNET_ENVIRONMENT="Console" ```
``` Add-Migration <migration-name-in-pascal-case> ```
``` Update-Database ```