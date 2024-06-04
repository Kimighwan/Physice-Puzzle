using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("------------- [ Core ]")]
    private int maxLevel = 4; // ���� �ִ�� ��ȯ�Ǵ� ����, �ε����� 4��, �����δ� 5��°
    public int scroe;
    public bool isOver;

    [Header("------------- [ Object Pooling ]")]
    [SerializeField]
    private GameObject donglePrefab;
    [SerializeField]
    private Transform dongleGroup;
    [SerializeField]
    private List<Dongle> donglePool;
    [SerializeField]
    private GameObject effectPrefab;
    [SerializeField]
    private Transform effectGroup;
    [SerializeField]
    private List<ParticleSystem> effectPool;
    [SerializeField]
    [Range(1, 30)]
    private int poolSize;
    private int poolCursor;
    private Dongle lastDongle;

    [Header("------------- [ Audio ]")]
    [SerializeField]
    private AudioSource bgmPlayer;
    [SerializeField]
    public AudioSource[] sfxPlayer;
    [SerializeField]
    private AudioClip[] sfxClip;
    public enum SfxName { Drop, Sum, Button, GameOver, Destroy };
    int sfxCursor;

    [Header("------------- [ UI ]")]
    [SerializeField]
    private GameObject endGroup;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    public Text maxScoreText;
    [SerializeField]
    public Text EndScore;
    [SerializeField]
    private ScoreUI scoreUI;
    [SerializeField]
    private GameObject scorePanel;

    [Header("------------- [ ETC ]")]
    [SerializeField]
    private GameObject line;
    [SerializeField]
    private GameObject bottom;
    void Awake()
    {
        Application.targetFrameRate = 60;

        donglePool = new List<Dongle>();
        effectPool = new List<ParticleSystem>();



        for (int index = 0; index < poolSize; index++)
        {
            MakeDongle();
        }

        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("PlayerPrefs", 0);
        }
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
        PlayerPrefs.SetInt("CurScore", 0);

        GameStart();
    }

    public void GameStart()
    {
        //End Group ��Ȱ��ȭ
        endGroup.SetActive(false);

        // ������Ʈ Ȱ��ȭ
        scoreText.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);

        // ���� �÷���
        bgmPlayer.Play();

        // ���� ���� - ���� ����
        Invoke("NextDongle", 0.8f);
    }

    void NextDongle()
    {
        if (isOver)
            return;

        lastDongle = GetDongle();
        lastDongle.level = Random.Range(0, 4);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine("WaitNext");
    }

    Dongle MakeDongle()
    {
        // ����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        //���� ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup);
        instantDongleObj.name = "Dongle " + donglePool.Count;
        Dongle instantDongle = instantDongleObj.GetComponent<Dongle>();
        instantDongle.manager = this;
        instantDongle.effect = instantEffect;
        donglePool.Add(instantDongle);

        return instantDongle;
    }

    Dongle GetDongle()
    {
        for(int index = 0; index < donglePool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf)
            {
                return donglePool[poolCursor];
            }
        }

        return MakeDongle();
    }

    IEnumerator WaitNext() // ������ ����� �� ���� ��ٸ��� �Լ�
    {
        while (lastDongle != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.8f);
        NextDongle();
    }


    public void TouchDown()
    {
        if (lastDongle == null)
            return;

        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null)
            return;

        lastDongle.Drop();
        lastDongle = null;
    }

    public void GameOver()
    {
        if (isOver)
        {
            return;
        }

        isOver = true;
        EndScore.text = scoreText.text;

        StartCoroutine("GameOverRoutin");
    }

    IEnumerator GameOverRoutin()
    {
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Dongle[] dongles = FindObjectsOfType<Dongle>();

        // 2. ����� ���� ��� ������ ����ȿ�� ��Ȱ��ȭ
        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].rigid.simulated = false;
        }

        // 3. 1���� �Ӹ���� �ϳ��� �����ؼ� �����
        for (int index = 0; index < dongles.Length; index++)
        {
            dongles[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        bgmPlayer.Stop();
        SfxPlay(SfxName.GameOver);

        // �ְ� ���� ����
        int maxScore = Mathf.Max(scroe, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        // ���ӿ��� UI
        endGroup.SetActive(true);
        scorePanel.SetActive(false);
        line.SetActive(false);


    }

    public void Reset()
    {
        SfxPlay(SfxName.Button); 
        SceneManager.LoadScene(0);
    }

    public void SfxPlay(SfxName type)
    {
        switch (type)  //  type = Drop, Sum, Button, GameOver, Destroy
        {
            case SfxName.Drop:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case SfxName.Sum:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case SfxName.Destroy:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
            case SfxName.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case SfxName.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        scoreText.text = scroe.ToString();
        scoreUI.PlayAnim(scroe);
    }
}
