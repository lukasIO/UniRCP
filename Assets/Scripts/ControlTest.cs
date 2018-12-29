using UnityEngine;
using RabbitControl;

public class ControlTest : MonoBehaviour {

    public float myFloat;
    public string myString;
    public int myInt;

    public Vector3 myVector3;

    public int[] myFloatArray;

    public enum testEnum { TEST, ONE, TWO}

    public testEnum myEnum;


    void Start () {
        SetupRabbitControls();
        myEnum = testEnum.TWO;
       
    }


    private void SetupRabbitControls()
    {
        var paramGroup = Rabbit.Server.CreateGroup("testGroup");

        myVector3
            .ToValueParameter(nameof(myVector3),paramGroup)
            .BindParameterTo(() => myVector3, x => myVector3 = x);

        myFloat
            .ToValueParameter(nameof(myFloat), paramGroup)
            .BindParameterTo(() => myFloat, x => myFloat = x);

        myString
            .ToValueParameter(nameof(myString))
            .BindParameterTo(() => myString, x => myString = x);

        myInt
            .ToValueParameter(nameof(myInt))
            .BindParameterTo(() => myInt, x => myInt = x);

        myEnum
            .ToEnumParameter(nameof(myEnum))
            .BindParameterTo(() => myEnum.ToString(), x => myEnum = (testEnum)System.Enum.Parse(typeof(testEnum), x));

        myFloatArray
            .ToArrayParameter(nameof(myFloatArray))
            .BindParameterTo(() => myFloatArray, x => myFloatArray = x);

        transform
            .BindTransformToRabbit("cube");

        var renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material.color
            .ToValueParameter("testColor")
            .BindParameterTo(() => renderer.material.color, x => renderer.material.color = x);

   
    }

}
