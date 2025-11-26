# PowerShell script to generate JWT tokens and update local-test.http file

param(
    [string]$ProjectPath = "Eshop.Api",
    [string]$TestFileTemplate = "local-test-template.http",
    [string]$TestFile = "local-test.http",
    [string]$Audience = "https://localhost"
)

Write-Host "Generating JWT tokens for testing..." -ForegroundColor Green

# Function to generate JWT token and extract the token value
function Generate-JwtToken {
    param(
        [string]$Role,
        [string]$Scope
    )
    
    Write-Host "Generating token for Role: $Role, Scope: $Scope" -ForegroundColor Yellow
    
    # Generate the JWT token using dotnet user-jwts create
    $tokenOutput = dotnet user-jwts create --role $Role --scope $Scope --project $ProjectPath --audience $Audience
    
    # Extract the token from the output
    # The token is typically in the format: Token: <actual_token>
    $tokenLine = $tokenOutput | Where-Object { $_ -match "Token:\s+(.+)" }
    
    if ($tokenLine) {
        $token = $matches[1].Trim()
        Write-Host "Generated token: $token" -ForegroundColor Green
        return $token
    } else {
        Write-Host "Failed to extract token from output" -ForegroundColor Red
        return $null
    }
}

# Function to update the test file with actual token values
function Update-TestFile {
    param(
        [string]$File,
        [string]$VariableName,
        [string]$TokenValue
    )
    
    $content = Get-Content $File -Raw
    
    # Replace the placeholder with actual token value
    $pattern = "($VariableName=)<[^>]+>"
    $replacement = "`$1$TokenValue"
    
    $updatedContent = $content -replace $pattern, $replacement
    
    Set-Content -Path $File -Value $updatedContent -NoNewline
    
    Write-Host "Updated $VariableName in $File" -ForegroundColor Cyan
}

# Main execution
try {
    # Copy the template file to the test file
    Write-Host "`nCopying template file to $TestFile`n" -ForegroundColor Green

    Copy-Item -Path $TestFileTemplate -Destination $TestFile -Force
    # Generate tokens for different roles and permissions
    
    # Staff with read permission
    $staffReadToken = Generate-JwtToken -Role "Staff" -Scope "products:read"
    if ($staffReadToken) {
        Update-TestFile -File $TestFile -VariableName "@STAFF_READ_TOKEN" -TokenValue $staffReadToken
    }
    
    # Staff with write permission
    $staffWriteToken = Generate-JwtToken -Role "Staff" -Scope "products:write"
    if ($staffWriteToken) {
        Update-TestFile -File $TestFile -VariableName "@STAFF_WRITE_TOKEN" -TokenValue $staffWriteToken
    }
    
    # Admin with read permission
    $adminReadToken = Generate-JwtToken -Role "Admin" -Scope "products:read"
    if ($adminReadToken) {
        Update-TestFile -File $TestFile -VariableName "@ADMIN_READ_TOKEN" -TokenValue $adminReadToken
    }
    
    # Admin with write permission
    $adminWriteToken = Generate-JwtToken -Role "Admin" -Scope "products:write"
    if ($adminWriteToken) {
        Update-TestFile -File $TestFile -VariableName "@ADMIN_WRITE_TOKEN" -TokenValue $adminWriteToken
    }
    
    Write-Host "`nAll JWT tokens have been generated and updated in $TestFile" -ForegroundColor Green
    Write-Host "You can now use the updated local-test.http file for testing." -ForegroundColor Green
    
} catch {
    Write-Host "An error occurred: $($_.Exception.Message)" -ForegroundColor Red
}