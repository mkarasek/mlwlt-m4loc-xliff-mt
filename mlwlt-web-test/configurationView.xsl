<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <!--
  <html>
    
  <style>
    table {background-color: #FFFFFF; margin-left: 20px;}
    tr.head td {background-color:#80c0FF;}
    td {background-color: #E0E0E0; padding:4px;}
  </style>
  -->
  <body>
    <h3>Supported ITS 2.0 Categories</h3>
    <ul>
    <xsl:for-each select="/configuration/supported-its-categories/category">
      <li><xsl:value-of select="."/></li>
    </xsl:for-each>
    </ul>
    
    <h3>Web Service MT Support</h3>
    <p>
      This web service has implemented following MT language pairs with corresponding domains.
    </p>
    <xsl:for-each select="/configuration/domain-mapping/language-pair">
      <div style="margin-left:30px;">
        <h4><xsl:value-of select="@source-language"/> to <xsl:value-of select="@target-language"/></h4>
        <table class="DomainMap" style="width:500px;">
          <thead style="background-color: #D53F3F; color: #FFFFFF;">
            <tr>
              <td style="color: #FFFFFF;">Implemented MT Domain</td>
              <td style="color: #FFFFFF;">Mapped to ITS domain</td>
            </tr>
          </thead>
        <xsl:for-each select="domain">
          <xsl:choose>
            <xsl:when test="count(its-domains/its-domain) > 0">
              <xsl:for-each select="its-domains/its-domain">
                <tr><td><xsl:value-of select="../../@name"/><xsl:text> </xsl:text>
                <xsl:if test="../../@name = ../../../@default-domain"><span class="defaultEngine">DEFAULT</span></xsl:if></td><td><xsl:value-of select="."/></td></tr>
              </xsl:for-each>
            </xsl:when>
            <xsl:otherwise>
              <tr><td>
                <xsl:value-of select="@name"/><xsl:text> </xsl:text>
                <xsl:if test="@name = ../@default-domain"><span class="defaultEngine">DEFAULT</span></xsl:if>
              </td><td>-</td></tr>
            </xsl:otherwise>
          </xsl:choose>

        </xsl:for-each>
        </table>
      </div>
    </xsl:for-each>
        
  </body>
  <!-- </html> -->
</xsl:template>

</xsl:stylesheet>