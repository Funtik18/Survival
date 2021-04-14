using UnityEngine.Events;

public class FireMenuPrimary : WindowUI
{
    public UnityAction onFuel;
    public UnityAction onCooking;
    public UnityAction onBoiling;

    public UnityAction onBack;

    public FireMenuButton buttonFuel;
    public FireMenuButton buttonCooking;
    public FireMenuButton buttonBoliling;


    private void Awake()
    {
        buttonFuel.onPressed += Fuel;
        buttonCooking.onPressed += Cooking;
        buttonBoliling.onPressed += Boiling;
    }

    private void Fuel()
    {
        onFuel?.Invoke();
    }
    private void Cooking()
    {
        onCooking?.Invoke();
    }
    private void Boiling()
    {
        onBoiling?.Invoke();
    }
}