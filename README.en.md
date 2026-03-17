<div align="center"><h1><img width="600" height="131" alt="68747470733a2f2f70616e2e73616d7979632e6465762f732f56596d4d5845" src="https://github.com/user-attachments/assets/d0316faa-c2d0-478f-a642-1e3c3651f1d4" /></h1></div>

<div class="section">
<div align="center"><h1>Shop_HZP_Credits for Swiftly2 ShopCore</h1></div>

<div align="center">
  <a href="./README.md"><img src="https://flagcdn.com/48x36/cn.png" alt="中文" width="48" height="36" /> <strong>中文版</strong></a>  
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="./README.en.md"><img src="https://flagcdn.com/48x36/gb.png" alt="English" width="48" height="36" /> <strong>English</strong></a>
</div>

---

<div align="center">
  
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Z8Z31PY52N)

</div>
<hr>

<div align="center"><strong>Shop credits reward system for CS2 Zombie Plague based on the Swiftly2 framework.</p></div>
<div align="center"><strong>Supports multiple configurable reward methods.</p></div>
<div align="center"><strong>This plugin requires both ShopCore and HanZombiePlagueS2 to be installed.</p></div>
<div align="center"><strong>HanZombiePlagueS2 : https://github.com/H-AN/HanZombiePlagueS2 </p></div>
<div align="center"><strong>ShopCore : https://github.com/SwiftlyS2-Plugins/ShopCore </p></div>
</div>

---

Plugin Configuration :

```
{
  "ShopHZPCreditsCFG": {
    "Enabled": true,  //Global switch
    "InfoCommand": "sw_money", //Command to display information
    "Global": {
      "InfectReward": 10,  //Credits gained for infecting a player as a zombie
      "HumanWinReward": 20, //All humans receive credits when humans win
      "ZombieWinReward": 5, //All zombies receive credits when zombies win
      "KillZombieReward": 5, //Credits gained for killing a zombie
      "KillHumanReward": 5, //Credits gained for zombies killing humans (Legion / non-infection modes)
      "PlaytimeIntervalMinutes": 1, //Playtime interval for rewards (minutes)
      "PlaytimeReward": 1, //Credits awarded per interval
      "DamageThreshold": 5000, //Damage dealt by humans required to earn credits (encourages attacking)
      "DamageReward": 1, //Credits awarded after reaching damage threshold
      "DamageTakenThreshold": 3000, //Damage taken as a zombie required to earn credits (encourages aggression)
      "DamageTakenReward": 1 //Credits awarded after reaching damage taken threshold
    },
    "ZombieClassRewards": [  // Extra rewards for specific zombie classes (optional)
      {
        "Enabled": true,  //Enable extra rewards for this zombie class
        "ClassName": "复仇之神", //Zombie name
        "InfectHumanBonus": 50, //Extra credits for infecting humans with this zombie
        "KillHumanBonus": 30, //Extra credits for killing humans with this zombie
        "KilledByHumanBonus": 100, //Extra credits for humans killing this zombie
        "DamageMultiplier": 2.0, /Damage dealt to this zombie multiplier
        "DamageTakenMultiplier": 3.0 //Damage taken as this zombie multiplier
      },
      {
        "Enabled": true,
        "ClassName": "母体僵尸",
        "InfectHumanBonus": 20,
        "KillHumanBonus": 25,
        "KilledByHumanBonus": 30,
        "DamageMultiplier": 1.5,
        "DamageTakenMultiplier": 1.1
      }
    ]
  }
}
```

Setting any reward value to 0 will disable that feature, and no rewards or messages will be triggered.
This plugin does not support hot-reloading configuration. Changes will take effect after a map change or server restart.
