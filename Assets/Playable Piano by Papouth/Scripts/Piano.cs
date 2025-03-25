using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using temp;

[RequireComponent(typeof(AudioSource))]
public class Piano : Interactable
{
    #region Variables
    [Header("Music Sheets")]
    [Tooltip("Sheets music that can be played")]
    [SerializeField] private Sprite[] sheets;
    [SerializeField] private GameObject baseSheet;
    private bool state;
    private int actualSheet;
    [SerializeField] private GameObject personalSheet;
    private bool state2;
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
    #endregion


    #region Built-in Methods
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        personalSheet.SetActive(state2);
        baseSheet.SetActive(state);
        actualSheet = 0;

        baseSheet.GetComponentInChildren<Image>().sprite = sheets[actualSheet];

        actualVolume = 0.5f;
    }

    #region Interaction Implementation
    public override void Interact()
    {
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
        baseSheet.SetActive(true);
        Debug.Log("Piano activated! Press ESC to exit.");

        // Disable player movement while playing
        if (playerCharacterController == null)
        {
            playerCharacterController = FindObjectOfType<PlayerCharacterController>();
        }

        /* if (playerCharacterController != null)
        {
            playerCharacterController.DisableMovement();
        } */
    }

    private void DeactivatePiano()
    {
        isPianoActive = false;
        baseSheet.SetActive(false);
        Debug.Log("Piano deactivated!");

        // Enable player movement
        /* if (playerCharacterController != null)
        {
            playerCharacterController.EnableMovement();
        } */ 
    }

    private void Update()
    {
        if (isPianoActive)
        {
            PianoInputs();
            // SheetDisplay();
            Volume();
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

    // Delete keys as needed
    // Commented out WASD keystrokes for now
    private void PianoInputs()
    {
        #region Whites and Blacks Keys
        // Key 1 and 1 UP / !
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                audioSource.PlayOneShot(notes[2]);
                keys[35].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            audioSource.PlayOneShot(notes[1]);
            keys[39].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key 2 and 2 UP / @
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                audioSource.PlayOneShot(notes[4]);
                keys[36].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            audioSource.PlayOneShot(notes[3]);
            keys[38].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key 4 and 4 UP / $
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                audioSource.PlayOneShot(notes[7]);
                keys[0].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            audioSource.PlayOneShot(notes[6]);
            keys[6].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key 5 and 5 UP / %
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                audioSource.PlayOneShot(notes[9]);
                keys[1].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            audioSource.PlayOneShot(notes[8]);
            keys[4].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key 6 and 6 UP / ^
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                audioSource.PlayOneShot(notes[11]);
                keys[2].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            audioSource.PlayOneShot(notes[10]);
            keys[5].GetComponent<Animator>().Play("WhiteMidRight");
        }


        // Key 8 and 8 UP / *
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                audioSource.PlayOneShot(notes[14]);
                keys[50].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            audioSource.PlayOneShot(notes[13]);
            keys[54].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key 9 and 9 UP / (
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                audioSource.PlayOneShot(notes[16]);
                keys[51].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            audioSource.PlayOneShot(notes[15]);
            keys[53].GetComponent<Animator>().Play("WhiteMid");
        }


        /*// Key A and A UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                audioSource.PlayOneShot(notes[45]);
                keys[21].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            audioSource.PlayOneShot(notes[44]);
            keys[27].GetComponent<Animator>().Play("WhiteLeft");
        }*/


        // Key Z and Z UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                audioSource.PlayOneShot(notes[55]);
                keys[22].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.PlayOneShot(notes[54]);
            keys[25].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key E and E UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                audioSource.PlayOneShot(notes[25]);
                keys[23].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            audioSource.PlayOneShot(notes[24]);
            keys[26].GetComponent<Animator>().Play("WhiteMidRight");
        }


        // Key T and T UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                audioSource.PlayOneShot(notes[50]);
                keys[45].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            audioSource.PlayOneShot(notes[49]);
            keys[49].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key Y and Y UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                audioSource.PlayOneShot(notes[58]);
                keys[46].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            audioSource.PlayOneShot(notes[57]);
            keys[48].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key I and I UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                audioSource.PlayOneShot(notes[32]);
                keys[14].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            audioSource.PlayOneShot(notes[31]);
            keys[20].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key O and O UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                audioSource.PlayOneShot(notes[41]);
                keys[15].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            audioSource.PlayOneShot(notes[40]);
            keys[18].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key P and P UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                audioSource.PlayOneShot(notes[43]);
                keys[16].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            audioSource.PlayOneShot(notes[42]);
            keys[19].GetComponent<Animator>().Play("WhiteMidRight");
        }


        /*// Key S and S UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                audioSource.PlayOneShot(notes[48]);
                keys[55].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            audioSource.PlayOneShot(notes[47]);
            keys[59].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key D and D UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                audioSource.PlayOneShot(notes[23]);
                keys[56].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            audioSource.PlayOneShot(notes[22]);
            keys[58].GetComponent<Animator>().Play("WhiteMidLeft");
        }*/


        // Key G and G UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                audioSource.PlayOneShot(notes[28]);
                keys[28].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            audioSource.PlayOneShot(notes[27]);
            keys[34].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key H and H UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                audioSource.PlayOneShot(notes[30]);
                keys[29].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            audioSource.PlayOneShot(notes[29]);
            keys[32].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key J and J UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                audioSource.PlayOneShot(notes[34]);
                keys[30].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            audioSource.PlayOneShot(notes[33]);
            keys[33].GetComponent<Animator>().Play("WhiteMidRight");
        }


        // Key L and L UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                audioSource.PlayOneShot(notes[37]);
                keys[40].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            audioSource.PlayOneShot(notes[36]);
            keys[44].GetComponent<Animator>().Play("WhiteLeft");
        }


        /*// Key W and W UP -
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                audioSource.PlayOneShot(notes[60]);
                keys[41].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            audioSource.PlayOneShot(notes[59]);
            keys[43].GetComponent<Animator>().Play("WhiteMidLeft");
        }*/


        // Key C and C UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                audioSource.PlayOneShot(notes[21]);
                keys[7].GetComponent<Animator>().Play("BlackLeft");
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            audioSource.PlayOneShot(notes[20]);
            keys[13].GetComponent<Animator>().Play("WhiteLeft");
        }


        // Key V and V UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                audioSource.PlayOneShot(notes[53]);
                keys[8].GetComponent<Animator>().Play("BlackMid");
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            audioSource.PlayOneShot(notes[52]);
            keys[11].GetComponent<Animator>().Play("WhiteMidLeft");
        }


        // Key B and B UP
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                audioSource.PlayOneShot(notes[19]);
                keys[9].GetComponent<Animator>().Play("BlackRight");
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            audioSource.PlayOneShot(notes[18]);
            keys[12].GetComponent<Animator>().Play("WhiteMidRight");
        }

        #endregion

        #region Whites Keys Only
        // Key 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            audioSource.PlayOneShot(notes[5]);
            keys[37].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key 7
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            audioSource.PlayOneShot(notes[12]);
            keys[3].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key 0
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            audioSource.PlayOneShot(notes[0]);
            keys[52].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key R
        if (Input.GetKeyDown(KeyCode.R))
        {
            audioSource.PlayOneShot(notes[46]);
            keys[24].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key U
        if (Input.GetKeyDown(KeyCode.U))
        {
            audioSource.PlayOneShot(notes[51]);
            keys[47].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key Q -
        if (Input.GetKeyDown(KeyCode.Q))
        {
            audioSource.PlayOneShot(notes[17]);
            keys[17].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key F
        /* if (Input.GetKeyDown(KeyCode.F))
        {
            audioSource.PlayOneShot(notes[26]);
            keys[57].GetComponent<Animator>().Play("WhiteRight");
        } */

        // Key K
        if (Input.GetKeyDown(KeyCode.K))
        {
            audioSource.PlayOneShot(notes[35]);
            keys[31].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key X
        if (Input.GetKeyDown(KeyCode.X))
        {
            audioSource.PlayOneShot(notes[56]);
            keys[42].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key N
        if (Input.GetKeyDown(KeyCode.N))
        {
            audioSource.PlayOneShot(notes[39]);
            keys[10].GetComponent<Animator>().Play("WhiteRight");
        }

        // Key M
        if (Input.GetKeyDown(KeyCode.M))
        {
            audioSource.PlayOneShot(notes[38]);
            keys[60].GetComponent<Animator>().Play("White");
        }

        #endregion
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
    private void Volume()
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
    }

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