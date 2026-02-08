
using System.Collections;
using UnityEngine;

public class Hamster : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite hamster;
    [SerializeField] private Sprite hamsterSleep;
    [SerializeField] private Sprite hamsterTwoLives;
    [SerializeField] private Sprite hamsterTwoLivesHit;
    [SerializeField] private Sprite hamsterTwoLivesSleep;
    [SerializeField] private Sprite hamsterBomb;

    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;

    private Vector2 startPosition = new Vector2(0f, -0.7f);
    private Vector2 endPosition = new Vector2(0f, 0.45f);
    private float showDuration = 0.5f;
    private float duration = 1f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    private bool hittable = true;
    public enum HamsterType { Standard, HardHat, Bomb };
    private HamsterType hamsterType;
    private float hardRate = 0.25f;
    private float bombRate = 0f;
    private int lives;
    private int hamsterIndex = 0;

    private IEnumerator ShowHide(Vector2 start, Vector2 end)
    {
        transform.localPosition = start;

        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = end;
        boxCollider2D.offset = boxOffset;
        boxCollider2D.size = boxSize;

        yield return new WaitForSeconds(duration);

        elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = start;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;

        if (hittable)
        {
            hittable = false;
            gameManager.Missed(hamsterIndex, hamsterType != HamsterType.Bomb);
        }
    }

    public void Hide()
    {
        transform.localPosition = startPosition;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }

    private IEnumerator QuickHide()
    {
        yield return new WaitForSeconds(0.25f);
        if (!hittable)
        {
            Hide();
        }
    }

    private void OnMouseDown()
    {
        if (hittable)
        {
            switch (hamsterType)
            {
                case HamsterType.Standard:
                    spriteRenderer.sprite = hamsterSleep;
                    gameManager.AddScore(hamsterIndex);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    hittable = false;
                    break;
                case HamsterType.HardHat:
                    if (lives == 2)
                    {
                        spriteRenderer.sprite = hamsterTwoLivesHit;
                        lives--;
                    }
                    else
                    {
                        spriteRenderer.sprite = hamsterTwoLivesSleep;
                        gameManager.AddScore(hamsterIndex);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        hittable = false;
                    }
                    break;
                case HamsterType.Bomb:
                    gameManager.GameOver(1);
                    break;
                default:
                    break;
            }
        }
    }

    private void CreateNext()
    {
        float random = Random.Range(0f, 1f);
        if (random < bombRate)
        {
            hamsterType = HamsterType.Bomb;
            spriteRenderer.sprite = hamsterBomb;
        }
        else
        {
            random = Random.Range(0f, 1f);
            if (random < hardRate)
            {
                hamsterType = HamsterType.HardHat;
                spriteRenderer.sprite = hamsterTwoLives;
                lives = 2;
            }
            else
            {
                hamsterType = HamsterType.Standard;
                spriteRenderer.sprite = hamster;
                lives = 1;
            }
        }
        hittable = true;
    }

    private void SetLevel(int level)
    {

        bombRate = Mathf.Min(level * 0.025f, 0.25f);

        hardRate = Mathf.Min(level * 0.025f, 1f);

        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }
    public void SetIndex(int index)
    {
        hamsterIndex = index;
    }
    public void StopGame()
    {
        hittable = false;
        StopAllCoroutines();
    }
}
