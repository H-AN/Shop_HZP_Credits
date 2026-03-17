<div align="center"><h1><img width="600" height="131" alt="68747470733a2f2f70616e2e73616d7979632e6465762f732f56596d4d5845" src="https://github.com/user-attachments/assets/d0316faa-c2d0-478f-a642-1e3c3651f1d4" /></h1></div>

<div class="section">
<div align="center"><h1>Shop_HZP_Credits for Swiftly2 ShopCore</h1></div>

<div align="center">
  <a href="./README.md"><img src="https://flagcdn.com/48x36/cn.png" alt="中文" width="48" height="36" /> <strong>中文版</strong></a>  
  &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <a href="./README.en.md"><img src="https://flagcdn.com/48x36/gb.png" alt="English" width="48" height="36" /> <strong>English</strong></a>
</div>

<hr>

<div align="center"><strong>基于 Swiftly2 框架开发的 CS2 僵尸瘟疫 商店点数发放。</p></div>
<div align="center"><strong>支持多种配置发放积分</p></div>
<div align="center"><strong>使用此插件必须同时安装ShopCore 与 HanZombiePlagueS2 </p></div>
<div align="center"><strong>HanZombiePlagueS2 : https://github.com/H-AN/HanZombiePlagueS2 </p></div>
<div align="center"><strong>ShopCore : https://github.com/SwiftlyS2-Plugins/ShopCore </p></div>
</div>

---

插件配置 : 

```
{
  "ShopHZPCreditsCFG": {
    "Enabled": true,  //全局开关
    "InfoCommand": "sw_money", //信息显示指令
    "Global": {
      "InfectReward": 10,  //作为丧尸感染玩家获取积分
      "HumanWinReward": 20, //人类胜利全体人类获取积分
      "ZombieWinReward": 5, //丧尸胜利全体丧尸获取积分
      "KillZombieReward": 5, //击杀丧尸获取积分
      "KillHumanReward": 5, //丧尸击杀人类获取积分 (军团模式于非感染模式)
      "PlaytimeIntervalMinutes": 1, //玩家在线时长获取积分 单位/分 
      "PlaytimeReward": 1, //在线时长达到后给予积分量
      "DamageThreshold": 5000, //人类造成多少伤害后可以获取积分 (鼓励人类攻击)
      "DamageReward": 1, //人类造成伤害达标后可以发放的数量
      "DamageTakenThreshold": 3000, //作为丧尸承受多少伤害后可以获得积分 (鼓励丧尸进攻)
      "DamageTakenReward": 1 //作为丧尸承受伤害达标后可以获得积分数量
    },
    "ZombieClassRewards": [  // 特殊丧尸额外积分奖励 (可选)
      {
        "Enabled": true,  //是否开启此丧尸组额外获取积分
        "ClassName": "复仇之神", //丧尸名称
        "InfectHumanBonus": 50, //使用此丧尸感染人类时可以获取的额外积分
        "KillHumanBonus": 30, //使用此丧尸击杀人类可以获取的额外积分
        "KilledByHumanBonus": 100, //人类击杀此丧尸可以额外获取的积分
        "DamageMultiplier": 2.0, //攻击此丧尸可以积累伤害量的倍率
        "DamageTakenMultiplier": 3.0 //作为此丧尸承受伤害的倍率
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
以上积分发放 填写为0代表此功能关闭,不会触发提示与积分发放.
此插件不支持配置热重载,更改配置后更换地图或者重启服务器生效
