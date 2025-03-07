#region Assembly MOD_PFAutoBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// C:\Games\Tale of Immortal v1.2.107.259\ModExportData\3334313052 _b__color=#811328__size=25_自动战斗__size___color___b_\ModCode\dll\MOD_PFAutoBattle.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using UnityEngine;

namespace KrunchyAutoBattle
{
    public class GetPointsOnMovingArc
    {
        private Vector2 leftDown;

        private Vector2 rightUp;

        private float angle;

        private bool clockwise = true;

        private const float Deg2Rad = (float)Math.PI / 180f;

        private UnitCtrlBase lastCtrlBase;

        internal GetPointsOnMovingArc(Vector2 leftDown, Vector2 rightUp)
        {
            this.leftDown = leftDown;
            this.rightUp = rightUp;
        }

        internal Vector2 AroundTrajectory()
        {
            if (ModMain.playerIsHaloTarget)
            {
                return GetPlayerPosiType(ModMain.enemyHaloPosi);
            }
            return GetPlayerPosiType(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi);
        }

        private Vector2 GetPlayerPosiType(Vector2 playerPosi)
        {
            return (0f <= playerPosi.x && playerPosi.x <= rightUp.x / 3f) ? ((0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f) ? new Vector2(rightUp.x / 2f, leftDown.y) : ((!(rightUp.y / 3f < playerPosi.y) || !(playerPosi.y <= rightUp.y * 2f / 3f)) ? new Vector2(leftDown.x, rightUp.y / 2f) : new Vector2(leftDown.x, leftDown.y))) : ((rightUp.x / 3f < playerPosi.x && playerPosi.x <= rightUp.x * 2f / 3f) ? ((0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f) ? new Vector2(rightUp.x, leftDown.y) : ((!(rightUp.y / 3f < playerPosi.y) || !(playerPosi.y <= rightUp.y * 2f / 3f)) ? new Vector2(leftDown.x, rightUp.y) : new Vector2(rightUp.x / 2f, leftDown.y))) : ((0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f) ? new Vector2(rightUp.x, rightUp.y / 2f) : ((!(rightUp.y / 3f < playerPosi.y) || !(playerPosi.y <= rightUp.y * 2f / 3f)) ? new Vector2(rightUp.x / 2f, rightUp.y) : new Vector2(rightUp.x, rightUp.y))));
        }

        private int CtrlBaseNineSquareGridPosi(Vector2 ctrlBasePosi)
        {
            if (0f <= ctrlBasePosi.x && ctrlBasePosi.x <= rightUp.x / 3f)
            {
                if (0f <= ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y / 3f)
                {
                    return 1;
                }

                if (rightUp.y / 3f < ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y * 2f / 3f)
                {
                    return 4;
                }

                return 7;
            }

            if (rightUp.x / 3f < ctrlBasePosi.x && ctrlBasePosi.x <= rightUp.x * 2f / 3f)
            {
                if (0f <= ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y / 3f)
                {
                    return 2;
                }

                if (rightUp.y / 3f < ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y * 2f / 3f)
                {
                    return 5;
                }

                return 8;
            }

            if (0f <= ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y / 3f)
            {
                return 3;
            }

            if (rightUp.y / 3f < ctrlBasePosi.y && ctrlBasePosi.y <= rightUp.y * 2f / 3f)
            {
                return 6;
            }

            return 9;
        }

        private float GetStartAngle(Vector2 monstPosi, Vector2 playerPosi)
        {
            Vector2 vector = playerPosi - monstPosi;
            float num = Mathf.Atan2(vector.y, vector.x);
            if (0f <= num && (double)num <= Math.PI / 2.0)
            {
                clockwise = true;
            }
            else if (Math.PI / 2.0 < (double)num && (double)num <= Math.PI)
            {
                clockwise = false;
            }
            else if (-Math.PI / 2.0 < (double)num && num < 0f)
            {
                clockwise = false;
            }
            else
            {
                clockwise = true;
            }

            return num;
        }

        private Vector2 FarAway(Vector2 monstPosi, Vector2 playerPosi, float awayDis)
        {
            Vector2 result = OppositePosi(monstPosi, playerPosi, awayDis);
            if (result.x >= leftDown.x && result.x <= rightUp.x && result.y >= leftDown.y && result.y <= rightUp.y)
            {
                return result;
            }

            return RandomNineSquareGridPosi(playerPosi);
        }

        private Vector2 OppositePosi(Vector2 monstPosi, Vector2 playerPosi, float r)
        {
            Vector2 normalized = (playerPosi - monstPosi).normalized;
            float num = Vector2.Distance(monstPosi, playerPosi);
            float num2 = r - num;
            return playerPosi + normalized * num2;
        }

        private Vector2 RandomNineSquareGridPosi(Vector2 playerPosi)
        {
            bool flag = CommonTool.Random(0, 2) == 1;
            float x = leftDown.x;
            float x2 = rightUp.x / 3f;
            float x3 = rightUp.x * 2f / 3f;
            float x4 = rightUp.x;
            float y = leftDown.y;
            float y2 = rightUp.y / 3f;
            float y3 = rightUp.y * 2f / 3f;
            float y4 = rightUp.y;
            if (0f <= playerPosi.x && playerPosi.x <= rightUp.x / 3f)
            {
                if (0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f)
                {
                    if (flag)
                    {
                        return GetGridRandomPosi(new Vector2(x, y2), new Vector2(x2, y3));
                    }

                    return GetGridRandomPosi(new Vector2(x2, y), new Vector2(x3, y2));
                }

                if (rightUp.y / 3f < playerPosi.y && playerPosi.y <= rightUp.y * 2f / 3f)
                {
                    if (flag)
                    {
                        return GetGridRandomPosi(new Vector2(x, y3), new Vector2(x2, y4));
                    }

                    return GetGridRandomPosi(new Vector2(x, y), new Vector2(x2, y2));
                }

                if (flag)
                {
                    return GetGridRandomPosi(new Vector2(x2, y3), new Vector2(x3, y4));
                }

                return GetGridRandomPosi(new Vector2(x, y3), new Vector2(x2, y4));
            }

            if (rightUp.x / 3f < playerPosi.x && playerPosi.x <= rightUp.x * 2f / 3f)
            {
                if (0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f)
                {
                    if (flag)
                    {
                        return GetGridRandomPosi(new Vector2(x, y), new Vector2(x2, y2));
                    }

                    return GetGridRandomPosi(new Vector2(x3, y), new Vector2(x4, y2));
                }

                if (rightUp.y / 3f < playerPosi.y && playerPosi.y <= rightUp.y * 2f / 3f)
                {
                    if (flag)
                    {
                        return GetGridRandomPosi(new Vector2(x3, y2), new Vector2(x4, y3));
                    }

                    return GetGridRandomPosi(new Vector2(x, y2), new Vector2(x2, y3));
                }

                if (flag)
                {
                    return GetGridRandomPosi(new Vector2(x3, y3), new Vector2(x4, y4));
                }

                return GetGridRandomPosi(new Vector2(x, y3), new Vector2(x2, y4));
            }

            if (0f <= playerPosi.y && playerPosi.y <= rightUp.y / 3f)
            {
                if (flag)
                {
                    return GetGridRandomPosi(new Vector2(x2, y), new Vector2(x3, y2));
                }

                return GetGridRandomPosi(new Vector2(x3, y2), new Vector2(x4, y3));
            }

            if (rightUp.y / 3f < playerPosi.y && playerPosi.y <= rightUp.y * 2f / 3f)
            {
                if (flag)
                {
                    return GetGridRandomPosi(new Vector2(x3, y), new Vector2(x4, y2));
                }

                return GetGridRandomPosi(new Vector2(x3, y3), new Vector2(x4, y4));
            }

            if (flag)
            {
                return GetGridRandomPosi(new Vector2(x3, y2), new Vector2(x4, y3));
            }

            return GetGridRandomPosi(new Vector2(x2, y3), new Vector2(x3, y4));
        }

        private Vector2 GetGridRandomPosi(Vector2 ld, Vector2 ru)
        {
            float x = CommonTool.Random(ld.x, ru.x);
            float y = CommonTool.Random(ld.y, ru.y);
            return new Vector2(x, y);
        }

        internal Vector2 GetPointOnMovingArc(UnitCtrlBase ctrlBase, Vector2 monstPosi, Vector2 playerPosi, float r)
        {
            float num = Vector2.Distance(playerPosi, monstPosi);
            float num2 = r * 5f / 6f;
            if (num < num2)
            {
                lastCtrlBase = null;
                return FarAway(monstPosi, playerPosi, r);
            }

            if (lastCtrlBase == null)
            {
                angle = GetStartAngle(monstPosi, playerPosi);
                lastCtrlBase = ctrlBase;
            }
            else if (!lastCtrlBase.Equals(ctrlBase))
            {
                angle = GetStartAngle(monstPosi, playerPosi);
                lastCtrlBase = ctrlBase;
            }

            for (int i = 15; i <= 360; i += 15)
            {
                if (clockwise)
                {
                    angle -= (float)i * ((float)Math.PI / 180f);
                }
                else
                {
                    angle += (float)i * ((float)Math.PI / 180f);
                }

                float x = monstPosi.x + r * Mathf.Cos(angle);
                float y = monstPosi.y + r * Mathf.Sin(angle);
                Vector2 result = new Vector2(x, y);
                if (result.x >= leftDown.x && result.x <= rightUp.x && result.y >= leftDown.y && result.y <= rightUp.y && (IsSameDir(result.x - monstPosi.x, playerPosi.x - monstPosi.x) || IsSameDir(result.y - monstPosi.y, playerPosi.y - monstPosi.y)))
                {
                    return result;
                }
            }

            return AroundTrajectory();
        }

        private bool IsSameDir(float f1, float f2)
        {
            if (f1 > 0f && f2 > 0f)
            {
                return true;
            }

            if (f1 < 0f && f2 < 0f)
            {
                return true;
            }

            return false;
        }
    }
}
#if false // Decompilation log
'194' items in cache
------------------
Resolve: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll'
------------------
Resolve: 'Il2Cppmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Il2Cppmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\Il2Cppmscorlib.dll'
------------------
Resolve: 'Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\MelonLoader\Managed\Assembly-CSharp.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\UnityEngine.CoreModule.dll'
------------------
Resolve: '0Harmony, Version=2.9.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: '0Harmony, Version=2.9.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\0Harmony.dll'
------------------
Resolve: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll'
------------------
Resolve: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Found single assembly: 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Load from: 'C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.Core.dll'
------------------
Resolve: 'UnhollowerBaseLib, Version=0.4.18.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnhollowerBaseLib, Version=0.4.18.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\UnhollowerBaseLib.dll'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\UnityEngine.UI.dll'
------------------
Resolve: 'DOTween, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'DOTween, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\MelonLoader\Managed\DOTween.dll'
------------------
Resolve: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: 'C:\Games\Tale of Immortal v1.2.107.259\Mod\modFQA\代码编写教程\ModMain\dll\UnityEngine.AnimationModule.dll'
------------------
Resolve: 'Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed'
Found single assembly: 'Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed'
Load from: 'C:\Program Files\IIS\Microsoft Web Deploy V3\Newtonsoft.Json.dll'
#endif
