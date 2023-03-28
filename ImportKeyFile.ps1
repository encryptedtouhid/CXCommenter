Param(
    [string]$keyFilePath,
    [string]$keyContainerName,
    [string]$keyFilePassword
)

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$cert.Import($keyFilePath, $keyFilePassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::PersistKeySet)

$cspParams = New-Object System.Security.Cryptography.CspParameters
$cspParams.KeyContainerName = $keyContainerName
$cspParams.Flags = [System.Security.Cryptography.CspProviderFlags]::UseMachineKeyStore

$rsaProvider = New-Object System.Security.Cryptography.RSACryptoServiceProvider
$rsaProvider.ImportParameters($cert.PrivateKey.ExportParameters($true))
$rsaProvider.PersistKeyInCsp = $true

$null = $rsaProvider.ToXmlString($true)
