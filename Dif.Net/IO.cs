using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dif.Net
{
    public static class IO
    {
        public static int interiorFileVersion = 0;

        public static bool debug = false;
        //This whole class is a huge hack for me being lazy to write all the read methods for all the classes

        static void DebugPrint(string s)
        {
            if (debug)
                Console.WriteLine(s);
        }

        public static T Read<T>(BinaryReader rb)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(rb.ReadInt32(), typeof(T));
            }
            else if (typeof(T) == typeof(short))
            {
                return (T)Convert.ChangeType(rb.ReadInt16(), typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)Convert.ChangeType(rb.ReadSingle(), typeof(T));
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)Convert.ChangeType(rb.ReadDouble(), typeof(T));
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(rb.ReadString(), typeof(T));
            }
            else if (typeof(T) == typeof(byte))
            {
                return (T)Convert.ChangeType(rb.ReadByte(), typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(rb.ReadBoolean(), typeof(T));
            }
            else if (typeof(T) == typeof(Dictionary<string, string>))
            {
                    var dict = new Dictionary<string, string>();
                    var size = rb.ReadInt32();
                    for (var i = 0; i < size; i++)
                    {
                        dict.Add(rb.ReadString(), rb.ReadString());
                    }
                    return (T)Convert.ChangeType(dict, typeof(T));
            }
            else if (typeof(T) == typeof(Vector3))
            {
                var vec = new Vector3(rb.ReadSingle(), rb.ReadSingle(), rb.ReadSingle());
                return (T)Convert.ChangeType(vec, typeof(T));
            }
            else if (typeof(T) == typeof(Plane))
            {
                var pln = new Plane(Read<Vector3>(rb), Read<float>(rb));
                return (T)Convert.ChangeType(pln, typeof(T));
            }
            else if (typeof(T) == typeof(Quaternion))
            {
                var quat = new Quaternion(Read<Vector3>(rb), Read<float>(rb));
                return (T)Convert.ChangeType(quat, typeof(T));
            }
            else if (typeof(T).IsGenericType)
            {
                if (typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    var size = Read<int>(rb);
                    var typ = typeof(T).GetGenericArguments()[0];
                    var vec = (System.Collections.IList)Activator.CreateInstance<T>();
                    for (var i = 0; i < size; i++)
                    {
                        vec.Add(Convert.ChangeType(Read(typ, rb), typ));
                    }
                    return (T)Convert.ChangeType(vec, typeof(T));
                }
                else if (typeof(IReadable).IsAssignableFrom(typeof(T)))
                {
                    var instance = (IReadable)Activator.CreateInstance(typeof(T));
                    instance.Read(rb);
                    return (T)Convert.ChangeType(instance, typeof(T));
                }
                else
                    throw new Exception("Cant read this yet");
            }
            else if (typeof(T).IsValueType)
            {
                DebugPrint("Reading type " + typeof(T).ToString() + " at " + rb.BaseStream.Position);
                var instance = Activator.CreateInstance(typeof(T));
                foreach (var field in typeof(T).GetFields())
                {
                    var versionattrib = field.GetCustomAttributes(typeof(VersionAttribute), false);
                    if (versionattrib.Length != 0)
                    {
                        var attrib = (VersionAttribute)versionattrib[0];

                        if (attrib.Exclusions != null)
                            if (attrib.Exclusions.Contains(interiorFileVersion))
                                continue;

                        if (interiorFileVersion > attrib.MaxVersion || interiorFileVersion < attrib.MinVersion)
                            continue; //No reading/writing incompatbiles
                    }
                    DebugPrint("Reading field " + field.Name + " at " + rb.BaseStream.Position);
                    if (field.FieldType.IsGenericType)
                    {
                        if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var objlist = (List<object>)Read(field.FieldType, rb); // List<object>
                            var genlist = (System.Collections.IList)Activator.CreateInstance(field.FieldType); //List<T>
                            foreach (var obj in objlist)
                            {
                                genlist.Add(Convert.ChangeType(obj, field.FieldType.GetGenericArguments()[0]));
                            }
                            field.SetValue(instance,genlist);
                        }
                        else
                        {
                            field.SetValue(instance, Read(field.FieldType, rb));
                        }
                    }
                    else
                        field.SetValue(instance, Read(field.FieldType, rb));
                }
                return (T)Convert.ChangeType(instance, typeof(T));
            }
            else if (typeof(IReadable).IsAssignableFrom(typeof(T)))
            {
                DebugPrint("Reading type " + typeof(T).ToString() + " at " + rb.BaseStream.Position);
                var instance = (IReadable)Activator.CreateInstance(typeof(T));
                instance.Read(rb);
                return (T)Convert.ChangeType(instance, typeof(T));
            }
            else
            {
                throw new Exception("Cant read this yet");
            }
        }

        public static object Read(Type type, BinaryReader rb)
        {

            if (type == typeof(int))
            {
                return rb.ReadInt32();
            }
            else if (type == typeof(short))
            {
                return rb.ReadInt16();
            }
            else if (type == typeof(float))
            {
                return rb.ReadSingle();
            }
            else if (type == typeof(double))
            {
                return rb.ReadDouble();
            }
            else if (type == typeof(string))
            {
                return rb.ReadString();
            }
            else if (type == typeof(byte))
            {
                return rb.ReadByte();
            }
            else if (type == typeof(bool))
            {
                return rb.ReadBoolean();
            }
            else if (type == typeof(Dictionary<string, string>))
            {
                var dict = new Dictionary<string, string>();
                var size = rb.ReadInt32();
                for (var i = 0; i < size; i++)
                {
                    dict.Add(rb.ReadString(), rb.ReadString());
                }
                return dict;
            }
            else if (type == typeof(Vector3))
            {
                var vec = new Vector3(rb.ReadSingle(), rb.ReadSingle(), rb.ReadSingle());
                return vec;
            }
            else if (type == typeof(Plane))
            {
                var pln = new Plane(Read<Vector3>(rb), Read<float>(rb));
                return pln;
            }
            else if (type == typeof(Quaternion))
            {
                var quat = new Quaternion(Read<Vector3>(rb), Read<float>(rb));
                return quat;
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var size = Read<int>(rb);
                    var typ = type.GetGenericArguments()[0];
                    var vec = new List<object>();
                    for (var i = 0; i < size; i++)
                    {
                        vec.Add(Read(typ, rb));
                    }
                    return vec;
                }
                else if (typeof(IReadable).IsAssignableFrom(type))
                {
                    var instance = (IReadable)Activator.CreateInstance(type);
                    instance.Read(rb);
                    return instance;
                }
                else
                    throw new Exception("Cant read this yet");
            }
            else if (type.IsValueType)
            {
                DebugPrint("Reading type " + type.ToString() + " at " + rb.BaseStream.Position);
                var instance = Activator.CreateInstance(type);
                foreach (var field in type.GetFields())
                {
                    var versionattrib = field.GetCustomAttributes(typeof(VersionAttribute), false);
                    if (versionattrib.Length != 0)
                    {
                        var attrib = (VersionAttribute)versionattrib[0];

                        if (attrib.Exclusions != null)
                            if (attrib.Exclusions.Contains(interiorFileVersion))
                                continue;

                        if (interiorFileVersion > attrib.MaxVersion || interiorFileVersion < attrib.MinVersion)
                            continue; //No reading/writing incompatbiles
                    }
                    DebugPrint("Reading field " + field.Name + " at " + rb.BaseStream.Position);
                    if (field.FieldType.IsGenericType)
                    {
                        if (field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var objlist = (List<object>)Read(field.FieldType, rb); // List<object>
                            var genlist = (System.Collections.IList)Activator.CreateInstance(field.FieldType); //List<T>
                            foreach (var obj in objlist)
                            {
                                genlist.Add(Convert.ChangeType(obj, field.FieldType.GetGenericArguments()[0]));
                            }
                            field.SetValue(instance, genlist);
                        }
                        else
                        {
                            field.SetValue(instance, Read(field.FieldType, rb));
                        }
                    }
                    else
                        field.SetValue(instance, Read(field.FieldType, rb));
                }
                return instance;
            }
            else if (typeof(IReadable).IsAssignableFrom(type))
            {
                DebugPrint("Reading type " + type.ToString() + " at " + rb.BaseStream.Position);
                var instance = (IReadable)Activator.CreateInstance(type);
                instance.Read(rb);
                return instance;
            }
            else
            {
                throw new Exception("Cant read this yet");
            }
        }

        public static void Write<T>(List<T> obj, BinaryWriter wb)
        {
            wb.Write(obj.Count);
            foreach (var o in obj)
            {
                Write(o, wb);
            }
        }

        public static void Write<T>(T obj, BinaryWriter wb)
        {
            if (typeof(T) == typeof(int))
            {
                wb.Write((int)Convert.ChangeType(obj, typeof(int)));
            }
            else if (typeof(T) == typeof(short))
            {
                wb.Write((short)Convert.ChangeType(obj, typeof(short)));
            }
            else if (typeof(T) == typeof(float))
            {
                wb.Write((float)Convert.ChangeType(obj, typeof(float)));
            }
            else if (typeof(T) == typeof(double))
            {
                wb.Write((double)Convert.ChangeType(obj, typeof(double)));
            }
            else if (typeof(T) == typeof(string))
            {
                wb.Write((string)Convert.ChangeType(obj, typeof(string)));
            }
            else if (typeof(T) == typeof(byte))
            {
                wb.Write((byte)Convert.ChangeType(obj, typeof(byte)));
            }
            else if (typeof(T) == typeof(bool))
            {
                wb.Write((bool)Convert.ChangeType(obj, typeof(bool)));
            }
            else if (typeof(T) == typeof(Dictionary<string, string>))
            {
                var dict = (Dictionary<string, string>)Convert.ChangeType(obj, typeof(Dictionary<string, string>));
                wb.Write(dict.Count);
                foreach (var kvp in dict)
                {
                    wb.Write(kvp.Key);
                    wb.Write(kvp.Value);
                }
            }
            else if (typeof(T) == typeof(Vector3))
            {
                var vec = (Vector3)Convert.ChangeType(obj, typeof(Vector3));
                wb.Write(vec.X);
                wb.Write(vec.Y);
                wb.Write(vec.Z);
            }
            else if (typeof(T) == typeof(Plane))
            {
                var pln = (Plane)Convert.ChangeType(obj, typeof(Plane));
                Write(pln.Normal, wb);
                Write(pln.D, wb);
            }
            else if (typeof(T) == typeof(Quaternion))
            {
                var quat = (Quaternion)Convert.ChangeType(obj, typeof(Quaternion));
                Write(quat.X, wb);
                Write(quat.Y, wb);
                Write(quat.Z, wb);
                Write(quat.W, wb);
            }
            else if (typeof(T).IsValueType)
            {
                DebugPrint("Writing type " + typeof(T).ToString() + " at " + wb.BaseStream.Position);
                foreach (var field in typeof(T).GetFields())
                {
                    var versionattrib = field.GetCustomAttributes(typeof(VersionAttribute), false);
                    if (versionattrib.Length != 0)
                    {
                        var attrib = (VersionAttribute)versionattrib[0];

                        if (attrib.Exclusions != null)
                            if (attrib.Exclusions.Contains(interiorFileVersion))
                                continue;

                        if (interiorFileVersion > attrib.MaxVersion || interiorFileVersion < attrib.MinVersion)
                            continue; //No reading/writing incompatbiles
                    }
                    DebugPrint("Writing field " + field.Name + " at " + wb.BaseStream.Position);
                    Write(field.FieldType,field.GetValue(obj), wb);
                }
            }
            else if (typeof(IWritable).IsAssignableFrom(typeof(T)))
            {
                DebugPrint("Writing type " + typeof(T).ToString() + " at " + wb.BaseStream.Position);
                var instance = (IWritable)obj;
                instance.Write(wb);
            }
            else
            {
                throw new Exception("Cant read this yet");
            }

        }

        public static void Write(Type type,object obj, BinaryWriter wb)
        {
            if (type == typeof(int))
            {
                wb.Write((int)Convert.ChangeType(obj, typeof(int)));
            }
            else if (type == typeof(short))
            {
                wb.Write((short)Convert.ChangeType(obj, typeof(short)));
            }
            else if (type == typeof(float))
            {
                wb.Write((float)Convert.ChangeType(obj, typeof(float)));
            }
            else if (type == typeof(double))
            {
                wb.Write((double)Convert.ChangeType(obj, typeof(double)));
            }
            else if (type == typeof(string))
            {
                wb.Write((string)Convert.ChangeType(obj, typeof(string)));
            }
            else if (type == typeof(byte))
            {
                wb.Write((byte)Convert.ChangeType(obj, typeof(byte)));
            }
            else if (type == typeof(bool))
            {
                wb.Write((bool)Convert.ChangeType(obj, typeof(bool)));
            }
            else if (type == typeof(Dictionary<string, string>))
            {
                var dict = (Dictionary<string, string>)Convert.ChangeType(obj, typeof(Dictionary<string, string>));
                wb.Write(dict.Count);
                foreach (var kvp in dict)
                {
                    wb.Write(kvp.Key);
                    wb.Write(kvp.Value);
                }
            }
            else if (type == typeof(Vector3))
            {
                var vec = (Vector3)Convert.ChangeType(obj, typeof(Vector3));
                wb.Write(vec.X);
                wb.Write(vec.Y);
                wb.Write(vec.Z);
            }
            else if (type == typeof(Plane))
            {
                var pln = (Plane)Convert.ChangeType(obj, typeof(Plane));
                Write(pln.Normal, wb);
                Write(pln.D, wb);
            }
            else if (type == typeof(Quaternion))
            {
                var quat = (Quaternion)Convert.ChangeType(obj, typeof(Quaternion));
                Write(quat.X, wb);
                Write(quat.Y, wb);
                Write(quat.Z, wb);
                Write(quat.W, wb);
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var l = (System.Collections.ICollection)obj;
                    Write(l.Count, wb);
                    foreach (var o in l)
                    {
                        Write(o.GetType(), o, wb);
                    }
                }
                else if (typeof(IWritable).IsAssignableFrom(type))
                {
                    DebugPrint("Writing type " + type.ToString() + " at " + wb.BaseStream.Position);
                    var instance = (IWritable)obj;
                    instance.Write(wb);
                }
            }
            else if (type.IsValueType)
            {
                DebugPrint("Writing type " + type.ToString() + " at " + wb.BaseStream.Position);
                foreach (var field in type.GetFields())
                {
                    var versionattrib = field.GetCustomAttributes(typeof(VersionAttribute), false);
                    if (versionattrib.Length != 0)
                    {
                        var attrib = (VersionAttribute)versionattrib[0];

                        if (attrib.Exclusions != null)
                            if (attrib.Exclusions.Contains(interiorFileVersion))
                                continue;

                        if (interiorFileVersion > attrib.MaxVersion || interiorFileVersion < attrib.MinVersion)
                            continue; //No reading/writing incompatbiles
                    }
                    DebugPrint("Writing field " + field.Name + " at " + wb.BaseStream.Position);
                    Write(field.FieldType,field.GetValue(obj), wb);
                }
            }
            else if (typeof(IWritable).IsAssignableFrom(type))
            {
                DebugPrint("Writing type " + type.ToString() + " at " + wb.BaseStream.Position);
                var instance = (IWritable)obj;
                instance.Write(wb);
            }
            else
            {
                throw new Exception("Cant read this yet");
            }

        }
    }
}
