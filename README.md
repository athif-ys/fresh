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

## Build and run

```bash
dotnet build
dotnet run
```

## Example OEM command ideas

- Launch a script that calls your vendor CLI.
- Run a notification automation.
- Trigger any custom command you use for battery profile switching.
