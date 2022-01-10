# Try to find Part4 library
# Once done this will define
#  DOCBOOK45_FOUND        - if system found DocBook 4.5 tooling
#  DOCBOOK45_INCLUDE_DIRS - Is empty
#  DOCBOOK45_LIBRARIES    - Is empty
#  DOCBOOK45_DEFINITIONS  - Is empty

set(DOCBOOK45_SOURCE_DIR ${CMAKE_CURRENT_LIST_DIR})
set(DOCBOOK45_BIN_DIR "${DOCBOOK45_SOURCE_DIR}/docbook45" CACHE STRING "Base directory to find docbook 4.5 tooling")

function(DOCBOOK45_FIND_BIN_DIR)
  if(IS_DIRECTORY "${DOCBOOK45_BIN_DIR}")
    set(DOCBOOK45_BIN_DIR "${DOCBOOK45_BIN_DIR}" PARENT_SCOPE)
    set(DOCBOOK45_BIN_DIR_FOUND 1 PARENT_SCOPE)
  else()
    set(DOCBOOK45_BIN_DIR_FOUND 0 PARENT_SCOPE)
  endif()
endfunction()

function(DOCBOOK45_FIND_TOOL tool toolvar tooldir)
  if (IS_DIRECTORY "${DOCBOOK45_BIN_DIR}/${tooldir}")
    if(NOT Docbook45_FIND_QUIETLY)
      message(STATUS "DOCBOOK45: '${tool}' at '${DOCBOOK45_BIN_DIR}/${tooldir}'")
    endif()
    set(${toolvar} ${DOCBOOK45_BIN_DIR}/${tooldir} PARENT_SCOPE)
    set(${toolvar}_FOUND 1 PARENT_SCOPE)
  else()
    if(NOT Docbook45_FIND_QUIETLY)
      message(STATUS "DOCBOOK45: DocBook Directory not found at '${DOCBOOK45_BIN_DIR}/${tooldir}'")
    endif()
    set(${toolvar}_FOUND 0 PARENT_SCOPE)
  endif()
endfunction(DOCBOOK45_FIND_TOOL)

function(DOCBOOK45_FIND_SAXON)
  docbook45_find_tool(saxon DOCBOOK45_SAXON_DIR "saxon655")
  set(DOCBOOK45_SAXON_DIR ${DOCBOOK45_SAXON_DIR} PARENT_SCOPE)
  set(DOCBOOK45_SAXON_DIR_FOUND ${DOCBOOK45_SAXON_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_SAXON)

function(DOCBOOK45_FIND_DOCBOOKXSL)
  docbook45_find_tool(docbook-xsl DOCBOOK45_XSL_DIR "docbook-xsl-1.79.1")
  set(DOCBOOK45_XSL_DIR ${DOCBOOK45_XSL_DIR} PARENT_SCOPE)
  set(DOCBOOK45_XSL_DIR_FOUND ${DOCBOOK45_XSL_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_DOCBOOKXSL)

function(DOCBOOK45_FIND_XSLTHL)
  docbook45_find_tool(xslthl DOCBOOK45_XSLTHL_DIR "xslthl-2.1.3")
  set(DOCBOOK45_XSLTHL_DIR ${DOCBOOK45_XSLTHL_DIR} PARENT_SCOPE)
  set(DOCBOOK45_XSLTHL_DIR_FOUND ${DOCBOOK45_XSLTHL_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_XSLTHL)

function(DOCBOOK45_FIND_XERCES)
  docbook45_find_tool(xerces DOCBOOK45_XERCES_DIR "xerces-2_11_0")
  set(DOCBOOK45_XERCES_DIR ${DOCBOOK45_XERCES_DIR} PARENT_SCOPE)
  set(DOCBOOK45_XERCES_DIR_FOUND ${DOCBOOK45_XERCES_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_XERCES)

function(DOCBOOK45_FIND_RESOLVER)
  docbook45_find_tool(xml-commons-resolver DOCBOOK45_RESOLVER_DIR "xml-commons-resolver-1.2")
  set(DOCBOOK45_RESOLVER_DIR ${DOCBOOK45_RESOLVER_DIR} PARENT_SCOPE)
  set(DOCBOOK45_RESOLVER_DIR_FOUND ${DOCBOOK45_RESOLVER_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_RESOLVER)

function(DOCBOOK45_FIND_DOCBOOK)
  docbook45_find_tool(docbook-xml-4.5 DOCBOOK45_DOCBOOK_DIR "docbook-xml-4.5")
  set(DOCBOOK45_DOCBOOK_DIR ${DOCBOOK45_DOCBOOK_DIR} PARENT_SCOPE)
  set(DOCBOOK45_DOCBOOK_DIR_FOUND ${DOCBOOK45_DOCBOOK_DIR_FOUND} PARENT_SCOPE)
endfunction(DOCBOOK45_FIND_DOCBOOK)

function(DOCBOOK45_FIND_CATALOG_SOURCE)
  if (IS_DIRECTORY "${DOCBOOK45_SOURCE_DIR}/docbook45/catalog")
    if(NOT Docbook45_FIND_QUIETLY)
      message(STATUS "DOCBOOK45: 'catalog' at '${DOCBOOK45_SOURCE_DIR}/docbook45/catalog'")
    endif()
    set(DOCBOOK45_CATALOG_SOURCE_DIR "${DOCBOOK45_SOURCE_DIR}/docbook45/catalog" PARENT_SCOPE)
    set(DOCBOOK45_CATALOG_SOURCE_DIR_FOUND 1 PARENT_SCOPE)
  else()
    if(NOT Docbook45_FIND_QUIETLY)
      message(STATUS "DOCBOOK45: DocBook catalogs not found at '${DOCBOOK45_SOURCE_DIR}/docbook45/catalog'")
    endif()
    set(DOCBOOK45_CATALOG_SOURCE_DIR_FOUND 0 PARENT_SCOPE)
  endif()
endfunction(DOCBOOK45_FIND_CATALOG_SOURCE)

if(NOT DOCBOOK45_FOUND)
  docbook45_find_bin_dir()
  docbook45_find_saxon()
  docbook45_find_docbookxsl()
  docbook45_find_xslthl()
  docbook45_find_xerces()
  docbook45_find_resolver()
  docbook45_find_docbook()
  docbook45_find_catalog_source()

  if (NOT DOCBOOK45_BIN_DIR_FOUND OR
      NOT DOCBOOK45_SAXON_DIR_FOUND OR
      NOT DOCBOOK45_XSL_DIR_FOUND OR
      NOT DOCBOOK45_XSLTHL_DIR_FOUND OR
      NOT DOCBOOK45_XERCES_DIR_FOUND OR
      NOT DOCBOOK45_RESOLVER_DIR_FOUND OR
      NOT DOCBOOK45_DOCBOOK_DIR_FOUND OR
      NOT DOCBOOK45_CATALOG_SOURCE_DIR_FOUND)
    set(DOCBOOK45_FOUND 0)
    if(Docbook45_FIND_REQUIRED)
      message(FATAL_ERROR "Docbook45 tools not found. Set option DOCBOOK45_BIN_DIR to the folder where the tools are kept")
    endif()
  else()
    set(DOCBOOK45_FOUND 1)
  endif()
endif()
