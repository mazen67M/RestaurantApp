# Restaurant API - Windows Firewall Rule
# Run this script as Administrator to allow API access from mobile devices

Write-Host "Adding Windows Firewall rule for Restaurant API..." -ForegroundColor Cyan

# Remove existing rule if it exists
netsh advfirewall firewall delete rule name="Restaurant API Port 5009" 2>$null

# Add new rule
netsh advfirewall firewall add rule name="Restaurant API Port 5009" dir=in action=allow protocol=TCP localport=5009

Write-Host "✅ Firewall rule added successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "The API on port 5009 is now accessible from your mobile device." -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Find your PC's IP: ipconfig" -ForegroundColor White
Write-Host "2. Update Flutter app if needed (currently: 192.168.1.13)" -ForegroundColor White
Write-Host "3. Test from phone browser: http://192.168.1.13:5009/api/menu/categories" -ForegroundColor White
