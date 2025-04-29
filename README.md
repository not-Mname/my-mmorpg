# mmorpg

#### 介绍
{
**这是一个MMORPG游戏的demo**
一款3D卡通风格的MMORPG游戏，类似于魔兽世界，拥有法师、战士、弓箭手三种职业，游戏拥有主城、野外、副本、竞技场等场景。游戏以角色养成、PVE副本配合、PVP对战为核心玩法。
}

#### 游戏架构
该游戏采用前后端分离架构，客户端基于Unity2018， 服务端基于.Net framework 3.5，数据库使用了SQL Server 2017，使用了Entity Framework用来对象关系映射，协议使用了Protobuf，日志使用了Log4net，Json解析运用了Newtonsoft.Json。
该游戏采用定制框架，采用类MVC思想将业务逻辑分为多层。Service层处理服务端与客户端之间网络收发，其中大部分Service都是一一对应的，如：FriendService客户端服务端都有，但是DBService是服务端独有的其就是对Entity Framework进行了简单的封装；Manager层用于管理游戏的各种实体或实现业务并负责控制Unity的大部分系统；Entities层用于存储游戏中一切实体的数据；Model层用于存储被管理层管理的对象；GameObject目录是客户端独有的，它主要是定义了游戏中实体控制器和一些其他的工具；

