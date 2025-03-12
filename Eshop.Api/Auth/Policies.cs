namespace Eshop.Api.Auth;

public static class Policies
{
    public const string StaffReadAccess = "staff_read_access";
    public const string StaffWriteAccess = "staff_write_access";

    public const string AdminReadAccess = "admin_read_access";
    public const string AdminWriteAccess = "admin_write_access";
}