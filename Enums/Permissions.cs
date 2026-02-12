namespace Enums
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsList(string module)
        {
            return new List<string>
        {
            $"Permissions.{module}.View",
            $"Permissions.{module}.Add",
            $"Permissions.{module}.Edit",
            $"Permissions.{module}.Delete"
        };
        }
    }
}
