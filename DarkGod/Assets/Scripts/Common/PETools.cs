using System;

namespace Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class PETools
    {
        public static int RDInt(int min, int max, Random rd = null)
        {
            rd ??= new Random();
            int val = rd.Next(min, max + 1);
            return val;
        }

        public static float RDFloat(float min, float max, Random rd = null)
        {
            rd ??= new Random();
            float val = min + (float)rd.NextDouble() * (max - min);
            return val;
        }
    }
}