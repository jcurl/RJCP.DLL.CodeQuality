<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:import href="http://docbook.sourceforge.net/release/xsl/current/html/docbook.xsl" />
  <xsl:import href="html-highlight.xsl" />

  <xsl:param name="html.stylesheet" select="'docbook.css'" />
  <xsl:param name="section.autolabel" select="1" />
  <xsl:param name="chapter.autolabel" select="1" />
  <xsl:param name="appendix.autolabel" select="1" />
  <xsl:param name="section.label.includes.component.label" select="1" />
  <xsl:param name="highlight.source" select="1" />
  <xsl:param name="use.extensions" select="1" />
  <xsl:param name="linenumbering.extension" select="1" />
  <xsl:param name="admon.graphics" select="1" />
  <xsl:param name="callout.graphics" select="1" />
  <xsl:param name="graphicsize.extension" select="1" />
</xsl:stylesheet>
