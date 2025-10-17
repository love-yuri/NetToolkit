cmake_minimum_required(VERSION 3.31)
project(YuriLogger)

set(CMAKE_CXX_STANDARD 14)

add_library(${PROJECT_NAME} SHARED library.cpp)

set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)

# MSVC 编译器选项
target_compile_options(${PROJECT_NAME} PRIVATE
    /EHsc /utf-8 # 基础选项
    $<$<CONFIG:Debug>:/Zi /Ob0 /RTC1>      # Debug 配置
    $<$<CONFIG:Release>:/O2 /Oi>               # Release 配置
    $<$<CXX_COMPILER_ID:MSVC>:/MP>
)

# 获取 DLL 输出路径
get_target_property(DLL_OUTPUT_DIR ${PROJECT_NAME} RUNTIME_OUTPUT_DIRECTORY)

# 定义拷贝命令（编译后执行）
add_custom_command(
    TARGET ${PROJECT_NAME} POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy
    "$<TARGET_FILE:${PROJECT_NAME}>"          # 源文件（DLL）
    "${CMAKE_SOURCE_DIR}/win-x64/native/"  # 目标目录
    COMMENT "dll 已经拷贝到: ${CMAKE_SOURCE_DIR}/win-x64/native/"
)