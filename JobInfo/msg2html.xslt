<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" omit-xml-declaration="yes"/>
  
  <!--<xsl:output method="html" version="1.0" encoding="iso-8859-1" indent="yes" omit-xml-declaration="no"/>-->
  
  <!--https://html-color-codes.info/-->
  <xsl:template match="@* | node()">
    <xsl:element name="html">
      <xsl:element name="head">
        <xsl:element name="style">
          .size {
          white-space: nowrap; <!-- /* Отменяем перенос текста */-->
          overflow: hidden; <!-- /* Обрезаем содержимое */-->
          padding: 3px; <!-- /* Поля */-->
          text-overflow: ellipsis; <!-- /* Многоточие */-->
          border: 2px solid #C7CA0B; <!-- /* СИНЯЯ рамка */-->
          border-radius: 5px; <!-- /* Радиус скругления */-->
          background: #F5F6BC; <!-- /* Цвет фона */-->
          }
          .size:hover {
          background: #f0f0f0;<!-- /* Цвет фона */-->
          white-space: normal; <!--/* Обычный перенос текста */-->
          border: 1px solid #fff;<!-- /* Белая рамка */-->
          border-radius: 10px;<!-- /* Радиус скругления */-->
          }
        </xsl:element >
      </xsl:element >

      <xsl:element name="body">
        <xsl:element name="div">
          <xsl:attribute name="style">
            <xsl:text>style='float:left;margin:10px;'</xsl:text>
          </xsl:attribute>



          <xsl:apply-templates select="reply"/>
        <!--            <xsl:copy>
          <xsl:apply-templates select="@* | node()"/>
          
        </xsl:copy>-->
        <xsl:apply-templates select="text"/>
          <xsl:if test="not(text)">
            <xsl:value-of select="node()" />
          </xsl:if>
      </xsl:element >
      </xsl:element >
    </xsl:element >
  </xsl:template>

  <xsl:template match="text">
    <xsl:value-of select="node()" />
  </xsl:template>

  <xsl:template match="reply">
    <xsl:element name="div">
      <xsl:attribute name="class">
        <xsl:text>size</xsl:text>
      </xsl:attribute>

      <xsl:apply-templates select="text" />
      <!--        <xsl:copy>
            <xsl:apply-templates select="text"/>
        </xsl:copy>-->
    </xsl:element >
  </xsl:template>

</xsl:stylesheet>