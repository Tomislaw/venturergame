using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelectionUI : MonoBehaviour
{
    private bool isMale = true;

    public bool GetIsMale()
    { return isMale; }

    public void SetIsMale(bool value)
    {
        malePressed.SetActive(value);
        femalePressed.SetActive(!value);
        male.SetActive(!value);
        female.SetActive(value);
        isMale = value;
    }

    public GameObject malePressed;
    public GameObject femalePressed;

    public GameObject male;
    public GameObject female;

    private void OnEnable()
    {
        SetIsMale(isMale);
    }
}