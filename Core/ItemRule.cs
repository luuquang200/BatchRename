using System;

namespace Core
{
    public class ItemRule: ICloneable
    {
        public string NameRule { get; set; } = "";
        public string Data { get; set; } = "";

        public object Clone()
        {
           return MemberwiseClone();
        }
    }
}