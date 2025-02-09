namespace BattleSimulatorAPI.Repositories.Models.Enums
{
    public enum ElementTypeEnum : int
    {
        Fire = 1,
        Grass = 2,
        Ground = 3,
        Electric = 4,
        Water = 5,
        Ice = 6,
        Dragon = 7,
        Dark = 8,
        Neutral = 9
    }

    public static class ElementTypeEnumExtension
    {

        public static double Effectiveness(this ElementTypeEnum type1, ElementTypeEnum type2)
        {
            double effeciveness = 1.0;

            switch (type1)
            {
                case ElementTypeEnum.Fire:
                    {
                        if (type2 == ElementTypeEnum.Grass) { effeciveness = 2.0; }
                        if (type2 == ElementTypeEnum.Ice) { effeciveness = 2.0; }
                        if (type2 == ElementTypeEnum.Water) { effeciveness = 0.5; }
                        break;
                    }
                case ElementTypeEnum.Grass:
                    {
                        if (type2 == ElementTypeEnum.Fire) { effeciveness = 0.5; }
                        if (type2 == ElementTypeEnum.Ground) { effeciveness = 2.0; }
                        break;
                    }
                case ElementTypeEnum.Ground:
                    {
                        if (type2 == ElementTypeEnum.Electric) { effeciveness = 2.0; }
                        if (type2 == ElementTypeEnum.Grass) { effeciveness = 0.5; }
                        break;
                    }
                case ElementTypeEnum.Electric:
                    {
                        if (type2 == ElementTypeEnum.Ground) { effeciveness = 0.5; }
                        if (type2 == ElementTypeEnum.Water) { effeciveness = 2.0; }
                        break;
                    }
                case ElementTypeEnum.Water:
                    {
                        if (type2 == ElementTypeEnum.Fire) { effeciveness = 2.0; }
                        if (type2 == ElementTypeEnum.Electric) { effeciveness = 0.5; }
                        break;
                    }
                case ElementTypeEnum.Ice:
                    {
                        if (type2 == ElementTypeEnum.Fire) { effeciveness = 0.5; }
                        if (type2 == ElementTypeEnum.Dragon) { effeciveness = 2.0; }
                        break;
                    }
                case ElementTypeEnum.Dragon:
                    {
                        if (type2 == ElementTypeEnum.Ice) { effeciveness = 0.5; }
                        if (type2 == ElementTypeEnum.Dark) { effeciveness = 2.0; }
                        break;
                    }
                case ElementTypeEnum.Dark:
                    {
                        if (type2 == ElementTypeEnum.Dragon) { effeciveness = 0.5; }
                        if (type2 == ElementTypeEnum.Neutral) { effeciveness = 2.0; }
                        break;
                    }
                case ElementTypeEnum.Neutral:
                    {
                        if (type2 == ElementTypeEnum.Dark) { effeciveness = 0.5; }
                        break;
                    }
                default:
                    {
                        effeciveness = 1.0;
                        break;
                    }
            }


            return effeciveness;
        }
    }
}
