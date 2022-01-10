# DocbookGEN for CMake

This project is to make it easier to allow you to build docbook output.
You would include this module as a submodule in your git repository and
reference the cmake files from there.

This allows you to update to a newer version of the cmake scripts at any
time without having to always copy updates to your repository.

This wouldn't be possible without the great help of the book by Bob Stayton
found at http://www.sagehill.net/docbookxsl

# Using this Module

## Quick Start Guide

### Cloning and Configuring your Git Repository

Make a clone of this git repository into your existing cmake repository.

```
$ git submodule add github.com:jcurl/docbookgen-cmake cmake/modules/docbook
```

Your base CMakeLists.txt file should contain at the top the following:

```
cmake_minimimum_required(VERSION 2.8)
project(Docbook45Test)

set(CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/cmake/modules/docbook ${CMAKE_MODULE_PATH}

find_package(Docbook45)

add_subdirectory(docbook45-html-bookcomplex)
```

Then inside your documentation folder (docbook45-html-bookcomplex), you would have
something like:

```
if(DOCBOOK45_FOUND)
  include(DocbookGen)

  set(DOCBOOK_FILES
    main.xml
    Part1Ch0.xml
    Part1Ch1.xml
    Part1Ch2.xml
    Part2Ch0.xml
    Part2Ch1.xml)

  add_docbook45_target(
    FORMAT html
    OUTPUT main.html
    INPUT ${DOCBOOK_FILES}
    IMAGEDIR images
    IMAGEDIR figures
    XSL docbook.xsl
    OLINK olinkdb.xml
    FILECOPY docbook.css)
endif(DOCBOOK45_FOUND)
```

### Building Prerequisites

You'll need to have Java installed on your local machine. In Ubuntu, you would
have something like the following:

```
$ sudo apt install jre-default
```

You'll also have to have the tooling somewhere on your computer (currently, the tooling
is not downloaded automatically for you, you need to obtain this and extract yourself).

* tools/docbook-xml-4.5
  - There should be the DTD files and the README file
  - Download from:
    - http://www.oasis-open.org/docbook/xml/4.5/docbook-xml-4.5.zip
* tools/docbook-xsl-1.79.1
  - There should be the installation instructions, and README file. There is
    no installation required, just unzip the file.
  - Download from:
    - http://sourceforge.net/projects/docbook/files/docbook-xsl/1.79.1/docbook-xsl-1.79.1.zip/download
* tools/saxon655
  - There should be the saxon XSLT 1.0 interpreter, "saxon.jar"
  - Download from:
    - http://prdownloads.sourceforge.net/saxon/saxon6-5-5.zip
* tools/xslthl-2.1.3
  - IThere should be the highlighter code, "xslthl-2.1.3.jar"
  - Download from:
    - http://sourceforge.net/projects/xslthl/files/xslthl/2.1.3/xslthl-2.1.3-dist.zip/download
* tools/xerces-2_11_0
  - There should be the two files "xercesImpl.jar" and "xml-apis.jar"
  - Download from:
    - http://mirror.dkd.de/apache//xerces/j/binaries/Xerces-J-bin.2.11.0.zip
* tools/xml-commons-resolver-1.2
  - There should be the file "resolver.jar"
  - Download from:
    - http://mirror.dkd.de/apache//xerces/xml-commons/binaries/xml-commons-resolver-1.2.zip

Put these in a folder, say your $HOME directory under a folder called "docbook45/tools"

### Building

Now you're ready to build.

```
$ mkdir build
$ cd build
$ cmake .. -DDOCBOOK45_BIN_DIR="$HOME/docbook45/tools"
```

