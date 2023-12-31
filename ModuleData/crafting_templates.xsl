<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="CraftingTemplate[@id='Pike']/UsablePieces">
		<xsl:copy>
			<xsl:apply-templates/>
			<UsablePiece piece_id="spear_pommel_12" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>