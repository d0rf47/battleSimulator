using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSimOne.Models
{
	public enum TypesEnum : int
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

    public static class TypesEnumExtension
    {
        
        public static double Effectiveness(this TypesEnum type1, TypesEnum type2)
        {
			double effeciveness = 1.0;

            switch (type1)
            {
                case TypesEnum.Fire:
                    {
                        if (type2 == TypesEnum.Grass) { effeciveness = 2.0; }
                        if (type2 == TypesEnum.Ice) { effeciveness = 2.0; }
                        if (type2 == TypesEnum.Water) { effeciveness = 0.5; }
                        break;
                    }
                case TypesEnum.Grass:
                    {
                        if (type2 == TypesEnum.Fire) { effeciveness = 0.5; }
                        if (type2 == TypesEnum.Ground) { effeciveness = 2.0; }
                        break;
                    }
                case TypesEnum.Ground:
                    {
                        if (type2 == TypesEnum.Electric) { effeciveness = 2.0; }
                        if (type2 == TypesEnum.Grass) { effeciveness = 0.5; }
                        break;
                    }
                case TypesEnum.Electric:
                    {
                        if (type2 == TypesEnum.Ground) { effeciveness = 0.5; }
                        if (type2 == TypesEnum.Water) { effeciveness = 2.0; }
                        break;
                    }
                case TypesEnum.Water:
                    {
                        if (type2 == TypesEnum.Fire) { effeciveness = 2.0; }
                        if (type2 == TypesEnum.Electric) { effeciveness = 0.5; }
                        break;
                    }
                case TypesEnum.Ice:
                    {
                        if (type2 == TypesEnum.Fire) { effeciveness = 0.5; }
                        if (type2 == TypesEnum.Dragon) { effeciveness = 2.0; }
                        break;
                    }
                case TypesEnum.Dragon:
                    {
                        if (type2 == TypesEnum.Ice) { effeciveness = 0.5; }
                        if (type2 == TypesEnum.Dark) { effeciveness = 2.0; }
                        break;
                    }
                case TypesEnum.Dark:
                    {
                        if (type2 == TypesEnum.Dragon) { effeciveness = 0.5; }
                        if (type2 == TypesEnum.Neutral) { effeciveness = 2.0; }
                        break;
                    }
                case TypesEnum.Neutral:
                    {
                        if (type2 == TypesEnum.Dark) { effeciveness = 0.5; }
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
