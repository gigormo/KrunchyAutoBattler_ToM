﻿using System.Linq;
using System.Reflection;
using EBattleTypeData;
using EGameTypeData;
using EMapTypeData;
using HarmonyLib;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Events;
using UnityEngine.UI;
using MelonLoader;
using Harmony;
using DG.Tweening;
using UnhollowerBaseLib;
using Unity.Collections;
using System.Collections.Generic;
using static UnityEngine.UISystemProfilerApi;



namespace KrunchyAutoBattle
{
    public class ModMain
    {
        private static int excelMID = -1382759740;

        private EffectBase effectCloseCollider;

        private TimerCoroutine corUpdate;

        private static HarmonyLib.Harmony harmony;

        private Il2CppSystem.Action<ETypeData> callOpenUIEnd;

        private Il2CppSystem.Action<ETypeData> onBattleStart;

        private Il2CppSystem.Action<ETypeData> onBattleExit;

        private TimerCoroutine corUpdateInBattleStart;

        private UIBattleInfo uIBattleInfo;

        private TimerCoroutine corAutoPlayerMoveAndSkill;

        private TimerCoroutine corAutoMoveChecking;

        private GameObject goEffect;

        private int skillFlag;

        private int propGetDis = 1500;

        private int autoDropDis = 5000;

        private GetPointsOnMovingArc getPointsOnMovingArc;

        private Vector2 playerLasPosi;

        private CameraCtrl.OrthoSizeData curSize;

        private static bool enableAuto = false;

        private int moveFlag;

        private SettingData settingData;

        private BattleRoomNode firstRoomNode;

        private int zhenlongCount;

        public static DataStruct<TimeScaleType, DataStruct<float>> changeSpeed;

        public static float gameSpeed = 1f;

        //public static System.Collections.Generic.Dictionary<Vector2Int, BattleRoomNode> dictionary = new System.Collections.Generic.Dictionary<Vector2Int, BattleRoomNode>();
        //public static System.Collections.Generic.HashSet<Vector2Int> hashSet = new System.Collections.Generic.HashSet<Vector2Int>();

        internal static bool IsEnableAutoSkills()
        {
            return enableAuto;
        }

        public void Init()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }

            if (harmony == null)
            {
                harmony = new HarmonyLib.Harmony("KrunchyAutoBattle");
            }
            //System.Type HashSetType = typeof(System.Collections.Generic.HashSet<>);
            //MethodInfo privateMethod = HashSetType.GetMethod("PrivateMethod", new System.Type[] { typeof(int) });

            MelonLoader.MelonLogger.Msg($"BattleRoomNode type: {typeof(BattleRoomNode)}");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
            corUpdate = g.timer.Frame((Il2CppSystem.Action)OnUpdate, 1, loop: true);
            callOpenUIEnd = (Il2CppSystem.Action<ETypeData>)OnOpenUIEnd;
            g.events.On(EGameType.OpenUIEnd, callOpenUIEnd, 0);
            onBattleStart = (Il2CppSystem.Action<ETypeData>)OnBattleStart;
            g.events.On(EBattleType.BattleStart, onBattleStart, 0);
            onBattleExit = (Il2CppSystem.Action<ETypeData>)OnBattleExit;
            g.events.On(EBattleType.BattleExit, onBattleExit, 0);
            FixXuliLimitTime();
            LogTool.Info("已经加载! [Auto Battle] has been loaded.");
        }

        private void OnOpenUIEnd(ETypeData e)
        {
            if (uIBattleInfo == null)
            {
                return;
            }

            OpenUIEnd openUIEnd = e.Cast<OpenUIEnd>();
            if (openUIEnd.uiType.uiName == UIType.BattleExit.uiName)
            {
                FixUIBattleExit();
            }
            else if (enableAuto)
            {
                bool flag = false;
                if (openUIEnd.uiType.uiName == UIType.DramaDialogue.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaFortuitous.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaBigTexture.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaSprite.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaSpriteSub.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaLetter.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaDialogueHeresy.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaSutraLetter.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaHerdNPC.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaImmortalAncestral.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaPotmon.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaDialogueBigTexture.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaFortuitousHonor.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaDialogueHonor.uiName)
                {
                    flag = true;
                }
                else if (openUIEnd.uiType.uiName == UIType.DramaDialogueBless.uiName)
                {
                    flag = true;
                }

                if (flag)
                {
                    enableAuto = false;
                    ShowStartButton();
                    StopAutoBattle();
                    UITipItem.AddTip(GameTool.LS("AutoBattleText_1"), 2f);
                }
            }
        }

        private void FixUIBattleExit()
        {
            UIBattleExit uI = g.ui.GetUI<UIBattleExit>(UIType.BattleExit);
            if (!(uI != null))
            {
                return;
            }

            bool originAuto = enableAuto;
            enableAuto = false;
            ShowStartButton();
            StopAutoBattle();
            uI.onCloseCall += (Il2CppSystem.Action)delegate
            {
                if (originAuto)
                {
                    enableAuto = true;
                    ShowIsAutoFightingIcon();
                    StartAutoBattle();
                }
            };
            GameObject gameObject = UnityEngine.Object.Instantiate(g.res.Load<GameObject>("UI/AutoSetting"), uI.transform);
            gameObject.name = "AutoSetting";
            gameObject.transform.localPosition += new Vector3(605f, -6f, 0f);
            gameObject.transform.Find("Title").GetComponent<Text>().text = GameTool.LS("AutoBattleText_25");
            RectTransform content = gameObject.transform.Find("ScrollView").gameObject.GetComponent<ScrollRect>().content;
            GameObject gameObject2 = content.Find("ToggleAutoStart").gameObject;
            gameObject2.transform.SetParent(content, worldPositionStays: false);
            gameObject2.GetComponent<Toggle>().isOn = settingData.AutoStart;
            gameObject2.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_2");
            System.Action<bool> action = delegate (bool isOn)
            {
                settingData.AutoStart = isOn;
            };
            gameObject2.GetComponent<Toggle>().onValueChanged.AddListener(action);
            if (g.data.globle.gameSetting.languageType == LanguageType.English)
            {
                gameObject2.GetComponentInChildren<Text>().transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(-20f, 0f);
                gameObject2.transform.Find("Background").GetComponent<RectTransform>().anchoredPosition += new Vector2(20f, 0f);
            }

            Toggle component = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component.isOn = settingData.AutoMove;
            component.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_3");
            System.Action<bool> action2 = delegate (bool isOn)
            {
                settingData.AutoMove = isOn;
            };
            component.onValueChanged.AddListener(action2);
            Toggle component2 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component2.isOn = settingData.AutoLeft;
            component2.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_4");
            System.Action<bool> action3 = delegate (bool isOn)
            {
                settingData.AutoLeft = isOn;
            };
            component2.onValueChanged.AddListener(action3);
            component2.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("AutoBattleText_26"));
            Toggle component3 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component3.isOn = settingData.AutoRight;
            component3.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_5");
            System.Action<bool> action4 = delegate (bool isOn)
            {
                settingData.AutoRight = isOn;
            };
            component3.onValueChanged.AddListener(action4);
            Toggle component4 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component4.isOn = settingData.AutoStep;
            component4.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_6");
            System.Action<bool> action5 = delegate (bool isOn)
            {
                settingData.AutoStep = isOn;
            };
            component4.onValueChanged.AddListener(action5);
            Toggle component5 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component5.isOn = settingData.AutoUltimate;
            component5.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_7");
            System.Action<bool> action6 = delegate (bool isOn)
            {
                settingData.AutoUltimate = isOn;
            };
            component5.onValueChanged.AddListener(action6);
            Toggle component6 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component6.isOn = settingData.AutoRule;
            component6.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_8");
            System.Action<bool> action7 = delegate (bool isOn)
            {
                settingData.AutoRule = isOn;
            };
            component6.onValueChanged.AddListener(action7);
            Toggle component7 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component7.isOn = settingData.AutoImmortalSkill;
            component7.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_9");
            System.Action<bool> action8 = delegate (bool isOn)
            {
                settingData.AutoImmortalSkill = isOn;
            };
            component7.onValueChanged.AddListener(action8);
            Toggle component8 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component8.isOn = settingData.AutoHealthBackProp;
            component8.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_10");
            System.Action<bool> action9 = delegate (bool isOn)
            {
                settingData.AutoHealthBackProp = isOn;
            };
            component8.onValueChanged.AddListener(action9);
            GameObject Go_SliderHealthBack = gameObject.transform.Find("GoSliderOut").gameObject;
            Go_SliderHealthBack.transform.SetParent(content, worldPositionStays: false);
            Go_SliderHealthBack.GetComponentInChildren<Slider>().minValue = 0f;
            Go_SliderHealthBack.GetComponentInChildren<Slider>().maxValue = 100f;
            Go_SliderHealthBack.GetComponentInChildren<Slider>().value = settingData.HealthLowerRate;
            Go_SliderHealthBack.transform.Find("TextDesc").gameObject.GetComponent<Text>().text = GameTool.LS("AutoBattleText_11");
            Go_SliderHealthBack.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.HealthLowerRate.ToString();
            System.Action<float> action10 = delegate (float value)
            {
                settingData.HealthLowerRate = (int)value;
                Go_SliderHealthBack.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.HealthLowerRate.ToString();
            };
            Go_SliderHealthBack.GetComponentInChildren<Slider>().onValueChanged.AddListener(action10);
            Toggle component9 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component9.isOn = settingData.AutoBattleProp;
            component9.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_12");
            System.Action<bool> action11 = delegate (bool isOn)
            {
                settingData.AutoBattleProp = isOn;
            };
            component9.onValueChanged.AddListener(action11);
            component9.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("AutoBattleText_13"));
            Toggle component10 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component10.isOn = settingData.AutoPiscesPendant;
            component10.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_14");
            System.Action<bool> action12 = delegate (bool isOn)
            {
                settingData.AutoPiscesPendant = isOn;
            };
            component10.onValueChanged.AddListener(action12);
            component10.gameObject.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("AutoBattleText_15"));
            Toggle component11 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component11.isOn = settingData.AutoGodEyeSkill;
            component11.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_16");
            System.Action<bool> action13 = delegate (bool isOn)
            {
                settingData.AutoGodEyeSkill = isOn;
            };
            component11.onValueChanged.AddListener(action13);
            Toggle component12 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component12.isOn = settingData.AutoDevilDemonSkill;
            component12.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_17");
            System.Action<bool> action14 = delegate (bool isOn)
            {
                settingData.AutoDevilDemonSkill = isOn;
            };
            component12.onValueChanged.AddListener(action14);
            Toggle component13 = UnityEngine.Object.Instantiate(gameObject2, content).GetComponent<Toggle>();
            component13.isOn = settingData.AutoDevilDemonAbsorb;
            component13.GetComponentInChildren<Text>().text = GameTool.LS("AutoBattleText_18");
            System.Action<bool> action15 = delegate (bool isOn)
            {
                settingData.AutoDevilDemonAbsorb = isOn;
            };
            component13.onValueChanged.AddListener(action15);
            GameObject Go_SliderOut = UnityEngine.Object.Instantiate(Go_SliderHealthBack, content, worldPositionStays: false);
            Go_SliderOut.GetComponentInChildren<Slider>().minValue = 50f;
            Go_SliderOut.GetComponentInChildren<Slider>().maxValue = 100f;
            Go_SliderOut.GetComponentInChildren<Slider>().value = settingData.DemonAbsorbAtRate;
            Go_SliderOut.transform.Find("TextDesc").gameObject.GetComponent<Text>().text = GameTool.LS("AutoBattleText_19");
            Go_SliderOut.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.DemonAbsorbAtRate.ToString();
            System.Action<float> action16 = delegate (float value)
            {
                settingData.DemonAbsorbAtRate = (int)value;
                Go_SliderOut.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.DemonAbsorbAtRate.ToString();
            };
            Go_SliderOut.GetComponentInChildren<Slider>().onValueChanged.AddListener(action16);
            GameObject Go_SliderTimeScale = UnityEngine.Object.Instantiate(Go_SliderHealthBack, content, worldPositionStays: false);
            Go_SliderTimeScale.GetComponentInChildren<Slider>().minValue = 0.1f;
            Go_SliderTimeScale.GetComponentInChildren<Slider>().maxValue = 5f;
            Go_SliderTimeScale.GetComponentInChildren<Slider>().value = settingData.TimeScale;
            Go_SliderTimeScale.transform.Find("TextDesc").gameObject.GetComponent<Text>().text = GameTool.LS("AutoBattleText_20");
            Go_SliderTimeScale.transform.Find("TextDesc").gameObject.GetComponent<Text>().fontSize = 16;
            Go_SliderTimeScale.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.TimeScale.ToString();
            System.Action<float> action17 = delegate (float value)
            {
                settingData.TimeScale = value;
                Go_SliderTimeScale.transform.Find("TextDesc/TextRate").gameObject.GetComponent<Text>().text = settingData.TimeScale.ToString();
            };
            Go_SliderTimeScale.GetComponentInChildren<Slider>().onValueChanged.AddListener(action17);
            GameObject go_ad = gameObject.transform.Find("ImageAd").gameObject;
            go_ad.SetActive(value: false);
            Button component14 = go_ad.transform.Find("IconClose").GetComponent<Button>();
            component14.onClick.RemoveAllListeners();
            System.Action action18 = delegate
            {
                go_ad.SetActive(value: false);
            };
            component14.onClick.AddListener(action18);
            Button component15 = gameObject.transform.Find("AdIconButton").GetComponent<Button>();
            component15.onClick.RemoveAllListeners();
            System.Action action19 = delegate
            {
                go_ad.SetActive(value: true);
            };
            component15.onClick.AddListener(action19);
            GameObject gameObject3 = new GameObject();
            gameObject3.AddComponent<RectTransform>().sizeDelta = new Vector2(360f, 50f);
            gameObject3.transform.SetParent(content, worldPositionStays: false);
            Button button = UnityEngine.Object.Instantiate(component15, gameObject3.transform, worldPositionStays: false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action19);
        }

        private void OnBattleStart(ETypeData e)
        {
            settingData = SettingData.GetSaveData();
            if (corUpdateInBattleStart != null)
            {
                g.timer.Stop(corUpdateInBattleStart);
            }

            if (g.world.battle.data.dungeonBaseItem.id == 15 || g.world.battle.data.dungeonBaseItem.id == 126)
            {
                return;
            }

            corUpdateInBattleStart = g.timer.Frame((System.Action)delegate
            {
                try
                {
                    BattleStartCorOnUpdate();
                }
                catch (System.Exception ex)
                {
                    UITipItem.AddTip($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 「自动战斗」疑似异常3，请反馈给作者", 3f);
                    LogTool.Error($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 疑似异常3，请反馈给作者 ");
                    LogTool.Error(ex.Message);
                }
            }, 1, loop: true);
        }

        private void OnBattleExit(ETypeData e)
        {
            if (corUpdateInBattleStart != null)
            {
                g.timer.Stop(corUpdateInBattleStart);
                corUpdateInBattleStart = null;
            }

            StopAutoBattle();
            uIBattleInfo = null;
            settingData.SaveData();
        }

        private void BattleStartCorOnUpdate()
        {
            if (!(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x > 0f))
            {
                return;
            }

            g.timer.Stop(corUpdateInBattleStart);
            firstRoomNode = SceneType.battle.battleMap.roomNode;
            float x = 1f;
            float x2 = SceneType.battle.battleMap.roomCenterPosi.x * 2f - 1f;
            float y = 1f;
            float y2 = SceneType.battle.battleMap.roomCenterPosi.y * 2f - 1f;
            getPointsOnMovingArc = new GetPointsOnMovingArc(new Vector2(x, y), new Vector2(x2, y2));
            uIBattleInfo = g.ui.GetUI<UIBattleInfo>(UIType.BattleInfo);
            if (uIBattleInfo != null)
            {
                ShowIsAutoFightingIcon();
                ShowStartButton();
            }

            SceneType.battle.battleMap.onBattlePassRoomOpenEffectCall += (Il2CppSystem.Action)(System.Action)delegate
            {
                if (enableAuto && settingData.AutoMove)
                {
                    OnPsaaRoom();
                    TimerCoroutine corCheck = null;
                    Vector3 lasPosi = SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
                    corCheck = SceneType.battle.timer.Time((System.Action)delegate
                    {
                        if (!SceneType.battle.battleMap.playerUnitCtrl.move.isMove)
                        {
                            if ((double)Vector2.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, lasPosi) < 0.1)
                            {
                                OnPsaaRoom();
                            }
                            else
                            {
                                SceneType.battle.timer.Stop(corCheck);
                            }
                        }
                        else
                        {
                            SceneType.battle.timer.Stop(corCheck);
                        }
                    }, 2f, loop: true);
                }
            };
            SceneType.battle.battleMap.onIntoRoomEnd += (Il2CppSystem.Action)(System.Action)delegate
            {
                if (enableAuto)
                {
                    StartAutoSkills();
                    if (settingData.AutoMove)
                    {
                        StartAutoMoveNotYanwu();
                    }
                }
            };
            if (SceneType.battle.battleMap.playerUnitCtrl.step != null)
            {
                SceneType.battle.battleMap.playerUnitCtrl.step.onCreateEndCall += (Il2CppSystem.Action)(System.Action)delegate
                {
                    if (settingData.AutoMove && enableAuto && SceneType.battle.battleMap.playerUnitCtrl.step.data.stepBaseItem.id == 510111)
                    {
                        StartAutoMoveNotYanwu();
                    }

                    DelEffectCloseCollider();
                    AddEffectCloseCollider();
                };
            }

            SceneType.battle.battleMap.playerUnitCtrl.onDieCall += (Il2CppSystem.Action)(System.Action)delegate
            {
                StopAutoBattle();
            };
            SceneType.battle.battleMap.onBattleEndCall += (Il2CppSystem.Action<bool>)(System.Action<bool>)delegate
            {
                StopAutoBattle();
                if (g.world.battle.data.dungeonBaseItem.type == 55)
                {
                    AddEffectCloseCollider();
                    DaojieMoveCenter();
                }
            };
            if (g.world.battle.data.dungeonBaseItem.type == 65 || g.world.battle.data.dungeonBaseItem.type == 56 || !settingData.AutoStart)
            {
                return;
            }

            TimerCoroutine autoStartCor = null;
            int count = 0;
            System.Action action = delegate
            {
                if (count >= 1)
                {
                    uIBattleInfo.uiStartBattle.btnStart.onClick.Invoke();
                    if (!uIBattleInfo.uiStartBattle.goGroupRoot.activeSelf)
                    {
                        enableAuto = true;
                        ShowIsAutoFightingIcon();
                        StartAutoBattle();
                    }

                    SceneType.battle.timer.Stop(autoStartCor);
                }
                else
                {
                    count++;
                }
            };
            autoStartCor = SceneType.battle.timer.Time(action, 0.3f, loop: true);
        }

        private void OnPsaaRoom()
        {
            if (g.world.battle.data.dungeonBaseItem.type == 55)
            {
                StopAutoMoveNotYanwu();
                DaojieMoveCenter();
            }
            else
            {
                StopAutoMoveNotYanwu();
                MoveToPassLevelEffect();
            }
        }

        private void ShowIsAutoFightingIcon()
        {
            try
            {
                uIBattleInfo.uiInfo.goInfoRoot.transform.Find("StartAutoBattle")?.gameObject.SetActive(value: false);
                if (uIBattleInfo.uiInfo.goInfoRoot.transform.parent.Find("AutoBattleInfo") == null)
                {
                    GameObject go = UnityEngine.Object.Instantiate(g.res.Load<GameObject>("UI/AutoBattleInfo"), uIBattleInfo.uiInfo.goInfoRoot.transform.parent);
                    go.name = "AutoBattleInfo";
                    go.transform.localPosition += new Vector3(-17f, -355f, 0f);
                    go.transform.Find("Text").GetComponent<Text>().text = GameTool.LS("AutoBattleText_22");
                    go.GetComponentInChildren<Button>().onClick.AddListener((System.Action)delegate
                    {
                        go.GetComponentInChildren<Button>().interactable = false;
                        go.GetComponentInChildren<Button>().interactable = true;
                        enableAuto = false;
                        ShowStartButton();
                        StopAutoBattle();
                    });
                }
                else
                {
                    uIBattleInfo.uiInfo.goInfoRoot.transform.parent.Find("AutoBattleInfo")?.gameObject.SetActive(value: true);
                }
            }
            catch (System.Exception ex)
            {
                UITipItem.AddTip($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 「自动战斗」疑似异常1，请反馈给作者", 3f);
                LogTool.Error($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 疑似异常1，请反馈给作者 ");
                LogTool.Error(ex.Message);
            }
        }

        private void ShowStartButton()
        {
            GameObject go;
            try
            {
                uIBattleInfo.uiInfo.goInfoRoot.transform.parent.Find("AutoBattleInfo")?.gameObject.SetActive(value: false);
                if (uIBattleInfo.uiInfo.goInfoRoot.transform.Find("StartAutoBattle") == null)
                {
                    go = UnityEngine.Object.Instantiate(g.res.Load<GameObject>("UI/StartAutoBattle"), uIBattleInfo.uiInfo.goInfoRoot.transform);
                    go.name = "StartAutoBattle";
                    go.transform.localPosition += new Vector3(-17f, 110f, 0f);
                    go.GetComponentInChildren<Button>().onClick.AddListener((System.Action)delegate
                    {
                        ClickAction();
                    });
                    go.AddComponent<UISkyTipEffect>().InitData(GameTool.LS("AutoBattleText_21"));
                }
                else
                {
                    uIBattleInfo.uiInfo.goInfoRoot.transform.Find("StartAutoBattle")?.gameObject.SetActive(value: true);
                }
            }
            catch (System.Exception ex)
            {
                UITipItem.AddTip($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 「自动战斗」疑似异常2，请反馈给作者", 3f);
                LogTool.Error($"副本id={g.world.battle.data.dungeonBaseItem.id} 类型={g.world.battle.data.dungeonBaseItem.type} 疑似异常2，请反馈给作者");
                LogTool.Error(ex.Message);
            }

            void ClickAction()
            {
                go.GetComponentInChildren<Button>().interactable = false;
                go.GetComponentInChildren<Button>().interactable = true;
                if (!(g.ui.GetUI<UIBattleExit>(UIType.BattleExit) != null))
                {
                    if (SceneType.battle.battleMap.isStartBattle && SceneType.battle.battleMap.isActiveBattle)
                    {
                        enableAuto = true;
                        ShowIsAutoFightingIcon();
                        StartAutoBattle();
                    }
                    else
                    {
                        TimerCoroutine autoStartCor = null;
                        System.Action action = delegate
                        {
                            SceneType.battle.timer.Stop(autoStartCor);
                            uIBattleInfo.uiStartBattle.btnStart.onClick.Invoke();
                            if (!uIBattleInfo.uiStartBattle.goGroupRoot.activeSelf)
                            {
                                enableAuto = true;
                                ShowIsAutoFightingIcon();
                                StartAutoBattle();
                            }
                        };
                        autoStartCor = SceneType.battle.timer.Frame(action, 1);
                    }
                }
            }
        }

        private void StartAutoBattle()
        {
            playerLasPosi = new Vector2(0f, 0f);
            enableAuto = true;
            ChangeGameSpeed();
            StartAutoMove();
            StartAutoSkills();
        }

        private void StartAutoBattleNotYanwu()
        {
            playerLasPosi = new Vector2(0f, 0f);
            enableAuto = true;
            StartAutoMoveNotYanwu();
            StartAutoSkills();
        }

        private void StopAutoBattle()
        {
            enableAuto = false;
            ResetGameSpeed();
            StopAutoMove();
            StopAutoSkills();
        }

        private void StopAutoBattleNotYanwu()
        {
            enableAuto = false;
            StopAutoMoveNotYanwu();
            StopAutoSkills();
        }

        private void ChangeGameSpeed()
        {
            if (Mathf.Approximately(settingData.TimeScale, 1f))
            {
                changeSpeed = null;
                GameTool.ResetTimeScale();
                return;
            }

            if (changeSpeed != null && GameTool.timeScales.Contains(changeSpeed))
            {
                GameTool.timeScales.Remove(changeSpeed);
            }

            changeSpeed = new DataStruct<TimeScaleType, DataStruct<float>>(TimeScaleType.SlowTime, new DataStruct<float>(settingData.TimeScale));
            GameTool.timeScales.Add(changeSpeed);
            GameTool.SetTimeScale(GameTool.GetMinTimeScale());
        }

        private void ResetGameSpeed()
        {
            if (changeSpeed != null && GameTool.timeScales.Contains(changeSpeed))
            {
                GameTool.timeScales.Remove(changeSpeed);
            }

            GameTool.SetTimeScale(GameTool.GetMinTimeScale());
        }

        private void StartAutoMove()
        {
            if (corAutoMoveChecking != null)
            {
                corAutoMoveChecking.Stop();
                corAutoMoveChecking = null;
            }

            if (!settingData.AutoMove || g.world.battle.data.dungeonBaseItem.type == 65)
            {
                return;
            }

            FootYanwu();
            AddEffectCloseCollider();
            propGetDis = SceneType.battle.battleMap.playerUnitCtrl.dropDis.value;
            SceneType.battle.battleMap.playerUnitCtrl.dropDis = new DynInt(autoDropDis);
            if (SceneType.battle.battleMap.isPassRoom)
            {
                if (g.world.battle.data.dungeonBaseItem.type == 55)
                {
                    DaojieMoveCenter();
                }
                else
                {
                    MoveToPassLevelEffect();
                }

                return;
            }

            SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            moveFlag = 0;
            RandomMove();
            corAutoMoveChecking = SceneType.battle.timer.Time((System.Action)delegate
            {
                if (!SceneType.battle.battleMap.playerUnitCtrl.move.isMove && (double)Vector2.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, playerLasPosi) < 0.1)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
                    moveFlag++;
                    RandomMove();
                }

                playerLasPosi = SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
            }, 1f, loop: true);
        }

        private void StopAutoMove()
        {
            if (corAutoMoveChecking != null)
            {
                corAutoMoveChecking.Stop();
                corAutoMoveChecking = null;
            }

            DelEffectCloseCollider();
            moveFlag = 1;
            if (SceneType.battle != null && SceneType.battle.battleMap != null && SceneType.battle.battleMap.playerUnitCtrl != null)
            {
                SceneType.battle.battleMap.playerUnitCtrl.dropDis = new DynInt(propGetDis);
                SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            }

            DestroyYanwu();
        }

        private void StartAutoMoveNotYanwu()
        {
            if (corAutoMoveChecking != null)
            {
                corAutoMoveChecking.Stop();
                corAutoMoveChecking = null;
            }

            if (!settingData.AutoMove || g.world.battle.data.dungeonBaseItem.type == 65)
            {
                return;
            }

            propGetDis = SceneType.battle.battleMap.playerUnitCtrl.dropDis.value;
            SceneType.battle.battleMap.playerUnitCtrl.dropDis = new DynInt(autoDropDis);
            if (SceneType.battle.battleMap.isPassRoom)
            {
                if (g.world.battle.data.dungeonBaseItem.type == 55)
                {
                    DaojieMoveCenter();
                }
                else
                {
                    MoveToPassLevelEffect();
                }

                return;
            }

            SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            moveFlag = 0;
            RandomMove();
            corAutoMoveChecking = SceneType.battle.timer.Time((System.Action)delegate
            {
                if (!SceneType.battle.battleMap.playerUnitCtrl.move.isMove && (double)Vector2.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, playerLasPosi) < 0.1)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
                    moveFlag++;
                    RandomMove();
                }

                playerLasPosi = SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
            }, 1f, loop: true);
        }

        private void StopAutoMoveNotYanwu()
        {
            if (corAutoMoveChecking != null)
            {
                corAutoMoveChecking.Stop();
                corAutoMoveChecking = null;
            }

            moveFlag = 1;
            if (SceneType.battle != null && SceneType.battle.battleMap != null && SceneType.battle.battleMap.playerUnitCtrl != null)
            {
                SceneType.battle.battleMap.playerUnitCtrl.dropDis = new DynInt(propGetDis);
                SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            }
        }

        private void FootYanwu()
        {
            if (g.world.playerUnit.data.dynUnitData.GetGrade() >= 9 || goEffect != null)
            {
                return;
            }

            SceneType.battle.effect.Create("Effect/Battle/Unit/yuhuayun", SceneType.battle.battleMap.playerUnitCtrl.posiDown.position, -1f, (System.Action<GameObject>)delegate (GameObject go)
            {
                if (go.GetComponent<EffectUnitNodeCtrl>() == null)
                {
                    go.AddComponent<EffectUnitNodeCtrl>();
                }

                go.GetComponent<EffectUnitNodeCtrl>().nodeType = EffectUnitNodeCtrl.UnitNodeType.Down;
                go.GetComponent<EffectUnitNodeCtrl>().isTarget = true;
                EffectUnitNodeCtrl.InitNode(go, SceneType.battle.battleMap.playerUnitCtrl);
                goEffect = go;
            });
        }

        private void DestroyYanwu()
        {
            if (g.world.playerUnit.data.dynUnitData.GetGrade() < 9 && goEffect != null)
            {
                goEffect.GetComponent<EffectUnitNodeCtrl>()?.DelOneEffect();
                UnityEngine.Object.Destroy(goEffect);
            }
        }

        private void AddEffectCloseCollider()
        {
            ConfBattleEffectItem item = g.conf.battleEffect.GetItem(excelMID + 1);
            effectCloseCollider = SceneType.battle.battleMap.playerUnitCtrl.AddEffect(item, SceneType.battle.battleMap.playerUnitCtrl, new SkillCreateData());
        }

        private void DelEffectCloseCollider()
        {
            if (effectCloseCollider != null)
            {
                SceneType.battle?.battleMap?.playerUnitCtrl?.DelEffect(effectCloseCollider);
            }
        }

        private void StartAutoSkills()
        {
            if (corAutoPlayerMoveAndSkill != null)
            {
                corAutoPlayerMoveAndSkill.Stop();
                corAutoPlayerMoveAndSkill = null;
            }

            zhenlongCount = 0;
            corAutoPlayerMoveAndSkill = SceneType.battle.timer.Time((System.Action)delegate
            {
                AutoBattleSkills();
            }, 0.2f, loop: true);
        }

        private void StopAutoSkills()
        {
            if (corAutoPlayerMoveAndSkill != null)
            {
                corAutoPlayerMoveAndSkill.Stop();
                corAutoPlayerMoveAndSkill = null;
            }
        }

        private void FixXuliLimitTime()
        {
            for (int i = 97001; i <= 97005; i++)
            {
                g.conf.battleShot97.GetItem(i).limitTime = g.conf.battleShot97.GetItem(i).validTimeMax;
            }
        }

        private void RecoverXuliLimitTime()
        {
            for (int i = 97001; i <= 97005; i++)
            {
                g.conf.battleShot97.GetItem(i).limitTime = "10000";
            }
        }

        private void UpdateCameraHeight(float cameraSize)
        {
            if (curSize != null)
            {
                SceneType.battle.cameraCtrl.orthoSizes.Remove(curSize);
            }

            curSize = new CameraCtrl.OrthoSizeData
            {
                orthoSize = cameraSize
            };
            SceneType.battle.cameraCtrl.orthoSizes.Add(curSize);
            SceneType.battle.camera.orthographicSize = SceneType.battle.cameraCtrl.orthoSize;
            SceneType.battle.battleMap.playerUnitCtrl.goMask.transform.localScale = Vector3.one * cameraSize / 5.4f * 1.4f;
        }

        private void AutoBattleSkills()
        {
            if (settingData.AutoMove && enableAuto)
            {
                if (goEffect == null && g.world.battle.data.dungeonBaseItem.type != 65)
                {
                    FootYanwu();
                }
            }
            else
            {
                DestroyYanwu();
            }

            if (!enableAuto || SceneType.battle.battleMap.isPassRoom)
            {
                return;
            }

            if (settingData.AutoPiscesPendant && SceneType.battle.battleMap.playerUnitCtrl.data.hp <= SceneType.battle.battleMap.playerUnitCtrl.data.maxHP.value / 3 && SceneType.battle.battleMap.playerUnitCtrl.piscesSkillBase != null && SceneType.battle.battleMap.playerUnitCtrl.piscesSkillBase.IsCreate(isTip: false))
            {
                SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UsePiscesPendant();
            }

            if (settingData.AutoRight)
            {
                bool flag = true;
                if (SceneType.battle.battleMap.playerUnitCtrl.data.skillsData != null && SceneType.battle.battleMap.playerUnitCtrl.data.skillsData.Count > 0)
                {
                    foreach (DataProps.PropsSkillData skillsDatum in SceneType.battle.battleMap.playerUnitCtrl.data.skillsData)
                    {
                        if (skillsDatum.skillBaseItem.id == 23112)
                        {
                            flag = ((zhenlongCount % 2 == 0) ? true : false);
                            break;
                        }
                    }

                    if (zhenlongCount == 2147483646)
                    {
                        zhenlongCount = 0;
                    }

                    zhenlongCount++;
                }

                if (flag)
                {
                    if (settingData.AutoStep)
                    {
                        if (SceneType.battle.battleMap.playerUnitCtrl.step != null)
                        {
                            if (!SceneType.battle.battleMap.playerUnitCtrl.step.IsCreate())
                            {
                                SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseSkill(MartialType.SkillRight);
                            }
                        }
                        else
                        {
                            SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseSkill(MartialType.SkillRight);
                        }
                    }
                    else
                    {
                        SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseSkill(MartialType.SkillRight);
                    }
                }
            }

            if (skillFlag == 0)
            {
                skillFlag = 1;
                if (settingData.AutoUltimate && SceneType.battle.battleMap.playerUnitCtrl.ultimate != null && SceneType.battle.battleMap.playerUnitCtrl.ultimate.IsCreate(isEffect: false, null, isTip: false))
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseUltimate();
                    return;
                }
            }

            if (skillFlag == 1)
            {
                skillFlag = 2;
                if (settingData.AutoRule && SceneType.battle.battleMap.playerUnitCtrl.fieldSkill != null && SceneType.battle.battleMap.playerUnitCtrl.fieldSkill.IsCreate(isTip: false))
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseRule();
                    return;
                }
            }

            if (skillFlag == 2)
            {
                skillFlag = 3;
                if (settingData.AutoGodEyeSkill && SceneType.battle.battleMap.playerUnitCtrl.eyeSkillBase != null && SceneType.battle.battleMap.playerUnitCtrl.eyeSkillBase.IsCreate(isTip: false))
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseGodEye();
                    return;
                }

                if (settingData.AutoDevilDemonSkill && SceneType.battle.battleMap.playerUnitCtrl.devilDemonSkill != null && SceneType.battle.battleMap.playerUnitCtrl.devilDemonSkill.IsCreate(isTip: false))
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseDevilDemon();
                    return;
                }
            }

            if (skillFlag == 3)
            {
                skillFlag = 4;
                if (settingData.AutoImmortalSkill && SceneType.battle.battleMap.playerUnitCtrl.immortalSkill != null)
                {
                    Il2CppSystem.Collections.Generic.List<ImmortalSkillBase>.Enumerator enumerator2 = SceneType.battle.battleMap.playerUnitCtrl.immortalSkill.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        ImmortalSkillBase current = enumerator2.Current;
                        if (current.IsCreate(isTip: false))
                        {
                            StopAutoBattleNotYanwu();
                            UnitCtrlBase nearUnit = SceneType.battle.unit.GetNearUnit(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, UnitType.Monst);
                            Vector2 vector;
                            Vector2 dire;
                            if (nearUnit != null)
                            {
                                vector = nearUnit.posiCenter.position;
                                dire = vector - (Vector2)SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
                            }
                            else
                            {
                                vector = SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
                                dire = new Vector2(0f, 0.5f);
                            }

                            current.Create(vector, dire, (System.Action)delegate
                            {
                                StartAutoBattleNotYanwu();
                            });
                            return;
                        }
                    }
                }
            }

            if (skillFlag == 4)
            {
                skillFlag = 5;
                if (settingData.AutoStep)
                {
                    if (SceneType.battle.battleMap.playerUnitCtrl.step != null)
                    {
                        if (!SceneType.battle.battleMap.playerUnitCtrl.step.isUseStep)
                        {
                            UseProp();
                        }
                    }
                    else
                    {
                        UseProp();
                    }
                }
                else
                {
                    UseProp();
                }
            }

            if (skillFlag == 5)
            {
                skillFlag = 0;
                if (settingData.AutoStep && SceneType.battle.battleMap.playerUnitCtrl.step != null && SceneType.battle.battleMap.playerUnitCtrl.step.IsCreate())
                {
                    SceneType.battle.battleMap.playerUnitCtrl.CreateStep();
                    return;
                }
            }

            skillFlag = 0;
        }

        private bool UseProp()
        {
            Il2CppSystem.Collections.Generic.List<PropItemBase>.Enumerator enumerator = SceneType.battle.battleMap.playerUnitCtrl.props.GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropItemBase current = enumerator.Current;
                if (current.data.cdUseTime < current.data.cdTime || current.data.propsData.propsCount == 0)
                {
                    continue;
                }

                if (current.data.propsData._propsItem.className == 401)
                {
                    if (ArtifactUse(current))
                    {
                        return true;
                    }

                    continue;
                }

                ConfItemPillItem item = g.conf.itemPill.GetItem(current.data.propsData._propsItem.id);
                if ((item == null || !HealthBackPropUse(item, current)) && settingData.AutoBattleProp)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(current.data.propsData.soleID, GetUsePosi());
                    return true;
                }
            }

            return false;
        }

        private bool ArtifactUse(PropItemBase prop)
        {
            if (settingData.AutoBattleProp)
            {
                if (prop.data.lastShotTime == -999f)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(prop.data.propsData.soleID, GetUsePosi());
                    return true;
                }

                PropItemDataArtifact propItemDataArtifact = new PropItemDataArtifact();
                propItemDataArtifact.Init(SceneType.battle.battleMap.playerUnitCtrl, prop, prop.data.propsData);
                if (prop.data.cdTime * 1000f != (float)propItemDataArtifact.cdUse.value)
                {
                    SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(prop.data.propsData.soleID, GetUsePosi());
                    return true;
                }
            }

            return false;
        }

        private Vector2 GetUsePosi()
        {
            UnitCtrlBase nearUnit = SceneType.battle.unit.GetNearUnit(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, UnitType.Monst);
            if (nearUnit != null)
            {
                return nearUnit.posiCenter.position;
            }

            return SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi;
        }

        private bool HealthBackPropUse(ConfItemPillItem itemPillItem, PropItemBase prop)
        {
            bool result = false;
            string[] array = itemPillItem.effectValue.Split('|');
            foreach (string value in array)
            {
                ConfRoleEffectItem item = g.conf.roleEffect.GetItem(value.ToInt());
                if (item == null)
                {
                    continue;
                }

                if (item.value.Contains("hp_"))
                {
                    int num = (int)((float)SceneType.battle.battleMap.playerUnitCtrl.data.maxHP.value * ((float)settingData.HealthLowerRate / 100f));
                    if (settingData.AutoHealthBackProp && SceneType.battle.battleMap.playerUnitCtrl.data.hp < num)
                    {
                        SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(prop.data.propsData.soleID, SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi);
                    }

                    result = true;
                    break;
                }

                if (item.value.Contains("mp_"))
                {
                    int num2 = (int)((float)SceneType.battle.battleMap.playerUnitCtrl.data.maxMP.value * ((float)settingData.HealthLowerRate / 100f));
                    if (settingData.AutoHealthBackProp && SceneType.battle.battleMap.playerUnitCtrl.data.mp < num2)
                    {
                        SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(prop.data.propsData.soleID, SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi);
                    }

                    result = true;
                    break;
                }

                if (item.value.Contains("sp_"))
                {
                    int num3 = (int)((float)SceneType.battle.battleMap.playerUnitCtrl.data.maxSP.value * ((float)settingData.HealthLowerRate / 100f));
                    if (settingData.AutoHealthBackProp && SceneType.battle.battleMap.playerUnitCtrl.data.sp < num3)
                    {
                        SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseProp(prop.data.propsData.soleID, SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi);
                    }

                    result = true;
                    break;
                }
            }

            return result;
        }

        private void RandomMove()
        {
            SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            Vector2 skillRangeRandomPosi = GetSkillRangeRandomPosi();
            SceneType.battle.battleMap.playerUnitCtrl.move.MovePosi(skillRangeRandomPosi, (float)SceneType.battle.battleMap.playerUnitCtrl.data.moveSpeed.value / 130f, (System.Action)delegate
            {
                if (enableAuto && settingData.AutoMove && !SceneType.battle.battleMap.playerUnitCtrl.isDie)
                {
                    if (SceneType.battle.battleMap.isPassRoom)
                    {
                        if (SceneType.battle.unit.GetNearUnit(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, UnitType.Monst) != null)
                        {
                            RandomMove();
                        }
                        else
                        {
                            moveFlag = 0;
                            if (g.world.battle.data.dungeonBaseItem.type == 55)
                            {
                                DaojieMoveCenter();
                            }
                            else
                            {
                                MoveToPassLevelEffect();
                            }
                        }
                    }
                    else
                    {
                        RandomMove();
                    }
                }
            }, Ease.Linear, isForceMoveEndCall: true);
        }

        private void StartAnim()
        {
            bool isLeft = SceneType.battle.battleMap.playerUnitCtrl.move.isLeft;
            LogTool.Info($"isLeft {isLeft}");
            Vector2 velocity = SceneType.battle.battleMap.playerUnitCtrl.move.GetVelocity();
            LogTool.Info($"moveVec {velocity.x} {velocity.y}");
            string text = (((isLeft && velocity.x > 0f) || (!isLeft && velocity.x < 0f)) ? "Run2" : "Run1");
            LogTool.Info("runName " + text);
            LogTool.Info($"disableRunAnim {SceneType.battle.battleMap.playerUnitCtrl.anim.disableRunAnim}");
            if (SceneType.battle.battleMap.playerUnitCtrl.anim.disableRunAnim == 0)
            {
                SceneType.battle.battleMap.playerUnitCtrl.anim.footAnim.SetBool(text, value: true);
                SceneType.battle.battleMap.playerUnitCtrl.anim.footAnim.SetBool((text == "Run1") ? "Run2" : "Run1", value: false);
                SceneType.battle.battleMap.playerUnitCtrl.anim.SetBool(text, value: true);
                SceneType.battle.battleMap.playerUnitCtrl.anim.SetBool((text == "Run1") ? "Run2" : "Run1", value: false);
                SceneType.battle.battleMap.playerUnitCtrl.foot1.SetActive(value: true);
                SceneType.battle.battleMap.playerUnitCtrl.foot2.SetActive(value: false);
            }
        }

        private void StopAnim()
        {
            SceneType.battle.battleMap.playerUnitCtrl.anim.footAnim.SetBool("Run1", value: false);
            SceneType.battle.battleMap.playerUnitCtrl.anim.footAnim.SetBool("Run2", value: false);
            SceneType.battle.battleMap.playerUnitCtrl.anim.SetBool("Run1", value: false);
            SceneType.battle.battleMap.playerUnitCtrl.anim.SetBool("Run2", value: false);
            SceneType.battle.battleMap.playerUnitCtrl.foot1.SetActive(value: false);
            SceneType.battle.battleMap.playerUnitCtrl.foot2.SetActive(value: true);
        }

        private Vector2 GetSkillRangeRandomPosi()
        {
            int num = GetSkillMinRange() / 100;
            if (num <= 0)
            {
                num = 2;
            }

            UnitCtrlBase nearUnit = SceneType.battle.unit.GetNearUnit(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, UnitType.Monst);
            if (nearUnit != null)
            {
                return getPointsOnMovingArc.GetPointOnMovingArc(nearUnit, nearUnit.posiCenter.position, SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, num);
            }

            return getPointsOnMovingArc.AroundTrajectory();
        }

        private int GetSkillMinRange()
        {
            int result = 0;
            if (SceneType.battle.battleMap.playerUnitCtrl.skills != null && SceneType.battle.battleMap.playerUnitCtrl.skills.Count > 0)
            {
                System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
                Il2CppSystem.Collections.Generic.List<SkillAttack>.Enumerator enumerator = SceneType.battle.battleMap.playerUnitCtrl.skills.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SkillAttack current = enumerator.Current;
                    if (current.GetAttackDis() > 0)
                    {
                        list.Add(current.GetAttackDis());
                    }
                }

                if (SceneType.battle.battleMap.playerUnitCtrl.ultimate != null)
                {
                    list.Add(SceneType.battle.battleMap.playerUnitCtrl.ultimate.GetAttackDis());
                }

                if (list.Count > 0)
                {
                    result = list.Min();
                }
            }

            return result;
        }

        private int GetRange(DataProps.PropsSkillData skillData)
        {
            int result = 0;
            if (skillData != null)
            {
                int missileID = skillData.skillBaseItem.missileID;
                string range = g.conf.battleMissile.GetItem(missileID).range;
                int result3;
                if (range.Contains("&"))
                {
                    if (int.TryParse(g.conf.battleSkillValue.GetValue(range, skillData.martialInfo.infoDataBase.skillValueData), out var result2))
                    {
                        result = result2;
                    }
                }
                else if (int.TryParse(range, out result3))
                {
                    result = result3;
                }
            }

            return result;
        }
        
        public System.Collections.Generic.List<BattleRoomNode> FindPathToUnfinishedRoom(BattleRoomNode currentRoom)
        {
            System.Collections.Generic.Dictionary<Vector2Int, BattleRoomNode> dictionary = new System.Collections.Generic.Dictionary<Vector2Int, BattleRoomNode>();
            Il2CppSystem.Collections.Generic.Queue<BattleRoomNode> queue = new Il2CppSystem.Collections.Generic.Queue<BattleRoomNode>();
            System.Collections.Generic.HashSet<Vector2Int> hashSet = new System.Collections.Generic.HashSet<Vector2Int>();
            queue.Enqueue(currentRoom);
            hashSet.Add(currentRoom.point);
            while (queue.Count > 0)
            {
                BattleRoomNode battleRoomNode = queue.Dequeue();
                if (!SceneType.battle.battleMap.IsPassRoom(battleRoomNode) && currentRoom.point != battleRoomNode.point)
                {
                    System.Collections.Generic.List<BattleRoomNode> list = new System.Collections.Generic.List<BattleRoomNode>();
                    for (BattleRoomNode battleRoomNode2 = battleRoomNode; battleRoomNode2 != null; battleRoomNode2 = ((!dictionary.ContainsKey(battleRoomNode2.point)) ? null : dictionary[battleRoomNode2.point]))
                    {
                        if (currentRoom.point != battleRoomNode2.point)
                        {
                            list.Insert(0, battleRoomNode2);
                        }
                    }

                    return list;
                }

                if (battleRoomNode.up != null && !hashSet.Contains(battleRoomNode.up.point))
                {
                    queue.Enqueue(battleRoomNode.up);
                    hashSet.Add(battleRoomNode.up.point);
                    if (battleRoomNode.up.point != currentRoom.point)
                    {
                        dictionary[battleRoomNode.up.point] = battleRoomNode;
                    }
                }

                if (battleRoomNode.down != null && !hashSet.Contains(battleRoomNode.down.point))
                {
                    queue.Enqueue(battleRoomNode.down);
                    hashSet.Add(battleRoomNode.down.point);
                    if (battleRoomNode.down.point != currentRoom.point)
                    {
                        dictionary[battleRoomNode.down.point] = battleRoomNode;
                    }
                }

                if (battleRoomNode.left != null && !hashSet.Contains(battleRoomNode.left.point))
                {
                    queue.Enqueue(battleRoomNode.left);
                    hashSet.Add(battleRoomNode.left.point);
                    if (battleRoomNode.left.point != currentRoom.point)
                    {
                        dictionary[battleRoomNode.left.point] = battleRoomNode;
                    }
                }

                if (battleRoomNode.right != null && !hashSet.Contains(battleRoomNode.right.point))
                {
                    queue.Enqueue(battleRoomNode.right);
                    hashSet.Add(battleRoomNode.right.point);
                    if (battleRoomNode.right.point != currentRoom.point)
                    {
                        dictionary[battleRoomNode.right.point] = battleRoomNode;
                    }
                }
            }

            return new System.Collections.Generic.List<BattleRoomNode>();
        }

        private Vector2 GetPassLevelPosi(BattleRoomNode currentRoom, BattleRoomNode nextRoom)
        {
            if (currentRoom.up != null && currentRoom.up.point == nextRoom.point)
            {
                return currentRoom.upPassLevelPosi;
            }

            if (currentRoom.down != null && currentRoom.down.point == nextRoom.point)
            {
                return currentRoom.downPassLevelPosi;
            }

            if (currentRoom.left != null && currentRoom.left.point == nextRoom.point)
            {
                return currentRoom.leftPassLevelPosi;
            }

            if (currentRoom.right != null && currentRoom.right.point == nextRoom.point)
            {
                return currentRoom.rightPassLevelPosi;
            }

            return Vector2.zero;
        }

        private void MoveToPassLevelEffect()
        {
            if (g.world.battle.data.dungeonBaseItem.type == 65)
            {
                return;
            }

            if (IsPassAllRoom())
            {
                ShowStartButton();
                StopAutoBattle();
                UITipItem.AddTip(GameTool.LS("AutoBattleText_23"), 2f);
                return;
            }

            SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            System.Collections.Generic.List<BattleRoomNode> list = FindPathToUnfinishedRoom(SceneType.battle.battleMap.roomNode);
            if (list.Count == 0)
            {
                ShowStartButton();
                StopAutoBattle();
                UITipItem.AddTip(GameTool.LS("AutoBattleText_24"), 2f);
                DestroyYanwu();
                return;
            }

            Vector2 targetPassLevelPosi = GetPassLevelPosi(SceneType.battle.battleMap.roomNode, list[0]);
            if (targetPassLevelPosi == Vector2.zero)
            {
                ShowStartButton();
                StopAutoBattle();
                UITipItem.AddTip(GameTool.LS("AutoBattleText_24"), 2f);
                DestroyYanwu();
            }
            else
            {
                MoveToLevel();
            }

            void MoveToLevel()
            {
                if (!((double)Vector2.Distance(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi, targetPassLevelPosi) < 0.5))
                {
                    SceneType.battle.battleMap.playerUnitCtrl.anim.ResetBool();
                    SceneType.battle.battleMap.playerUnitCtrl.move.SetDire(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x > targetPassLevelPosi.x);
                    SceneType.battle.battleMap.playerUnitCtrl.move.MovePosi(targetPassLevelPosi, (float)SceneType.battle.battleMap.playerUnitCtrl.data.moveSpeed.value / 130f, (System.Action)delegate
                    {
                        if (SceneType.battle.battleMap.isPassRoom && enableAuto && settingData.AutoMove && !SceneType.battle.battleMap.playerUnitCtrl.isDie)
                        {
                            MoveToLevel();
                        }
                    }, Ease.Linear, isForceMoveEndCall: true);
                }
            }
        }

        private bool IsPassAllRoom()
        {
            bool result = true;
            Il2CppSystem.Collections.Generic.List<BattleRoomNode>.Enumerator enumerator = SceneType.battle.room.room.allRoom.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BattleRoomNode current = enumerator.Current;
                if (!SceneType.battle.battleMap.IsPassRoom(current))
                {
                    result = false;
                }
            }

            return result;
        }

        private void DaojieMoveCenter()
        {
            SceneType.battle.battleMap.playerUnitCtrl.move.StopMove();
            if ((double)Vector2.Distance(SceneType.battle.battleMap.roomCenterPosi, SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi) > 0.5)
            {
                SceneType.battle.battleMap.playerUnitCtrl.anim.ResetBool();
                SceneType.battle.battleMap.playerUnitCtrl.move.SetDire(SceneType.battle.battleMap.playerUnitCtrl.move.lastPosi.x > SceneType.battle.battleMap.roomCenterPosi.x);
                SceneType.battle.battleMap.playerUnitCtrl.move.MovePosi(SceneType.battle.battleMap.roomCenterPosi, (float)SceneType.battle.battleMap.playerUnitCtrl.data.moveSpeed.value / 130f, (System.Action)delegate
                {
                    DaojieMoveCenter();
                },  Ease.Linear, isForceMoveEndCall: true);
            }
        }

        public void Destroy()
        {
            RecoverXuliLimitTime();
            g.timer.Stop(corUpdate);
            g.events.Off(EGameType.OpenUIEnd, callOpenUIEnd);
            g.events.Off(EBattleType.BattleStart, onBattleStart);
            g.events.Off(EBattleType.BattleExit, onBattleExit);
        }

        public void  OnUpdate()
        {
            if (SceneType.battle == null || SceneType.battle.battleMap == null)
            {
                return;
            }

            if (!settingData.AutoMove || !enableAuto)
            {
                DestroyYanwu();
            }

            if (!enableAuto || SceneType.battle.battleMap.playerUnitCtrl.isDie || SceneType.battle.battleMap.isPassRoom)
            {
                return;
            }

            if (settingData.AutoLeft)
            {
                SceneType.battle.battleMap.playerUnitCtrl.inputCtrl.UseSkill(MartialType.SkillLeft);
            }

            if (!settingData.AutoDevilDemonAbsorb || SceneType.battle.battleMap.playerUnitCtrl.devilDemonSkill == null)
            {
                return;
            }

            DevilDemonAbsorb devilDemonAbsorb = SceneType.battle.battleMap.playerUnitCtrl.devilDemonSkill.devilDemonAbsorb;
            if (!devilDemonAbsorb.QTEStart)
            {
                return;
            }

           Il2CppSystem.Collections.Generic.List<UnitCtrlMonst>.Enumerator enumerator = devilDemonAbsorb.absorbPreUnit.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UnitCtrlMonst current = enumerator.Current;
                if (devilDemonAbsorb.GetAbsorbRate(current) >= settingData.DemonAbsorbAtRate)
                {
                    Il2CppSystem.Collections.Generic.List<UnitCtrlMonst> list = new Il2CppSystem.Collections.Generic.List<UnitCtrlMonst>();
                    list.Add(current);
                    StopAutoBattleNotYanwu();
                    SceneType.battle.battleMap.playerUnitCtrl.devilDemonSkill.devilDemonAbsorb.AbsorbShow(list, (System.Action)delegate
                    {
                        StartAutoBattleNotYanwu();
                    });
                }
            }
        }
    }
}