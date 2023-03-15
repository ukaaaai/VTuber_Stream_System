using UnityEngine;

namespace detection
{
    public struct Param
    {
        public float ParamAngleX;
        public float ParamAngleY;
        public float ParamAngleZ;
        public float ParamEyeLOpen;
        public float ParamEyeROpen;
        public float ParamEyeBallX;
        public float ParamEyeBallY;
        public float ParamBrowLY;
        public float ParamBrowRY;
        public float ParamMouthForm;
        public float ParamMouthOpenY;
        public float ParamCheek;
        public float ParamBreath;
        private const int Offset = 128;
        private const int AdjRate = 127;

        public byte[] ToDataArray()
        {
            var data = new[]
            {
                (byte)((int)ParamAngleX * AdjRate / 30 + Offset),
                (byte)((int)ParamAngleY * AdjRate / 30 + Offset),
                (byte)((int)ParamAngleZ * AdjRate / 30 + Offset),
                (byte)((int)ParamEyeLOpen * AdjRate),
                (byte)((int)ParamEyeROpen * AdjRate),
                (byte)((int)ParamEyeBallX * AdjRate + Offset),
                (byte)((int)ParamEyeBallY * AdjRate + Offset),
                (byte)((int)ParamBrowLY * AdjRate + Offset),
                (byte)((int)ParamBrowRY * AdjRate + Offset),
                (byte)((int)ParamMouthForm * AdjRate + Offset),
                (byte)((int)ParamMouthOpenY * AdjRate),
                (byte)((int)ParamCheek * AdjRate),
                (byte)((int)ParamBreath * AdjRate)
            };
            return data;
        }

        public static Param? FromByteArray(byte[] data)
        {
            if (data.Length < 24)
            {
                return null;
            }
            var param = new Param
            {
                ParamAngleX = data[0] * 30 / AdjRate - Offset,
                ParamAngleY = data[1] * 30 / AdjRate - Offset,
                ParamAngleZ = data[2] * 30 / AdjRate - Offset,
                ParamEyeLOpen = data[3] / (float)AdjRate,
                ParamEyeROpen = data[4] / (float)AdjRate,
                ParamEyeBallX = data[5] / (float)AdjRate - Offset,
                ParamEyeBallY = data[6] / (float)AdjRate - Offset,
                ParamBrowLY = data[7] / (float)AdjRate - Offset,
                ParamBrowRY = data[8] / (float)AdjRate - Offset,
                ParamMouthForm = data[9] / (float)AdjRate - Offset,
                ParamMouthOpenY = data[10] / (float)AdjRate,
                ParamCheek = data[11] / (float)AdjRate,
                ParamBreath = data[12] / (float)AdjRate
            };
            return param;
        }
        
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
