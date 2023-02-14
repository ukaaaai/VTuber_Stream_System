namespace detection
{
    public struct Param
    {
        public sbyte ParamAngleX;
        public sbyte ParamAngleY;
        public sbyte ParamAngleZ;
        public sbyte ParamEyeLOpen;
        public sbyte ParamEyeLSmile;
        public sbyte ParamEyeROpen;
        public sbyte ParamEyeRSmile;
        public sbyte ParamEyeBallX;
        public sbyte ParamEyeBallY;
        public sbyte ParamBrowLY;
        public sbyte ParamBrowRY;
        public sbyte ParamBrowLX;
        public sbyte ParamBrowRX;
        public sbyte ParamBrowLAngle;
        public sbyte ParamBrowRAngle;
        public sbyte ParamBrowLForm;
        public sbyte ParamBrowRForm;
        public sbyte ParamMouthForm;
        public sbyte ParamMouthOpenY;
        public sbyte ParamCheek;
        public sbyte ParamBodyAngleX;
        public sbyte ParamBodyAngleY;
        public sbyte ParamBodyAngleZ;
        public sbyte ParamBreath;

        public byte[] ToDataArray()
        {
            var data = new[]
            {
                (byte)ParamAngleX,
                (byte)ParamAngleY,
                (byte)ParamAngleZ,
                (byte)ParamEyeLOpen,
                (byte)ParamEyeLSmile,
                (byte)ParamEyeROpen,
                (byte)ParamEyeRSmile,
                (byte)ParamEyeBallX,
                (byte)ParamEyeBallY,
                (byte)ParamBrowLY,
                (byte)ParamBrowRY,
                (byte)ParamBrowLX,
                (byte)ParamBrowRX,
                (byte)ParamBrowLAngle,
                (byte)ParamBrowRAngle,
                (byte)ParamBrowLForm,
                (byte)ParamBrowRForm,
                (byte)ParamMouthForm,
                (byte)ParamMouthOpenY,
                (byte)ParamCheek,
                (byte)ParamBodyAngleX,
                (byte)ParamBodyAngleY,
                (byte)ParamBodyAngleZ,
                (byte)ParamBreath
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
                ParamAngleX = (sbyte)data[0],
                ParamAngleY = (sbyte)data[1],
                ParamAngleZ = (sbyte)data[2],
                ParamEyeLOpen = (sbyte)data[3],
                ParamEyeLSmile = (sbyte)data[4],
                ParamEyeROpen = (sbyte)data[5],
                ParamEyeRSmile = (sbyte)data[6],
                ParamEyeBallX = (sbyte)data[7],
                ParamEyeBallY = (sbyte)data[8],
                ParamBrowLY = (sbyte)data[9],
                ParamBrowRY = (sbyte)data[10],
                ParamBrowLX = (sbyte)data[11],
                ParamBrowRX = (sbyte)data[12],
                ParamBrowLAngle = (sbyte)data[13],
                ParamBrowRAngle = (sbyte)data[14],
                ParamBrowLForm = (sbyte)data[15],
                ParamBrowRForm = (sbyte)data[16],
                ParamMouthForm = (sbyte)data[17],
                ParamMouthOpenY = (sbyte)data[18],
                ParamCheek = (sbyte)data[19],
                ParamBodyAngleX = (sbyte)data[20],
                ParamBodyAngleY = (sbyte)data[21],
                ParamBodyAngleZ = (sbyte)data[22],
                ParamBreath = (sbyte)data[23]
            };
            return param;
        }
    }
}
