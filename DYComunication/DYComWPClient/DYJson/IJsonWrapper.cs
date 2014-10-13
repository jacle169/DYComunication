using System.Collections;


namespace DYComWPClient
{
    /// <summary> 
    /// dyjson���ͼ�
    /// </summary> 
    public enum JsonType
    {
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        None,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Object,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Array,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        String,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Int,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Long,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Double,
        /// <summary> 
        /// dyjson���ͼ�
        /// </summary> 
        Boolean
    }

    /// <summary> 
    /// dyjson��ͼ
    /// </summary> 
    public interface IJsonWrapper : IList, IDictionary
    {
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsArray   { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsBoolean { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsDouble  { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsInt     { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsLong    { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsObject  { get; }
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool IsString  { get; }

        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        bool     GetBoolean ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        double   GetDouble ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        int      GetInt ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        JsonType GetJsonType ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        long     GetLong ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        string   GetString ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetBoolean  (bool val);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetDouble   (double val);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetInt      (int val);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetJsonType (JsonType type);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetLong     (long val);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void SetString   (string val);
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        string ToJson ();
        /// <summary> 
        /// dyjson��ͼ
        /// </summary> 
        void   ToJson (JsonWriter writer);
    }
}
