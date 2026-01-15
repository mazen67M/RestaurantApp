# PowerShell script to add [Authorize] attribute to all admin pages
# This adds security to prevent unauthorized access

$adminPagesPath = "H:\Restaurant APP\src\RestaurantApp.Web\Components\Pages\Admin"

# List of admin pages to secure (excluding Login.razor)
$pages = @(
    "Orders\Index.razor",
    "Menu\Index.razor",
    "Categories\Index.razor",
    "Branches\Index.razor",
    "Offers\Index.razor",
    "Users\Index.razor",
    "Reviews\Index.razor",
    "Loyalty\Index.razor",
    "Deliveries\Index.razor",
    "Reports\Index.razor"
)

foreach ($page in $pages) {
    $filePath = Join-Path $adminPagesPath $page
    
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Check if already has [Authorize]
        if ($content -notmatch '\[Authorize') {
            # Find the line with @rendermode
            if ($content -match '(@rendermode\s+\w+)') {
                # Add the authorization lines after @rendermode
                $newContent = $content -replace '(@rendermode\s+\w+)', "`$1`r`n@using Microsoft.AspNetCore.Authorization`r`n@attribute [Authorize(Roles = ""Admin"")]"
                
                Set-Content -Path $filePath -Value $newContent -NoNewline
                Write-Host "✅ Added [Authorize] to $page" -ForegroundColor Green
            }
        } else {
            Write-Host "⏭️  $page already has [Authorize]" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ File not found: $page" -ForegroundColor Red
    }
}

Write-Host "`n🎉 Authorization added to all admin pages!" -ForegroundColor Cyan
