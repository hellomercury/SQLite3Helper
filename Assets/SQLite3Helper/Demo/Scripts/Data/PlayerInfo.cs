/*
 * --->SQLite3 dataSyncBase table structure.<---
 * --->This class code is automatically generated。<---
 * --->If you need to modify, please place the custom code between <Self Code Begin> and <Self Code End>.
 *                                                                                    --szn
 */

namespace SQLite3Helper.DataStruct
{
    public enum PlayerInfoEnum
    {
        ID,
        name,
        iconid,
        isNewPlayer,
        Max
    }

    public class PlayerInfo : SyncBase
    {
        private readonly int hashCode;

        [SQLite3Constraint(SQLite3Constraint.PrimaryKey )]
        [Sync((int)PlayerInfoEnum.ID)]
        public int ID { get; private set; }  //index

        [Sync((int)PlayerInfoEnum.name)]
        public string name { get; set; }  //player name

        [Sync((int)PlayerInfoEnum.iconid)]
        public int iconid { get; set; }  //avatar id

        [Sync((int)PlayerInfoEnum.isNewPlayer)]
        public bool isNewPlayer { get; set; }  //is new player

        public PlayerInfo()
        {
        }

        public PlayerInfo(int InID, string Inname, int Iniconid, bool InisNewPlayer)
        {
            hashCode = InID;
            ID = InID;
            name = Inname;
            iconid = Iniconid;
            isNewPlayer = InisNewPlayer;
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
            return "PlayerInfo : ID = " + ID+ ", name = " + name+ ", iconid = " + iconid+ ", isNewPlayer = " + isNewPlayer;
        }

        public override bool Equals(object InObj)
        {
            if (null == InObj) return false;
            else return InObj is PlayerInfo && (InObj as PlayerInfo).ID == ID;
        }
    }
}
