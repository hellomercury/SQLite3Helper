/*
 * --->SQLite3 dataSyncBase table structure.<---
 * --->This class code is automatically generated。<---
 * --->If you need to modify, please place the custom code between <Self Code Begin> and <Self Code End>.
 *                                                                                    --szn
 */

using Framework.Reflection.SQLite3Helper;
using Framework.Reflection.Sync;


namespace SQLite3TableDataTmpl
{
    public enum ItemEnum
    {
        ID,
        Name,
        Icon,
        Des,
        PileUpperLimit,
        Skill,
        ItemBuyInfo,
        UnlockInfo,
        Max
    }

    public class Item : SyncBase
    {
        [SQLite3Constraint(SQLite3Constraint.Unique | SQLite3Constraint.NotNull )]
        [Sync((int)ItemEnum.ID)]
        public int ID { get; private set; }  //物品ID

        [Sync((int)ItemEnum.Name)]
        public string Name { get; set; }  //物品名称

        [Sync((int)ItemEnum.Icon)]
        public string Icon { get; set; }  //物品图标

        [Sync((int)ItemEnum.Des)]
        public string Des { get; set; }  //物品描述

        [Sync((int)ItemEnum.PileUpperLimit)]
        public int PileUpperLimit { get; set; }  //最大堆叠数量

        [Sync((int)ItemEnum.Skill)]
        public int Skill { get; set; }  //技能

        [Sync((int)ItemEnum.ItemBuyInfo)]
        public int ItemBuyInfo { get; set; }  //物品购买信息

        [Sync((int)ItemEnum.UnlockInfo)]
        public bool UnlockInfo { get; set; }  //解锁信息

        public Item()
        {
        }

        public Item(int InID, string InName, string InIcon, string InDes, int InPileUpperLimit, int InSkill, int InItemBuyInfo, bool InUnlockInfo)
        {
            ID = InID;
            Name = InName;
            Icon = InIcon;
            Des = InDes;
            PileUpperLimit = InPileUpperLimit;
            Skill = InSkill;
            ItemBuyInfo = InItemBuyInfo;
            UnlockInfo = InUnlockInfo;
        }

        //-------------------------------*Self Code Begin*-------------------------------
        //Custom code.
        //-------------------------------*Self Code End*   -------------------------------
        

        public override string ToString()
        {
            return "Item : ID = " + ID+ ", Name = " + Name+ ", Icon = " + Icon+ ", Des = " + Des+ ", PileUpperLimit = " + PileUpperLimit+ ", Skill = " + Skill+ ", ItemBuyInfo = " + ItemBuyInfo+ ", UnlockInfo = " + UnlockInfo;
        }

    }
}
