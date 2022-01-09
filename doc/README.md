# Documentation <!-- omit in toc -->

This contains information on how to convert the documentation to other readable
formats.

Documentation is written in DocBook 4.5 format.

- [1. Building Documentation](#1-building-documentation)
  - [1.1. Build Installation Prerequisites](#11-build-installation-prerequisites)
  - [1.2. Building on Ubuntu 20.04](#12-building-on-ubuntu-2004)
- [2. Current Issues](#2-current-issues)

## 1. Building Documentation

### 1.1. Build Installation Prerequisites

Building this documentation requires the installation of:

- CMake
- Pandoc
- Latex

### 1.2. Building on Ubuntu 20.04

On Ubuntu, installation can be done with:

```cmd
# apt install cmake
# apt install pandoc
# apt install texlive-latex-recommended
# apt install texlive-xetex
```

From the `doc` directory,

```cmd
$ mkdir build
$ cd build
$ cmake ..
$ make
```

The resultant PDF file is found in the path
`build/product/AccessorBaseBestPractices.pdf`

## 2. Current Issues

The original documentation is written in DocBook 4.5 using XML Mind Editor. Code
sections are in the `literal` or `code` tag. Unfortunately, when converting to
PDF using latex, extra spaces are taken into account, when they should be
ignored.
