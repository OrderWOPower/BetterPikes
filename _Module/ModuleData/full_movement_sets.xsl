<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="full_movement_set[@id='pike']/movement_set[@movement_mode='walking']/@id">
		<xsl:attribute name="id">
			<xsl:value-of select="'walk_pike'"/>
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="full_movement_set[@id='pike']/movement_set[@movement_mode='running']/@id">
		<xsl:attribute name="id">
			<xsl:value-of select="'run_pike'"/>
		</xsl:attribute>
	</xsl:template>
</xsl:stylesheet>