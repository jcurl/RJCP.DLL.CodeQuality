if(DOCBOOK45_FOUND)
  include(DocbookGen)

  set(DOCBOOK_FILES
    AccessorBaseBestPractices.xml)

  add_docbook45_target(
    FORMAT pdf
    OUTPUT AccessorBaseBestPractices.pdf
    INPUT ${DOCBOOK_FILES}
    IMAGEDIR images
    XSL docbookfo.xsl
    OLINK olinkdb.xml
    FILECOPY docbook.css)

endif(DOCBOOK45_FOUND)
