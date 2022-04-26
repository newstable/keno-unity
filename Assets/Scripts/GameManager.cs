using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using SimpleJSON;



public class GameManager : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void GameController(string msg);

    public Transform prefab;
    public Text[] BallText = new Text[20];
    public Sprite WinTexture;
    public Sprite CLickTexture;
    public Sprite NormalTexture;
    public Text[] PointGroup = new Text[10];
    public Text Cost;
    public Text Balance;
    public Text Win;
    public Image EffectImage;
    public static APIForm apiform;
    public static Globalinitial _global;
    public InputField myInput;
    public GameObject alert;
    public Sprite a_Bet;
    public Sprite a_Win;
    public Sprite a_Response;
    public Sprite a_Lose;
    public Sprite a_Server;

    bool startSet = true;
    bool[] ClickArray = new bool[80];
    float BalanceNumber;
    float BetAmount =100;
    int[] BallRandomNum = new int[20];
    int OneSecond;
    int ClickNumber;
    string TextValue = "";
    string Token ="";

    private Transform NumberArray;
    private List<int> RandomChoose = new List<int>(0);

    void Start()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
Debug.Log("come here");
            GameController("Ready");
#endif
    }


    void Update()
    {

    }

    public void PlusAmount()
    {
        if (startSet)
        {
            if(BetAmount+100f> Single.Parse(Balance.GetComponent<Text>().text))
            {
                myInput.GetComponent<InputField>().text = Balance.GetComponent<Text>().text;
            }
            else
            {
                BetAmount = BetAmount + 100f;
                myInput.GetComponent<InputField>().text = BetAmount.ToString();
            }
        }
    }

    public void MinuseAmount()
    {
        if (startSet)
        {
            if (float.Parse(Cost.GetComponent<Text>().text)-100f > 0)
            {
                BetAmount = BetAmount - 100f;
                myInput.GetComponent<InputField>().text = BetAmount.ToString();
            }
        }
    }

    public void BetChange()
    {
        BetAmount = float.Parse(Cost.GetComponent<Text>().text);
        if (BetAmount < 100)
        {
            BetAmount = 100f;
            myInput.GetComponent<InputField>().text = (BetAmount).ToString();
        }
        else if(BetAmount> Single.Parse(Balance.GetComponent<Text>().text))
        {
            myInput.GetComponent<InputField>().text = Balance.GetComponent<Text>().text;
        }
    }
    public void GameStart()
    {
        BalanceNumber = float.Parse(Balance.GetComponent<Text>().text);
        if (startSet && ClickNumber > 1 && BalanceNumber - BetAmount >= 0 && BetAmount>0)
        {
            GameObject.FindGameObjectWithTag("Start").GetComponent<Animator>().enabled = false;
            GameObject.FindGameObjectWithTag("Start").GetComponent<Button>().interactable = false;
            startSet = false;
            TextValue = String.Join(",", RandomChoose.ToArray());
            StartCoroutine(Server());
        }
    }

    private void BalanceManager(float balance)
    {
        Balance.GetComponent<Text>().text =(BalanceNumber + balance).ToString();
    }
    public void QuickPick()
    {
        if (startSet)
        {
            for (int i = 0; i < 80; i++)
            {
                GameObject.FindGameObjectsWithTag("FromTo")[i].GetComponent<Image>().sprite = NormalTexture;
            }
            ClickNumber = 0;
            CleanChoose();
            RandomChoose.Clear();
            for (int i = 0; i < 10; i++)
            {
                RandomBallNumber(RandomChoose);
                Debug.Log(RandomChoose[i]);
                ClickNumber++;
            }
            Times();
            for (int i = 0; i < 10; i++)
            {
                GameObject.FindGameObjectsWithTag("FromTo")[RandomChoose[i] - 1].GetComponent<Image>().sprite = CLickTexture;
                ClickArray[RandomChoose[i] - 1] = true;
            }
        }
    }
    private void CleanClickArray()
    {
        for (int i = 0; i < 80; i++)
        {
            ClickArray[i] = false;
        }
    }
    private void RandomBallNumber(List<int> Myarrays)
    {
        int random = 0;
        random = Random.Range(1, 80);
        if (Myarrays.IndexOf(random) == -1)
        {
            Myarrays.Add(random);
        }
        else
        {
            RandomBallNumber(Myarrays);
        }
    }

    private void numberDeploy()
    {

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                NumberArray = Instantiate(prefab,Vector2.zero, Quaternion.identity);
                NumberArray.transform.SetParent(GameObject.FindGameObjectWithTag("CountTag").transform);
                NumberArray.name = "MyButtons";
                NumberArray.GetComponent<RectTransform>().anchorMax = new Vector2(0.03f + (j+1) * 0.095f,0.97f-0.11f *i);
                NumberArray.GetComponent<RectTransform>().anchorMin = new Vector2(0.03f+j * 0.095f,0.97f-(0.02f+0.11f*(i+1)));
                NumberArray.GetComponent<RectTransform>().offsetMax = new Vector2(-28,-28);
                NumberArray.GetComponent<RectTransform>().offsetMin = new Vector2(20,0);
                NumberArray.localScale = new Vector2(1f, 1f);
                NumberArray.GetComponent<NumberButton>().setId(i * 10 + j);
                GameObject.FindGameObjectsWithTag("Problems")[i * 10 + j].GetComponent<Text>().text = (i * 10 + j + 1).ToString();
            }
        }
    }

    public void handleClickNumber(int number)
    {
        if (startSet)
        {
            if (ClickArray[number])
            {
                GameObject.FindGameObjectsWithTag("FromTo")[number].GetComponent<Image>().sprite = NormalTexture;
                ClickNumber--;
                ClickArray[number] = false;
                RandomChoose.Remove(number + 1);
            }
            else
            {
                if (ClickNumber < 10)
                {
                    Debug.Log(number);
                    Debug.Log(GameObject.FindGameObjectsWithTag("FromTo")[number]);
                    GameObject.FindGameObjectsWithTag("FromTo")[number].GetComponent<Image>().sprite = CLickTexture;
                    RandomChoose.Add(number + 1);
                    ClickNumber++;
                    ClickArray[number] = true;
                }
            }
            Times();
        }
    }

    private void CleanChoose()
    {
        // for(int i =0;i<RandomChoose)
        for (int i = 0; i < 80; i++)
        {
            GameObject.FindGameObjectsWithTag("FromTo")[i].GetComponent<Image>().sprite = NormalTexture;
        }
        CleanClickArray();
    }

    private void Clear()
    {
        if (startSet)
        {
            ClickNumber = 0;
            CleanChoose();
            RandomChoose.Clear();
            Times();
        }
    }

    private void Times()
    {
        switch (ClickNumber)
        {
            case 2:
                GetTextEventHandler();
                PointGroup[0].text = "1";
                PointGroup[1].text = "9";
                break;
            case 3:
                GetTextEventHandler();
                PointGroup[1].text = "2";
                PointGroup[2].text = "47";
                break;
            case 4:
                GetTextEventHandler();
                PointGroup[1].text = "2";
                PointGroup[2].text = "5";
                PointGroup[3].text = "91";
                break;
            case 5:
                GetTextEventHandler();
                PointGroup[2].text = "3";
                PointGroup[3].text = "12";
                PointGroup[4].text = "820";
                break;
            case 6:
                GetTextEventHandler();
                PointGroup[2].text = "3";
                PointGroup[3].text = "4";
                PointGroup[4].text = "70";
                PointGroup[5].text = "1600";
                break;
            case 7:
                GetTextEventHandler();
                PointGroup[2].text = "1";
                PointGroup[3].text = "2";
                PointGroup[4].text = "21";
                PointGroup[5].text = "400";
                PointGroup[6].text = "7000";
                break;
            case 8:
                GetTextEventHandler();
                PointGroup[3].text = "2";
                PointGroup[4].text = "12";
                PointGroup[5].text = "100";
                PointGroup[6].text = "1650";
                PointGroup[7].text = "10000";
                break;
            case 9:
                GetTextEventHandler();
                PointGroup[3].text = "1";
                PointGroup[4].text = "6";
                PointGroup[5].text = "44";
                PointGroup[6].text = "335";
                PointGroup[7].text = "4700";
                PointGroup[8].text = "10000";
                break;
            case 10:
                GetTextEventHandler();
                PointGroup[4].text = "5";
                PointGroup[5].text = "24";
                PointGroup[6].text = "142";
                PointGroup[7].text = "1000";
                PointGroup[8].text = "4500";
                PointGroup[9].text = "10000";
                break;
            default:
                GetTextEventHandler();
                break;
        }
    }

    private void GetTextEventHandler()
    {
        for (int i = 0; i < 10; i++)
        {
            PointGroup[i].text = "0";
        }
    }

    private void allclean()
    {
        for (int i = 0; i < 20; i++)
        {
            BallText[i].text = "";
        }
        for (int i = 0; i < RandomChoose.Count(); i++)
        {
            GameObject.FindGameObjectsWithTag("FromTo")[RandomChoose[i] - 1].GetComponent<Image>().sprite = CLickTexture;
        }
    }
    private IEnumerator deploy()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(1);
            BallText[i].text = apiform.BallNumbers[i].ToString();
            if (RandomChoose.IndexOf(apiform.BallNumbers[i]) != -1)
            {
                GameObject.FindGameObjectsWithTag("FromTo")[apiform.BallNumbers[i] - 1].GetComponent<Image>().sprite = WinTexture;
            }
        }
        Balance.GetComponent<Text>().text =(float.Parse(Balance.GetComponent<Text>().text) + apiform.WinMoney).ToString();
        Win.GetComponent<Text>().text = "$" + (apiform.WinMoney).ToString();
        if (apiform.WinMoney > 0)
        {
            alert.GetComponent<Image>().sprite = a_Win;
            alert.SetActive(true);
            yield return new WaitForSeconds(1.65f);
            alert.SetActive(false);
        }
        else
        {
            alert.GetComponent<Image>().sprite = a_Lose;
            alert.SetActive(true);
            yield return new WaitForSeconds(1.65f);
            alert.SetActive(false);
        }
        setStart();
    }
    private IEnumerator Server()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", Token);
        form.AddField("betValue", BetAmount.ToString());
        form.AddField("ChooseNumbers", TextValue);
        form.AddField("ClickNumber", ClickNumber);
        _global = new Globalinitial();
        Debug.Log(_global.BaseUrl);
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl+"api/start-Keno", form);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            alert.GetComponent<Image>().sprite = a_Server;
            alert.SetActive(true);
            yield return new WaitForSeconds(1.65f);
            alert.SetActive(false);
            setStart();
        }
        else
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));
            if (apiform.Message == "Success")
            {
                BalanceManager(-BetAmount);
                allclean();
                StartCoroutine(deploy());
            }else if(apiform.Message == "Bet Error")
            {
                alert.GetComponent<Image>().sprite = a_Bet;
                alert.SetActive(true);
                yield return new WaitForSeconds(1.65f);
                setStart();
                alert.SetActive(false);
            }else if(apiform.Message=="Server Error")
            {
                alert.GetComponent<Image>().sprite = a_Response;
                alert.SetActive(true);
                yield return new WaitForSeconds(1.65f);
                alert.SetActive(false);
                setStart();
            }
        }
    }
    public void RequestToken(string data)
    {
        Debug.Log("success");
        JSONNode usersInfo = JSON.Parse(data);
        Token = usersInfo["token"];
        Balance.GetComponent<Text>().text =usersInfo["amount"];
        CleanClickArray();
        numberDeploy();
        Times();
    }
    private void setStart()
    {
        startSet = true;
        GameObject.FindGameObjectWithTag("Start").GetComponent<Animator>().enabled = true;
        GameObject.FindGameObjectWithTag("Start").GetComponent<Button>().interactable = true;
    }
}