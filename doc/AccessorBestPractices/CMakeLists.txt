if(DOCBOOK45_FOUND)
  include(DocbookGen)

  set(DOCBOOK_FILES
    AccessorBaseBestPractices.xml)

  add_docbook45_target(
    FORMAT html
    OUTPUT AccessorBaseBestPractices.html
    INPUT ${DOCBOOK_FILES}
    IMAGEDIR images
    XSL docbook.xsl
    OLINK olinkdb.xml
    FILECOPY docbook.css)

endif(DOCBOOK45_FOUND)
