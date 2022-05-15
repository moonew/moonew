using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    
    public AudioClip getMessageSound;
    public AudioClip sendMessageSound;
    public AudioClip[] typingSound;




    public void ShowHeroineMessage(string line)
    {
        string charName             = line.Split(':')[0];
        string sentence             = line.Split(':')[1];
        Transform chatBoxTransform  = GameObject.Find("Chat_Boxes").transform;

        //message box Prefab
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Heroine"), chatBoxTransform);

        //message box element.
        Transform messageBox            = obj.transform.GetChild(0);
        Text messageText                = messageBox.GetChild(0).GetComponent<Text>();
        Image profilePhoto              = obj.transform.GetChild(1).GetComponent<Image>();
        Text charNameText               = obj.transform.GetChild(2).GetComponent<Text>();
        Image emoji                     = obj.transform.GetChild(4).GetComponent<Image>();
        


        //load Profile Photo and Input charName...
        switch (charName)
        {
            
            case "이슬비":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_sb");
                    charNameText.text = charName;
                }
                break;
            case "강나린":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_nr");
                    charNameText.text = charName;
                }
                break;
            case "안시은":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_se");
                    charNameText.text = charName;
                }
                break;
            default:
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_empty");
                    charNameText.text = charName;
                }
                break;
        }

        
        //input sentence
        if(sentence.Contains("<")&& sentence.Contains(">"))
        {
            string adjustSentence               = sentence.Split('<', '>')[0];
            string fileName                     = sentence.Split('<', '>')[1];
            messageText.text                    = adjustSentence;
            emoji.sprite                        = Resources.Load<Sprite>("Images/Emoji/" + fileName);
            messageBox.transform.localPosition = new Vector3(140f, -240, 0);
            

        }
        else
        {
            messageText.text = sentence;
        }

        






        //Where will the message boxes be placed??

        
        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;

        for (int i = 0; i < chatBoxTransform.transform.childCount - 1; i++)
        {

            try
            {
                //이모티콘을 사용한 경우...
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    nextBoxPosY += space;
                }

            }
            catch
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }
            
        }
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(100, -150 - nextBoxPosY);




        //The size of the panel varies depending on the number of message boxes...
        for (int i = 0; i < chatBoxTransform.childCount; i++)
        {

            try
            {
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    panelHeight += space;
                }
            }
            catch
            {
                panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                panelHeight += space;
            }
        }

        chatBoxTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);
  
        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if (panelHeight + 50f >= maskHeight)
        {
            chatBoxTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }

        obj.GetComponent<CanvasGroup>().alpha = 1;
        
        messageBox.gameObject.SetActive(true);

    }


    public void ShowPlayerMessage(string line)
    {

        string sentence = line.Split(':')[1];
        Transform chatBoxes = GameObject.Find("Chat_Boxes").transform;

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Player"), chatBoxes);
        Transform box = obj.transform.GetChild(0);
        Text messageText = box.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;


        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        box.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);



        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;


        for (int i = 0; i < chatBoxes.transform.childCount - 1; i++)
        {

            try
            {
                if (chatBoxes.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    nextBoxPosY += chatBoxes.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    nextBoxPosY += space;
                }

            }
            catch
            {
                nextBoxPosY += chatBoxes.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }

        }

        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(75, -150 - nextBoxPosY);


        for (int i = 0; i < chatBoxes.childCount; i++)
        {

            try
            {
                if (chatBoxes.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    panelHeight += chatBoxes.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    panelHeight += space;
                }
            }
            catch
            {
                panelHeight += chatBoxes.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                panelHeight += space;
            }
        }

        chatBoxes.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);




        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if (panelHeight + 50 >= maskHeight)
        {
            chatBoxes.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);
        }


        box.gameObject.SetActive(true);
    }



    public IEnumerator RunShowHeroineMessage(string line)
    {
        string charName = line.Split(':')[0];
        string sentence = line.Split(':')[1];
        Transform chatBoxTransform = GameObject.Find("Chat_Boxes").transform;

        //message box Prefab
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Heroine"), chatBoxTransform);

        //message box element.
        Transform messageBoxTransform = obj.transform.GetChild(0);
        Text messageText = messageBoxTransform.GetChild(0).GetComponent<Text>();
        Image profilePhoto = obj.transform.GetChild(1).GetComponent<Image>();
        Text charNameText = obj.transform.GetChild(2).GetComponent<Text>();
        Image emoji = obj.transform.GetChild(4).GetComponent<Image>();
        messageBoxTransform.gameObject.SetActive(true);



        //load Profile Photo and Input charName...
        switch (charName)
        {

            case "이슬비":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_sb");
                    charNameText.text = charName;
                }
                break;
            case "강나린":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_nr");
                    charNameText.text = charName;
                }
                break;
            case "안시은":
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_se");
                    charNameText.text = charName;
                }
                break;
            default:
                {
                    profilePhoto.sprite = Resources.Load<Sprite>("Images/Chats/chat_empty");
                    charNameText.text = charName;
                }
                break;
        }


        //input sentence
        if (sentence.Contains("<") && sentence.Contains(">"))
        {
            string adjustSentence = sentence.Split('<', '>')[0];
            string fileName = sentence.Split('<', '>')[1];
            messageText.text = adjustSentence;
            emoji.sprite = Resources.Load<Sprite>("Images/Emoji/" + fileName);
            emoji.gameObject.SetActive(true);
            messageBoxTransform.transform.localPosition = new Vector3(140f, -240, 0);
        }
        else
        {
            messageText.text = sentence;
        }


        yield return new WaitForEndOfFrame();

        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        messageBoxTransform.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);






        //Where will the message boxes be placed??


        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;

        for (int i = 0; i < chatBoxTransform.transform.childCount - 1; i++)
        {

            try
            {   
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    nextBoxPosY += space;
                }

            }
            catch
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }

        }
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(100, -150 - nextBoxPosY);




        //The size of the panel varies depending on the number of message boxes...
        for (int i = 0; i < chatBoxTransform.childCount; i++)
        {
            try
            {   
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeSelf == true)
                {
                    panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    panelHeight += space;

                    Debug.Log("emo : " + panelHeight);
                }
            }
            catch
            {
                panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                panelHeight += space;

                Debug.Log("basic : " + panelHeight);
            }
        }



        chatBoxTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);

        

        
        
        

        
        


        if (ConfigData.fastMode == false)
        {
            yield return StartCoroutine(RunLoadingHeroineMessage(obj, sentence));
        }
        else
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.volume = ConfigData.effectVolume;
            audioSource.PlayOneShot(getMessageSound);
        }
        
        
        emoji.GetComponent<CanvasGroup>().alpha = 1;
        obj.GetComponent<CanvasGroup>().alpha = 1;

        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;

        if (panelHeight + 50 >= maskHeight)
            chatBoxTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);


    }


    public IEnumerator RunShowPlayerMessage(string line)
    {
        string sentence = line.Split(':')[1];
        Transform chatBoxTransform = GameObject.Find("Chat_Boxes").transform;

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Player"), chatBoxTransform);
        Transform messageBoxTransform = obj.transform.GetChild(0);
        Text messageText = messageBoxTransform.GetChild(0).GetComponent<Text>();
        messageText.text = sentence;

        yield return new WaitForEndOfFrame();

        float boxHeight = messageText.GetComponent<Text>().preferredHeight + 22.5f > 73f ?
            messageText.GetComponent<Text>().preferredHeight + 22.5f : 73f;

        messageBoxTransform.GetComponent<RectTransform>().sizeDelta
            = new Vector2(messageText.GetComponent<Text>().preferredWidth + 42.5f, boxHeight);



        float panelHeight = 0;
        float nextBoxPosY = 0;
        float space = 50f;

        for (int i = 0; i < chatBoxTransform.transform.childCount - 1; i++)
        {

            try
            {
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeSelf == true)
                {
                    nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    nextBoxPosY += space;
                }

            }
            catch
            {
                nextBoxPosY += chatBoxTransform.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                nextBoxPosY += space;
            }

        }

        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(75, -150 - nextBoxPosY);


        for (int i = 0; i < chatBoxTransform.childCount; i++)
        {

            try
            {
                if (chatBoxTransform.GetChild(i).GetChild(4).gameObject.activeInHierarchy == true)
                {
                    panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 245.2f;
                    panelHeight += space;
                }
            }
            catch
            {
                panelHeight += chatBoxTransform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta.y + 45.2f;
                panelHeight += space;
            }
        }

        chatBoxTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(1920f, panelHeight + 150f);


        float maskHeight = GameObject.Find("Chat_Bg").GetComponent<RectTransform>().sizeDelta.y;



        if (ConfigData.fastMode == false)
        {
            yield return StartCoroutine(RunLoadingPlayerMessage(sentence));
        }
        else
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.volume = ConfigData.effectVolume;
            audioSource.PlayOneShot(sendMessageSound);
        }


        messageBoxTransform.gameObject.SetActive(true);
        


        if (panelHeight + 50 >= maskHeight)
            chatBoxTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, panelHeight - maskHeight + 150, 0);

    }





  





    public void ShowChatPanel()
    {
        if (GameObject.Find("ChatPanel") == null)
        {
            GameObject panel = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Panel"), GameObject.Find("Cam").transform);
            panel.name = "ChatPanel";
            panel.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public void HideChatPanel()
    {
        if(GameObject.Find("ChatPanel"))
        {
            Destroy(GameObject.Find("ChatPanel"));
        }
        
        Resources.UnloadUnusedAssets();        
    }

    public IEnumerator RunHideChatPanelImmidately()
    {
        if (GameObject.Find("ChatPanel"))
        {
            GameObject chatPanel = GameObject.Find("ChatPanel");
            chatPanel.SetActive(false);
            Destroy(chatPanel);
        }

        yield return new WaitForSeconds(0.25f);
        Resources.UnloadUnusedAssets();
    }


    


    public IEnumerator RunShowChatPanel()
    {
        if (GameObject.Find("ChatPanel") == null)
        {
            GameObject panel = Instantiate(Resources.Load<GameObject>("Prefabs/Chat_Panel"), GameObject.Find("Cam").transform);
            panel.name = "ChatPanel";
            yield return StartCoroutine(SpriteController.FadeIn(panel, 1.5f));
            yield return new WaitForSeconds(0.75f);
        }
    }


    public IEnumerator RunHideChatPanel()
    {
        GameObject panel = GameObject.Find("ChatPanel");
        yield return StartCoroutine(SpriteController.FadeOut(panel, 1.75f));
        Destroy(panel);
        yield return new WaitForEndOfFrame();
        Resources.UnloadUnusedAssets();
        yield return new WaitForSeconds(0.5f);
    }


    


    
    

    IEnumerator RunLoadingHeroineMessage(GameObject obj, string sentence)
    {
        GameObject loadingAnim = obj.transform.GetChild(3).gameObject;
        AudioSource audioSource = GetComponent<AudioSource>();
        float typingTime = Mathf.Lerp(0.5f, 2f, sentence.Length / 20f);
        float time = 0;

        loadingAnim.SetActive(true);
        

        while (time <= typingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        loadingAnim.SetActive(false);
        audioSource.volume = ConfigData.effectVolume;
        audioSource.PlayOneShot(getMessageSound);
    }


    IEnumerator RunLoadingPlayerMessage(string sentence)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        float typingTime = Mathf.Lerp(0.5f, 2f, sentence.Length / 20f);
        float time = 0;

        audioSource.volume = ConfigData.effectVolume;
        audioSource.PlayOneShot(typingSound[Random.Range(0, typingSound.Length)]);

        while (time <= typingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.PlayOneShot(sendMessageSound);
    }
    


}
