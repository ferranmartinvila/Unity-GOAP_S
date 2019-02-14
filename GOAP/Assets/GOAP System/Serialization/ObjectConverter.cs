using System;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using System.Linq;

namespace GOAP_T.Serialization
{
    public class ObjectConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return typeof(UnityEngine.Object) == type;
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
       {

            var database = Serializer.Context.Get<List<UnityEngine.Object>>();
            var o = instance as UnityEngine.Object;

            var index = -1;
            for (var i = 0; i < database.Count; i++)
            {
                if (ReferenceEquals(database[i], o))
                {
                    index = i;
                    break;
                }
            }

            //this is done to avoid serializing 0 because it's default value of int and will not be printed,
            //which is done for performance. Thus we always start from index 1.
            if (database.Count == 0)
            {
                database.Add(null);
            }

            if (index <= 0)
            {
                index = database.Count;
                database.Add(o);
            }

            serialized = new fsData(index);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            var database = Serializer.Context.Get<List<UnityEngine.Object>>();
            var index = (int)data.AsInt64;

            if (index >= database.Count)
                return fsResult.Warn("A Unity Object reference has not been deserialized");

            instance = database[index];
            return fsResult.Success;
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return null;
        }
    
    }
}
