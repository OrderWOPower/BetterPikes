<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="CraftedItem[@crafting_template='Pike']/Pieces/Piece[@Type='Handle']/@id">
		<xsl:attribute name="id">
			<xsl:value-of select="'spear_handle_7'"/>
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="CraftedItem[@crafting_template='Pike']/Pieces/Piece[@Type='Handle']/@scale_factor">
		<xsl:attribute name="scale_factor">
			<xsl:value-of select="225"/>
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="CraftedItem[@crafting_template='Pike']/Pieces">
		<xsl:copy>
			<xsl:apply-templates/>
			<Piece id="spear_pommel_12" Type="Pommel" scale_factor="100" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
