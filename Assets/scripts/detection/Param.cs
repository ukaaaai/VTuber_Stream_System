using System;
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

        public static Param FromByteArray(byte[] data)
        {
            if (data.Length < 24)
            {
                throw new Exception("Data length is not enough");
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

        private Param Clamp()
        {
            return new Param
            {
                ParamAngleX = Math.Clamp(ParamAngleX, -30, 30),
                ParamAngleY = Math.Clamp(ParamAngleY, -30, 30),
                ParamAngleZ = Math.Clamp(ParamAngleZ, -30, 30),
                ParamEyeLOpen = Math.Clamp(ParamEyeLOpen, 0, 1),
                ParamEyeROpen = Math.Clamp(ParamEyeROpen, 0, 1),
                ParamEyeBallX = Math.Clamp(ParamEyeBallX, -1, 1),
                ParamEyeBallY = Math.Clamp(ParamEyeBallY, -1, 1),
                ParamBrowLY = Math.Clamp(ParamBrowLY, -1, 1),
                ParamBrowRY = Math.Clamp(ParamBrowRY, -1, 1),
                ParamMouthForm = Math.Clamp(ParamMouthForm, -1, 1),
                ParamMouthOpenY = Math.Clamp(ParamMouthOpenY, 0, 1),
                ParamCheek = Math.Clamp(ParamCheek, 0, 1),
                ParamBreath = Math.Clamp(ParamBreath, 0, 1)
            };
        }
        
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        public static Param FromJson(string json)
        {
            return JsonUtility.FromJson<Param>(json);
        }

        public static Param operator -(Param param1, Param param2)
        {
            return new Param
            {
                ParamAngleX = param1.ParamAngleX - param2.ParamAngleX,
                ParamAngleY = param1.ParamAngleY - param2.ParamAngleY,
                ParamAngleZ = param1.ParamAngleZ - param2.ParamAngleZ,
                ParamEyeLOpen = param1.ParamEyeLOpen - param2.ParamEyeLOpen,
                ParamEyeROpen = param1.ParamEyeROpen - param2.ParamEyeROpen,
                ParamEyeBallX = param1.ParamEyeBallX - param2.ParamEyeBallX,
                ParamEyeBallY = param1.ParamEyeBallY - param2.ParamEyeBallY,
                ParamBrowLY = param1.ParamBrowLY - param2.ParamBrowLY,
                ParamBrowRY = param1.ParamBrowRY - param2.ParamBrowRY,
                ParamMouthForm = param1.ParamMouthForm - param2.ParamMouthForm,
                ParamMouthOpenY = param1.ParamMouthOpenY - param2.ParamMouthOpenY,
                ParamCheek = param1.ParamCheek,
                ParamBreath = param1.ParamBreath - param2.ParamBreath
            }.Clamp();
        }

        public static Param operator +(Param param1, Param param2)
        {
            return new Param
            {
                ParamAngleX = param1.ParamAngleX + param2.ParamAngleX,
                ParamAngleY = param1.ParamAngleY + param2.ParamAngleY,
                ParamAngleZ = param1.ParamAngleZ + param2.ParamAngleZ,
                ParamEyeLOpen = param1.ParamEyeLOpen + param2.ParamEyeLOpen,
                ParamEyeROpen = param1.ParamEyeROpen + param2.ParamEyeROpen,
                ParamEyeBallX = param1.ParamEyeBallX + param2.ParamEyeBallX,
                ParamEyeBallY = param1.ParamEyeBallY + param2.ParamEyeBallY,
                ParamBrowLY = param1.ParamBrowLY + param2.ParamBrowLY,
                ParamBrowRY = param1.ParamBrowRY + param2.ParamBrowRY,
                ParamMouthForm = param1.ParamMouthForm + param2.ParamMouthForm,
                ParamMouthOpenY = param1.ParamMouthOpenY + param2.ParamMouthOpenY,
                ParamCheek = param1.ParamCheek,
                ParamBreath = param1.ParamBreath + param2.ParamBreath
            }.Clamp();
        }
        
        public static Param operator *(Param param, double rate)
        {
            return new Param
            {
                ParamAngleX = param.ParamAngleX * (float)rate,
                ParamAngleY = param.ParamAngleY * (float)rate,
                ParamAngleZ = param.ParamAngleZ * (float)rate,
                ParamEyeLOpen = param.ParamEyeLOpen * (float)rate,
                ParamEyeROpen = param.ParamEyeROpen * (float)rate,
                ParamEyeBallX = param.ParamEyeBallX * (float)rate,
                ParamEyeBallY = param.ParamEyeBallY * (float)rate,
                ParamBrowLY = param.ParamBrowLY * (float)rate,
                ParamBrowRY = param.ParamBrowRY * (float)rate,
                ParamMouthForm = param.ParamMouthForm * (float)rate,
                ParamMouthOpenY = param.ParamMouthOpenY * (float)rate,
                ParamCheek = param.ParamCheek,
                ParamBreath = param.ParamBreath * (float)rate
            }.Clamp();
        }

        public static Param operator *(double rate, Param param)
        {
            return new Param
            {
                ParamAngleX = param.ParamAngleX * (float)rate,
                ParamAngleY = param.ParamAngleY * (float)rate,
                ParamAngleZ = param.ParamAngleZ * (float)rate,
                ParamEyeLOpen = param.ParamEyeLOpen * (float)rate,
                ParamEyeROpen = param.ParamEyeROpen * (float)rate,
                ParamEyeBallX = param.ParamEyeBallX * (float)rate,
                ParamEyeBallY = param.ParamEyeBallY * (float)rate,
                ParamBrowLY = param.ParamBrowLY * (float)rate,
                ParamBrowRY = param.ParamBrowRY * (float)rate,
                ParamMouthForm = param.ParamMouthForm * (float)rate,
                ParamMouthOpenY = param.ParamMouthOpenY * (float)rate,
                ParamCheek = param.ParamCheek,
                ParamBreath = param.ParamBreath * (float)rate
            }.Clamp();
        }
        
        public static Param operator /(Param param, double rate)
        {
            return new Param
            {
                ParamAngleX = param.ParamAngleX / (float)rate,
                ParamAngleY = param.ParamAngleY / (float)rate,
                ParamAngleZ = param.ParamAngleZ / (float)rate,
                ParamEyeLOpen = param.ParamEyeLOpen / (float)rate,
                ParamEyeROpen = param.ParamEyeROpen / (float)rate,
                ParamEyeBallX = param.ParamEyeBallX / (float)rate,
                ParamEyeBallY = param.ParamEyeBallY / (float)rate,
                ParamBrowLY = param.ParamBrowLY / (float)rate,
                ParamBrowRY = param.ParamBrowRY / (float)rate,
                ParamMouthForm = param.ParamMouthForm / (float)rate,
                ParamMouthOpenY = param.ParamMouthOpenY / (float)rate,
                ParamCheek = param.ParamCheek,
                ParamBreath = param.ParamBreath / (float)rate
            }.Clamp();
        }

        public static Param operator /(double rate, Param param)
        {
            return new Param
            {
                ParamAngleX = param.ParamAngleX / (float)rate,
                ParamAngleY = param.ParamAngleY / (float)rate,
                ParamAngleZ = param.ParamAngleZ / (float)rate,
                ParamEyeLOpen = param.ParamEyeLOpen / (float)rate,
                ParamEyeROpen = param.ParamEyeROpen / (float)rate,
                ParamEyeBallX = param.ParamEyeBallX / (float)rate,
                ParamEyeBallY = param.ParamEyeBallY / (float)rate,
                ParamBrowLY = param.ParamBrowLY / (float)rate,
                ParamBrowRY = param.ParamBrowRY / (float)rate,
                ParamMouthForm = param.ParamMouthForm / (float)rate,
                ParamMouthOpenY = param.ParamMouthOpenY / (float)rate,
                ParamCheek = param.ParamCheek,
                ParamBreath = param.ParamBreath / (float)rate
            }.Clamp();
        }
    }
}
