cmake_minimum_required(VERSION 2.8)
project(DocBook45Test NONE)

set(CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/cmake/modules/docbook ${CMAKE_MODULE_PATH})

find_package(Docbook45)

add_subdirectory(AccessorBestPractices)
