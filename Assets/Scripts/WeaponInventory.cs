using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponInventory : MonoBehaviour
{
    public PlayerController playerController;
    public SaveController saveController;
    public GameObject weaponPivot;
    
    public GameObject[] allWeapons; // Todas las armas posibles
    public GameObject[] equippedWeapons = new GameObject[3];
    private int currentWeaponIndex = 0;

    public GameObject[] wPrefab;
    public Sprite handIcon;
    [SerializeField] private Image uiImage;
    [SerializeField] private TMP_Text noSlot;


    void Start()
    {
        saveController = GameObject.FindWithTag("Manager").GetComponent<SaveController>();
        
        if(saveController.data.takenCollectables.Contains("Sword"))
        {
            AddWeapon(wPrefab[0]);
        }

        if (SaveManager.Instance.currentData.takenCollectables.Contains("Hand"))
        {
            EquipHand();
        }
        else
        {
            BlockCursor();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && SaveManager.Instance.currentData.takenCollectables.Contains("Hand")) EquipHand();
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        if (Input.GetKeyDown(KeyCode.Q)) NextWeapon();
        if (Input.GetKeyDown(KeyCode.E)) PreviousWeapon();
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        // Instanciar el arma como hijo del weaponpivot
        GameObject newWeapon = Instantiate(weaponPrefab, weaponPivot.transform);
        newWeapon.GetComponent<Weapon>().playerStats = this.gameObject.GetComponent<PlayerStats>();
        newWeapon.GetComponent<DamagePlayer>().controller = playerController;

        playerController.weaponAnim = newWeapon.GetComponent<Animator>();
        playerController.weapon = newWeapon.GetComponent<Weapon>();
        //newWeapon.SetActive(false);

        // Buscar un slot vacío
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            if (equippedWeapons[i] == null)
            {
                equippedWeapons[i] = newWeapon;
                Debug.Log("Arma añadida al slot " + i);
                return;
            }
        }

        Debug.Log("Inventario lleno, no se pudo añadir el arma.");
    }

    public void EquipHand()
    {
        foreach (GameObject weapon in equippedWeapons)
        {
            if (weapon != null) weapon.SetActive(false);
        }

        uiImage.sprite = handIcon;
        noSlot.text = "1";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerController.weaponAnim = null;
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length) return;

        if (equippedWeapons[index] != null)
        {
            foreach (GameObject weapon in equippedWeapons)
            {
                if (weapon != null) weapon.SetActive(false);
            }

            BlockCursor();


            equippedWeapons[index].SetActive(true);
            var Weapon = equippedWeapons[index].GetComponent<Weapon>();
            uiImage.sprite = Weapon.icon;
            noSlot.text = (index+2).ToString() ;
            //asignar anim del arma en playercontrolles
            playerController.weaponAnim = equippedWeapons[index].GetComponent<Animator>();
            currentWeaponIndex = index;
        }
    }

    void BlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % equippedWeapons.Length;
        EquipWeapon(nextIndex);
    }

    void PreviousWeapon()
    {
        int prevIndex = (currentWeaponIndex - 1 + equippedWeapons.Length) % equippedWeapons.Length;
        EquipWeapon(prevIndex);
    }
}
