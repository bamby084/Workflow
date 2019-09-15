using System;

namespace CSVRealese
{

    public enum EnumDataType
    {
       // Number = 0,
        String = 1,
        Date = 2,
        Boolean = 3,
        Integer = 4,
        LongNumber = 5
    }
    public enum EnumTrimType
    {
        None = 0,
        Left = 1,
        Right = 2,
        Both = 3
    }
    public class MetaDataClass
    {
        public String Name { get; set; }
        public EnumDataType DataType { get; set; }
       // public String strFormat { get; set; }
        public int StartPos { get; set; }
        public int Length { get; set; }
       // public int Precision { get; set; }
        public EnumTrimType TrimSpaces { get; set; }
        public string OpenFileType { get; set; }
        public string RootArrayName { get; set; }
        public string InputFile { get; set; }
        public string FileEncoding { get; set; }
        public string DataTypeStr { get; set; }
    }

    public class GirdColumnClass
    {
        public String Name { get; set; }
        public EnumDataType DataType { get; set; }
        public int StartPos { get; set; }
        public int Length { get; set; }
        public EnumTrimType TrimSpaces { get; set; }
   
    }

    public class MetaDataClassUpdated
    {
        public String Name { get; set; }
      //  public EnumDataType DataType { get; set; }
        // public String strFormat { get; set; }
        public int StartPos { get; set; }
        public int Length { get; set; }
        // public int Precision { get; set; }
        public EnumTrimType TrimSpaces { get; set; }
        public string OpenFileType { get; set; }
        public string RootArrayName { get; set; }
        public string InputFile { get; set; }
        public string FileEncoding { get; set; }
        public string DataTypeStr { get; set; }

    }
}