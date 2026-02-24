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

        public static List<string> GenerateAllPermissions()
        {
            var AllPermissions = new List<string>();
            var modules = Enum.GetValues(typeof(Modules));
            foreach (var module in modules)
            {
                AllPermissions.AddRange(GeneratePermissionsList(module.ToString()));
            }
            return AllPermissions;
        }

        public static class Employees
        {
            public const string View = "Permissions.Employees.View";
            public const string Add = "Permissions.Employees.Add";
            public const string Edit = "Permissions.Employees.Edit";
            public const string Delete = "Permissions.Employees.Delete";
        }
        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Add = "Permissions.Departments.Add";
            public const string Edit = "Permissions.Departments.Edit";
            public const string Delete = "Permissions.Departments.Delete";
        }
        public static class GeneralSettings
        {
            public const string View = "Permissions.GeneralSettings.View";
            public const string Add = "Permissions.GeneralSettings.Add";
            public const string Edit = "Permissions.GeneralSettings.Edit";
            public const string Delete = "Permissions.GeneralSettings.Delete";
        }
        public static class OfficialHolidays
        {
            public const string View = "Permissions.OfficialHolidays.View";
            public const string Add = "Permissions.OfficialHolidays.Add";
            public const string Edit = "Permissions.OfficialHolidays.Edit";
            public const string Delete = "Permissions.OfficialHolidays.Delete";
        }
        public static class AttendAndLeave
        {
            public const string View = "Permissions.AttendAndLeave.View";
            public const string Add = "Permissions.AttendAndLeave.Add";
            public const string Edit = "Permissions.AttendAndLeave.Edit";
            public const string Delete = "Permissions.AttendAndLeave.Delete";
        }
        public static class Payroll
        {
            public const string View = "Permissions.Payroll.View";
            public const string Add = "Permissions.Payroll.Add";
            public const string Edit = "Permissions.Payroll.Edit";
            public const string Delete = "Permissions.Payroll.Delete";
        }
    }
}
