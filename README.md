# ChargeGuard (Windows)

A lightweight Windows Forms app to monitor battery charge and react when a target percentage is reached.

## What it can do

- Monitor battery percentage on a schedule.
- Alert you when battery reaches your target (for example, 80%).
- Optionally run an OEM command/tool when threshold is reached.

## Important limitation

Windows does **not** provide a universal API to directly stop charging on all laptops.
Actual charging limits are often controlled by OEM firmware utilities (Lenovo Vantage, Dell Power Manager, ASUS MyASUS, etc.).

ChargeGuard solves this by:

1. Monitoring battery level.
2. Alerting you exactly at your configured threshold.
3. Optionally running your own command that calls an OEM utility.

---

## Install on Windows

You have two options:

### Option A: Run from source (quickest)

1. Install **.NET SDK 8.0** (x64) from Microsoft: https://dotnet.microsoft.com/download/dotnet/8.0
2. Open **PowerShell** in this project folder.
3. Run:

```powershell
dotnet restore
dotnet run -c Release
```

This starts the app directly.

### Option B: Build a standalone `.exe` (recommended for daily use)

1. Install **.NET SDK 8.0** from Microsoft.
2. In PowerShell, from this project folder, run:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

3. After publish completes, open:

```text
bin\Release\net8.0-windows\win-x64\publish\
```

4. Run `ChargeGuard.exe`.

You can copy that `publish` folder anywhere (for example `C:\Apps\ChargeGuard`) and create a desktop shortcut to `ChargeGuard.exe`.

---

## Start automatically with Windows (optional)

1. Press `Win + R`, enter `shell:startup`, and press Enter.
2. Create a shortcut to `ChargeGuard.exe` in that folder.

---

## SmartScreen / antivirus note

Because this is an unsigned app, Windows SmartScreen may warn on first run.
Use **More info → Run anyway** only if you trust your own build.

---

## Usage tips

- Set **Stop target (%)** to your preferred value (for example 80).
- Keep **Trigger only when charging** enabled.
- If your laptop vendor has a CLI/API, paste that command in the optional command box to enforce thresholds automatically.

## Example OEM command ideas

- Launch a script that calls your vendor CLI.
- Run a notification automation.
- Trigger any custom command you use for battery profile switching.
