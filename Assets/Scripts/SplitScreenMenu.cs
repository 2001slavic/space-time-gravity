using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class SplitScreenMenu : MonoBehaviour
{

    private readonly Vector3 gamepadRefPos = new Vector3(0, 50, 0);
    private readonly Vector3 gamepadRefOffset = new Vector3(0, -70, 0);
    private readonly Vector3 gamepadColumnOffset = new Vector3(-135, 0, 0);

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject backButton;


    [SerializeField]
    private List<Gamepad> p1Column;
    [SerializeField]
    private List<Gamepad> p2Column;

    [SerializeField]
    private GameObject gamepadIconPrefab;

    [SerializeField]
    private Sprite unconfirmedSprite;
    [SerializeField]
    private Sprite confirmedSprite;

    [SerializeField]
    private Image player1ConfirmImage;
    [SerializeField]
    private Image player2ConfirmImage;

    private bool player1Confirmed;
    private bool player2Confirmed;

    private struct GamepadInfoStruct
    {
        public GameObject icon;
        public bool mayChangeSide;
    }

    [SerializeField]
    private Dictionary<Gamepad, GamepadInfoStruct> gamepadInfo;

    private int countUnassignedGamepads;

    private void AddGamepad(Gamepad gamepad, Vector3 position)
    {
        GameObject gamepadIcon = Instantiate(gamepadIconPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        gamepadIcon.transform.SetParent(canvas.transform);
        gamepadIcon.GetComponent<RectTransform>().anchoredPosition3D = position;
        gamepadIcon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        gamepadIcon.transform.GetChild(0).GetComponent<Image>().color = UnityEngine.Random.ColorHSV();

        gamepadInfo[gamepad] = new GamepadInfoStruct { icon = gamepadIcon, mayChangeSide = true };
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        p1Column = new List<Gamepad>();
        p2Column = new List<Gamepad>();
        gamepadInfo = new Dictionary<Gamepad, GamepadInfoStruct>();

        countUnassignedGamepads = 0;


        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            AddGamepad(Gamepad.all[i], gamepadRefPos + gamepadRefOffset * i);
        }

        player1ConfirmImage.sprite = unconfirmedSprite;
        player2ConfirmImage.sprite = unconfirmedSprite;

        player1ConfirmImage.gameObject.SetActive(false);
        player2ConfirmImage.gameObject.SetActive(false);

        player1Confirmed = false;
        player2Confirmed = false;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;

        foreach (KeyValuePair<Gamepad, GamepadInfoStruct> entry in gamepadInfo)
        {
            Destroy(entry.Value.icon);
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Check if a gamepad was added
        if (change == InputDeviceChange.Added && device is Gamepad)
        {
            Gamepad gamepad = device as Gamepad;
            AddGamepad(gamepad, gamepadRefPos + gamepadRefOffset * countUnassignedGamepads);
            
        }
        // Check if a gamepad was removed
        else if (change == InputDeviceChange.Removed && device is Gamepad)
        {
            Gamepad gamepad = device as Gamepad;
            p1Column.Remove(gamepad);
            p2Column.Remove(gamepad);
            Destroy(gamepadInfo[gamepad].icon);
            gamepadInfo.Remove(gamepad);
        }
    }

    private void MoveUpOtherGamepads(string column)
    {
        if (column == "p1")
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                Vector3 gamepadPosition = gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D;
                Vector3 firstInListPosition = gamepadRefPos + gamepadColumnOffset;
                if (p1Column.Contains(gamepad) && gamepadPosition != firstInListPosition)
                {
                    gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D -= gamepadRefOffset;
                }
            }
        }
        else if (column == "p2")
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                Vector3 gamepadPosition = gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D;
                Vector3 firstInListPosition = gamepadRefPos - gamepadColumnOffset;
                if (p2Column.Contains(gamepad) && gamepadPosition != firstInListPosition)
                {
                    gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D -= gamepadRefOffset;
                }
            }
        }
        else if (column == "middle")
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                Vector3 gamepadPosition = gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D;
                Vector3 firstInListPosition = gamepadRefPos;
                if (!p1Column.Contains(gamepad) && !p2Column.Contains(gamepad) && gamepadPosition != firstInListPosition)
                {
                    gamepadInfo[gamepad].icon.GetComponent<RectTransform>().anchoredPosition3D -= gamepadRefOffset;
                }
            }
        }
        else
        {
            Debug.LogWarning("No such column " + column);
        }
    }

    private void SetMayChangeSide(Gamepad gamepad, bool value)
    {
        GamepadInfoStruct info = gamepadInfo[Gamepad.current];
        info.mayChangeSide = value;
        gamepadInfo[Gamepad.current] = info;
    }

    private void HandleSideChange()
    {
        if (Gamepad.current.leftStick.x.value == 0)
        {
            SetMayChangeSide(Gamepad.current, true);
        }

        if (!gamepadInfo[Gamepad.current].mayChangeSide)
        {
            return;
        }

        if (p1Column.Contains(Gamepad.current) && !player1Confirmed && Gamepad.current.leftStick.x.value > 0.5f)
        {
            int countUnassignedGamepads = Gamepad.all.Count - p1Column.Count - p2Column.Count;
            Vector3 position = gamepadRefPos + gamepadRefOffset * countUnassignedGamepads;
            gamepadInfo[Gamepad.current].icon.GetComponent<RectTransform>().anchoredPosition3D = position;
            p1Column.Remove(Gamepad.current);
            MoveUpOtherGamepads("p1");
            Debug.Log(Gamepad.current.name + " moved to unassigned");
            SetMayChangeSide(Gamepad.current, false);
        }
        else if (p2Column.Contains(Gamepad.current) && !player2Confirmed && Gamepad.current.leftStick.x.value < -0.5f)
        {
            int countUnassignedGamepads = Gamepad.all.Count - p1Column.Count - p2Column.Count;
            Vector3 position = gamepadRefPos + gamepadRefOffset * countUnassignedGamepads;
            gamepadInfo[Gamepad.current].icon.GetComponent<RectTransform>().anchoredPosition3D = position;
            p2Column.Remove(Gamepad.current);
            MoveUpOtherGamepads("p2");
            Debug.Log(Gamepad.current.name + " moved to unassigned");
            SetMayChangeSide(Gamepad.current, false);
        }
        else if (!p1Column.Contains(Gamepad.current) && !p2Column.Contains(Gamepad.current))
        {
            if (Gamepad.current.leftStick.x.value > 0.5f)
            {
                Vector3 position = gamepadRefPos + gamepadRefOffset * p2Column.Count - gamepadColumnOffset;
                gamepadInfo[Gamepad.current].icon.GetComponent<RectTransform>().anchoredPosition3D = position;
                p2Column.Add(Gamepad.current);
                MoveUpOtherGamepads("middle");
                Debug.Log(Gamepad.current.name + " assigned to P2");
                Debug.Log("P2 count " + p2Column.Count);
                SetMayChangeSide(Gamepad.current, false);
            }
            else if (Gamepad.current.leftStick.x.value < -0.5f)
            {
                Vector3 position = gamepadRefPos + gamepadRefOffset * p1Column.Count + gamepadColumnOffset;
                gamepadInfo[Gamepad.current].icon.GetComponent<RectTransform>().anchoredPosition3D = position;
                p1Column.Add(Gamepad.current);
                MoveUpOtherGamepads("middle");
                Debug.Log(Gamepad.current.name + " assigned to P1");
                Debug.Log("P1 count " + p1Column.Count);
                SetMayChangeSide(Gamepad.current, false);
            }
        }
    }

    private void HandlePlayersConfirmation()
    {
        if (p1Column.Count == 1 && p2Column.Count == 1)
        {
            player1ConfirmImage.gameObject.SetActive(true);
            player2ConfirmImage.gameObject.SetActive(true);

            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (p1Column.Contains(Gamepad.current))
                    player1Confirmed = !player1Confirmed;
                else
                    player2Confirmed = !player2Confirmed;
            }
        }
        else
        {
            player1ConfirmImage.sprite = unconfirmedSprite;
            player2ConfirmImage.sprite = unconfirmedSprite;
            player1ConfirmImage.gameObject.SetActive(false);
            player2ConfirmImage.gameObject.SetActive(false);
            player1Confirmed = false;
            player2Confirmed = false;
        }

        if (player1Confirmed)
        {
            player1ConfirmImage.sprite = confirmedSprite;
        }
        else
        {
            player1ConfirmImage.sprite = unconfirmedSprite;
        }
        if (player2Confirmed)
        {
            player2ConfirmImage.sprite = confirmedSprite;
        }
        else
        {
            player2ConfirmImage.sprite = unconfirmedSprite;
        }

        if (player1Confirmed && player2Confirmed)
        {
            SplitScreenMode.isSelected = true;
            SplitScreenMode.player1Gamepad = p1Column[0];
            SplitScreenMode.player2Gamepad = p2Column[0];
            Debug.Log("Start!");
            SceneManager.LoadScene(MultiplayerSelectedLevel.level);
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        countUnassignedGamepads = Gamepad.all.Count - p1Column.Count - p2Column.Count;
        int maxLength = new[] { p1Column.Count, countUnassignedGamepads, p2Column.Count }.Max();
        backButton.GetComponent<RectTransform>().anchoredPosition3D = gamepadRefPos + gamepadRefOffset * Mathf.Max(1, maxLength);

        HandleSideChange();
        HandlePlayersConfirmation();
        
    }
}
