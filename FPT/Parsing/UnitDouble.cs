﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Parsing
{
    public class UnitDouble
    {
        public const string FORMATTING_STRING_DEFAULT = "###,###,###,###,##0.############";

        public UnitDouble(UnitDouble copy)
        {
            this.UnitType = copy.UnitType;
            this.Value = copy.Value;
            this.Unit = copy.Unit;
            this.DesiredUnit = copy.DesiredUnit;
            this.Converter = copy.Converter;
            this.Reduce = copy.Reduce;
        }

        public UnitTypes UnitType { get; set; }

        public decimal Value { get; set; }

        public Enum Unit { get; set; }

        /// <summary>
        /// The unit you wish to have printed out instead of being reduced
        /// </summary>
        public Enum DesiredUnit { get; set; }

        public UnitConverter Converter { get; set; }

        public bool Reduce { get; set; }

        public UnitDouble(decimal value)
        {
            this.Value = value;
            this.Reduce = true;
        }

        public UnitDouble(decimal value, UnitTypes unitType, Enum unit, UnitConverter convert)
            : this(value)
        {
            this.UnitType = unitType;
            this.Converter = convert;
            this.Unit = unit;
        }

        public override string ToString()
        {
            var value = new UnitDouble(this);

            if (this.Converter != null && Reduce)
            {
                value = this.Converter.GetReducedUnit(this);
            }
            else if (this.Converter != null && !Reduce)
            {
                value.Value = this.Converter.Convert(value.Value, Converter.BaseUnit, value.DesiredUnit);
                value.Unit = value.DesiredUnit;
            }

            string formatting = UnitDouble.FORMATTING_STRING_DEFAULT + " ";

            if (Reduce && value.Value > 10000000000000000)
            {
                formatting = "E";
            }

            if (value.UnitType == UnitTypes.Currency)
            {
                formatting = "###,###,###,###,##0.############ ;-###,###,###,###,##0.############ ";
                value.Value = Math.Round(value.Value, 2);
            }

            if (value.UnitType == UnitTypes.Hexadecimal)
            {
                return "0x" + ((long)value.Value).ToString("X");
            }
            else if (value.UnitType == UnitTypes.Octal)
            {
                return "0" + Convert.ToString(((long)value.Value), 8);
            }
            else if (value.UnitType == UnitTypes.Binary)
            {
                return "0b" + Convert.ToString(((long)value.Value), 2);
            }

            value.Value = Math.Round(value.Value, 6);
            string unitlabel = String.Empty;

            if (this.Converter != null)
            {
                unitlabel = value.Unit.ToString();


                if (value.Value != 1)
                {
                    var temp = value.Unit.GetAttributeOfType<UnitPluralAttribute>();

                    if (temp != null)
                    {
                        unitlabel = temp.FirstOrDefault().Plural;
                    }
                }
                else
                {
                    var templbl = value.Unit.GetAttributeOfType<DisplayAttribute>();
                    if (templbl != null)
                    {
                        unitlabel = templbl.FirstOrDefault().Display;
                    }
                }
            }

            return (value.Value.ToString(formatting) + (this.Converter != null ? unitlabel : "")).Trim();
        }

        #region Operators


        public static UnitDouble operator +(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble(left.Value + right.Value, unittype, unit, convert);
        }
        public static UnitDouble operator -(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble(left.Value - right.Value, unittype, unit, convert);
        }
        public static UnitDouble operator /(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble(left.Value / right.Value, unittype, unit, convert);
        }
        public static UnitDouble operator %(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble(left.Value % right.Value, unittype, unit, convert);
        }
        public static UnitDouble operator *(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble(left.Value * right.Value, unittype, unit, convert);
        }
        public static UnitDouble operator &(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble((decimal)((long)left.Value & (long)right.Value), unittype, unit, convert);
        }
        public static UnitDouble operator |(UnitDouble left, UnitDouble right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            if (false == confirmCompatibleTypes(left, right, out convert, out unittype, out unit))
            {
                throw new Exception(left.UnitType.ToString() + right.UnitType.ToString());
            }

            return new UnitDouble((decimal)((long)left.Value | (long)right.Value), unittype, unit, convert);
        }
        public static UnitDouble operator <<(UnitDouble left, int right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            confirmCompatibleTypes(left, null, out convert, out unittype, out unit);

            return new UnitDouble((decimal)((long)left.Value << right), unittype, unit, convert);
        }
        public static UnitDouble operator >>(UnitDouble left, int right)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            confirmCompatibleTypes(left, null, out convert, out unittype, out unit);

            return new UnitDouble((decimal)((long)left.Value >> right), unittype, unit, convert);
        }
        public static UnitDouble operator ~(UnitDouble left)
        {
            UnitConverter convert;
            UnitTypes unittype;
            Enum unit;

            confirmCompatibleTypes(left, null, out convert, out unittype, out unit);

            return new UnitDouble((decimal)(~(long)left.Value), unittype, unit, convert);
        }
        #endregion

        private static bool confirmCompatibleTypes(UnitDouble left, UnitDouble right, out UnitConverter convert, out UnitTypes unittype, out Enum unit)
        {
            if (right != null && left.Converter != right.Converter && right.UnitType != UnitTypes.None && left.UnitType != UnitTypes.None)
            {
                convert = null;
                unittype = UnitTypes.None;
                unit = null;
                return false;
            }

            convert = (left.UnitType == UnitTypes.None && right != null ? right.Converter : left.Converter);
            unittype = (left.UnitType == UnitTypes.None && right != null ? right.UnitType : left.UnitType);
            unit = (left.UnitType == UnitTypes.None && right != null ? right.Unit : left.Unit);

            return true;
        }
    }
}
