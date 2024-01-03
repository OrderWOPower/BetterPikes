﻿<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="WeaponDescription[@id='TwoHandedPolearm_Pike']/AvailablePieces">
		<xsl:copy>
			<xsl:apply-templates/>
			<AvailablePiece id="spear_pommel_12"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>