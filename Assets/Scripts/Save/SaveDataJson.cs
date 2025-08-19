using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveDataJson : MonoBehaviour
{
    private PlayerData playerData;
    private string filePath;
    private PlayerData data;

    private Dictionary<string, Func<object>> playerDataMap;


    [Header("Map Data")]
    public TextAsset jsonMapData;
    public MapList MapData = new MapList();
    [Serializable]public class Map
    {
        public int MapName;
        public string[] ItemList;
        public int[] QuantityList;
        public string[] Moving;
        public int Time;
    }
    [Serializable]public class MapList
    {
            public Map[] map;
    }


    [Header("Mini Game Data")]
    public TextAsset jsonMiniGameData;
    public MinigameList MinigameData = new MinigameList();
    [Serializable]public class Minigame
    {
        public string AreaName;
        public string[] ItemList;
        public int[] QuantityList;
        public int[] Milestones;
    }
    [Serializable]public class MinigameList
    {
            public Minigame[] map;
    }
    
    void Awake()
    {
        MapData = JsonUtility.FromJson<MapList>(jsonMapData.text);
        MinigameData = JsonUtility.FromJson<MinigameList>(jsonMiniGameData.text);

        playerData = PlayerData.Instance;
        filePath = Application.dataPath + Path.AltDirectorySeparatorChar + $"TripleMatchCity_PlayerData.json";
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Path.Combine(Application.persistentDataPath, "TripleMatchCity_PlayerData.json");
        }

        if(!File.Exists(filePath)) SaveNewData();
        else LoadData();

        InitializePlayerDataMap();
    }
    
    private void SaveNewData()
    {
        SaveData();
        LoadData();
    }

    private void SaveData()
    {
        data = playerData;
        string json = JsonUtility.ToJson(playerData);

        using(StreamWriter writer = new StreamWriter(filePath)) writer.Write(json);
    }

    private void LoadData()
    {
        string json = File.ReadAllText(filePath);
        // string json = string.Empty;
        // using(StreamReader reader = new StreamReader(filePath)) json = reader.ReadToEnd();
        if(json == ""){
            //Nếu file json rỗng
            SaveNewData();
            return;
        }   

        data = JsonUtility.FromJson<PlayerData>(json);

        playerData.SetPlayerData
        (
            data.RemoveAds, data.Rate, data.Music, data.Sound, data.Vibration, data.PlayAgain, data.TakeLevelReward, data.OpenedMap,
            data.Gold, data.Energy, data.Magnet, data.Undo, data.Compass, data.FreezeTimer, data.Thunder, data.AddTime, data.Hint, data.CompassMiniGame, data.EnergyTimer,
            data.InfiniteEnergy, data.DailyReward, data.DailyRewardStack, data.StarChest, data.RewardStarChest,
            data.ChallengeRemainingTime, data.CompletedChallengeMap, 
            data.ListItemSlotChallenge1, data.ListItemSlotChallenge2, data.ListItemSlotChallenge3, data.ListItemSlotChallenge4, data.ListItemSlotChallenge5, 
            data.ListChallenge1, data.ListChallenge2, data.ListChallenge3, data.ListChallenge4, data.ListChallenge5,
            data.OpenedMiniGameMap, data.AddMoreItem, data.ShowAllItem, 
            data.ItemMap1, data.ItemMap2, data.ItemMap3, data.ItemMap4, data.ItemMap5, data.ItemMap6, data.ItemMap7, data.ItemMap8, data.ItemMap9,
            data.ShowHiddenItems1, data.ShowHiddenItems2, data.ShowHiddenItems3, data.ShowHiddenItems4, data.ShowHiddenItems5, data.ShowHiddenItems6,
            data.ShowHiddenItems7, data.ShowHiddenItems8, data.ShowHiddenItems9
        );

        // return data;
    }

    private void InitializePlayerDataMap()
    {
        playerDataMap = new Dictionary<string, Func<object>>
        {
            { "RemoveAds", () => data.RemoveAds},
            { "Rate", () => data.Rate},
            { "Music", () => data.Music},
            { "Sound", () => data.Sound},
            { "Vibration", () => data.Vibration},
            { "PlayAgain", () => data.PlayAgain},
            { "TakeLevelReward", () => data.TakeLevelReward},
            { "OpenedMap", () => data.OpenedMap},
            { "Gold", () => data.Gold},
            { "Energy", () => data.Energy},
            { "Magnet", () => data.Magnet},
            { "Undo", () => data.Undo},
            { "Compass", () => data.Compass},
            { "FreezeTimer", () => data.FreezeTimer},
            { "Thunder", () => data.Thunder},
            { "Hint", () => data.Hint},
            { "AddTime", () => data.AddTime},
            { "CompassMiniGame", () => data.CompassMiniGame},
            { "EnergyTimer", () => data.EnergyTimer},
            { "InfiniteEnergy", () => data.InfiniteEnergy},
            { "DailyReward", () => data.DailyReward},
            { "DailyRewardStack", () => data.DailyRewardStack},
            { "StarChest", () => data.StarChest},
            { "RewardStarChest", () => data.RewardStarChest},
            { "ChallengeRemainingTime", () => data.ChallengeRemainingTime},
            { "CompletedChallengeMap", () => data.CompletedChallengeMap},
            { "ListItemSlotChallenge1", () => data.ListItemSlotChallenge1},
            { "ListItemSlotChallenge2", () => data.ListItemSlotChallenge2},
            { "ListItemSlotChallenge3", () => data.ListItemSlotChallenge3},
            { "ListItemSlotChallenge4", () => data.ListItemSlotChallenge4},
            { "ListItemSlotChallenge5", () => data.ListItemSlotChallenge5},
            { "ListChallenge1", () => data.ListChallenge1},
            { "ListChallenge2", () => data.ListChallenge2},
            { "ListChallenge3", () => data.ListChallenge3},
            { "ListChallenge4", () => data.ListChallenge4},
            { "ListChallenge5", () => data.ListChallenge5},
            { "OpenedMiniGameMap", () => data.OpenedMiniGameMap},
            { "AddMoreItem", () => data.AddMoreItem},
            { "ShowAllItem", () => data.ShowAllItem},
            { "ItemMap1", () => data.ItemMap1},
            { "ItemMap2", () => data.ItemMap2},
            { "ItemMap3", () => data.ItemMap3},
            { "ItemMap4", () => data.ItemMap4},
            { "ItemMap5", () => data.ItemMap5},
            { "ItemMap6", () => data.ItemMap6},
            { "ItemMap7", () => data.ItemMap7},
            { "ItemMap8", () => data.ItemMap8},
            { "ItemMap9", () => data.ItemMap9},
            { "ShowHiddenItems1", () => data.ShowHiddenItems1},
            { "ShowHiddenItems2", () => data.ShowHiddenItems2},
            { "ShowHiddenItems3", () => data.ShowHiddenItems3},
            { "ShowHiddenItems4", () => data.ShowHiddenItems4},
            { "ShowHiddenItems5", () => data.ShowHiddenItems5},
            { "ShowHiddenItems6", () => data.ShowHiddenItems6},
            { "ShowHiddenItems7", () => data.ShowHiddenItems7},
            { "ShowHiddenItems8", () => data.ShowHiddenItems8},
            { "ShowHiddenItems9", () => data.ShowHiddenItems9}
        };
    }

    
    public object TakePlayerData(string name)
    {
        if (playerDataMap.TryGetValue(name, out var getter))
        {
            return getter();
        }
        return null;
    }


    public MapList TakeMapData()
    {
        return MapData;
    }

    public MinigameList TakeMiniGameData()
    {
        return MinigameData;
    }

    public void SaveData(string name, object val, int mapNum = 0)
    {
        // lưu dữ liệu tại thời gian thực
        if(name == "AddMoreItem" || name == "ShowAllItem") playerData.SetPlayerData(name, val, mapNum);
        else playerData.SetPlayerData(name, val);
        SaveData();
    }

    public void RemoveItemFromList (string name, string val)
    {
        playerData.RemoveItemFromList(name, val);
        SaveData();
    }

    public void ChangeChallengeRemainingTime (int index, int val)
    {
        playerData.ChangeChallengeRemainingTime(index, val);
        SaveData();
    }

    public object GetData(string name)
    {
        // lấy 1 dữ liệu cự thể đã lưu
        object  player = TakePlayerData(name);
        return player;
    }

    public PlayerData GetData()
    {
        // lấy toàn bộ dữ liệu đã lưu
        return data;
    }
}
