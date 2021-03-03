using System;
using FullSerializer;


public static class FullSerializerAPI
{
	private static readonly fsSerializer _serializer = new fsSerializer();

    public static string Serialize(Type type, object value, bool isPretty = false,bool isEncryption = true)
	{
		// serialize the data
		fsData data;
		_serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        if (isPretty)
        {
            string jsonStr = fsJsonPrinter.PrettyJson(data);
            if(isEncryption)
                jsonStr = StringEncryption.EncryptDES(jsonStr);   //加密
            return jsonStr;
        }
        else
        {
            string jsonStr = fsJsonPrinter.CompressedJson(data);
            if (isEncryption)
                jsonStr = StringEncryption.EncryptDES(jsonStr);   //加密
            return jsonStr;
            //return fsJsonPrinter.CompressedJson(data);
        }
	}

	public static object Deserialize(Type type, string serializedState)
	{
        serializedState = StringEncryption.DecryptDES(serializedState); //////解密
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

		// step 2: deserialize the data
		object deserialized = null;
		_serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

		return deserialized;
	}

    public static T Deserialize<T>(string jsonStr) where T : new()
    {
        fsData data = fsJsonParser.Parse(jsonStr);
        T deserialized = new T();
        _serializer.TryDeserialize(data, ref deserialized);
        return deserialized;
    }
}