set(SOURCE_FILES AccessorBaseBestPractices.xml)

add_document(
  TARGET Accessor_PDF
  OUTPUT_FILE AccessorBaseBestPractices.pdf
  SOURCES ${SOURCE_FILES}
  RESOURCE_FILES metadata.yaml rsrc/header.latex rsrc/syntaxhighlight.theme
  RESOURCE_DIRS images
  PANDOC_DIRECTIVES -t latex
                    -f docbook
                    --metadata-file metadata.yaml
                    -H header.latex
                    --highlight-style syntaxhighlight.theme
                    --toc
                    --number-sections
                    --pdf-engine pdflatex
                    --toc-depth 3
)
