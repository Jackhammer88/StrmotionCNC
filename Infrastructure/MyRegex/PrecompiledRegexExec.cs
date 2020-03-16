using PrecompiledRegex;

namespace Infrastructure.MyRegex
{
    public static class PrecompiledRegexExec
    {
        public static CNCRegex General { get; } = new CNCRegex();
        public static CNCGCodes GCodes { get; } = new CNCGCodes();
        public static CNCMCodes MCodes { get; } = new CNCMCodes();
        public static CNCFCode FCodes { get; } = new CNCFCode();
        public static CNCTCode TCode { get; } = new CNCTCode();
        public static CNCSCode SCode { get; } = new CNCSCode();
        public static CNCCoordinates Coordinates { get; } = new CNCCoordinates();
        public static CNCArcValues Arcs { get; } = new CNCArcValues();
    }
}
