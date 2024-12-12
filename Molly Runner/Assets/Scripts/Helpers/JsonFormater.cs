using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class FailCallbackError
{
    public bool success = false;
    public string msg;
}

[Serializable]
public class GetNetworkMessage
{
    public int data;
    public string message;
}

[Serializable]
public class User
{
    public bool success;
    public UserData data;
}

[Serializable]
public class UserData
{
    public UserInfo user;
}

[Serializable]
public class UserInfo
{
    public string access_token;
    public int level;
    public int coin;
    public string device_id;
}

[Serializable]
public class GetUser
{
    public bool success;
    public Data data;
}

[Serializable]
public class Data
{
    public GetUserData user;
}

[Serializable]
public class GetUserData
{
    public int id;
    public string access_token;
    public int level;
    public int income_level;
    public int molly_count;
    public string device_id;
}


[Serializable]
public class Register
{
    public bool success;
    public RegisterData data;
}
[Serializable]
public class RegisterData
{
    public RegisterInfo user;
}

[Serializable]
public class RegisterInfo
{
    public int id;
    public string access_token;
    public int level;
    public int income_level;
    public int molly_count;
    public string device_id;
    public string device_type;

}

[Serializable]
public class Login
{
    public bool success;
    public LoginData data;
}

[Serializable]
public class LoginData
{
    public string access_token;
}


[Serializable]
public class AddReward
{
    public bool success = false;
    public int reward;
}

[Serializable]
public class GetReward
{
    public bool success = false;
    public GetRewardData data;
}

[Serializable]
public class GetRewardData
{
    public int coin;
}


[Serializable]
public class RemoveCoin
{
    public bool success = false;
    public int removedCoin;
    public int totalCoin;
}


[Serializable]
public class IncreaseMolly
{
    public bool success;
    public int totalCoin;
    public int level;
}

[Serializable]
public class UpgradeMolly
{
    public bool success;
    public int totalCoin;
    public int molly_count;
}

[Serializable]
public class Item
{
    public bool success;
    public List<ItemData> data;
}


[Serializable]
public class ItemData
{
    public string name;
    public int item_id;
    public string category_type;
    public int in_game_price;
    public int attack_value;
    public int defence_value;
    public bool isLocked;
}

[Serializable]
public class ItemAddForUser
{
    public bool success;
    public string msg;
    public int coin;
}

[Serializable]
public class LeaderboardResponse
{
    public bool success;
    public List<LeaderboardData> data;
}

[Serializable]
public class LeaderboardData
{
    public int coin;
    public int points;
    public string username;
    public int id;
    public int level;
}

