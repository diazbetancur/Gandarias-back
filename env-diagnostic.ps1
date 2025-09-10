# Environment Variable Diagnostic Script - Windows PowerShell
# This script identifies and fixes problematic environment variables with template syntax

param(
    [switch]$Fix = $false
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

function Test-EnvironmentVariables {
    Write-ColorMessage "?? Checking environment variables for template syntax..." $Yellow
    Write-Host ""
    
    $problematicVars = @()
    $checkVars = @("SMTP_PORT", "SMTP_ENABLE_SSL", "DATABASE_URL", "JWT_SECRET_KEY", "SMTP_SERVER", "SMTP_USER", "SMTP_PASSWORD")
    
    foreach ($varName in $checkVars) {
        $value = [Environment]::GetEnvironmentVariable($varName, "User")
        $systemValue = [Environment]::GetEnvironmentVariable($varName, "Machine")
        $processValue = [Environment]::GetEnvironmentVariable($varName, "Process")
        
        # Check all scopes
        $allValues = @($value, $systemValue, $processValue) | Where-Object { $_ -ne $null }
        
        foreach ($val in $allValues) {
            if ($val -match '\$\{.*\}' -or $val -eq '${' + $varName + '}') {
                Write-ColorMessage "? PROBLEMATIC: $varName = $val" $Red
                $problematicVars += @{Name=$varName; Value=$val; Scope="Multiple"}
            }
            elseif (![string]::IsNullOrWhiteSpace($val)) {
                Write-ColorMessage "? OK: $varName = $($val.Substring(0, [Math]::Min($val.Length, 20)))..." $Green
            }
        }
        
        if ($allValues.Count -eq 0) {
            Write-ColorMessage "??  NOT SET: $varName" $Blue
        }
    }
    
    return $problematicVars
}

function Fix-EnvironmentVariables {
    param([array]$ProblematicVars)
    
    Write-ColorMessage "?? Fixing problematic environment variables..." $Yellow
    Write-Host ""
    
    foreach ($var in $ProblematicVars) {
        Write-ColorMessage "Removing problematic variable: $($var.Name)" $Yellow
        
        # Remove from all scopes
        [Environment]::SetEnvironmentVariable($var.Name, $null, "User")
        [Environment]::SetEnvironmentVariable($var.Name, $null, "Process")
        
        # Try to remove from Machine scope (requires admin)
        try {
            [Environment]::SetEnvironmentVariable($var.Name, $null, "Machine")
            Write-ColorMessage "? Removed $($var.Name) from all scopes" $Green
        }
        catch {
            Write-ColorMessage "??  Removed $($var.Name) from User/Process (Machine scope requires admin)" $Yellow
        }
    }
}

function Show-RecommendedConfiguration {
    Write-ColorMessage "?? RECOMMENDED CONFIGURATION:" $Blue
    Write-Host ""
    Write-ColorMessage "For DEVELOPMENT (appsettings.Development.json):" $Green
    Write-Host @"
{
  "EmailService": {
    "smtpServer": "restaurantegandarias-com.correoseguro.dinaserver.com",
    "smtpPort": "587",
    "smtpUser": "horarios@restaurantegandarias.com",
    "smtpPassword": "123Gandarias456!",
    "EnableSsl": true
  }
}
"@
    Write-Host ""
    Write-ColorMessage "For PRODUCTION (App Runner Environment Variables):" $Green
    Write-Host "SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com"
    Write-Host "SMTP_PORT=587"
    Write-Host "SMTP_USER=horarios@restaurantegandarias.com"
    Write-Host "SMTP_PASSWORD=123Gandarias456!"
    Write-Host "SMTP_ENABLE_SSL=true"
    Write-Host ""
    Write-ColorMessage "??  NEVER use template syntax like `${VARIABLE_NAME}` in environment variables!" $Red
}

function Test-ApplicationStart {
    Write-ColorMessage "?? Testing application start..." $Yellow
    Write-Host ""
    
    try {
        # Try to build first
        Write-ColorMessage "Building application..." $Blue
        $buildResult = dotnet build --configuration Debug --no-restore --verbosity quiet 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-ColorMessage "? Build successful" $Green
            
            # Try a quick startup test
            Write-ColorMessage "Testing startup (5 second timeout)..." $Blue
            $startupTest = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias --no-build" -PassThru -NoNewWindow
            Start-Sleep -Seconds 5
            
            if (!$startupTest.HasExited) {
                Write-ColorMessage "? Application started successfully" $Green
                Stop-Process -Id $startupTest.Id -Force -ErrorAction SilentlyContinue
                return $true
            } else {
                Write-ColorMessage "? Application failed to start" $Red
                return $false
            }
        } else {
            Write-ColorMessage "? Build failed: $buildResult" $Red
            return $false
        }
    }
    catch {
        Write-ColorMessage "? Startup test failed: $($_.Exception.Message)" $Red
        return $false
    }
}

# Main execution
function Main {
    Write-ColorMessage "?? Environment Variable Diagnostic Tool" $Green
    Write-ColorMessage "=====================================" $Blue
    Write-Host ""
    
    $problematicVars = Test-EnvironmentVariables
    
    if ($problematicVars.Count -gt 0) {
        Write-Host ""
        Write-ColorMessage "?? FOUND $($problematicVars.Count) PROBLEMATIC VARIABLES!" $Red
        Write-ColorMessage "These variables contain template syntax (${VARIABLE}) which causes FormatException" $Red
        Write-Host ""
        
        if ($Fix) {
            Fix-EnvironmentVariables -ProblematicVars $problematicVars
            Write-Host ""
            Write-ColorMessage "?? Please restart your terminal/IDE after fixing environment variables" $Yellow
        } else {
            Write-ColorMessage "Run with -Fix parameter to automatically remove problematic variables:" $Yellow
            Write-ColorMessage ".\env-diagnostic.ps1 -Fix" $Yellow
        }
    } else {
        Write-ColorMessage "? No problematic environment variables found!" $Green
        
        Write-Host ""
        $startupSuccess = Test-ApplicationStart
        
        if ($startupSuccess) {
            Write-ColorMessage "?? Application should start without FormatException errors!" $Green
        }
    }
    
    Write-Host ""
    Show-RecommendedConfiguration
}

# Run the main function
Main