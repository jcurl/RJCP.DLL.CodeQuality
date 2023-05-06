# Documentation <!-- omit in toc -->

This contains information on how to convert the documentation to other readable
formats.

Documentation is written in DocBook 4.5 format.

- [1. Building Documentation](#1-building-documentation)
  - [1.1. Build Installation Prerequisites](#11-build-installation-prerequisites)
  - [1.2. Building on Ubuntu 20.04](#12-building-on-ubuntu-2004)
- [2. Current Issues](#2-current-issues)
  - [2.1. Pandoc on Ubuntu 22.04 and earlier](#21-pandoc-on-ubuntu-2204-and-earlier)

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

### 2.1. Pandoc on Ubuntu 22.04 and earlier

The version of pandoc that comes with Ubuntu 22.04 LTS is very old, `pandoc
2.9.2.1` from 2020-03-23!

When building the `AccessorBaseBestPractices.pdf`, the formatting in section
3.1, for Instantiable Classes, of bullet points, exceeds margins. A fix has been
provided in newer versions [GitHub
#7821](https://github.com/jgm/pandoc/issues/7821). For this, you'll need a newer
version, `pandoc 2.17` (2022-01-12).

Follow the instructions to
[install](https://github.com/jgm/pandoc/blob/main/INSTALL.md). These
instructions (run on Ubuntu 22.04) will install to `$HOME/.cabal/bin`.

```sh
# apt install cabal-install
# cd pandoc
# cabal update
# cabal install pandoc-cli
```

Then to build the documentation using your own version of pandoc, run:

```sh
$ cd doc/build
$ cmake .. -DPANDOC_EXECUTABLE=$HOME/.cabal/bin/pandoc
```