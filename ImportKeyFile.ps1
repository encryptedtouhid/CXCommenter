Param(
    [string]$keyFilePath,
    [string]$keyFilePassword,
    [string]$storeLocation = "CurrentUser",
    [string]$storeName = "My"
)

$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList @($keyFilePath, $keyFilePassword, [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::PersistKeySet)

$certificateStore = New-Object System.Security.Cryptography.X509Certificates.X509Store -ArgumentList $storeName, $storeLocation
$certificateStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
$certificateStore.Add($cert)
$certificateStore.Close()
