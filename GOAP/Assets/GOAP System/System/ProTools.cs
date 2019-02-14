using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GOAP_S.PRO_TOOLS
{
    public static class ProTools
    {
        //Universal allocate class method =======
        public static T AllocateClass<T>(this object myobj)
        {
            //Get the class to allocate type
            System.Type class_ty = ((MonoScript)myobj).GetClass();
            //Instantiate a class of type class_ty
            object x = System.Activator.CreateInstance(class_ty, false);
            //Return the allocated class casted to type T
            return (T)x;
        }

        //Find assets by type T =================
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            //Get all the assets GUID
            string[] guids = AssetDatabase.FindAssets(null, new[] { "Assets" });
            for (int i = 0; i < guids.Length; i++)
            {
                //Transform GUIDs to paths
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                //Get type T assets using the paths
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    //Add the asset to the list of type T
                    assets.Add(asset);
                }
            }
            return assets;
        }

        /*public static T Cast<T>(this object myobj)
{
    System.Type ty = myobj.GetType();
    if (ty == UnityEngine.Object)
    {
        return myobj;
    }
    try
    {
        return (T)Convert.ChangeType(myobj, typeof(T));
    }
    catch (InvalidCastException)
    {
        return default(T);
    }
}*/


        /*public static T Cast<T>(this object myobj)
{
    System.Type objectType = myobj.GetType();
    System.Type target = typeof(T);
    object x = System.Activator.CreateInstance(target, false);
    MemberInfo[] member_info_array = objectType.GetMembers();
    List<MemberInfo> list = new List<MemberInfo>(objectType.GetMembers());

    var z = from source in list
            where source.MemberType == MemberTypes.Property
            select source;

    var d = from source in target.GetMembers().//.ToList()
            where source.MemberType == MemberTypes.Property
            select source;
    List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name).ToList().Contains(memberInfo.Name)).ToList();
    PropertyInfo propertyInfo;
    object value;
    foreach (var memberInfo in members)
    {
        propertyInfo = typeof(T).GetProperty(memberInfo.Name);
        value = myobj.GetType().GetProperty(memberInfo.Name).GetValue(myobj, null);

        propertyInfo.SetValue(x, value, null);
    }
    return (T)x;
}*/
        /*
            MemberInfo[] member_info_array = class_ty.GetMembers();
            Action_GS t = new MoveAction_GS();
            PropertyInfo property_info;
            object val;
            /*foreach (var variable in member_info_array)
            {
                if (variable.MemberType == MemberTypes.Field)
                {
                    property_info = typeof(T).GetProperty(variable.Name);
                    val = myobj.GetType().GetProperty(variable.Name).GetValue(myobj, null);

                    property_info.SetValue(x, val, null);
                }
            }

            
        }
        
             List<MemberInfo> list = new List<MemberInfo>(objectType.GetMembers());

             var z = from source in list
                     where source.MemberType == MemberTypes.Property
                     select source;

             var d = from source in target.GetMembers()//.ToList()
                     where source.MemberType == MemberTypes.Property
                     select source;

             List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name).ToList().Contains(memberInfo.Name)).ToList();
             PropertyInfo propertyInfo;
             object value;
             foreach (var memberInfo in members)
             {
                 propertyInfo = typeof(T).GetProperty(memberInfo.Name);
                 value = myobj.GetType().GetProperty(memberInfo.Name).GetValue(myobj, null);

                 propertyInfo.SetValue(x, value, null);
             }
             return (T)x;
         }
         
        public static object CloneObject(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            UnityEditor.Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance,
                null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
        }*/
    }
}
