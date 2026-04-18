// LudumDare59 : Signal

// (A) laser ray comes from starting point, player needs to make it reach the end point, by placing specific pieces on the board (mirror, splitter, inverter...)
// (B) beam comes towards player base, need to use negative color of the beam to cancel it (have limited palette)
// (C) decipher game: unknown signal, need to decode it using something (maybe adding different pieces to process it and it changes output..) for example: easy puzzle, change CASE.
// (D) need to shoot signal towards target, but planets on the way make the path curved, so need to try to hit correct angle?
// (E) you are the signal: need to move or give direction, limited moves, obstacles?
// (F) puzzle game: grid with pieces, one empty slot, sliding puzzle, move pieces to make the signal reach the end point, but pieces have different properties (mirror, splitter, inverter...)
// (G) joke game: "SIGN AL", need to write "AL" in the paper (no instructions)

using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using UnityEngine;

public class App : MonoBehaviour
{
    [Header("Laser")]
    public LineRenderer lineRenderer;
    public Transform hitFx;
    public AudioSource laserAudioSource;
    [ColorUsage(showAlpha: true, hdr: true)]
    public Color goalReachedColor;

    [Header("Game")]
    public Level[] levels;
    public UIFader uiFader;
    public GameObject startScreen;
    public GameObject winScreen;
    int currentLevelIndex = 0;

    [Header("Interaction")]
    public LayerMask obstacleMask;
    public LayerMask goalMask;
    public LayerMask clickableMask;
    public LayerMask hasObjectsMask;
    public LayerMask mirrorMask;
    public Transform mouseIndicator;
    bool canClick = true;
    Camera cam;

    [Header("Audio")]
    public AudioClip moveAudio;
    public AudioClip cannotMoveAudio;
    public AudioClip levelStartAudio;
    public AudioClip goalReachedAudio;
    public AudioClip rotateAudio;

    AudioSource aus;

    bool gameCompleted = false;

    IEnumerator Start()
    {
        aus = GetComponent<AudioSource>();
        cam = Camera.main;
        yield return 0;
        yield return 0;
        yield return 0;
        ActiveLevel(currentLevelIndex);
    }

    void Update()
    {
        if (canClick == false || gameCompleted == true) return;

        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        var hit = Physics2D.OverlapCircle(mousePos, 0.1f, clickableMask);

        if (hit) mouseIndicator.position = hit.transform.position;
        else mouseIndicator.position = Vector3.one * 1000; // hide)

        if (Input.GetMouseButtonDown(0))
        {
            if (hit)
            {
                // check which side has empty space, and move piece there
                var hitUp = Physics2D.OverlapCircle(hit.transform.position + Vector3.up, 0.1f, hasObjectsMask);
                var hitDown = Physics2D.OverlapCircle(hit.transform.position + Vector3.down, 0.1f, hasObjectsMask);
                var hitLeft = Physics2D.OverlapCircle(hit.transform.position + Vector3.left, 0.1f, hasObjectsMask);
                var hitRight = Physics2D.OverlapCircle(hit.transform.position + Vector3.right, 0.1f, hasObjectsMask);

                if (!hitUp) hit.transform.position += Vector3.up;
                else if (!hitDown) hit.transform.position += Vector3.down;
                else if (!hitLeft) hit.transform.position += Vector3.left;
                else if (!hitRight) hit.transform.position += Vector3.right;

                if (!hitUp || !hitDown || !hitLeft || !hitRight) aus.PlayOneShot(moveAudio);
                else aus.PlayOneShot(cannotMoveAudio);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (hit)
            {
                var mirror = hit.GetComponent<Mirror>();
                if (mirror)
                {
                    mirror.Rotate();
                    aus.PlayOneShot(rotateAudio);
                }
            }
        }
    }

    void ActiveLevel(int level)
    {
        uiFader.FadeFromBlack();
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].gameObject.SetActive(i == level);
            if (i == level)
            {
                levels[i].Init();
            }
        }
        aus.PlayOneShot(levelStartAudio);
        StartCoroutine(LaserLoop());
    }

    IEnumerator LaserLoop()
    {
        float maxLaserLength = (Vector3.Distance(levels[currentLevelIndex].startPoint.position, levels[currentLevelIndex].endPoint.position)) * 0.77f;

        while (true && gameCompleted == false)
        {
            List<Vector3> currentDir = new List<Vector3>();
            currentDir.Add(Vector3.right);
            var startPos = levels[currentLevelIndex].startPoint.position;
            startPos.x += 0.55f; // only works in horizontal right
            Vector3 currentPos = startPos;

            lineRenderer.positionCount = 1;

            lineRenderer.SetPosition(0, startPos);

            //Debug.Log("Start laser loop");

            // until hit something
            bool moving = true;
            while (moving)
            {
                //Debug.DrawLine(startPos, currentPos, Color.green,0.5f);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);

                var hit = Physics2D.OverlapCircle(currentPos, 0.1f, mirrorMask);
                if (hit)
                {
                    Debug.Log("Hit mirror " + hit.name);
                    var newDir = hit.GetComponent<Mirror>().currentDirection;
                    currentDir.Add(newDir);

                    currentPos = hit.GetComponent<Mirror>().transform.position;// currentDir[currentDir.Count - 1];
                    //lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);

                    Debug.DrawLine(currentPos, currentPos + newDir, Color.red, 0.5f);

                    // then move out from mirror
                    currentPos += newDir;
                    lineRenderer.positionCount++;
                    //lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPos);

                }
                else
                {
                    // check if hit anything in this pos
                    hit = Physics2D.OverlapCircle(currentPos, 0.1f, obstacleMask);
                    if (hit)
                    {
                        hitFx.position = currentPos;
                        //Debug.Log("Hit " + hit.name);
                        // check goal
                        laserAudioSource.pitch = 1.25f;

                        hit = Physics2D.OverlapCircle(currentPos, 0.1f, goalMask);
                        if (hit)
                        {
                            laserAudioSource.pitch = 1f;
                            //Debug.Log("Goal " + hit.name);

                            //levels[currentLevelIndex].endPoint.GetComponent<SpriteRenderer>().color = goalReachedColor;
                            levels[currentLevelIndex].GoalReached(goalReachedColor);
                            if (canClick)
                            {
                                uiFader.FadeToBlack();
                                StartCoroutine(NextLevelAfterDelay());
                            }
                            canClick = false;


                        }
                        moving = false;
                    }
                    else // continue, empty area
                    {
                        hitFx.position = Vector3.one * 1000; // hide hit fx
                        currentPos += currentDir[currentDir.Count - 1];
                        lineRenderer.positionCount++;

                        laserAudioSource.pitch = 0.9f;
                        laserAudioSource.volume = Mathf.Lerp(0, 1f, Remap(lineRenderer.positionCount, 0, maxLaserLength, 0, 1));
                    }
                }

               // yield return 0;
                //yield return new WaitForSeconds(0.5f);
            }
            yield return 0;
        }
    } // IEnumerator LaserLoop

    IEnumerator NextLevelAfterDelay()
    {
        float delay = 1;
        aus.PlayOneShot(goalReachedAudio);
        yield return new WaitForSeconds(delay);
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("Game completed!");
            gameCompleted = true;
            winScreen.SetActive(true);
        }
        else
        {
            ActiveLevel(currentLevelIndex);
            uiFader.FadeFromBlack();
            canClick = true;
        }
    }

    float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
    {
        return targetFrom + (source - sourceFrom) * (targetTo - targetFrom) / (sourceTo - sourceFrom);
    }

} // class
