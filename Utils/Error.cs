using System;
using System.ComponentModel;
using System.Reflection;

namespace NetLib
{
    public enum ConnErrors
    {
        [Description("IP位址無效")]
        AddressInvalid,
        [Description("Net協定不正確")]
        NetworkInvalid,
        [Description("Protocal不支援")]
        NotSupportProtocal,
        [Description("Socket型態不正確")]
        SocketTypeIsNotCorrect,
    }

    public enum ServerErrors
    {
        [Description("There are same name between two handlers.")]
        HandlerSameName,
        [Description("Not enough connection count can use")]
        ConnectionCountNotEnough
    }

    public enum UtilErrors
    {
        [Description("Address格式不支援，以後會支援")]
        AddressNotSupport
    }

    public static class ErrorHelper
    {
        public static string GetErrorsDescription(this Enum value)
        {

            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes.Length > 0 ?
                   attributes[0].Description : value.ToString();
        }
    }
}
