# JWT Key Generator and Validator - PowerShell
# This script generates and validates JWT keys for proper HS256 algorithm compliance

param(
    [switch]$Generate = $false,
    [switch]$Validate = $false,
    [string]$TestKey = ""
)

# Colors for PowerShell
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"
$Blue = "Cyan"

function Write-ColorMessage {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Test-JWTKey {
    param([string]$Key)
    
    Write-ColorMessage "?? Validating JWT Key..." $Yellow
    Write-Host ""
    
    if ([string]::IsNullOrEmpty($Key)) {
        Write-ColorMessage "? No key provided for validation" $Red
        return $false
    }
    
    $keyLength = $Key.Length
    $keyBits = $keyLength * 8
    
    Write-ColorMessage "Key Analysis:" $Blue
    Write-Host "  Length: $keyLength characters"
    Write-Host "  Bits: $keyBits bits"
    Write-Host "  Sample: $($Key.Substring(0, [Math]::Min(20, $keyLength)))..."
    Write-Host ""
    
    if ($keyBits -lt 256) {
        Write-ColorMessage "? INVALID: Key too short!" $Red
        Write-Host "  Minimum required: 32 characters (256 bits)"
        Write-Host "  Current: $keyLength characters ($keyBits bits)"
        Write-Host "  Missing: $([Math]::Max(0, 32 - $keyLength)) characters"
        return $false
    } elseif ($keyBits -eq 256) {
        Write-ColorMessage "? VALID: Key meets minimum requirements" $Green
        return $true
    } else {
        Write-ColorMessage "? EXCELLENT: Key exceeds minimum requirements" $Green
        return $true
    }
}

function Generate-SecureJWTKey {
    Write-ColorMessage "?? Generating secure JWT keys..." $Yellow
    Write-Host ""
    
    # Method 1: Using .NET RNG (most compatible)
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    $keyBytes = New-Object byte[] 32  # 32 bytes = 256 bits
    $rng.GetBytes($keyBytes)
    $base64Key = [System.Convert]::ToBase64String($keyBytes)
    
    # Method 2: Using random alphanumeric (easier to manage)
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"
    $alphaKey = -join ((1..48) | ForEach-Object { $chars[(Get-Random -Maximum $chars.Length)] })
    
    Write-ColorMessage "Generated JWT Keys:" $Green
    Write-Host ""
    Write-ColorMessage "Option 1 - Base64 Encoded (Recommended):" $Blue
    Write-Host "JWT_SECRET_KEY=$base64Key"
    Write-Host ""
    Write-ColorMessage "Option 2 - Alphanumeric:" $Blue  
    Write-Host "JWT_SECRET_KEY=$alphaKey"
    Write-Host ""
    
    # Validate both keys
    Write-ColorMessage "Validation Results:" $Yellow
    Write-Host "Base64 Key:" -NoNewline
    if (Test-JWTKey -Key $base64Key) {
        Write-Host " ? Valid" -ForegroundColor Green
    } else {
        Write-Host " ? Invalid" -ForegroundColor Red
    }
    
    Write-Host "Alpha Key:" -NoNewline
    if (Test-JWTKey -Key $alphaKey) {
        Write-Host " ? Valid" -ForegroundColor Green
    } else {
        Write-Host " ? Invalid" -ForegroundColor Red
    }
    
    return @{
        Base64 = $base64Key
        Alphanumeric = $alphaKey
    }
}

function Check-CurrentConfiguration {
    Write-ColorMessage "?? Checking current JWT configuration..." $Yellow
    Write-Host ""
    
    # Check environment variable
    $envKey = [Environment]::GetEnvironmentVariable("JWT_SECRET_KEY")
    if ($envKey) {
        Write-ColorMessage "Environment Variable JWT_SECRET_KEY:" $Blue
        Test-JWTKey -Key $envKey | Out-Null
    } else {
        Write-ColorMessage "?? JWT_SECRET_KEY environment variable not set" $Yellow
    }
    
    Write-Host ""
    
    # Check appsettings files
    $settingsFiles = @(
        "Api-Gandarias\appsettings.json",
        "Api-Gandarias\appsettings.Development.json",
        "Api-Gandarias\appsettings.Production.json"
    )
    
    foreach ($file in $settingsFiles) {
        if (Test-Path $file) {
            Write-ColorMessage "Checking $file..." $Blue
            try {
                $content = Get-Content $file -Raw | ConvertFrom-Json
                $jwtKey = $content.jwtKey
                
                if ($jwtKey -and !$jwtKey.StartsWith('${')) {
                    Write-Host "  JWT Key found:" -NoNewline
                    if (Test-JWTKey -Key $jwtKey) {
                        Write-Host " ? Valid" -ForegroundColor Green
                    } else {
                        Write-Host " ? Invalid" -ForegroundColor Red
                    }
                } elseif ($jwtKey -and $jwtKey.StartsWith('${')) {
                    Write-Host "  Template variable: $jwtKey" -ForegroundColor Yellow
                } else {
                    Write-Host "  No JWT key found" -ForegroundColor Yellow
                }
            } catch {
                Write-Host "  Error reading file: $($_.Exception.Message)" -ForegroundColor Red
            }
        } else {
            Write-ColorMessage "  File not found: $file" $Yellow
        }
        Write-Host ""
    }
}

function Show-DeploymentInstructions {
    param([hashtable]$Keys)
    
    Write-ColorMessage "?? DEPLOYMENT INSTRUCTIONS" $Green
    Write-Host ""
    Write-ColorMessage "For Development:" $Blue
    Write-Host "1. Update appsettings.Development.json:"
    Write-Host "   `"jwtKey`": `"$($Keys.Base64)`""
    Write-Host ""
    Write-ColorMessage "For Production (App Runner):" $Blue
    Write-Host "1. Set environment variable:"
    Write-Host "   JWT_SECRET_KEY=$($Keys.Base64)"
    Write-Host ""
    Write-Host "2. Verify appsettings.Production.json has:"
    Write-Host "   `"jwtKey`": `"`${JWT_SECRET_KEY}`""
    Write-Host ""
    Write-ColorMessage "Security Notes:" $Yellow
    Write-Host "• Never commit JWT keys to source control"
    Write-Host "• Use different keys for different environments"
    Write-Host "• Keys should be at least 32 characters (256 bits)"
    Write-Host "• Consider rotating keys periodically"
    Write-Host "• Store keys securely (environment variables, key vaults)"
}

function Show-ErrorDetails {
    Write-ColorMessage "?? JWT ERROR ANALYSIS" $Red
    Write-Host ""
    Write-ColorMessage "The error you're seeing:" $Yellow
    Write-Host "IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'HS256'"
    Write-Host "the key size must be greater than: '256' bits, key has '136' bits"
    Write-Host ""
    Write-ColorMessage "What this means:" $Blue
    Write-Host "• Your JWT secret key is only 17 characters (136 bits)"
    Write-Host "• HS256 algorithm requires minimum 32 characters (256 bits)"
    Write-Host "• The application can't create JWT tokens with the current key"
    Write-Host ""
    Write-ColorMessage "Root causes:" $Yellow
    Write-Host "• JWT_SECRET_KEY environment variable not set in production"
    Write-Host "• Using truncated or default key"
    Write-Host "• Configuration issue in App Runner"
    Write-Host ""
    Write-ColorMessage "Solution:" $Green
    Write-Host "• Generate a proper JWT key (see options above)"
    Write-Host "• Set JWT_SECRET_KEY environment variable in App Runner"
    Write-Host "• Redeploy the application"
}

# Main execution
function Main {
    Write-ColorMessage "?? JWT KEY GENERATOR AND VALIDATOR" $Green
    Write-ColorMessage "==================================" $Blue
    Write-Host ""
    
    if ($Generate) {
        $keys = Generate-SecureJWTKey
        Write-Host ""
        Show-DeploymentInstructions -Keys $keys
        
    } elseif ($Validate -and $TestKey) {
        Test-JWTKey -Key $TestKey | Out-Null
        
    } else {
        Show-ErrorDetails
        Write-Host ""
        Check-CurrentConfiguration
        Write-Host ""
        Write-ColorMessage "?? ACTIONS AVAILABLE:" $Yellow
        Write-Host "• Generate new JWT keys: .\jwt-key-fix.ps1 -Generate"
        Write-Host "• Validate specific key: .\jwt-key-fix.ps1 -Validate -TestKey 'your-key'"
        Write-Host "• Check current config: .\jwt-key-fix.ps1"
    }
}

# Run the main function
Main