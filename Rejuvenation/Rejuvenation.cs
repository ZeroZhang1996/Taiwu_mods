using Harmony12;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace Rejuvenation
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool notRejuvenate = false;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

    }

    public static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry.ModLogger Logger;

        public static Settings setting;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            setting = Settings.Load<Settings>(modEntry);

            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;

            modEntry.OnGUI = OnGUI;

            //if (enabled)
            {
                string resdir = System.IO.Path.Combine(modEntry.Path, "Data");
                Logger.Log(" resdir :" + resdir);
                BaseResourceMod.Main.registModResDir(modEntry, resdir);
            }

            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        public static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (!value)
                return false;

            enabled = value;

            return true;
        }

        private static int dateId = 0;
        private static bool isAdd = false;

        public static void DoRemoveChooseGongFa()
        {
            int key = DateFile.instance.MianActorID();
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] = 0;
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] = 0;
            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][2] = 0;
            DateFile.instance.gongFaBookPages.Remove(150369);
            DateFile.instance.RemoveMainActorEquipGongFa(150369);
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            bool cond = (DateFile.instance == null || DateFile.instance.actorsDate == null || !DateFile.instance.actorsDate.ContainsKey(DateFile.instance.mianActorId));
            if (cond)
            {
                GUILayout.Label("未加载存档！");
                return;
            }
            if (!DateFile.instance.gongFaDate.ContainsKey(150369))
            {
                GUILayout.Label("增量数据未正常加载！");
                return;
            }

            DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 0, 0, 0, true);

            if (!DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150369))
            {
                if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150002))
                {
                    int readPageNum = 0;
                    int[] bookPages = (!DateFile.instance.gongFaBookPages.ContainsKey(150002)) ? new int[10] : DateFile.instance.gongFaBookPages[150002];
                    for (int i = 0; i < 10; i++)
                    {
                        if (bookPages[i] != 0)
                        {
                            readPageNum++;
                        }
                    }
                    GUILayout.Label("须得「沛然诀」大成，方能领悟「天长地久不老长春功」。");
                    GUILayout.BeginHorizontal("Box");
                    GUILayout.Label("修习进度：" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150002][0].ToString() + "%");
                    GUILayout.Label("研读进度：" + readPageNum.ToString() + "/10");
                    GUILayout.EndHorizontal();
                }
                else
                {
                    int[] allQi = DateFile.instance.GetActorAllQi(DateFile.instance.mianActorId);
                    GUILayout.Label("须以上乘内功为根基，方能修炼「天长地久不老长春功」。");
                    GUILayout.BeginHorizontal("Box");
                    GUILayout.Label("当前内力：" + (allQi[0] + allQi[1] + allQi[2] + allQi[3] + allQi[4]).ToString() + "/500");
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("年龄：");
                DateFile.instance.actorsDate[DateFile.instance.mianActorId][11] = GUILayout.TextField(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11]);
                GUILayout.Label("修习进度：");
                DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] = int.Parse(GUILayout.TextField(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0].ToString()));
                GUILayout.Label("研读进度：");
                DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1] = int.Parse(GUILayout.TextField(DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1].ToString()));
                GUILayout.EndHorizontal();
                GUILayout.Label("修炼<color=#E4504DFF>「天长地久不老长春功」</color>有成者，每到三十岁便会返老还童。");
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("研读进度：" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][1].ToString() + "/10");
                GUILayout.Label("修习进度：" + DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0].ToString() + "%"
                    + (setting.notRejuvenate ? " -1%/时节" : ""));
                GUILayout.EndHorizontal();
                int bloodNum = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].ContainsKey(21 + i))
                    {
                        bloodNum += DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i];
                    }
                }
                GUILayout.Label("返老还童，功力散去，有伐毛洗髓之效，此时需以「血露」调和阴阳，方可避免走火入魔。");
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label("持有血露：" + bloodNum.ToString() + "/30");
                setting.notRejuvenate = GUILayout.Toggle((DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] > 25) ? setting.notRejuvenate : false,"压制返老还童");
                GUILayout.EndHorizontal();
            }
        }
    }

    [HarmonyPatch(typeof(UIDate), "TrunChange")]
    public static class UIDate_TrunChange_Patch
    {
        private static void Prefix()
        {
            if (!Main.enabled)
            {
                return;
            }

            if (!DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150369))
            {
                int[] allQi = DateFile.instance.GetActorAllQi(DateFile.instance.mianActorId);
                if (allQi[0] + allQi[1] + allQi[2] + allQi[3] + allQi[4] >= 450)
                {
                    DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 50, 0, 0, false);
                }
                else if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId].ContainsKey(150002))
                {
                    int[] bookPages = (!DateFile.instance.gongFaBookPages.ContainsKey(150002)) ? new int[10] : DateFile.instance.gongFaBookPages[150002];
                    for (int i = 0; i < 10; i++)
                    {
                        if (bookPages[i] == 0)
                        {
                            return;
                        }
                    }
                    if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150002][0] == 100)
                    {
                        DateFile.instance.ChangeActorGongFa(DateFile.instance.mianActorId, 150369, 0, 0, 0, true);
                    }
                }
            }
            else
            {
                if (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][11]) >= 30)
                {
                    if (Main.setting.notRejuvenate)
                    {
                        if (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] > 0)
                        {
                            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0]--;
                        }
                    }
                    else
                    {
                        int power = 0;
                        while (DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] > 25)
                        {
                            DateFile.instance.actorGongFas[DateFile.instance.mianActorId][150369][0] -= 25;
                            power++;
                        }
                        for (int j = 0; j < power; j++)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                int num = Random.Range(0, 6);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][61 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][61 + num]) + 1).ToString();
                            }
                            for (int i = 0; i < 16; i++)
                            {
                                int num = Random.Range(0, 16);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][501 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][501 + num]) + 1).ToString();
                            }
                            for (int i = 0; i < 14; i++)
                            {
                                int num = Random.Range(0, 14);
                                DateFile.instance.actorsDate[DateFile.instance.mianActorId][601 + num] = (int.Parse(DateFile.instance.actorsDate[DateFile.instance.mianActorId][601 + num]) + 1).ToString();
                            }
                        }

                        int bloodNeed = 30;
                        for (int i = 0; i < 9; i++)
                        {
                            if (DateFile.instance.actorItemsDate[DateFile.instance.mianActorId].ContainsKey(21 + i) && bloodNeed > 0)
                            {
                                if (bloodNeed >= DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i])
                                {
                                    bloodNeed -= DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i];
                                    DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i] = 0;
                                }
                                else
                                {
                                    DateFile.instance.actorItemsDate[DateFile.instance.mianActorId][21 + i] -= bloodNeed;
                                    bloodNeed = 0;
                                }
                            }
                        }
                        for (int i = 1, j = 0; i <= bloodNeed; i++)
                        {
                            List<string> list2 = new List<string>();
                            if (i <= 5 / 2)
                            {
                                j = 1;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][11].Split(new char[] { '|' }));
                            }
                            else if (i <= 5)
                            {
                                j = 2;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][12].Split(new char[] { '|' }));
                            }
                            else
                            {
                                j = 3;
                                list2 = new List<string>(DateFile.instance.bodyInjuryDate[9][13].Split(new char[] { '|' }));
                            }
                            int id = int.Parse(list2[Random.Range(0, list2.Count)]);
                            int num10 = i * i;
                            ActorMenu.instance.ChangeMianQi(DateFile.instance.mianActorId, j * num10, 5);
                            DateFile.instance.AddInjury(DateFile.instance.mianActorId, id, num10, false);
                        }
                    }
                }
            }
        }
    }
}
