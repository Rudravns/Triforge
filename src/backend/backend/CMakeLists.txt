cmake_minimum_required(VERSION 3.12)

project(backend)

add_library(backend SHARED
    "src/backend.cpp"
)

target_include_directories(backend PUBLIC include)

set_property(TARGET backend PROPERTY CXX_STANDARD 20)