#include "library.h"
#include "yuri_log.hpp"

void Info(const char *msg) {
  yuri::Log(yuri::Log::LogLevel::Info) << msg;
}

void Warn(const char *msg) {
  yuri::Log(yuri::Log::LogLevel::Warning) << msg;
}

void SetLogFilePath(const char *msg) {
  yuri::Log::filePath() = msg;
}

void SetWriteMode(const unsigned char mode) {
  yuri::Log::writeMode() = mode;
}

void Error(const char *msg) {
  yuri::Log(yuri::Log::LogLevel::Error) << msg;
}
