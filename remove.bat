@echo off
@echo 开始卸载
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /u %~dp0MailService.exe
pause