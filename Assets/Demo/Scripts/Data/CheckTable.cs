namespace SQLite3Helper.DataStruct
{

    public enum CheckTableEnum
    {
        ID,
        Name,
        Description,
        Max
    }

    public class CheckTable : SyncBase
    {
        [SQLite3Constraint(SQLite3Constraint.AutoIncrement)]
        [Sync((int)CheckTableEnum.ID)]
        public int ID { get; set; }

        [Sync((int)CheckTableEnum.Name)]
        public string Name { get; set; }

        [Sync((int)CheckTableEnum.Description)]
        public string Description { get; set; }


        public CheckTable()
        {

        }


        public CheckTable(int InID, string InName, string InDescription)
        {
            ID = InID;
            Name = InName;
            Description = InDescription;
        }
    }   
}