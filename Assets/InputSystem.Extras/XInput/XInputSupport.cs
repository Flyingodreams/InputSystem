namespace ISX.XInput
{
    /// <summary>
    /// Adds support for XInput controllers.
    /// </summary>
    [InputPlugin]
    public static class XInputSupport
    {
        public static void Initialize()
        {
            // Base template for Xbox-style gamepad.
            InputSystem.RegisterTemplate<XInputController>();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // XInput controllers on Windows.
            // State layout is XINPUT_GAMEPAD.
            // See https://msdn.microsoft.com/en-us/library/windows/desktop/microsoft.directx_sdk.reference.xinput_gamepad(v=vs.85).aspx
            InputSystem.RegisterTemplate(@"{
""name"" : ""XInputControllerWindows"",
""extend"" : ""XInputController"",
""format"" : ""XINP"",
""device"" : { ""interface"" : ""XInput"" },
""controls"" : [
    { ""name"" : ""dpad"", ""offset"" : 0, ""bit"" : 0 },
    { ""name"" : ""dpad/up"", ""offset"" : 0, ""bit"" : 0 },
    { ""name"" : ""dpad/down"", ""offset"" : 0, ""bit"" : 1 },
    { ""name"" : ""dpad/left"", ""offset"" : 0, ""bit"" : 2 },
    { ""name"" : ""dpad/right"", ""offset"" : 0, ""bit"" : 3 },
    { ""name"" : ""start"", ""offset"" : 0, ""bit"" : 4 },
    { ""name"" : ""select"", ""offset"" : 0, ""bit"" : 5 },
    { ""name"" : ""leftStickPress"", ""offset"" : 0, ""bit"" : 6 },
    { ""name"" : ""rightStickPress"", ""offset"" : 0, ""bit"" : 7 },
    { ""name"" : ""leftShoulder"", ""offset"" : 0, ""bit"" : 8 },
    { ""name"" : ""rightShoulder"", ""offset"" : 0, ""bit"" : 9 },
    { ""name"" : ""buttonSouth"", ""offset"" : 0, ""bit"" : 12 },
    { ""name"" : ""buttonEast"", ""offset"" : 0, ""bit"" : 13 },
    { ""name"" : ""buttonWest"", ""offset"" : 0, ""bit"" : 14 },
    { ""name"" : ""buttonNorth"", ""offset"" : 0, ""bit"" : 15 },
    { ""name"" : ""leftTrigger"", ""offset"" : 2, ""format"" : ""BYTE"" },
    { ""name"" : ""rightTrigger"", ""offset"" : 3, ""format"" : ""BYTE"" },
    { ""name"" : ""leftStick"", ""offset"" : 4, ""format"" : ""VC2S"" },
    { ""name"" : ""leftStick/x"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/left"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/right"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/y"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/up"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/down"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick"", ""offset"" : 8, ""format"" : ""VC2S"" },
    { ""name"" : ""rightStick/x"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/left"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/right"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/y"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/up"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/down"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" }
] }");
#endif

            ////TODO: it would be totally rad if instead of going to JSON in code here,
            ////      you could just create a new state struct representing the changed
            ////      state layout and then feed that into the template system; essentially,
            ////      InputReport below would become a real C# struct
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            // Xbox one controller on OSX. State layout can be found here:
            // https://github.com/360Controller/360Controller/blob/master/360Controller/ControlStruct.h
            // struct InputReport
            // {
            //     byte command;
            //     byte size;
            //     short buttons;
            //     byte triggerLeft;
            //     byte triggerRight;
            //     short leftX;
            //     short leftY;
            //     short rightX;
            //     short rightY;
            // }
            // Report size is 14 bytes. First two bytes are header information for the report.
            ////TODO: come up with a way that allows us to snip that data out of the state we store and the
            ////      state we compare
            ////TODO: rumble and LED output
            InputSystem.RegisterTemplate(@"{
""name"" : ""XboxGamepadOSX"",
""extend"" : ""XInputController"",
""format"" : ""HID"",
""device"" : { ""interface"" : ""HID"", ""product"" : ""Xbox.*Controller"" },
""controls"" : [
    { ""name"" : ""leftShoulder"", ""offset"" : 2, ""bit"" : 8 },
    { ""name"" : ""rightShoulder"", ""offset"" : 2, ""bit"" : 9 },
    { ""name"" : ""leftStickPress"", ""offset"" : 2, ""bit"" : 6 },
    { ""name"" : ""rightStickPress"", ""offset"" : 2, ""bit"" : 7 },
    { ""name"" : ""buttonSouth"", ""offset"" : 2, ""bit"" : 12 },
    { ""name"" : ""buttonEast"", ""offset"" : 2, ""bit"" : 13 },
    { ""name"" : ""buttonWest"", ""offset"" : 2, ""bit"" : 14 },
    { ""name"" : ""buttonNorth"", ""offset"" : 2, ""bit"" : 15 },
    { ""name"" : ""dpad"", ""offset"" : 2, ""bit"" : 0 },
    { ""name"" : ""dpad/up"", ""offset"" : 0, ""bit"" : 0 },
    { ""name"" : ""dpad/down"", ""offset"" : 0, ""bit"" : 1 },
    { ""name"" : ""dpad/left"", ""offset"" : 0, ""bit"" : 2 },
    { ""name"" : ""dpad/right"", ""offset"" : 0, ""bit"" : 3 },
    { ""name"" : ""start"", ""offset"" : 2, ""bit"" : 4 },
    { ""name"" : ""select"", ""offset"" : 2, ""bit"" : 5 },
    { ""name"" : ""xbox"", ""offset"" : 2, ""bit"" : 10, ""template"" : ""Button"" },
    { ""name"" : ""leftTrigger"", ""offset"" : 4, ""format"" : ""BYTE"" },
    { ""name"" : ""rightTrigger"", ""offset"" : 5, ""format"" : ""BYTE"" },
    { ""name"" : ""leftStick"", ""offset"" : 6, ""format"" : ""VC2S"" },
    { ""name"" : ""leftStick/x"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/left"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/right"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/y"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/up"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""leftStick/down"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick"", ""offset"" : 10, ""format"" : ""VC2S"" },
    { ""name"" : ""rightStick/x"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/left"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/right"", ""offset"" : 0, ""format"" : ""SHRT"", ""parameters"" : ""normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/y"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/up"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" },
    { ""name"" : ""rightStick/down"", ""offset"" : 2, ""format"" : ""SHRT"", ""parameters"" : ""invert,normalize,normalizeMin=-0.5,normalizeMax=0.5"" }
] }");
#endif
        }
    }
}
