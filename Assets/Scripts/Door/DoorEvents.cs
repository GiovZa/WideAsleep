using System;

public static class DoorEvents
{
    public static event Action OnDoorStateChanged;

    public static void DoorStateChanged()
    {
        OnDoorStateChanged?.Invoke();
    }
}
