using System;
using HarmonyLib;

namespace KrunchyAutoBattle
{
	// Token: 0x02000008 RID: 8
	[HarmonyPatch(typeof(MartialTool), "IsMouseDownSkill")]
	public class PatchMartialTool
	{
		// Token: 0x06000063 RID: 99 RVA: 0x0000626C File Offset: 0x0000446C
		public static void Postfix(ref bool __result, SkillBase skillBase)
		{
			if (ModMain.IsEnableAutoSkills() && !SceneType.battle.battleMap.isPassRoom)
			{
				foreach (SkillAttack skillAttack in SceneType.battle.battleMap.playerUnitCtrl.skills)
				{
					if (skillBase.data.mainSkillID == skillAttack.data.mainSkillID)
					{
						__result = true;
						return;
					}
				}
			}
		}
	}
}
