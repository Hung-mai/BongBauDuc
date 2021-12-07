using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BongBauDuc_Player : MonoBehaviour
{
    public static BongBauDuc_Player ins;

    public SkinnedMeshRenderer dau;
    public SkinnedMeshRenderer than;
    private bool isJustRevive;
    public GameObject collisionEff;
    [HideInInspector] public Animator playerAnimator;
    public float moveSpeed;
    public BongBauDuc_Joystick joystick;
    private Vector3 huongQuayMat;
    private Transform skin;

    private float a = 1.0f;
    private int changeC = -1;


    private void Awake() {
        ins = this;
    }

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        skin = transform.GetChild(0);
    }

    private void Update() {
        if(isJustRevive)
        {
            if(a >= 0.8f)
            {
                changeC = -1;
            }

            if(a <= 0.1f)
            {
                changeC = 1;
            }

            a += 3 * Time.deltaTime * changeC;
            than.material.color = new Color(1, 1, 1, a);
            dau.material.color = new Color(1, 1, 1, a);
        }
    }

    private void FixedUpdate() 
	{
        if(BongBauDuc_GameManager.ins.playerAlive == false) return;
        if(BongBauDuc_GameManager.ins.isEndGame == true) return;

        Vector3 moveVector = (transform.right * joystick.Horizontal + transform.forward * joystick.Vertical).normalized;

        if(moveVector != Vector3.zero)
        {

            BongBauDuc_GameManager.ins.StartGame();

            transform.Translate(moveVector * moveSpeed * Time.deltaTime);
            // transform.position = Vector3.Lerp(transform.position, transform.position + moveVector, moveSpeed * Time.deltaTime);
            // chuyển anim chạy
            playerAnimator.SetBool("isRun", true);
            huongQuayMat = moveVector;
        }
        else
        {
            playerAnimator.SetBool("isRun", false);
        }

        skin.rotation = Quaternion.LookRotation(huongQuayMat);
	}

    public void PlayerWin()
    {
        playerAnimator.SetBool("isWin", true);
    }

    public IEnumerator PlayerDie()
    {
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        transform.GetComponent<Rigidbody>().AddForce(200 * Vector3.up, ForceMode.Force);
        Destroy(playerAnimator);
        BongBauDuc_GameManager.ins.playerAlive = false;
        yield return new WaitForSeconds(1);
        BongBauDuc_GameManager.ins.EndGamePlayerDie();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("BBD_fish"))
        {
            StartCoroutine(PlayerDie());
            var ce = Instantiate(collisionEff, other.contacts[0].point, Quaternion.identity);
            Destroy(ce, 0.2f);
        }
    }

    public IEnumerator JustRevive()
    {
        MoveToLayer(transform, 10);

        than.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        than.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        than.material.SetInt("_ZWrite", 0);
        than.material.DisableKeyword("_ALPHATEST_ON");
        than.material.DisableKeyword("_ALPHABLEND_ON");
        than.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        than.material.renderQueue = 3000;

        isJustRevive = true;
        
        yield return new WaitForSeconds(3);
        MoveToLayer(transform, 8);
        
        than.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        than.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        than.material.SetInt("_ZWrite", 1);
        than.material.DisableKeyword("_ALPHATEST_ON");
        than.material.DisableKeyword("_ALPHABLEND_ON");
        than.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        than.material.renderQueue = -1;

        isJustRevive = false;

        than.material.color = new Color(1, 1, 1, 1);
        dau.material.color = new Color(1, 1, 1, 1);
    }

    void MoveToLayer(Transform root, int layer) {
        root.gameObject.layer = layer;
        foreach(Transform child in root)
            MoveToLayer(child, layer);
    }
}
