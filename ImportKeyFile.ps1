Param(
    [string]$keyFilePath,
    [string]$keyContainerName,
    [string]$keyFilePassword
)

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList @($keyFilePath, $keyFilePassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::PersistKeySet)

$cspParams = New-Object System.Security.Cryptography.CspParameters
$cspParams.KeyContainerName = $keyContainerName
$cspParams.Flags = [System.Security.Cryptography.CspProviderFlags]::UseMachineKeyStore

$rsaProvider = [System.Security.Cryptography.RSACryptoServiceProvider]::new($cspParams)

$privateKeyXml = $cert.PrivateKey.ToXmlString($true)
$rsaProvider.FromXmlString($privateKeyXml)
$rsaProvider.PersistKeyInCsp = $true

$null = $rsaProvider.ToXmlString($true)
