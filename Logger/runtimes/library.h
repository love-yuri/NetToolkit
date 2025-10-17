#pragma once

extern "C" void __declspec(dllexport) Info(const char *msg);
extern "C" void __declspec(dllexport) Error(const char *msg);
extern "C" void __declspec(dllexport) Warn(const char *msg);
extern "C" void __declspec(dllexport) SetLogFilePath(const char *msg);
extern "C" void __declspec(dllexport) SetWriteMode(unsigned char mode);
