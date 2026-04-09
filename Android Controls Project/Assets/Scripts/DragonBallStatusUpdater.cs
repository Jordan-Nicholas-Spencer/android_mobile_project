using UnityEngine;

public class DragonBallStatusUpdater : MonoBehaviour
{
    [Header("Dragon Ball References")]
    public DragonBallData[] allDragonBalls;

    [Header("Timing")]
    public float updateInterval = 300f;  // 300 = 5 min. Use 30 for testing.

    [Header("References")]
    public GameManager gameManager;      // Drag GameManager in here

    private float timer = 0f;

    private string[][] statusPool = new string[][]
    {
        // 1-Star
        new string[] {
            "On permanent display in the Smithsonian National Museum of Natural History, Washington D.C. Kept in a sealed climate-controlled case on the second floor.",
            "Reported stolen from the Smithsonian. Interpol has issued an international alert. Last known location: D.C. International Airport cargo bay.",
            "Recovered by the FBI and transferred to a classified facility in Fort Meade, Maryland. Exact location undisclosed.",
            "Spotted in an anonymous online auction with a starting bid of $47 million. The listing was suddenly removed."
        },
        // 2-Star
        new string[] {
            "Located 4,200 meters beneath the surface of the Pacific Ocean, resting in the Mariana Trench. Remote submersibles sent to retrieve it have lost contact within 500 meters of the signal.",
            "A joint NOAA and JAXA research team detected a low-frequency energy pulse emanating from the trench floor. All dive operations have been suspended pending investigation.",
            "Seismic activity near the Challenger Deep has shifted the ball's position. New signal triangulation places it wedged beneath a hydrothermal vent field. Retrieval classified as impossible with current technology.",
            "The USS research vessel Discoverer reported visual confirmation via deep-sea drone before the feed cut out. The last frame showed a shadow moving across the seafloor larger than the ship itself."
        },
        // 3-Star
        new string[] {
            "Last seen in the possession of a nomadic merchant traveling the Silk Road. Current location believed to be somewhere in the Gobi Desert.",
            "Traded at a bazaar in Kashgar for a flock of 40 goats. New owner unaware of its significance.",
            "Discovered by archaeologists near the ruins of Dunhuang. Currently under analysis at Peking University.",
            "Confiscated by Chinese customs at the Mongolian border. Classified as an unregistered cultural artifact."
        },
        // 4-Star
        new string[] {
            "Encased in lava beneath Mount Fuji, Japan. Volcanologists report unusual crystalline formations forming around the magma chamber.",
            "Seismic activity has pushed the ball closer to the surface. The exclusion zone around Fuji has been expanded.",
            "A team in heat-resistant suits has retrieved it. Currently en route to the University of Tokyo for study.",
            "Lost again during the 2024 Fuji tremor. Believed to have descended back into the magma chamber."
        },
        // 5-Star
        new string[] {
            "Secured in a private vault at the Bank of Switzerland, Geneva. Listed in their records only as 'Item 7: Unclassified Artifact.'",
            "Quietly transferred to a private collector in Monaco. The transaction was conducted entirely in cryptocurrency.",
            "Reported missing from the Monaco estate after a break-in. No suspects have been identified.",
            "Recovered by Interpol in Vienna. Currently held as evidence in an ongoing international investigation."
        },
        // 6-Star
        new string[] {
            "Re-entry detected over the South Atlantic at 03:42 UTC. The object survived atmospheric entry intact and impacted the Ross Ice Shelf, Antarctica. A 12-meter crater has been identified via satellite imagery.",
            "A British Antarctic Survey team reached the impact site after a 4-day traverse. The ball was found partially melted into the ice shelf at a depth of 3 meters. Extraction equipment is being flown in.",
            "Extraction was abandoned after the drill assembly failed at minus 40 degrees Celsius. The ball remains embedded in the Ross Ice Shelf. A permanent monitoring station is being established at the site.",
            "Unusually warm conditions have caused partial melting of the surrounding ice. The ball has shifted deeper. Scientists report faint luminescence visible through 2 meters of ice at night."
        },
        // 7-Star
        new string[] {
            "Deep in the Amazon rainforest, Brazil. An indigenous tribe has protected it for generations, believing it to be a gift from the sky.",
            "Removed from tribal lands by an NGO claiming to act 'for its protection.' International protests have erupted.",
            "Returned to the tribe following UN intervention and international pressure. A new protective shrine has been built.",
            "The shrine's location is now kept secret after an attempted theft. Satellite imagery of the surrounding area has been mysteriously corrupted."
        }
    };

    private int[] statusIndex;

    void Start()
    {
        statusIndex = new int[allDragonBalls.Length];

        for (int i = 0; i < allDragonBalls.Length; i++)
        {
            if (allDragonBalls[i] == null) continue;
            int s = allDragonBalls[i].starCount - 1;
            if (s >= 0 && s < statusPool.Length)
                allDragonBalls[i].UpdateStatus(statusPool[s][0]);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            MoveSomeBalls();
        }
    }

    void MoveSomeBalls()
    {
        int count = Random.Range(2, 4);
        int[] shuffled = Shuffle(allDragonBalls.Length);

        for (int i = 0; i < count; i++)
        {
            int idx = shuffled[i];
            if (allDragonBalls[idx] == null) continue;

            int s = allDragonBalls[idx].starCount - 1;
            if (s < 0 || s >= statusPool.Length) continue;

            // Advance to next status
            statusIndex[idx] = (statusIndex[idx] + 1) % statusPool[s].Length;
            allDragonBalls[idx].UpdateStatus(statusPool[s][statusIndex[idx]]);

            // Move to new random position
            Vector2 newPos = Random.insideUnitCircle * 7f;
            allDragonBalls[idx].transform.position = new Vector3(newPos.x, newPos.y, 0f);

            // Tell GameManager this ball's location is no longer confirmed —
            // remove it from the found set so the player must re-investigate
            if (gameManager != null)
                gameManager.DragonBallLost(allDragonBalls[idx].gameObject.GetInstanceID());
        }
    }

    int[] Shuffle(int length)
    {
        int[] arr = new int[length];
        for (int i = 0; i < length; i++) arr[i] = i;
        for (int i = length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = arr[i]; arr[i] = arr[j]; arr[j] = tmp;
        }
        return arr;
    }
}