using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTCInputManager{

    public enum Button
    {
        Primary_trigger,
        Second_Primary_trigger,
        Primary_grip,
        Second_Primary_grip,
        Primary_Menu,
        Sencond_Menu,
        Primary_touchpad,
        Sencond_Primary_touchpad
    }

    public enum Axis1D
    {
        Primary_trigger,
        Second_Primary_trigger,
    }

    public enum Axis2D
    {
        Primary_touchpad,
        Second_Primary_touchpad
    }

    public static bool GetButtonDown(Button button)
    {
        switch (button)
        {
            case Button.Primary_trigger:
                return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);
                break;
            case Button.Second_Primary_trigger:
                return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
                break;
            case Button.Primary_grip:
                return OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);
                break;
            case Button.Second_Primary_grip:
                return OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger);
                break;
            case Button.Primary_Menu:
                return OVRInput.GetDown(OVRInput.Button.Four);
                break;
            case Button.Sencond_Menu:
                return OVRInput.GetDown(OVRInput.Button.Two);
                break;
            case Button.Primary_touchpad:
                return OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick);
                break;
            case Button.Sencond_Primary_touchpad:
                return OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick);
            default:
                break;
        }

        return false;
    }

    public static bool Get(Button button)
    {
        switch (button)
        {
            case Button.Primary_trigger:
                return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);
                break;
            case Button.Second_Primary_trigger:
                return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
                break;
            case Button.Primary_grip:
                return OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
                break;
            case Button.Second_Primary_grip:
                return OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
                break;
            case Button.Primary_Menu:
                return OVRInput.Get(OVRInput.Button.Four);
                break;
            case Button.Sencond_Menu:
                return OVRInput.Get(OVRInput.Button.Two);
            case Button.Primary_touchpad:
                return OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
                break;
            case Button.Sencond_Primary_touchpad:
                return OVRInput.Get(OVRInput.Button.SecondaryThumbstick);
                break;
            default:
                break;
        }

        return false;
    }

    public static float Get(Axis1D axis)
    {
        switch (axis)
        {
            case Axis1D.Primary_trigger:
                return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
                break;
            case Axis1D.Second_Primary_trigger:
                return OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
                break;
            default:
                break;
        }

        return 0;
    }

    public static Vector2 Get(Axis2D axis)
    {
        switch (axis)
        {
            case Axis2D.Primary_touchpad:
                return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                break;
            case Axis2D.Second_Primary_touchpad:
                return OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
                break;
            default:
                break;
        }

        return Vector2.zero;
    }

}
