## SAI Troubleshooter

A simple WPF application for running built-in Windows diagnostics:

- SFC (/scannow)
- DISM (/online /cleanup-image /restorehealth)
- CHKDSK (C: /scan)

## Requirements
- Windows 11
- Administrator privileges

## Build
```bash
dotnet build
