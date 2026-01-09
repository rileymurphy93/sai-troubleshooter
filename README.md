## SAI Troubleshooter

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![UI](https://img.shields.io/badge/UI-WPF-purple)

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
