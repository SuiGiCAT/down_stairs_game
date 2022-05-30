using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sora : MonoBehaviour
{
    [SerializeField] float lf_moveSpeed = 5f;
    [SerializeField] float up_moveSpeed = 10f;
    GameObject currentBrick;
    [SerializeField] GameObject Hpbar;
    [SerializeField] int hp;
    int score;
    [SerializeField]Text scoreText;
    Animator anim;
    AudioSource ads;
    // Start is called before the first frame update 
    float scoreTime;
    bool pauseFlag;
    bool brave;
    float braveTime;
    int jumpTime;
    float jumpHight;
    static float JUMP_HIGHT = 1.8f;
    [SerializeField]GameObject replayButton;
    [SerializeField]GameObject pauseButton;

    [SerializeField]GameObject braveText;

    void Start()
    {
        hp = 8;
        score = 0;
        scoreTime = 0f;
        jumpTime = 0;
        anim = GetComponent<Animator>();
        ads = GetComponent<AudioSource>();
        ads.Play();
        pauseFlag = false;
        jumpHight = 0;
        brave = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            anim.SetBool("move",true);
            // 指定物体在三个轴上移动的量
            transform.Translate(lf_moveSpeed * Time.deltaTime, 0, 0);
            //慢： 0.01f * Time.deltaTime = 0.01f * 1 * 1 （机子循环一次需要1s
            //快： 0.01f * Time.deltaTime = 0.01f * 0.5 * 2 （机子循环一次需要0.5s，1s循环2次
            GetComponent<SpriteRenderer>().flipX = true;
            //解决每台电脑运作速度不同的问题
            if(Input.GetKey(KeyCode.Space))
            {
                if (jumpHight< JUMP_HIGHT && jumpTime<=2) {
                    transform.Translate(0, up_moveSpeed * Time.deltaTime, 0);
                    jumpHight += up_moveSpeed * Time.deltaTime;
                }
            }
        }else if(Input.GetKey(KeyCode.LeftArrow))
        {
            anim.SetBool("move",true);
            GetComponent<SpriteRenderer>().flipX = false;
            transform.Translate(-lf_moveSpeed * Time.deltaTime, 0, 0);
            if(Input.GetKey(KeyCode.Space))
            {
                if (jumpHight< JUMP_HIGHT && jumpTime<=2) {
                    transform.Translate(0, up_moveSpeed * Time.deltaTime, 0);
                    jumpHight += up_moveSpeed * Time.deltaTime;
                }
            }
        }
        else if(Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("move",true);
            if (jumpHight< JUMP_HIGHT && jumpTime<=2) {
                transform.Translate(0, up_moveSpeed * Time.deltaTime, 0);
                jumpHight += up_moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(lf_moveSpeed * Time.deltaTime, 0, 0);
            }
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(-lf_moveSpeed * Time.deltaTime, 0 ,0);
            }
        }else if(Input.GetKeyUp(KeyCode.P)){  
            if (pauseFlag == false){
                    pauseFlag = true;
                    Pause();
            }else{
                    pauseFlag = false;
                    reStart();
            }
        }else{
            anim.SetBool("move",false);
        }
        if(Input.GetKeyUp(KeyCode.Space)){
            jumpTime += 1;  
            jumpHight = 0f;
        }      
        UpdateScore();
        if (brave) braveTimecord();
    }
    //有无碰撞物体？ 非触发器
    private void OnCollisionEnter2D(Collision2D other) 
    {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "brick_hp")
        {
            //借由法向量来判断碰撞到的边是哪一个边,只有在砖块上面才被判定为可以跳跃
            if(other.contacts[0].normal == new Vector2(0f, 1f))
            {
                //Debug.Log("撞到砖块上面了！HP+1");
                jumpHight = 0f;
                jumpTime = 0;
                ModifyHp(1);
                currentBrick = other.gameObject;
                other.gameObject.GetComponent<AudioSource>().Play();
            }
            Debug.Log(other.contacts[0].normal);
            Debug.Log(other.contacts[1].normal);
        }else if(other.gameObject.tag == "brick_normal"){
            //借由法向量来判断碰撞到的边是哪一个边,只有在砖块上面才被判定为可以跳跃
            if(other.contacts[0].normal == new Vector2(0f, 1f))
            {
                jumpHight = 0f;
                jumpTime = 0;
                currentBrick = other.gameObject;
                other.gameObject.GetComponent<AudioSource>().Play();
            }
            Debug.Log(other.contacts[0].normal);
            Debug.Log(other.contacts[1].normal);
        }else if(other.gameObject.tag == "brick_brave"){
            if(other.contacts[0].normal == new Vector2(0f, 1f))
            {
                jumpHight = 0f;
                jumpTime = 0;
                currentBrick = other.gameObject;
                other.gameObject.GetComponent<AudioSource>().Play();
                if (brave) braveTime = 0;
                braveText.SetActive(true);
                brave = true;
                anim.SetTrigger("brave");
            }
            Debug.Log(other.contacts[0].normal);
            Debug.Log(other.contacts[1].normal);
        }
        else if(other.gameObject.tag == "cier")
        {
            if(other.contacts[0].normal == new Vector2(0f, 1f))
            {
                jumpHight = 0f;
                jumpTime = 0;
                currentBrick = other.gameObject; 
                if (brave == false){
                    //Debug.Log("撞到刺儿了！HP-2");   
                    ModifyHp(-2);
                    anim.SetTrigger("hurt");
                }
                other.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if(other.gameObject.tag == "bottom")
        {
            //到底了，游戏结束
            other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Die();
        }
    }
    //有无经过物体？
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "top_cier")
        {
            //撞到顶部刺儿！
            if (currentBrick != null) 
                currentBrick.GetComponent<BoxCollider2D>().enabled = false;
            if (brave == false){
                anim.SetTrigger("hurt");
                ModifyHp(-2);   
            }
        }
    }
    void ModifyHp(int num){
        hp += num;
        if(hp >= 8) hp = 8; 
        else if(hp <= 0) {
            hp = 0; 
            Die();
        } 
        UpdateHpBar();      
    }

    void UpdateHpBar() {
        for(int i = 0; i < Hpbar.transform.childCount; i++){
            if (i < hp){
                //比如血量为3，那么编号0，1，2的子物件都将显示出来
                Hpbar.transform.GetChild(i).gameObject.SetActive(true);
            }
            else{
                Hpbar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void UpdateScore(){
        scoreTime += Time.deltaTime;
        if (scoreTime > 2f){
            score++;
            scoreTime = 0f;//时间归零
            scoreText.text = "スコア:" + score.ToString();
        }

    }
    void Die(){
        ads.Pause();
        Time.timeScale = 0f;
        replayButton.SetActive(true);
    }

    public void Replay(){
        // 时间1倍
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
    public void Pause(){
        ads.Pause();
        anim.SetBool("move",false);
        Time.timeScale = 0f;
        pauseButton.SetActive(true);
    }
    public void reStart(){
        ads.Play();
        Time.timeScale = 1f;
        pauseButton.SetActive(false);
    }
    public void braveTimecord(){
        braveTime += Time.deltaTime;
        if (braveTime > 5f)
            {
                brave = false;
                braveText.SetActive(false);
                braveTime = 0;
            }
    }
}
