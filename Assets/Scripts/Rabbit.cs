using UnityEngine;
using RCP;
using RCP.Parameters;
using RCP.Transporter;
using System;
using BindingsRx.Bindings;
using BindingsRx.Filters;
using BindingsRx;
using RabbitControl.Conversion;


namespace RabbitControl
{

    public class Rabbit : MonoBehaviour
    {

        public string RemoteHost = "127.0.0.1";
        public int RemotePort = 4444;


        private static RCPServer _server;
        public static RCPServer Server
        {
            get
            {
                if (_server == null)
                {
                    _server = new RCPServer();
                }
                return _server;
            }
        }

        private void Start()
        {
            Initialize();
        }


        private void Initialize()
        {
            Server.AddTransporter(new WebsocketServerTransporter(RemoteHost, RemotePort));     
            Server.ParameterAdded += (s,p) => { Server.Update(); };
            Server.ParameterRemoved += (s, p) => { Server.Update(); };
            Debug.Log("RabbitControl initialized");       
        }
        
    }

    namespace Conversion
    {
        public static class Converters
        {
            //public static System.Numerics.Vector3 ToSystemVector(this UnityEngine.Vector3 vec)
            //{
            //    return new System.Numerics.Vector3(vec.x, vec.y, vec.z);
            //}

            //public static UnityEngine.Vector3 ToUnityVector(this System.Numerics.Vector3 vec)
            //{
            //    return new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
            //}

            public static Vector4 ToVector(this UnityEngine.Quaternion vec)
            {
                return new Vector4(vec.x, vec.y, vec.z, vec.w);
            }

            public static UnityEngine.Quaternion ToQuaternion(this Vector4 vec)
            {
                return new UnityEngine.Quaternion(vec.x, vec.y, vec.z, vec.w);
            }
        }
    }


    public static class Extensions
    {

        public static ValueParameter<T> ToValueParameter<T>(this T input, string name, GroupParameter groupParameter = null)
        {
            var param = Rabbit.Server.CreateValueParameter<T>(name, groupParameter);
            param.Default = input;
            param.Value = input;
            return param;
        }

      

        public static EnumParameter ToEnumParameter<T>(this T input, string name, GroupParameter groupParameter = null)
        {
            var param = Rabbit.Server.CreateEnumParameter(name, groupParameter);
            param.Entries = System.Enum.GetNames(typeof(T));
            param.Default = input.ToString();
            return param;
        }

        public static ArrayParameter<T> ToArrayParameter<T>(this T[] input, string name, GroupParameter groupParameter = null)
        {
            var param = Rabbit.Server.CreateArrayParameter<T>(name, groupParameter);
            param.Default = input;
            return param;
        }

        public static GroupParameter BindTransformToRabbit(this Transform input, string name, GroupParameter groupParameter = null)
        {
            var transformGroup = Rabbit.Server.CreateGroup(name + " transform", groupParameter);

            input.BindPositionToRabbit(groupParameter: transformGroup);
            input.BindRotationToRabbit(groupParameter: transformGroup);
            input.BindScaleToRabbit(groupParameter: transformGroup);

            return transformGroup;
        }

        public static ValueParameter<Vector3> BindPositionToRabbit(this Transform input, string name = "Position", GroupParameter groupParameter = null)
        {         
            var positionParameter = Rabbit.Server.CreateValueParameter<Vector3>(name);
            positionParameter.Default = input.position;
            positionParameter.Value = input.position;
            input.BindPositionTo(() => positionParameter.Value, x => positionParameter.Value = x);

            return positionParameter;
        }

        public static ValueParameter<Vector3> BindScaleToRabbit(this Transform input, string name = "Scale", GroupParameter groupParameter = null)
        {
            var scaleParameter = Rabbit.Server.CreateValueParameter<Vector3>(name);
            scaleParameter.Default = input.localScale;
            scaleParameter.Value = input.localScale;
            input.BindScaleTo(() => scaleParameter.Value, x => scaleParameter.Value = x);

            return scaleParameter;
        }

        public static ValueParameter<Vector4> BindRotationToRabbit(this Transform input, string name = "Rotation", GroupParameter groupParameter = null)
        {
            var rotationParameter = Rabbit.Server.CreateValueParameter<Vector4>(name);
            rotationParameter.Default = input.rotation.ToVector();
            rotationParameter.Value = input.rotation.ToVector();
            input.BindRotationTo(() => rotationParameter.Value.ToQuaternion(), x => rotationParameter.Value = x.ToVector());

            return rotationParameter;
        }

        //public static ValueParameter<Color> BindColorToRabbit(this UnityEngine.UI.Image input, string name = "Color", GroupParameter groupParameter = null)
        //{
        //    var colorParam = Rabbit.Server.CreateValueParameter<System.Numerics.Vector4>(name);
        //    rotationParameter.Default = input.rotation.ToSystemVector();
        //    rotationParameter.Value = input.rotation.ToSystemVector();
        //    input.bind(() => rotationParameter.Value.ToQuaternion(), x => rotationParameter.Value = x.ToSystemVector());

        //    return rotationParameter;
        //}

        public static IDisposable BindParameterTo<T>(this ValueParameter<T> input, Func<T> getter, Action<T> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<T>[] filters)
        {
            return GenericBindings.Bind(() => input.Value, x => input.Value = x, getter, setter, bindingType, filters);
        }

        public static IDisposable BindParameterTo(this EnumParameter input, Func<string> getter, Action<string> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<string>[] filters)
        {
            return GenericBindings.Bind(() => input.Value, x => input.Value = x, getter, setter, bindingType, filters);
        }

        public static IDisposable BindParameterTo<T>(this ArrayParameter<T> input, Func<T[]> getter, Action<T[]> setter, BindingTypes bindingType = BindingTypes.Default, params IFilter<T[]>[] filters)
        {
            return GenericBindings.Bind(() => input.Value, x => input.Value = x, getter, setter, bindingType, filters);
        }

        



    }

    
}
