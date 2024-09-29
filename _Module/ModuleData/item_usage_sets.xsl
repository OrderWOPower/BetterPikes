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
	<xsl:template match="item_usage_set[@id='polearm_pike']/usages/usage[@style='attack_up']">
		<usage
			style="attack_up"
			ready_action="act_ready_thrust_staff"
			quick_release_action="act_quick_release_thrust_staff"
			release_action="act_release_thrust_staff"
			quick_blocked_action="act_quick_blocked_thrust_staff"
			blocked_action="act_blocked_thrust_staff"
			is_mounted="False"
			require_free_left_hand="True"
			strike_type="thrust"
			begin_hand_position="0,-0.6,-0.1"
			begin_hand_rotation="0,-90"
			begin_arm_rotation="0,0"
			begin_arm_length="0"
			end_hand_position="0,1.3,-0.1"
			end_hand_rotation="0,-90"
			end_arm_rotation="0,0"
			end_arm_length="0" />
	</xsl:template>
</xsl:stylesheet>