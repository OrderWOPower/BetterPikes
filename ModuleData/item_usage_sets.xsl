<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output omit-xml-declaration="yes"/>
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="item_usage_set[@id='polearm_pike']">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
			<idles>
				<idle action="act_idle_pike_1" is_left_stance="False" require_free_left_hand="False" />
			</idles>
			<movement_sets>
				<movement_set id="pike" />
			</movement_sets>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>