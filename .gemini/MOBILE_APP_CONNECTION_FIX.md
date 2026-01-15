# 🔧 Mobile App Connection Fix

## Issue Identified

The mobile app is trying to connect to the API but getting connection errors.

---

## ✅ Solution

### For Physical Android Device:

**1. Find Your PC's IP Address:**

```powershell
# Run this in PowerShell:
ipconfig

# Look for "IPv4 Address" under your WiFi adapter
# Example: 192.168.1.13
```

**2. Update Flutter App Configuration:**

The app is already configured to use `192.168.1.13:5009`.

**3. Ensure Both Devices on Same Network:**
- PC and phone must be on the same WiFi network
- Check phone's WiFi settings

**4. Test API Connectivity:**

From your phone's browser, visit:
```
http://192.168.1.13:5009/api/menu/categories
```

If you see JSON data, the connection works!

**5. Check Windows Firewall:**

```powershell
# Allow port 5009 through firewall:
netsh advfirewall firewall add rule name="Restaurant API" dir=in action=allow protocol=TCP localport=5009
```

---

### For Android Emulator:

If using Android Emulator instead of physical device:

**Update `constants.dart` line 10:**
```dart
: 'http://10.0.2.2:5009/api';  // For Android Emulator
```

---

### For iOS Simulator:

**Update `constants.dart` line 10:**
```dart
: 'http://localhost:5009/api';  // For iOS Simulator
```

---

## 🧪 Quick Test

**Test from mobile browser:**
1. Open Chrome/Safari on your phone
2. Go to: `http://192.168.1.13:5009/api/menu/categories`
3. Should see JSON response

**Expected Response:**
```json
{
  "success": true,
  "data": [...]
}
```

---

## Common Issues

### Issue: "This site can't be reached"

**Causes:**
1. ❌ PC and phone on different WiFi networks
2. ❌ Windows Firewall blocking port 5009
3. ❌ Wrong IP address
4. ❌ API server not running

**Solutions:**
1. ✅ Connect both to same WiFi
2. ✅ Add firewall rule (see above)
3. ✅ Run `ipconfig` to get correct IP
4. ✅ Check API server is running

### Issue: "ERR_ADDRESS_INVALID" with 0.0.0.0

**Cause:** Trying to access `0.0.0.0` directly

**Solution:** Use actual IP address (`192.168.1.13`) not `0.0.0.0`

---

## Current Configuration

**API Server:**
- Listening on: `0.0.0.0:5009` (all interfaces)
- Accessible at: `http://192.168.1.13:5009`
- Status: ✅ Running

**Mobile App:**
- Configured for: `http://192.168.1.13:5009/api`
- Device Type: Physical Android
- Status: ⏳ Needs network connectivity

---

## Hot Reload After Changes

If you change `constants.dart`:

```bash
# In Flutter terminal, press:
r  # Hot reload
R  # Hot restart (if hot reload doesn't work)
```

---

## Verification Checklist

- [ ] API server running on port 5009
- [ ] PC IP address is `192.168.1.13` (verify with `ipconfig`)
- [ ] Phone and PC on same WiFi network
- [ ] Firewall allows port 5009
- [ ] Can access API from phone browser
- [ ] Flutter app restarted after any config changes

---

**If all else fails:**

1. Stop Flutter app
2. Run: `flutter clean`
3. Run: `flutter pub get`
4. Run: `flutter run`
