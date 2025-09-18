# ERMS API Spec

# 執行應用程式並指定環境
dotnet run --environment Production

# 或透過 Windows 服務
$env = Production
appsettings($env).jsonWebSetting:UseKestrelWebHost = true // Use IIS set false
ERMS.Api.exe --environment Production