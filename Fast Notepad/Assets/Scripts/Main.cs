using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    [SerializeField] private GameObject appUI;
    [SerializeField] private GameObject noteUI;
    [SerializeField] private GameObject noteFolderUI;
    [SerializeField] private GameObject noteRecycleUI;
    [SerializeField] private GameObject noteSearchedUI;
    [SerializeField] private GameObject sideUI;
    [SerializeField] private GameObject newFolderUI;
    [SerializeField] private GameObject folderNotesUI;
    [SerializeField] private GameObject recycleNotesUI;
    [SerializeField] private GameObject folderEditUI;
    [SerializeField] private GameObject folderDeleteUI;
    [SerializeField] private GameObject longpressUI;
    [SerializeField] private GameObject longpressFolderUI;
    [SerializeField] private GameObject longpressBinUI;
    [SerializeField] private GameObject[] noteLists;
    [SerializeField] private GameObject[] noteListsFolder;
    [SerializeField] private GameObject[] noteListsRecycle;
    [SerializeField] private GameObject[] noteListsSearch;
    [SerializeField] private GameObject sideNewBtn;
    [SerializeField] private GameObject sideNewfolderBtn;
    [SerializeField] private InputField inputNote;
    [SerializeField] private InputField inputNoteFolder;
    [SerializeField] private InputField inputNewName;
    [SerializeField] private InputField inputSearch;
    [SerializeField] private Text textRecycleBin;
    [SerializeField] private Text mynotesText;    
    [SerializeField] private Text recycleText;
    [SerializeField] private Text folderText;
    [SerializeField] private Text folderNameText;
    [SerializeField] private string selectedType;
    [SerializeField] private string selectedText;
    [SerializeField] private int listNum;
    [SerializeField] private int listNumFolder;
    [SerializeField] private int listNumRecycle;
    [SerializeField] private int selectedNum;

    List<int> searchType = new List<int>();
    List<int> searchNum = new List<int>();

    private bool isPressed = false;
    private bool isLongPressed = false;
    private bool isEditing = false;
    private bool isSearching = false;

    void Start()
    {
        listNum = PlayerPrefs.GetInt("NO_LIST_NUM");
        listNumFolder = PlayerPrefs.GetInt("FOL_LIST_NUM");
        listNumRecycle = PlayerPrefs.GetInt("REY_LIST_NUM");
        RefreshLists();
        RefreshListsFolder();
        RefreshRecycleBin();    
    }

    void Update()
    {
        appUI.transform.localScale = new Vector3(Screen.width / 720f, Screen.height / 1440f, 1f);

        if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;
            isLongPressed = false;
            StartCoroutine(LongPressCoroutine());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            StopAllCoroutines();
        }
    }

    public void NewNoteMain()
    {          
        inputNote.text = "";
        isEditing = false;
    }

    // save notes
    public void SaveNoteMain()
    {
        if(inputNote.text != "")
        {
            if (!isEditing)
            {
                listNum++;
                PlayerPrefs.SetInt("NO_LIST_NUM", listNum);        
            }
         
            DateTime currentDate = DateTime.Now;
            string dateStr = currentDate.DayOfWeek.ToString().Substring(0, 3) + " " + currentDate.Day + ", "
                + currentDate.Year + " " + currentDate.ToString().Split(" ")[1];

            if(int.Parse(currentDate.ToString().Split(" ")[1].Split(":")[0]) <= 12)
            {
                dateStr += " AM";
            }
            else
            {
                dateStr += " PM";
            }
            
            PlayerPrefs.SetString("NO_NOTE" + listNum, inputNote.text);
            PlayerPrefs.SetString("NO_DATE" + listNum, dateStr);                

            if (isSearching)
            {
                for(int i = 0; i < searchNum.Count; i++)
                {
                    if(searchType[i] == 1 && searchNum[i] == listNum)
                    {
                        noteListsSearch[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("NO_NOTE" + searchNum[i]);
                        noteListsSearch[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("NO_DATE" + searchNum[i]);

                        if (PlayerPrefs.GetInt("NO_COMP" + searchNum[i]) == 1)
                        {
                            noteListsSearch[i].transform.GetChild(3).gameObject.SetActive(true);
                        }
                    }                 
                }
            }

            UpdateLists(listNum);
        }

        isEditing = false;
        isSearching = false;
        RefreshLists();
    }

    // show note details
    public void NoteListClick()
    {
        if (!isLongPressed)
        {
            listNum = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
            listNum = PlayerPrefs.GetInt("NO_LIST_NUM") - listNum + 1;
            isEditing = true;
            inputNote.text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            selectedText = inputNote.text;
            selectedType = EventSystem.current.currentSelectedGameObject.tag;
            noteUI.SetActive(true);
        }  
    }

    public void FolderListClick()
    {
        if (!isLongPressed)
        {
            listNumFolder = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
            listNumFolder = PlayerPrefs.GetInt("FOL_LIST_NUM") - listNumFolder + 1;
            isEditing = true;
            inputNoteFolder.text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            selectedText = inputNoteFolder.text;
            selectedType = EventSystem.current.currentSelectedGameObject.tag;
            noteFolderUI.SetActive(true);
        }
    }

    public void SearchListClick()
    {
        int listNumSearch = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
        isSearching = true;

        if (searchType[listNumSearch - 1] == 1)
        {
            listNum = searchNum[listNumSearch - 1];         
            isEditing = true;
            inputNote.text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            selectedText = inputNote.text;
            selectedType = EventSystem.current.currentSelectedGameObject.tag;
            noteUI.SetActive(true);
        }
        else
        {
            listNumFolder = searchNum[listNumSearch - 1];         
            isEditing = true;
            inputNoteFolder.text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            selectedText = inputNoteFolder.text;
            selectedType = EventSystem.current.currentSelectedGameObject.tag;
            noteFolderUI.SetActive(true);
        }
    }

    public void BinListClick()
    {
        listNumRecycle = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
        listNumRecycle = PlayerPrefs.GetInt("BIN_LIST_NUM") - listNumRecycle + 1;        
        textRecycleBin.text = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
        recycleNotesUI.SetActive(true);
    }

    // new folder
    public void NewFolder()
    {
        if(sideNewfolderBtn.transform.GetChild(2).gameObject.activeSelf == true)
        {
            newFolderUI.SetActive(true);            
            sideUI.SetActive(false);
        }
        else
        {
            folderEditUI.SetActive(true);
            sideUI.SetActive(false);
        }
    }

    public void NewFolderOk()
    {
        if(inputNewName.text != "")
        {
            newFolderUI.SetActive(false);
            sideNewfolderBtn.transform.GetChild(2).gameObject.SetActive(false);
            sideNewfolderBtn.transform.GetChild(3).gameObject.SetActive(true);
            sideNewBtn.SetActive(true);
            sideNewBtn.transform.GetChild(2).GetComponent<Text>().text = inputNewName.text;
            PlayerPrefs.SetInt("NEW_FOLDER", 1);
            PlayerPrefs.SetString("FOLDER_NAME", inputNewName.text);
            FolderNameClick();
        }
    }

    public void FolderNameClick()
    {       
        GotoMain();
        folderNotesUI.SetActive(true);
        folderNameText.text = folderNameText.text = PlayerPrefs.GetString("FOLDER_NAME");
    }

    public void RenameFolder()
    {
        newFolderUI.SetActive(true);
        folderEditUI.SetActive(false);
    }

    public void DeleteFolder()
    {
        folderDeleteUI.SetActive(true);
        folderEditUI.SetActive(false);
    }

    public void DeleteFolderOk()
    {
        folderDeleteUI.SetActive(false);

        int listCount = PlayerPrefs.GetInt("FOL_LIST_NUM");

        for(int i = 0; i < listCount; i++)
        {
            selectedText = PlayerPrefs.GetString("FOL_NOTE" + (i + 1));
            PlayerPrefs.SetInt("FOL_COMP" + (i + 1), 0);
            AddToRecycleBin();            
        }

        PlayerPrefs.SetInt("FOL_LIST_NUM", 0);
        GotoMain();
        RefreshListsFolder();
    }

    public void NewNoteFolder()
    {
        inputNoteFolder.text = "";
        isEditing = false;
        noteFolderUI.SetActive(true);
    }

    public void SaveNoteFolder()
    {
        if (inputNoteFolder.text != "")
        {
            if (!isEditing)
            {
                listNumFolder++;
                PlayerPrefs.SetInt("FOL_LIST_NUM", listNumFolder);
            }

            DateTime currentDate = DateTime.Now;
            string dateStr = currentDate.DayOfWeek.ToString().Substring(0, 3) + " " + currentDate.Day + ", "
                + currentDate.Year + " " + currentDate.ToString().Split(" ")[1];

            if (int.Parse(currentDate.ToString().Split(" ")[1].Split(":")[0]) <= 12)
            {
                dateStr += " AM";
            }
            else
            {
                dateStr += " PM";
            }

            PlayerPrefs.SetString("FOL_NOTE" + listNumFolder, inputNoteFolder.text);
            PlayerPrefs.SetString("FOL_DATE" + listNumFolder, dateStr);            

            if (isSearching)
            {
                for (int i = 0; i < searchNum.Count; i++)
                {
                    if (searchType[i] == 2 && searchNum[i] == listNumFolder)
                    {
                        noteListsSearch[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_NOTE" + searchNum[i]);
                        noteListsSearch[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_DATE" + searchNum[i]);

                        if (PlayerPrefs.GetInt("FOL_COMP" + searchNum[i]) == 1)
                        {
                            noteListsSearch[i].transform.GetChild(3).gameObject.SetActive(true);
                        }
                    }
                }
            }

            UpdateListsFolder(listNumFolder);
        }

        isEditing = false;
        isSearching = false;
        RefreshListsFolder();
    }

    public void SideMenuShow()
    {
        listNum = PlayerPrefs.GetInt("NO_LIST_NUM");
        listNumFolder = PlayerPrefs.GetInt("FOL_LIST_NUM");
        listNumRecycle = PlayerPrefs.GetInt("BIN_LIST_NUM");

        mynotesText.text = listNum.ToString();
        folderText.text = listNumFolder.ToString();
        recycleText.text = listNumRecycle.ToString();
    }

    public void GotoMain()
    {
        noteUI.SetActive(false);
        noteFolderUI.SetActive(false);
        noteSearchedUI.SetActive(false);
        folderNotesUI.SetActive(false);
        noteRecycleUI.SetActive(false);
        recycleNotesUI.SetActive(false);
        sideUI.SetActive(false);
    }

    // refresh note lists
    public void RefreshLists()
    {
        for(int i = 0; i < 20; i++)
        {
            noteLists[i].SetActive(false);
            noteLists[i].transform.GetChild(3).gameObject.SetActive(false);
        }
        
        int listCount = PlayerPrefs.GetInt("NO_LIST_NUM");
        mynotesText.text = listCount.ToString();
        
        for(int i = 0; i < listCount; i++)
        {
            noteLists[i].SetActive(true);
            noteLists[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("NO_NOTE" + (listCount - i));
            noteLists[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("NO_DATE" + (listCount - i));

            if(PlayerPrefs.GetInt("NO_COMP" + (listCount - i)) == 1)
            {
                noteLists[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public void RefreshListsFolder()
    {
        if(PlayerPrefs.GetInt("NEW_FOLDER") == 1)
        {
            sideNewfolderBtn.transform.GetChild(2).gameObject.SetActive(false);
            sideNewfolderBtn.transform.GetChild(3).gameObject.SetActive(true);
            sideNewBtn.SetActive(true);
            sideNewBtn.transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetString("FOLDER_NAME");
            folderNameText.text = PlayerPrefs.GetString("FOLDER_NAME");
        }
        else
        {
            sideNewfolderBtn.transform.GetChild(2).gameObject.SetActive(true);
            sideNewfolderBtn.transform.GetChild(3).gameObject.SetActive(false);
            sideNewBtn.SetActive(false);
        }

        if(PlayerPrefs.GetInt("FOL_LIST_NUM") == 0)
        {
            sideNewBtn.SetActive(false);
            sideNewfolderBtn.transform.GetChild(2).gameObject.SetActive(true);
            sideNewfolderBtn.transform.GetChild(3).gameObject.SetActive(false);
        }
        
        for (int i = 0; i < 20; i++)
        {
            noteListsFolder[i].SetActive(false);
            noteListsFolder[i].transform.GetChild(3).gameObject.SetActive(false);
        }

        int listCount = PlayerPrefs.GetInt("FOL_LIST_NUM");
        folderText.text = listCount.ToString();

        for (int i = 0; i < listCount; i++)
        {
            noteListsFolder[i].SetActive(true);
            noteListsFolder[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_NOTE" + (listCount - i));
            noteListsFolder[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_DATE" + (listCount - i));

            if (PlayerPrefs.GetInt("FOL_COMP" + (listCount - i)) == 1)
            {
                noteListsFolder[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    public void RefreshRecycleBin()
    {
        for (int i = 0; i < 20; i++)
        {
            noteListsRecycle[i].SetActive(false);
        }

        int listCount = PlayerPrefs.GetInt("BIN_LIST_NUM");
        recycleText.text = listCount.ToString();

        for (int i = 0; i < listCount; i++)
        {
            noteListsRecycle[i].SetActive(true);
            noteListsRecycle[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("BIN_NOTE" + (listCount - i));
            noteListsRecycle[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("BIN_DATE" + (listCount - i));
        }
    }

    // update note lists
    public void UpdateLists(int newIndex)
    {
        int listCount = PlayerPrefs.GetInt("NO_LIST_NUM");     

        if(newIndex < listCount)
        {          
            string tempNote = PlayerPrefs.GetString("NO_NOTE" + newIndex);
            string tempDate = PlayerPrefs.GetString("NO_DATE" + newIndex);
            int tempComp = PlayerPrefs.GetInt("NO_COMP" + newIndex);

            for (int i = newIndex; i < listCount; i++)
            {
                PlayerPrefs.SetString("NO_NOTE" + i, PlayerPrefs.GetString("NO_NOTE" + (i + 1)));
                PlayerPrefs.SetString("NO_DATE" + i, PlayerPrefs.GetString("NO_DATE" + (i + 1)));
                PlayerPrefs.SetInt("NO_COMP" + i, PlayerPrefs.GetInt("NO_COMP" + (i + 1)));
            }

            PlayerPrefs.SetString("NO_NOTE" + listCount, tempNote);
            PlayerPrefs.SetString("NO_DATE" + listCount, tempDate);
            PlayerPrefs.SetInt("NO_COMP" + listCount, tempComp);
        }
    }

    public void UpdateListsFolder(int newIndex)
    {
        int listCount = PlayerPrefs.GetInt("FOL_LIST_NUM");

        if (newIndex < listCount)
        {
            string tempNote = PlayerPrefs.GetString("FOL_NOTE" + newIndex);
            string tempDate = PlayerPrefs.GetString("FOL_DATE" + newIndex);
            int tempComp = PlayerPrefs.GetInt("FOL_COMP" + newIndex);

            for (int i = newIndex; i < listCount; i++)
            {
                PlayerPrefs.SetString("FOL_NOTE" + i, PlayerPrefs.GetString("FOL_NOTE" + (i + 1)));
                PlayerPrefs.SetString("FOL_DATE" + i, PlayerPrefs.GetString("FOL_DATE" + (i + 1)));
                PlayerPrefs.SetInt("FOL_COMP" + i, PlayerPrefs.GetInt("FOL_COMP" + (i + 1)));
            }

            PlayerPrefs.SetString("FOL_NOTE" + listCount, tempNote);
            PlayerPrefs.SetString("FOL_DATE" + listCount, tempDate);
            PlayerPrefs.SetInt("FOL_COMP" + listCount, tempComp);
        }
    }

    public void CopyTextToClipboard(string textToCopy)
    {
        GUIUtility.systemCopyBuffer = textToCopy;
        Debug.Log("Text copied to clipboard: " + textToCopy);
    }

    // long press UI
    public void CopyText()
    {
        CopyTextToClipboard(selectedText);
        longpressUI.SetActive(false);
        longpressFolderUI.SetActive(false);
    }

    public void Completed()
    {                
        if (string.Equals(selectedType, "note"))
        {
            int listCount = PlayerPrefs.GetInt("NO_LIST_NUM");
            PlayerPrefs.SetInt("NO_COMP" + (listCount - listNum + 1), 1);
            longpressUI.SetActive(false);
            RefreshLists();
        }
        else if (string.Equals(selectedType, "folder"))
        {
            int listCount = PlayerPrefs.GetInt("FOL_LIST_NUM");
            PlayerPrefs.SetInt("FOL_COMP" + (listCount - listNumFolder + 1), 1);
            longpressFolderUI.SetActive(false);
            RefreshListsFolder();
        }
    }

    public void DeleteNoteFromList()
    {
        longpressUI.SetActive(false);
        longpressFolderUI.SetActive(false);

        if (string.Equals(selectedType, "note"))
        {
            DeleteNote(listNum);
        }
        else if(string.Equals(selectedType, "folder"))
        {
            DeleteNoteFolder(listNumFolder);
        }
        else if(string.Equals(selectedType, "recycle"))
        {
            DeleteNoteBin(listNumRecycle);
        }
    }

    public void DeleteNote(int deleteIndex)
    {
        int listCount = PlayerPrefs.GetInt("NO_LIST_NUM");
        deleteIndex = listCount - deleteIndex + 1;

        for (int i = deleteIndex; i < listCount; i++)
        {
            PlayerPrefs.SetString("NO_NOTE" + i, PlayerPrefs.GetString("NO_NOTE" + (i + 1)));
            PlayerPrefs.SetString("NO_DATE" + i, PlayerPrefs.GetString("NO_DATE" + (i + 1)));
            PlayerPrefs.SetInt("NO_COMP" + i, PlayerPrefs.GetInt("NO_COMP" + (i + 1)));
        }

        PlayerPrefs.SetInt("NO_LIST_NUM", listCount - 1);
        RefreshLists();
        AddToRecycleBin();
    }

    public void DeleteNoteFolder(int deleteIndex)
    {
        int listCount = PlayerPrefs.GetInt("FOL_LIST_NUM");
        deleteIndex = listCount - deleteIndex + 1;

        for (int i = deleteIndex; i < listCount; i++)
        {
            PlayerPrefs.SetString("FOL_NOTE" + i, PlayerPrefs.GetString("FOL_NOTE" + (i + 1)));
            PlayerPrefs.SetString("FOL_DATE" + i, PlayerPrefs.GetString("FOL_DATE" + (i + 1)));
            PlayerPrefs.SetInt("FOL_COMP" + i, PlayerPrefs.GetInt("FOL_COMP" + (i + 1)));
        }

        PlayerPrefs.SetInt("FOL_LIST_NUM", listCount - 1);
        RefreshListsFolder();
        AddToRecycleBin();
    }

    public void DeleteNoteBin(int deleteIndex)
    {
        int listCount = PlayerPrefs.GetInt("BIN_LIST_NUM");
        deleteIndex = listCount - deleteIndex + 1;

        for (int i = deleteIndex; i < listCount; i++)
        {
            PlayerPrefs.SetString("BIN_NOTE" + i, PlayerPrefs.GetString("BIN_NOTE" + (i + 1)));
            PlayerPrefs.SetString("BIN_DATE" + i, PlayerPrefs.GetString("BIN_DATE" + (i + 1)));      
        }

        PlayerPrefs.SetInt("BIN_LIST_NUM", listCount - 1);
        RefreshRecycleBin();
        longpressBinUI.SetActive(false);
    }

    public void AddToRecycleBin()
    {
        listNumRecycle = PlayerPrefs.GetInt("BIN_LIST_NUM");
        listNumRecycle++;

        DateTime currentDate = DateTime.Now;
        string dateStr = currentDate.DayOfWeek.ToString().Substring(0, 3) + " " + currentDate.Day + ", "
            + currentDate.Year + " " + currentDate.ToString().Split(" ")[1] + " " + currentDate.ToString().Split(" ")[2];

        PlayerPrefs.SetString("BIN_NOTE" + listNumRecycle, selectedText);
        PlayerPrefs.SetString("BIN_DATE" + listNumRecycle, dateStr);
        PlayerPrefs.SetInt("BIN_LIST_NUM", listNumRecycle);
        RefreshRecycleBin();
    }

    public void Search()
    {
        int listCountNote = PlayerPrefs.GetInt("NO_LIST_NUM");
        int listCountFolder = PlayerPrefs.GetInt("FOL_LIST_NUM");
        searchType = new List<int>();
        searchNum = new List<int>();

        // search letters
        for(int i = 1; i <= listCountNote; i++)
        {
            if(PlayerPrefs.GetString("NO_NOTE" + i).Contains(inputSearch.text))
            {
                searchType.Add(1);
                searchNum.Add(i);
            }
        }

        for(int i = 1; i <= listCountFolder; i++)
        {
            if(PlayerPrefs.GetString("FOL_NOTE" + i).Contains(inputSearch.text))
            {
                searchType.Add(2);
                searchNum.Add(i);
            }
        }

        // show search lists
        for (int i = 0; i < 20; i++)
        {
            noteListsSearch[i].SetActive(false);
            noteListsSearch[i].transform.GetChild(3).gameObject.SetActive(false);
        }

        for (int i = 0; i < searchType.Count; i++)
        {
            noteListsSearch[i].SetActive(true);

            if(searchType[i] == 1)
            {
                noteListsSearch[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("NO_NOTE" + searchNum[i]);
                noteListsSearch[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("NO_DATE" + searchNum[i]);

                if (PlayerPrefs.GetInt("NO_COMP" + searchNum[i]) == 1)
                {
                    noteListsSearch[i].transform.GetChild(3).gameObject.SetActive(true);
                }
            }
            else
            {
                noteListsSearch[i].transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_NOTE" + searchNum[i]);
                noteListsSearch[i].transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("FOL_DATE" + searchNum[i]);

                if (PlayerPrefs.GetInt("FOL_COMP" + searchNum[i]) == 1)
                {
                    noteListsSearch[i].transform.GetChild(3).gameObject.SetActive(true);
                }
            }            
        }                        
    }

    // Right side menu
    public void UndoEdit()
    {
        inputNote.text = selectedText;
        inputNoteFolder.text = selectedText;
    }

    public void DeleteEdit()
    {
        if(selectedType == "note")
        {
            DeleteNote(PlayerPrefs.GetInt("NO_LIST_NUM") - listNum + 1);
            RefreshLists();
            noteUI.SetActive(false);
        }
        else if(selectedType == "folder")
        {
            DeleteNoteFolder(PlayerPrefs.GetInt("FOL_LIST_NUM") - listNumFolder + 1);
            RefreshListsFolder();
            noteFolderUI.SetActive(false);
        }
        else if(selectedType == "recycle")
        {
            DeleteNoteBin(PlayerPrefs.GetInt("BIN_LIST_NUM") - listNumRecycle + 1);
            RefreshRecycleBin();
            noteRecycleUI.SetActive(false);
        }
    }

    public void TotheEnd()
    {
        if (selectedType == "note")
        {
            SetFocusToEndOfInputField(inputNote);
        }
        else if (selectedType == "folder")
        {
            SetFocusToEndOfInputField(inputNoteFolder);
        }
        else if (selectedType == "recycle")
        {
            
        }
    }

    private void SetFocusToEndOfInputField(InputField inputField)
    {       
        // Set the caret position to the end of the text
        inputField.caretPosition = inputField.text.Length;

        // Move the text selection cursor to the end of the text
        inputField.selectionAnchorPosition = inputField.text.Length;
        inputField.selectionFocusPosition = inputField.text.Length;

        // Activate the input field to ensure focus
        inputField.ActivateInputField();
    }

    // long press
    IEnumerator LongPressCoroutine()
    {
        yield return new WaitForSeconds(1.2f);

        if (isPressed)
        {        
            isLongPressed = true;            
            selectedType = EventSystem.current.currentSelectedGameObject.tag;            

            if(string.Equals(selectedType, "note"))
            {
                longpressUI.SetActive(true);
                selectedText = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
                listNum = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
            }
            else if(string.Equals(selectedType, "folder"))
            {
                longpressFolderUI.SetActive(true);
                listNumFolder = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
                selectedText = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            }
            else if(string.Equals(selectedType, "recycle"))
            {
                longpressBinUI.SetActive(true);
                listNumRecycle = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(" ")[1]);
                selectedText = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text;
            }
        }
    }
}
