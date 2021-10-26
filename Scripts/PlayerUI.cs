using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthBarFill;

    private PlayerController controller;
    [SerializeField]
    Text ammoText;

    [SerializeField]
	GameObject pauseMenu;

    private Player player;

    private WeaponManager weaponManager;

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    void Start()
    {
        PauseMenu.IsOn = false;
    }

    void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPct());
        SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets);
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthBarFill.localScale = new Vector3(1f, _amount,1f);
    }

    void SetAmmoAmount(int _amount)
    {
        ammoText.text = _amount.ToString();
    }
}
