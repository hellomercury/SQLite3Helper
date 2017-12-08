﻿/*
 * --->SQLite3 dataSyncBase table structure.<---
 * --->This class code is automatically generated。<---
 * --->If you need to modify, please place the custom code between <Self Code Begin> and <Self Code End>.
 *                                                                                    --szn
 */

namespace SQLite3Helper.DataStruct
{
    public enum ItemEnum
    {
        ID,
        Name,
        Icon,
        Des,
        Max
    }

    public class Item : SyncBase
    {
        private readonly int hashCode;

        [SQLite3Constraint(SQLite3Constraint.NotNull | SQLite3Constraint.PrimaryKey | SQLite3Constraint.Unique )]
        [Sync((int)ItemEnum.ID)]
        public int ID { get; private set; }  //Item ID

        [Sync((int)ItemEnum.Name)]
        public string Name { get; set; }  //Item name

        [Sync((int)ItemEnum.Icon)]
        public string Icon { get; set; }  //Item icon

        [Sync((int)ItemEnum.Des)]
        public string Des { get; set; }  //Item describe

        public Item()
        {
        }

        public Item(int InID, string InName, string InIcon, string InDes)
        {
            hashCode = InID;
            ID = InID;
            Name = InName;
            Icon = InIcon;
            Des = InDes;
        }

        //-------------------------------*Self Code Begin*-------------------------------
        //Custom code.
        //-------------------------------*Self Code End*   -------------------------------
        

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override string ToString()
        {
            return "Item : ID = " + ID+ ", Name = " + Name+ ", Icon = " + Icon+ ", Des = " + Des;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is Item && (InObj as Item).ID == ID;
        }
    }
}
