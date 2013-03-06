<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
	<xsl:output encoding="UTF-8" indent="yes" />

	<xsl:template match="/">

		<configuration>

    <supported-its-categories>
      <xsl:for-each select ="//supported-its-categories/category">
        <category><xsl:value-of select="."/></category>        
      </xsl:for-each>
    </supported-its-categories>
		
		<domain-mapping>
			<xsl:for-each select="//language-pair">
				<language-pair>
					<xsl:attribute name="source-language"><xsl:value-of select="@source-language"/></xsl:attribute>
          <xsl:attribute name="target-language"><xsl:value-of select="@target-language"/></xsl:attribute>
          <xsl:attribute name="default-domain"><xsl:value-of select="@default-domain"/></xsl:attribute>
          <xsl:for-each select="domain">
						<domain>
							<xsl:attribute name="name"><xsl:value-of select="@name"/></xsl:attribute>
							<xsl:for-each select="its-domains">
								<its-domains>
								<xsl:for-each select="its-domain">
									<its-domain>
										<xsl:value-of select="."/>
									</its-domain>
								</xsl:for-each>
								</its-domains>
							</xsl:for-each>
						</domain>
					</xsl:for-each>
				</language-pair>
			</xsl:for-each>
		</domain-mapping>
	
		</configuration>
	</xsl:template>

</xsl:stylesheet>
