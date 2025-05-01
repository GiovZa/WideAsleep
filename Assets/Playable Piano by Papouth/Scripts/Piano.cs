using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using playerChar;

[RequireComponent(typeof(AudioSource))]
public class Piano : Interactable
{
    #region Variables
    [SerializeField] private GameObject baseSheet;
    private int actualSheet;
    [SerializeField] private GameObject personalSheet;
    private bool isPianoActive = false;

    [Header("Notes Particles")]
    [Tooltip("Particles emitted by pressing keys")]
    [SerializeField] private ParticleSystem[] particlesPiano;
    [SerializeField] private bool enableParticle;

    [SerializeField] private float actualVolume;
    [SerializeField] private TextMeshProUGUI volAmount;

    [SerializeField] private AudioClip[] notes;
    [SerializeField] private GameObject[] keys;

    private AudioSource audioSource;
    private PlayerCharacterController playerCharacterController;

    [SerializeField] private GameObject keyboard;

    private List<KeyCode> recordedKeys = new List<KeyCode>();

    private List<KeyCode> solutionKeys = new List<KeyCode>
    {
        KeyCode.I,
        KeyCode.P,
        KeyCode.D,
        KeyCode.T,
        KeyCode.D
    };

    #endregion


    #region Built-in Methods
    private void Awake()
    {
        isPianoActive = false;

        audioSource = GetComponent<AudioSource>();

        personalSheet.SetActive(isPianoActive);
        baseSheet.SetActive(isPianoActive);

        actualSheet = 0;

        // baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];

        actualVolume = 0.5f;

        if (keyboard == null)
        {
            keyboard = GameObject.Find("Keyboard");
        }

        if (keyboard != null)
        {
            keyboard.SetActive(isPianoActive);
        }
        else
        {
            Debug.LogWarning("Keyboard UI not found!");
        }
    }

    #region Interaction Implementation
    public override void Interact()
    {
        if (!NoteManager.Instance.HasEnoughNotes())
        {
            Debug.Log("You need more notes to use the piano!");
            return;
        }

        if (!isPianoActive)
        {
            ActivatePiano();
        }
        else
        {
            DeactivatePiano();
        }
    }

    private void ActivatePiano()
    {
        isPianoActive = true;
        baseSheet.SetActive(isPianoActive);

        Debug.Log("Piano activated! Press ESC to exit.");

        if (keyboard != null)
        {
            keyboard.SetActive(isPianoActive);
        }

        // Disable player movement while playing
        if (playerCharacterController == null)
        {
            playerCharacterController = FindObjectOfType<PlayerCharacterController>();
        }

        if (playerCharacterController != null)
        {
            playerCharacterController.DisableMovement();
        }
    }

    private void DeactivatePiano()
    {
        isPianoActive = false;
        baseSheet.SetActive(isPianoActive);

        if (keyboard != null)
        {
            keyboard.SetActive(isPianoActive);
        }

        recordedKeys.Clear();

        // Enable player movement
        if (playerCharacterController != null)
        {
            playerCharacterController.EnableMovement();
        }

        Debug.Log("Piano deactivated!");
    }

    private void Update()
    {
        if (isPianoActive)
        {
            PianoInputs();
            // SheetDisplay();
            // Volume();
            Particle();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeactivatePiano();
            }
        }
    }
    #endregion

    #region Customs Methods
    /// <summary>
    /// Piano Inputs Management
    /// </summary>

    private void PianoInputs()
    {
        #region Whites and Blacks Keys
        // Key 1 and 1 UP / !
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayKey(2, 35, "BlackLeft", KeyCode.Alpha1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayKey(1, 39, "WhiteLeft", KeyCode.Alpha1);
        }


        // Key 2 and 2 UP / @
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayKey(4, 36, "BlackMid", KeyCode.Alpha2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayKey(3, 38, "WhiteMidLeft", KeyCode.Alpha2);
        }


        // Key 4 and 4 UP / $
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayKey(7, 0, "BlackLeft", KeyCode.Alpha4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayKey(6, 6, "WhiteLeft", KeyCode.Alpha4);
        }


        // Key 5 and 5 UP / %
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                PlayKey(9, 1, "BlackMid", KeyCode.Alpha5);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayKey(8, 4, "WhiteMidLeft", KeyCode.Alpha5);
        }


        // Key 6 and 6 UP / ^
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                PlayKey(11, 2, "BlackRight", KeyCode.Alpha6);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayKey(10, 5, "WhiteMidRight", KeyCode.Alpha6);
        }


        // Key 8 and 8 UP / *
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                PlayKey(14, 50, "BlackLeft", KeyCode.Alpha8);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            PlayKey(13, 54, "WhiteLeft", KeyCode.Alpha8);
        }


        // Key 9 and 9 UP / (
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                PlayKey(16, 51, "BlackRight", KeyCode.Alpha9);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PlayKey(15, 53, "WhiteMid", KeyCode.Alpha9);
        }


        // Key A and A UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayKey(45, 21, "BlackLeft", KeyCode.A);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            PlayKey(44, 27, "WhiteLeft", KeyCode.A);
        }


        // Key Z and Z UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayKey(55, 22, "BlackMid", KeyCode.Z);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayKey(54, 25, "WhiteMidLeft", KeyCode.Z);
        }


        // Key E and E UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayKey(25, 23, "BlackRight", KeyCode.E);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PlayKey(24, 26, "WhiteMidRight", KeyCode.E);
        }

        // SOLUTION
        // Key T and T UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                PlayKey(50, 45, "BlackLeft", KeyCode.T);
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            PlayKey(49, 49, "WhiteLeft", KeyCode.T);
        }


        // Key Y and Y UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                PlayKey(58, 46, "BlackMid", KeyCode.Y);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayKey(57, 48, "WhiteMidLeft", KeyCode.Y);
        }

        // SOLUTION
        // Key I and I UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                PlayKey(32, 14, "BlackLeft", KeyCode.I);
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            PlayKey(31, 20, "WhiteLeft", KeyCode.I);
        }

        // SOLUTION
        // Key O and O UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                PlayKey(41, 15, "BlackMid", KeyCode.O);
            }
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            PlayKey(40, 18, "WhiteMidLeft", KeyCode.O);
        }


        // Key P and P UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PlayKey(43, 16, "BlackRight", KeyCode.P);
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PlayKey(42, 19, "WhiteMidRight", KeyCode.P);
        }


        // Key S and S UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                PlayKey(48, 55, "BlackLeft", KeyCode.S);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PlayKey(47, 59, "WhiteLeft", KeyCode.S);
        }

        // SOLUTION
        // Key D and D UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayKey(23, 56, "BlackMid", KeyCode.D);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PlayKey(22, 58, "WhiteMidLeft", KeyCode.D);
        }


        // Key G and G UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                PlayKey(28, 28, "BlackLeft", KeyCode.G);
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            PlayKey(27, 34, "WhiteLeft", KeyCode.G);
        }


        // Key H and H UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayKey(30, 29, "BlackMid", KeyCode.H);
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            PlayKey(29, 32, "WhiteMidLeft", KeyCode.H);
        }


        // Key J and J UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                PlayKey(34, 30, "BlackRight", KeyCode.J);
            }
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            PlayKey(33, 33, "WhiteMidRight", KeyCode.J);
        }


        // Key L and L UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayKey(37, 40, "BlackLeft", KeyCode.L);
            }
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            PlayKey(36, 44, "WhiteLeft", KeyCode.L);
        }


        // Key W and W UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                PlayKey(60, 41, "BlackMid", KeyCode.W);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            PlayKey(59, 43, "WhiteMidLeft", KeyCode.W);
        }


        // Key C and C UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                PlayKey(21, 7, "BlackLeft", KeyCode.C);
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            PlayKey(20, 13, "WhiteLeft", KeyCode.C);
        }


        // Key V and V UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                PlayKey(53, 8, "BlackMid", KeyCode.V);
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            PlayKey(52, 11, "WhiteMidLeft", KeyCode.V);
        }


        // Key B and B UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                PlayKey(19, 9, "BlackRight", KeyCode.B);
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            PlayKey(18, 12, "WhiteMidRight", KeyCode.B);
        }

        #endregion

        #region Whites Keys Only
        // Key 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayKey(5, 37, "WhiteRight", KeyCode.Alpha3);
        }

        // Key 7
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PlayKey(12, 3, "WhiteRight", KeyCode.Alpha7);
        }

        // Key 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayKey(0, 52, "WhiteRight", KeyCode.Alpha0);
        }

        // Key R
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayKey(46, 24, "WhiteRight", KeyCode.R);
        }

        // SOLUTION
        // Key U
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayKey(51, 47, "WhiteRight", KeyCode.U);
        }

        // Key Q -
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayKey(17, 17, "WhiteRight", KeyCode.Q);
        }

        // Key F
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayKey(26, 57, "WhiteRight", KeyCode.F);
        }

        // Key K
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayKey(35, 31, "WhiteRight", KeyCode.K);
        }

        // Key X
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayKey(56, 42, "WhiteRight", KeyCode.X);
        }

        // Key N
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayKey(39, 10, "WhiteRight", KeyCode.N);
        }

        // Key M
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayKey(38, 60, "White", KeyCode.M);
        }

        #endregion
    }

    private void PlayKey(int noteIndex, int keyIndex, string animation, KeyCode key)
    {
        audioSource.PlayOneShot(notes[noteIndex]);
        keys[keyIndex].GetComponent<Animator>().Play(animation);
        RecordKey(key);
    }

    private void RecordKey(KeyCode key)
    {
        recordedKeys.Add(key);
        Debug.Log("Recorded: " + key);

        if (recordedKeys.Count == 5)
        {
            CheckPuzzleSolution();

            recordedKeys.Clear(); // Clear and record next 5
            Debug.Log("Cleared sequence!");
        }

        return;
    }

    private void CheckPuzzleSolution()
    {
        if (recordedKeys.Count < solutionKeys.Count)
        {
            return;
        }

        for (int i = 0; i < solutionKeys.Count; i++)
        {
            if (recordedKeys[i] != solutionKeys[i])
            {
                Debug.Log("Incorrect sequence!");
                return;
            }
        }

        Debug.Log("Correct sequence!");
        OnPuzzleSolved();
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("Puzzle Solved!");
    }

    /// <summary>
    /// Sheets Displaying Management
    /// </summary>
    /*private void SheetDisplay()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Displaying sheets UI [on/off] if spacebar is pressed
            baseSheet.SetActive(!state);
            state = !state;
        }

        if (baseSheet.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) NextSheet();
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) PreviousSheet();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Displaying personnal sheets [on/off]
            personalSheet.SetActive(!state2);
            state2 = !state2;
        }
    }*/

    /// <summary>
    /// Go to next music sheet
    /// </summary>
    /*public void NextSheet()
    {
        if (actualSheet < sheets.Length - 1)
        {
            baseSheet.GetComponentInChildren<Image>().sprite = null;

            actualSheet++;

            baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];
        }
        else if (actualSheet >= sheets.Length - 1)
        {
            baseSheet.GetComponentInChildren<Image>().sprite = null;

            actualSheet = 0;

            baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];
        }
    }*/

    /// <summary>
    /// Go to previous music sheet
    /// </summary>
    /*public void PreviousSheet()
    {
        if (actualSheet > 0)
        {
            baseSheet.GetComponentInChildren<Image>().sprite = null;

            actualSheet--;

            baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];
        }
        else if (actualSheet <= 0)
        {
            baseSheet.GetComponentInChildren<Image>().sprite = null;

            actualSheet = sheets.Length - 1;

            baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];
        }
    }*/

    /// <summary>
    /// Increase or decrease piano volume
    /// </summary>

    // Set piano vol after testing
    /*private void Volume()
    {
        audioSource.volume = actualVolume;

        if (actualVolume < 0.1f)
        {
            volAmount.text = "0 %";
        }
        else { volAmount.text = string.Format("{0:#} %", audioSource.volume * 100); }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (actualVolume < 0.91f) actualVolume += 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (actualVolume >= 0.1f) actualVolume -= 0.1f;
        }
    }*/

    /// <summary>
    /// Access to an online music sheets library
    /// </summary>
    /*public void LibrarySheet()
    {
        Application.OpenURL("https://virtualpiano.net/music-sheets/");
    }*/

    /// <summary>
    /// Display particle when keys are pressed
    /// </summary>

    // Change to when certain keys are pressed
    private void Particle()
    {
        if (enableParticle)
        {
            if (Input.anyKeyDown)
            {
                particlesPiano[Random.Range(0, particlesPiano.Length)].Play();
            }
        }
    }

    #endregion
}

#endregion