using UnityEngine.Events;

public static class PlayerEvents
{
    private readonly static UnityEvent _characterDead = new UnityEvent();

    public static UnityEvent CharacterDead { get => _characterDead; }
}
