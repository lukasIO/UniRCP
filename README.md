# UniRCP
Unity Implementation of the Rabbit Control Protocol (https://github.com/rabbitControl/RCP)

## Dependencies:
- UniRx (https://github.com/neuecc/UniRx)
- BindingsRx (https://github.com/grofit/bindingsrx)
- RCPSharp (https://github.com/lukasIO/rcp-csharp -> forked and adjusted datatypes for unity)

## What is this
A reactive server implementation for a rapid setup of remote controlling (almost) everything within unity during runtime.<br>
Using RabbitControl, a client application (i.e. (here)[https://github.com/rabbitControl/rcp-ts-client] which is running online (here)[https://rabbitcontrol.github.io/client/] automatically generates a UI for remote controlling the parameters that you choose to expose via this RCP server.

## How does it work
Place the RabbitControl Prefab from this Repo in your Scene.<br>
By default it will use Websockets as the transport layer.

To expose some fields/parameters/transforms to the RCP Server, have a look at the ControlTest.cs file, for a single float it would look like this:
```c#
using RabbitControl;

public class ControlTest : MonoBehaviour {

    public float myFloat;
   
    void Start () {
        SetupRabbitControls();     
    }

    private void SetupRabbitControls()
    {    
        myFloat
            .ToValueParameter(nameof(myFloat))    //create a RCP Parameter out of your variable and expose it to/on the server
            .BindParameterTo(() => myFloat, x => myFloat = x); //two way binding using of the value using BindingsRx internally
     }
```

## Features
- Two-Way Binding (you can choose not to though)
- Supports a lot of basic datatypes (int, float, string, enum, ...)
- grouping of multiple parameters (also nested groups)
- this is a WIP, not all datatypes are supported yet, very much untested.

