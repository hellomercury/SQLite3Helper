namespace Framework.Reflection.SQLite3Helper
{
    public class SQLite3Utility
    {
        public static string ConvertSQLite3ConstraintToStr(SQLite3Constraint InConstraint)
        {
            string result = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) != 0)
                result += " PrimaryKey ";
            if ((InConstraint & SQLite3Constraint.Unique) != 0)
                result += " Unique ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) != 0)
                result += " AutoIncrement ";
            if ((InConstraint & SQLite3Constraint.NotNull) != 0)
                result += " NotNull ";

            return result == string.Empty ? string.Empty : result.Remove(result.Length - 1, 1);
        }
    }
}