@echo off
@echo 开始安装【服务】
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\installutil.exe %~dp0MailService.exe
pause