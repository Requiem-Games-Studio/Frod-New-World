using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject weaponPivot;
    
    public GameObject[] allWeapons; // Todas las armas posibles
    public GameObject[] equippedWeapons = new GameObject[3];
    private int currentWeaponIndex = 0;

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
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

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Length) return;

        foreach (GameObject weapon in equippedWeapons)
        {
            if (weapon != null) weapon.SetActive(false);
        }

        if (equippedWeapons[index] != null)
        {
            equippedWeapons[index].SetActive(true);
            //asignar anim del arma en playercontrolles
            playerController.weaponAnim = equippedWeapons[index].GetComponent<Animator>();
            currentWeaponIndex = index;
        }
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
