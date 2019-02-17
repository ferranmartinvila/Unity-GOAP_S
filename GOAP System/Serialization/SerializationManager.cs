using System;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using FullSerializer.Internal;

/*
 namespace UnityEngine
{
    [RequiredByNativeCode]
    public interface ISerializationCallbackReceiver
    {
        //
        // Summary:
        //     Implement this method to receive a callback after Unity deserializes your object.
        [RequiredByNativeCode]
        void OnAfterDeserialize();
        //
        // Summary:
        //     Implement this method to receive a callback before Unity serializes your object.
        [RequiredByNativeCode]
        void OnBeforeSerialize();
    }
}
*/

namespace GOAP_S.Serialization
{
    public static class SerializationManager //Static because this don't save anything, only serialize/deserialize the data in an external JSON file.
    {
        private static fsSerializer                 serializer = new fsSerializer(); //The serializer class that let us serialize/deserialize the desired data
        private static Dictionary<string, fsData>   data_dic = new Dictionary<string, fsData>(); //This dictonary contains all the serialized data
        private static object                       serialization_lock_obj = new object(); //Used to lock the serialization process, so the other processes can't acces to this object during the serialization
        private static bool                         converter_added = false; //Before the serialization and the deserialization process we check if the serializer have the objects converter using this boolean
        
        //Serialization process
        public static string Serialize(object value, Type value_type, List<UnityEngine.Object> object_references)
        {
            //Lock the serialization object
            lock(serialization_lock_obj)
            {
                //Add the converter in negative case
                if(converter_added == false)
                {
                    serializer.AddConverter(new ObjectConverter());
                    converter_added = true;
                }

                //Set the serializer object references context
                if(object_references != null)
                {
                    serializer.Context.Set<List<UnityEngine.Object>>(object_references);
                }

                //Serialize the value in a fsData
                fsData data;
                serializer.TrySerialize(value_type, value, out data).AssertSuccess();

                //Insert the serialized data in the dic
                data_dic[fsJsonPrinter.CompressedJson(data)] = data;

                //Serialize the data in the JSON file
                return fsJsonPrinter.CompressedJson(data);
            }
        }

        //Deserialization process
        public static object Deserialize(Type value_type, string serialized_state, List<UnityEngine.Object> object_references)
        {
            lock(serialization_lock_obj)
            {
                if(converter_added == false)
                {
                    serializer.AddConverter(new ObjectConverter());
                    converter_added = true;
                }

                if(object_references != null)
                {
                    serializer.Context.Set<List<UnityEngine.Object>>(object_references);
                }

                fsData data = null;
                data_dic.TryGetValue(serialized_state, out data);
                if(data == null)
                {
                    data = fsJsonParser.Parse(serialized_state);
                    data_dic[serialized_state] = data;
                }

                object deserialized = null;
                serializer.TryDeserialize(data, value_type, ref deserialized).AssertSuccess();

                return deserialized;
            }
        }

        public static T Deserialize<T>(string serialized_state, List<UnityEngine.Object> object_references = null)
        {
            return (T)Deserialize(typeof(T), serialized_state, object_references);
        }
    }
}