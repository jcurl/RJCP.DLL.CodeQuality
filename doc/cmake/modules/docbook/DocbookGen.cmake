# Copyright (c) 2016 Jason Curl <jcurl@arcor.de>

# This is where to find this module, to find other files required for building
# (particularly the catalog file templates).
set(DOCBOOK45_SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR})

# Build a document based on DocBook 4.5 XML files.
#
# add_docbook45_target(FORMAT format
#                      OUTPUT output
#                      INPUT input1.xml input2.xml
#                      IMAGEDIR imagedir
#                      XSL docbook.xsl
#                      OLINK olinkdb.xml
#                      FILECOPY file1.css)
#
# FORMAT: The output format. We only support 'html' for now.
# OUTPUT: The name of the output file (or multiple files if split)
# INPUT: a list of input XML files that form part of the docbook to regenerate
#  the output on any change. The first file is the main docbook file used
#  to reference all other xml fils (it is the first file given to the java
#  docbook processor)
# IMAGEDIR: List of directories relative to current directory that contains
#  images to copy in the output
# XSL: Location where the docbook xsl file can be found for
#  customisations. If this is not specified, it uses the default in the docbook
#  sources "html/docbook.xsl". This file automatically becomes part of the
#  build dependencies.
# OLINK: The name of the .in database file that is processed and used for
#  olink targets inside of your docbook file. If you don't use olink, then you
#  should ignore this option. It must be in the same directory as where you
#  use this rule. Note, do not add the ".in" at the end of the file, even
#  though on disk it is a "olink.xml.in" file. This file automatically becomes
#  part of the build dependencies.
# FILECOPY: A list of extra files to copy to the target (e.g. CSS files, or other
#  extra files for proper rendering)
#
# Customisable variables are:
# * DOCBOOK45_BIN_DIR: The base directory (without leading slash) where to find
#   the Java archives and tooling to generate DocBook output.
#
# Variables created after execution of this rule for use in your CMakeLists.txt
# are:
# * DOCBOOK45_SAXON_DIR: Saxon tooling (saxon655)
# * DOCBOOK45_XERCES_DIR: Xerces (xerces-2_11_0)
# * DOCBOOK45_RESOLVER_DIR: Apache XML resolver (xml-commons-resolver-1.2)
# * DOCBOOK45_XSL_DIR: DocBook XSL (docbook-xsl-1.79.1)
# * DOCBOOK45_XSLTHL_DIR: XSLT Highlighting (xslthl-2.1.3)
# * DOCBOOK45_CATALOG_DIR: Catalog generated output
function(ADD_DOCBOOK45_TARGET)
  # Parse the input parameters and set up the lists required
  set(argmode "")
  set(format "")
  set(output "")
  set(target "")
  set(outputs "")
  set(input "")
  set(inputs "")
  set(imagedirs "")
  set(xslfile "")
  set(olink "")
  set(extra "")
  foreach(arg IN LISTS ARGV)
    if("${arg}" STREQUAL "FORMAT")
      set(argmode "FORMAT")
    elseif("${arg}" STREQUAL "OUTPUT")
      set(argmode "OUTPUT")
    elseif("${arg}" STREQUAL "INPUT")
      set(argmode "INPUT")
    elseif("${arg}" STREQUAL "IMAGEDIR")
      set(argmode "IMAGEDIR")
    elseif("${arg}" STREQUAL "XSL")
      set(argmode "XSL")
    elseif("${arg}" STREQUAL "OLINK")
      set(argmode "OLINK")
    elseif("${arg}" STREQUAL "FILECOPY")
      set(argmode "FILECOPY")
    else()
      if ("${argmode}" STREQUAL "FORMAT")
        if("${format}" STREQUAL "")
          set(format ${arg})
        else()
          message(FATAL_ERROR "DOCBOOK45: Can only specify FORMAT once")
        endif()
      elseif("${argmode}" STREQUAL "OUTPUT")
        if("${output}" STREQUAL "")
          set(target ${arg})
        endif()
        list(APPEND outputs ${arg})
      elseif("${argmode}" STREQUAL "INPUT")
        if("${input}" STREQUAL "")
          set(input ${arg})
        endif()
        list(APPEND inputs ${arg})
      elseif("${argmode}" STREQUAL "IMAGEDIR")
        list(APPEND imagedirs ${arg})
      elseif("${argmode}" STREQUAL "XSL")
        if("${xslfile}" STREQUAL "")
          set(xslfile ${arg})
        else()
          message(FATAL_ERROR "DOCBOOK45: Can only specify XSL once")
        endif()
      elseif("${argmode}" STREQUAL "OLINK")
        if("${olink}" STREQUAL "")
          set(olink ${arg})
          if(NOT EXISTS ${CMAKE_CURRENT_SOURCE_DIR}/${olink}.in OR
              IS_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/${olink}.in)
            message(FATAL_ERROR "DOCBOOK45: ${olink}.in isn't in current path")
          endif()
        else()
          message(FATAL_ERROR "DOCBOOK45: Can only specify OLINK once")
        endif()
      elseif("${argmode}" STREQUAL "FILECOPY")
        if(IS_ABSOLUTE ${arg})
          message(FATAL_ERROR "DOCBOOK45: FileCopy rules must be relative")
        endif()
        list(APPEND extra ${arg})
      else()
        message(FATAL_ERROR "DOCBOOK45: Unexpected argument ${arg} in ADD_DOCBOOK45_TARGET")
      endif()
    endif()
  endforeach()

  if("${olink}" STREQUAL "")
    if(EXISTS ${CMAKE_CURRENT_SOURCE_DIR}/olinkdb.xml.in)
      set(olink "olinkdb.xml")
      message(STATUS "DOCBOOK45: Assuming OLINK option 'olinkdb.xml'")
    endif()
  endif()

  if ("${target}" STREQUAL "")
    message(FATAL_ERROR "DOCBOOK45: No output specified")
  else()
    if(IS_ABSOLUTE ${target})
      set(output ${target})
    else()
      set(output ${CMAKE_CURRENT_BINARY_DIR}/${format}/${target})
    endif()
  endif()

  if(NOT DOCBOOK45_FOUND)
    message(FATAL_ERROR "DOCBOOK45: Didn't find all docbook 4.5 tools required")
  endif()

  # We only support Unix platforms, until we can test on others
  if (UNIX)
    set(CP cp -f)
  else()
    message(FATAL_ERROR "DOCBOOK45: Don't know how to make targets for current platform")
  endif()

  if(format STREQUAL "html")
    if("${xslfile}" STREQUAL "")
      set(xslfile "${DOCBOOK45_XSL_DIR}/html/docbook.xsl")
      message("DOCBOOK45: Using default XSL at ${xslfile}")
    endif()
  elseif(format STREQUAL "pdf")
    if("${xslfile}" STREQUAL "")
      set(xslfile "${DOCBOOK45_XSL_DIR}/fo/docbook.xsl")
      message("DOCBOOK45: Using default XSL at ${xslfile}")
    endif()
  else()
    message(FATAL_ERROR "DOCBOOK45: ${format} not supported by DocbookGen cmake module")
  endif()

  set(DOCBOOK45_CATALOG_DIR "${CMAKE_BINARY_DIR}/catalog" CACHE STRING "Catalog generated output directory")
  make_directory(${DOCBOOK45_CATALOG_DIR})
  configure_file(
    ${DOCBOOK45_CATALOG_SOURCE_DIR}/CatalogManager.properties.in
    ${DOCBOOK45_CATALOG_DIR}/CatalogManager.properties)
  configure_file(
    ${DOCBOOK45_CATALOG_SOURCE_DIR}/catalog.xml.in
    ${DOCBOOK45_CATALOG_DIR}/catalog.xml)
  if(NOT "${olink}" STREQUAL "")
    configure_file(
      ${CMAKE_CURRENT_SOURCE_DIR}/${olink}.in
      ${CMAKE_CURRENT_BINARY_DIR}/${olink})
    set(olinkbuild target.database.document="${CMAKE_CURRENT_BINARY_DIR}/${olink}")
    set(olinkdep ${CMAKE_CURRENT_SOURCE_DIR}/${olink}.in)
  endif()

  set(DOCBOOK45_SAXON_DIR ${DOCBOOK45_SAXON_DIR} CACHE STRING "Location of Saxon 6.5.5")
  set(DOCBOOK45_XERCES_DIR ${DOCBOOK45_XERCES_DIR} CACHE STRING "Location of Xerces XML Parser 2.11.0")
  set(DOCBOOK45_RESOLVER_DIR ${DOCBOOK45_RESOLVER_DIR} CACHE STRING "Location of apache XML Resolver 1.2")
  set(DOCBOOK45_XSL_DIR ${DOCBOOK45_XSL_DIR} CACHE STRING "Location of Docbook XSL 1.79.1")
  set(DOCBOOK45_XSLTHL_DIR ${DOCBOOK45_XSLTHL_DIR} CACHE STRING "Location of XSL THL 2.1.3")

  set(working "${CMAKE_CURRENT_BINARY_DIR}/${format}")
  make_directory(${working})

  # Look for all images, add them as copy commands so they get copied
  set(imagetargets "")
  foreach(dir ${imagedirs})
    docbook45_find_images(${working} ${dir} imagetargets)
  endforeach()
  docbook45_filecopy(${working} extra imagetargets)

  set(CLASSPATH "${DOCBOOK45_SAXON_DIR}/saxon.jar:${DOCBOOK45_XERCES_DIR}/xercesImpl.jar:${DOCBOOK45_XERCES_DIR}/xml-apis.jar:${DOCBOOK45_RESOLVER_DIR}/resolver.jar:${DOCBOOK45_XSL_DIR}/extensions/saxon65.jar:${DOCBOOK45_XSLTHL_DIR}/xslthl-2.1.3.jar:${CMAKE_BINARY_DIR}/catalog")
  set(XSLTHLCONF "file://${DOCBOOK45_XSL_DIR}/highlighting/xslthl-config.xml")

  set(JAVADOC java -cp ${CLASSPATH}
    -Djavax.xml.parsers.DocumentBuilderFactory=org.apache.xerces.jaxp.DocumentBuilderFactoryImpl
    -Djavax.xml.parsers.SAXParserFactory=org.apache.xerces.jaxp.SAXParserFactoryImpl
    -Dorg.apache.xerces.xni.parser.XMLParserConfiguration=org.apache.xerces.parsers.XIncludeParserConfiguration
    -Dxslthl.config=${XSLTHLCONF}
    com.icl.saxon.StyleSheet
    -x org.apache.xml.resolver.tools.ResolvingXMLReader
    -y org.apache.xml.resolver.tools.ResolvingXMLReader
    -r org.apache.xml.resolver.tools.CatalogResolver
    -u)

  add_custom_command(OUTPUT ${target}.stamp
    COMMAND ${JAVADOC} ${input} ${xslfile} collect.xref.targets="only" targets.filename="${CMAKE_CURRENT_BINARY_DIR}/target.db"
    COMMAND ${JAVADOC} -o ${output} ${input} ${xslfile} ${olinkbuild} current.docid=Network
    COMMAND cmake -E touch ${CMAKE_CURRENT_BINARY_DIR}/${target}.stamp
    DEPENDS ${inputs} ${xslfile} ${olinkdep}
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR})

  add_custom_command(OUTPUT ${target}.fo.stamp
    COMMAND ${JAVADOC} ${input} ${xslfile} collect.xref.targets="only" targets.filename="${CMAKE_CURRENT_BINARY_DIR}/target.db"
    COMMAND ${JAVADOC} -o ${output}.fo ${input} ${xslfile} ${olinkbuild} current.docid=Network
    COMMAND fop ${output}.fo ${output}
    COMMAND cmake -E touch ${CMAKE_CURRENT_BINARY_DIR}/${target}.fo.stamp
    DEPENDS ${inputs} ${xslfile} ${olinkdep}
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR})

  if(format STREQUAL "html")
    set(TARGETDEPENDS ${imagetargets} ${target}.stamp)
  elseif(format STREQUAL "pdf")
    set(TARGETDEPENDS ${imagetargets} ${target}.fo.stamp)
  endif()

  add_custom_target(
    ${target} ALL
    DEPENDS ${TARGETDEPENDS})
endfunction(ADD_DOCBOOK45_TARGET)






macro(MAKE_WINDOWS_PATH pathname)
  # An extra \\ escape is necessary to get a \ through CMake's processing.
  string(REPLACE "/" "\\" ${pathname} "${${pathname}}")
  # Enclose with UNESCAPED quotes.  This means we need to escape our
  # quotes once here, i.e. with \"
  set(${pathname} \"${${pathname}}\")
endmacro(MAKE_WINDOWS_PATH)



function(DOCBOOK45_FIND_IMAGES outdir imgdir imgtargets)
  if (IS_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/${imgdir}")
    make_directory(${working}/${imgdir})
    set(targets "")
    FILE(GLOB files RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} ${CMAKE_CURRENT_SOURCE_DIR}/${imgdir}/*)
    foreach(file ${files})
      list(APPEND targets "${working}/${file}")
      add_custom_command(
	OUTPUT ${working}/${file}
	COMMAND ${CP} "${CMAKE_CURRENT_SOURCE_DIR}/${file}" "${working}/${file}"
	DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/${file}")
    endforeach()
    foreach(file ${targets})
      list(APPEND ${imgtargets} ${file})
    endforeach()
    set(${imgtargets} ${${imgtargets}} PARENT_SCOPE)
  else()
    message(STATUS "DOCBOOK45: Directory ${CMAKE_CURRENT_SOURCE_DIR}/${imgdir} not found")
  endif()
endfunction()

function(DOCBOOK45_FILECOPY outdir files imgtargets)
  set(targets "")
  foreach(file ${${files}})
    list(APPEND targets "${working}/${file}")
    add_custom_command(
      OUTPUT ${working}/${file}
      COMMAND ${CP} "${CMAKE_CURRENT_SOURCE_DIR}/${file}" "${working}/${file}"
      DEPENDS "${CMAKE_CURRENT_SOURCE_DIR}/${file}")
  endforeach()
  foreach(file ${targets})
    list(APPEND ${imgtargets} ${file})
  endforeach()
  set(${imgtargets} ${${imgtargets}} PARENT_SCOPE)
endfunction()
