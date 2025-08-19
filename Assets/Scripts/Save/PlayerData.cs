using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData
{
    public bool RemoveAds = false;
    public bool Rate = false;
    public bool Music = true;
    public bool Sound = true;
    public bool Vibration = true;
    public bool PlayAgain = false;
    public int TakeLevelReward = 0;

    public int OpenedMap = 1;

    public int Gold = 0;
    public int Energy = 5;
    public int Magnet = 0;
    public int Undo = 0;
    public int Compass = 0;
    public int FreezeTimer = 0;
    public int Thunder = 0;
    public int AddTime = 0;
    public int Hint = 0;
    public int CompassMiniGame = 0;

    public string EnergyTimer = null;

    public string InfiniteEnergy = null;
    public string DailyReward = null;
    public int DailyRewardStack = 0;

    public int StarChest = 0;
    public int RewardStarChest = 0;

    public List<int> ChallengeRemainingTime = new List<int>{1800,1800,1800,1800,1800};

    public List<int> CompletedChallengeMap = new List<int>();

    public List<string> ListItemSlotChallenge1 = new List<string>();
    public List<string> ListItemSlotChallenge2 = new List<string>();
    public List<string> ListItemSlotChallenge3 = new List<string>();
    public List<string> ListItemSlotChallenge4 = new List<string>();
    public List<string> ListItemSlotChallenge5 = new List<string>();


    public List<string> ListChallenge1 = new List<string>();
    public List<string> ListChallenge2 = new List<string>();
    public List<string> ListChallenge3 = new List<string>();
    public List<string> ListChallenge4 = new List<string>();
    public List<string> ListChallenge5 = new List<string>();

    [Header("Mini game")]
    public int OpenedMiniGameMap = 1;

    public bool[] AddMoreItem = {};
    public bool[] ShowAllItem = {};

    public List<string> ItemMap1 = new List<string>();
    public List<string> ItemMap2 = new List<string>();
    public List<string> ItemMap3 = new List<string>();
    public List<string> ItemMap4 = new List<string>();
    public List<string> ItemMap5 = new List<string>();
    public List<string> ItemMap6 = new List<string>();
    public List<string> ItemMap7 = new List<string>();
    public List<string> ItemMap8 = new List<string>();
    public List<string> ItemMap9 = new List<string>();

    public List<string> ShowHiddenItems1 = new List<string>();
    public List<string> ShowHiddenItems2 = new List<string>();
    public List<string> ShowHiddenItems3 = new List<string>();
    public List<string> ShowHiddenItems4 = new List<string>();
    public List<string> ShowHiddenItems5 = new List<string>();
    public List<string> ShowHiddenItems6 = new List<string>();
    public List<string> ShowHiddenItems7 = new List<string>();
    public List<string> ShowHiddenItems8 = new List<string>();
    public List<string> ShowHiddenItems9 = new List<string>();


    private static readonly Lazy<PlayerData> _instance = new Lazy<PlayerData>(() => new PlayerData());
    public static PlayerData Instance => _instance.Value;

    private PlayerData() { }

    public void SetPlayerData
    (
        bool removeAds = false, bool rate = false, bool music = true, 
        bool sound = true, bool vibration = true, bool playAgain = false, int takeLevelReward = 0, int openedMap = 1, 
        int gold = 0, int energy = 5, int magnet = 0, int undo = 0, int compass = 0, int freezeTimer = 0, int thunder = 0, int addTime = 0,
        int hint = 0, int compassMiniGame = 0, string energyTimer = null,
        string infiniteEnergy = null, string dailyReward = null, int dailyRewardStack = 0, int starChest = 0, int rewardStarChest = 0,
        List<int> challengeRemainingTime = null, List<int> completedChallengeMap = null, 
        List<string> listItemSlotChallenge1 = null, List<string> listItemSlotChallenge2 = null, List<string> listItemSlotChallenge3 = null, List<string> listItemSlotChallenge4 = null, List<string> listItemSlotChallenge5 = null,
        List<string> listChallenge1 = null, List<string> listChallenge2 = null, List<string> listChallenge3 = null, List<string> listChallenge4 = null, List<string> listChallenge5 = null,

        int openedMiniGameMap = 1, bool[] addMoreItem = null, bool[] showAllItem = null,
        List<string> itemMap1 = null, List<string> itemMap2 = null, List<string> itemMap3 = null, List<string> itemMap4 = null, List<string> itemMap5 = null,
        List<string> itemMap6 = null, List<string> itemMap7 = null, List<string> itemMap8 = null, List<string> itemMap9 = null,
        List<string> showHiddenItems1 = null, List<string> showHiddenItems2 = null, List<string> showHiddenItems3 = null, List<string> showHiddenItems4 = null, List<string> showHiddenItems5 = null,
        List<string> showHiddenItems6 = null, List<string> showHiddenItems7 = null, List<string> showHiddenItems8 = null, List<string> showHiddenItems9 = null)
    {
        RemoveAds = removeAds;
        Rate = rate;
        Music = music;
        Sound = sound;
        Vibration = vibration;
        PlayAgain = playAgain;
        TakeLevelReward = takeLevelReward;

        OpenedMap = openedMap;
        Gold = gold;
        Energy = energy;
        Magnet = magnet;
        Undo = undo;
        Compass = compass;
        FreezeTimer = freezeTimer;
        Thunder = thunder;
        AddTime = addTime;
        Hint = hint;
        CompassMiniGame = compassMiniGame;

        EnergyTimer = energyTimer;

        InfiniteEnergy = infiniteEnergy;

        DailyReward = dailyReward;
        DailyRewardStack = dailyRewardStack;

        StarChest = starChest;
        RewardStarChest = rewardStarChest;

        ChallengeRemainingTime = challengeRemainingTime?.ToList() ?? new List<int>();
        CompletedChallengeMap = completedChallengeMap?.ToList() ?? new List<int>();
        ListItemSlotChallenge1 = listItemSlotChallenge1?.ToList() ?? new List<string>();
        ListItemSlotChallenge2 = listItemSlotChallenge2?.ToList() ?? new List<string>();
        ListItemSlotChallenge3 = listItemSlotChallenge3?.ToList() ?? new List<string>();
        ListItemSlotChallenge4 = listItemSlotChallenge4?.ToList() ?? new List<string>();
        ListItemSlotChallenge5 = listItemSlotChallenge5?.ToList() ?? new List<string>();
        ListChallenge1 = listChallenge1?.ToList() ?? new List<string>();
        ListChallenge2 = listChallenge2?.ToList() ?? new List<string>();
        ListChallenge3 = listChallenge3?.ToList() ?? new List<string>();
        ListChallenge4 = listChallenge4?.ToList() ?? new List<string>();
        ListChallenge5 = listChallenge5?.ToList() ?? new List<string>();

        OpenedMiniGameMap = openedMiniGameMap;
        // AddMoreItem = addMoreItem?.ToList() ?? new List<bool>();
        // ShowAllItem = showAllItem?.ToList() ?? new List<bool>();
        AddMoreItem = addMoreItem;
        ShowAllItem = showAllItem;
        ItemMap1 = itemMap1?.ToList() ?? new List<string>();
        ItemMap2 = itemMap2?.ToList() ?? new List<string>();
        ItemMap3 = itemMap3?.ToList() ?? new List<string>();
        ItemMap4 = itemMap4?.ToList() ?? new List<string>();
        ItemMap5 = itemMap5?.ToList() ?? new List<string>();
        ItemMap6 = itemMap6?.ToList() ?? new List<string>();
        ItemMap7 = itemMap7?.ToList() ?? new List<string>();
        ItemMap8 = itemMap8?.ToList() ?? new List<string>();
        ItemMap9 = itemMap9?.ToList() ?? new List<string>();
        ShowHiddenItems1 = showHiddenItems1?.ToList() ?? new List<string>();
        ShowHiddenItems2 = showHiddenItems2?.ToList() ?? new List<string>();
        ShowHiddenItems3 = showHiddenItems3?.ToList() ?? new List<string>();
        ShowHiddenItems4 = showHiddenItems4?.ToList() ?? new List<string>();
        ShowHiddenItems5 = showHiddenItems5?.ToList() ?? new List<string>();
        ShowHiddenItems6 = showHiddenItems6?.ToList() ?? new List<string>();
        ShowHiddenItems7 = showHiddenItems7?.ToList() ?? new List<string>();
        ShowHiddenItems8 = showHiddenItems8?.ToList() ?? new List<string>();
        ShowHiddenItems9 = showHiddenItems9?.ToList() ?? new List<string>();
    }

    public void SetPlayerData<T>(string name, T val)
    {
        try
        {
            switch (name)
            {
                case nameof(RemoveAds): RemoveAds = (bool)(object)val; break;
                case nameof(Rate): Rate = (bool)(object)val; break;
                case nameof(Music): Music = (bool)(object)val; break;
                case nameof(Sound): Sound = (bool)(object)val; break;
                case nameof(Vibration): Vibration = (bool)(object)val; break;
                case nameof(PlayAgain): PlayAgain = (bool)(object)val; break;
                case nameof(TakeLevelReward): TakeLevelReward = (int)(object)val; break;
                case nameof(OpenedMap): OpenedMap = (int)(object)val; break;
                case nameof(Gold): Gold = (int)(object)val; break;
                case nameof(Energy): Energy = (int)(object)val; break;
                case nameof(Magnet): Magnet = (int)(object)val; break;
                case nameof(Undo): Undo = (int)(object)val; break;
                case nameof(Compass): Compass = (int)(object)val; break;
                case nameof(FreezeTimer): FreezeTimer = (int)(object)val; break;
                case nameof(Thunder): Thunder = (int)(object)val; break;
                case nameof(AddTime): AddTime = (int)(object)val; break;
                case nameof(Hint): Hint = (int)(object)val; break;
                case nameof(CompassMiniGame): CompassMiniGame = (int)(object)val; break;
                case nameof(EnergyTimer): EnergyTimer = val as string; break;
                case nameof(InfiniteEnergy): InfiniteEnergy = val as string; break;
                case nameof(DailyReward): DailyReward = val as string; break;
                case nameof(DailyRewardStack): DailyRewardStack = (int)(object)val; break;
                case nameof(StarChest): StarChest = (int)(object)val; break;
                case nameof(RewardStarChest): RewardStarChest = (int)(object)val; break;
                case nameof(ChallengeRemainingTime): AddItemToList(ChallengeRemainingTime, (int)(object)val); break;
                case nameof(CompletedChallengeMap): AddItemToList(CompletedChallengeMap, (int)(object)val); break;
                case nameof(ListItemSlotChallenge1): AddItemToList(ListItemSlotChallenge1, (string)(object)val); break;
                case nameof(ListItemSlotChallenge2): AddItemToList(ListItemSlotChallenge2, (string)(object)val); break;
                case nameof(ListItemSlotChallenge3): AddItemToList(ListItemSlotChallenge3, (string)(object)val); break;
                case nameof(ListItemSlotChallenge4): AddItemToList(ListItemSlotChallenge4, (string)(object)val); break;
                case nameof(ListItemSlotChallenge5): AddItemToList(ListItemSlotChallenge5, (string)(object)val); break;
                case nameof(ListChallenge1): AddItemToList(ListChallenge1, (string)(object)val); break;
                case nameof(ListChallenge2): AddItemToList(ListChallenge2, (string)(object)val); break;
                case nameof(ListChallenge3): AddItemToList(ListChallenge3, (string)(object)val); break;
                case nameof(ListChallenge4): AddItemToList(ListChallenge4, (string)(object)val); break;
                case nameof(ListChallenge5): AddItemToList(ListChallenge5, (string)(object)val); break;
                case nameof(OpenedMiniGameMap): OpenedMiniGameMap = (int)(object)val; break;
                // case nameof(AddMoreItem): AddItemToList(AddMoreItem, val as bool); break;
                // case nameof(ShowAllItem): AddItemToList(ShowAllItem, val as bool); break;
                case nameof(ItemMap1): AddItemToList(ItemMap1, val as string); break;
                case nameof(ItemMap2): AddItemToList(ItemMap2, val as string); break;
                case nameof(ItemMap3): AddItemToList(ItemMap3, val as string); break;
                case nameof(ItemMap4): AddItemToList(ItemMap4, val as string); break;
                case nameof(ItemMap5): AddItemToList(ItemMap5, val as string); break;
                case nameof(ItemMap6): AddItemToList(ItemMap6, val as string); break;
                case nameof(ItemMap7): AddItemToList(ItemMap7, val as string); break;
                case nameof(ItemMap8): AddItemToList(ItemMap8, val as string); break;
                case nameof(ItemMap9): AddItemToList(ItemMap9, val as string); break;
                case nameof(ShowHiddenItems1): AddItemToList(ShowHiddenItems1, val as string); break;
                case nameof(ShowHiddenItems2): AddItemToList(ShowHiddenItems2, val as string); break;
                case nameof(ShowHiddenItems3): AddItemToList(ShowHiddenItems3, val as string); break;
                case nameof(ShowHiddenItems4): AddItemToList(ShowHiddenItems4, val as string); break;
                case nameof(ShowHiddenItems5): AddItemToList(ShowHiddenItems5, val as string); break;
                case nameof(ShowHiddenItems6): AddItemToList(ShowHiddenItems6, val as string); break;
                case nameof(ShowHiddenItems7): AddItemToList(ShowHiddenItems7, val as string); break;
                case nameof(ShowHiddenItems8): AddItemToList(ShowHiddenItems8, val as string); break;
                case nameof(ShowHiddenItems9): AddItemToList(ShowHiddenItems9, val as string); break;
                default: throw new ArgumentException($"Invalid property name: {name}");
            }
        }
        catch (InvalidCastException ex)
        {
            throw new ArgumentException($"Invalid value type for property {name}", ex);
        }
    }

    public void SetPlayerData(string name, object val, int mapNum)
    {
        switch (name)
        {
            case "AddMoreItem": AddItemToList(AddMoreItem, val, name, mapNum); break;
            case "ShowAllItem": AddItemToList(ShowAllItem, val, name, mapNum); break;
            default: throw new ArgumentException($"Invalid property name: {name}");
        }
    }

    public void AddItemToList(bool[]itemMap, object val, string name, int mapNum)
    {
        if(itemMap == null) itemMap = new bool[] {};
        if(val != null){
            int length = itemMap.Length;
            if(length < mapNum){
                Array.Resize(ref itemMap, mapNum);
                itemMap[mapNum - 1] = (bool)val;
            } else itemMap[mapNum - 1] = (bool)val;
        } else itemMap = new bool[] {};

        if(name == "AddMoreItem") this.AddMoreItem = itemMap;
        else if(name == "ShowAllItem") this.ShowAllItem = itemMap;
    }

    private void AddItemToList<T>(List<T> list, T item)
    {
        if(item == null) list.Clear();
        else if (item != null && !list.Contains(item))
        {
            list.Add(item);
        }
    }

    // public void RemoveItemFromList<T>(List<T> list, T item)
    // {
    //     list.Remove(item);
    // }

    public void RemoveItemFromList (string list, string item)
    {
        if(list == "ListItemSlotChallenge1") ListItemSlotChallenge1.Remove(item);
        else if(list == "ListItemSlotChallenge2") ListItemSlotChallenge2.Remove(item);
        else if(list == "ListItemSlotChallenge3") ListItemSlotChallenge3.Remove(item);
        else if(list == "ListItemSlotChallenge4") ListItemSlotChallenge4.Remove(item);
        else if(list == "ListItemSlotChallenge5") ListItemSlotChallenge5.Remove(item);
    }

    public void ChangeChallengeRemainingTime (int index, int item)
    {
        ChallengeRemainingTime[index] = item;
    }
}