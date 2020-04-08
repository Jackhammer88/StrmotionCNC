namespace Infrastructure.Constants
{
    public static class PVariables
    {
        public const string FeedrateValue = "P8151";

        public const string MaximalCuttingPower = "P1100";
        public const string MinimalCuttingPower = "P1101";

        public const string CuttingSpeed = "P1103";
        public const string GravSpeed = "P2203";

        public const string MinimalGravPower = "P2201";
        public const string MaximalGravPower = "P2200";

        public const string Y2Offset = "P8014";

        public const string BurnTime = "P1407";

        public const string Shooting = "P7028";

        public const string PBJogMinusFirstMotor = "P7012";
        public const string PBJogPlusFirstMotor = "P7013";
        public const string PBJogMinusSecondMotor = "P7014";
        public const string PBJogPlusSecondMotor = "P7015";

        public const string PBStart = "P7005";
        public const string PBStop = "P7006";

        public const string PManualButtonState = "P7001";
        public const string PBAutoButtonState = "P7002";
        public const string PMDIButtonState = "P7003";
        public const string Homing = "P7004";
        public const string PBTestButtonState = "P7011";

        public const string MachineStateInternal = "P8147";

        //TODO: Назначить переменную
        public const string LaserFocusChangingRequired = "P1111";
        public const string LaserFocusChanging = "P1112";
        public const string LaserCurrentFocus = "P1112";
    }
}
