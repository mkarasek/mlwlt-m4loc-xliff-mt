<?xml version="1.0" encoding="UTF-8" standalone="yes"?> 
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output omit-xml-declaration="yes" indent="no"/>

  <xsl:template match="node()">
			<xsl:choose>
				<xsl:when test="name(.) = ''"><span><xsl:value-of select="."/></span></xsl:when>
				<xsl:otherwise>
					<div class="eleContainer">
						<div class="eleBegin">&lt;<span class="eleName"><xsl:value-of select="name(.)"/></span><xsl:apply-templates select="@*"/>&gt;</div>
						<xsl:if test="node()"><div class="eleBody"><xsl:apply-templates select="node()"/></div></xsl:if>
						<div class="eleEnd">&lt;/<span class="eleName"><xsl:value-of select="name(.)"/></span>&gt;</div>
					</div>
				</xsl:otherwise>
			</xsl:choose>
	</xsl:template>

	<xsl:template match="@*"><xsl:text> </xsl:text><span class="eleAttrib"><xsl:value-of 
		select="name(.)"/></span>=&quot;<span class="eleAttrib"><xsl:value-of select="."/></span>&quot;</xsl:template>

</xsl:stylesheet>
