<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="CraftedItem[@id='simple_pike_t2' or @id='fine_pike_t4' or @id='thamaskene_pike_t4' or @id='vlandia_pike_1_t5']/Pieces/Piece[@Type='Handle']/@scale_factor">
		<xsl:attribute name="scale_factor">
			<xsl:value-of select="225"/>
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="CraftedItem[@id='simple_pike_t2' or @id='fine_pike_t4' or @id='thamaskene_pike_t4' or @id='vlandia_pike_1_t5']/Pieces">
		<xsl:copy>
			<xsl:apply-templates/>
			<Piece id="spear_pommel_12" Type="Pommel" scale_factor="100" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>