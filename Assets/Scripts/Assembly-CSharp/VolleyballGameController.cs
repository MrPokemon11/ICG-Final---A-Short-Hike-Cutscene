using System.Collections;
using UnityEngine;

public class VolleyballGameController : MonoBehaviour, IInteractableComponent
{
	public static bool EASY_MODE;

	[Header("Links")]
	public GameObject ballPrefab;

	public VolleyballOpponent opponent;

	public NPCIKAnimator referee;

	public RangedInteractable refereeInteractable;

	public AudioClip whistle;

	public Animator flagAnimator;

	[Header("Dialogue")]
	public string gameStartDialogue = "VolleyballGameStart";

	public string gameStartTag = "AllowGameStart";

	public string hitsTag = "BallHits";

	public string gameEndDialogue = "VolleyballGameEnd";

	public string poppedBallDialogue = "PoppedBallEnd";

	[Header("Enemy Pass Settings")]
	public float enemyHitSafeTime = 0.25f;

	public float hitGroundSafeTime = 0.25f;

	public float serveTime = 1f;

	public int simpleHits = 2;

	public int easyHits = 2;

	public int mediumHits = 4;

	public Range enemyPassTimeSimple = new Range(1.3f, 1.5f);

	public Range enemyPassTimeEasy = new Range(1f, 1.4f);

	public Range enemyPassTimeMedium = new Range(0.8f, 1.2f);

	public Range enemyPassTimeHard = new Range(0.65f, 1.1f);

	public BoxCollider playerAimRegionEasy;

	public BoxCollider playerAimRegionMedium;

	public BoxCollider playerAimRegionHard;

	[Header("Player Pass Settings")]
	public Range playerPassTime = new Range(1.25f, 1.75f);

	public float spikeMultiplier = 0.4f;

	public float enemyBackUpUnits = 2f;

	public BoxCollider enemyAimRegion;

	[Header("Auto Aim Settings")]
	public float autoAimRadiusCamX = 7f;

	public float autoAimRadiusCamZ = 10.5f;

	public float autoAimBallHeight = 5f;

	public float armAnimationTime = 0.21f;

	private Timer hitGroundTimer;

	private float enemyHitTime;

	private WhackingActions whackingActions;

	private Volleyball orphanedBall;

	private int hits;

	private Player player;

	private IInteractable interactable;

	public Volleyball ball { get; private set; }

	public bool playerShouldCatch { get; private set; }

	public bool gameStarted { get; private set; }

	bool IInteractableComponent.enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	private void Start()
	{
		player = Singleton<GameServiceLocator>.instance.levelController.player;
		interactable = GetComponent<IInteractable>();
	}

	public void Interact()
	{
		StartDialogue(gameStartDialogue).onConversationFinish += OnConversationFinish;
	}

	private IConversation StartDialogue(string dialogueNode)
	{
		return Singleton<ServiceLocator>.instance.Locate<DialogueController>().StartConversation(dialogueNode, opponent.transform);
	}

	private void OnConversationFinish()
	{
		if (Singleton<GlobalData>.instance.gameData.tags.GetBool(gameStartTag))
		{
			StartGame();
		}
	}

	public void OnBallHitsGround()
	{
		if (playerShouldCatch && !(Time.time < enemyHitTime + enemyHitSafeTime) && hitGroundTimer == null)
		{
			hitGroundTimer = this.RegisterTimer(hitGroundSafeTime, delegate
			{
				hitGroundTimer = null;
				EndGame(popped: false);
			});
		}
	}

	public void OnBallWhackedByPlayer()
	{
		Timer.Cancel(hitGroundTimer);
		hitGroundTimer = null;
		if (playerShouldCatch && gameStarted)
		{
			hits++;
			Singleton<GlobalData>.instance.gameData.tags.SetFloat(hitsTag, hits);
			PassBallToEnemy();
		}
	}

	public void PopBall()
	{
		EndGame(popped: true);
	}

	private void StartGame()
	{
		if (!gameStarted)
		{
			gameStarted = true;
			interactable.enabled = false;
			player.onHoldableUsed += OnPlayerHoldableUsed;
			hits = 0;
			Singleton<GlobalData>.instance.gameData.tags.SetFloat(hitsTag, 0f);
			SpawnBall();
			PassBallToEnemy(opponent.transform.position + Random.insideUnitSphere, serveTime);
			player.ikAnimator.lookAt = ball.transform;
			player.walkFacingTarget = ball.transform;
			referee.lookAt = ball.transform;
			refereeInteractable.enabled = false;
		}
	}

	private void EndGame(bool popped)
	{
		if (gameStarted)
		{
			gameStarted = false;
			interactable.enabled = true;
			player.onHoldableUsed -= OnPlayerHoldableUsed;
			refereeInteractable.enabled = true;
			player.ikAnimator.lookAt = null;
			player.walkFacingTarget = null;
			referee.lookAt = null;
			if (ball != null)
			{
				ball.Orphan();
				orphanedBall = ball;
			}
			ball = null;
			if ((bool)whackingActions)
			{
				whackingActions.ForceNextWhack(null);
			}
			if (!popped)
			{
				Singleton<GameServiceLocator>.instance.achievements.SetLeaderboard("Beachstickball", hits * 2);
				BlowWhistle();
				StartDialogue(gameEndDialogue);
			}
			else
			{
				StartDialogue(poppedBallDialogue);
			}
		}
	}

	private void BlowWhistle()
	{
		referee.animator.SetTrigger("Whistle");
		StackResourceSortingKey stackResourceSortingKey = referee.ShowEmotion(Emotion.EyesClosed);
		this.RegisterTimer(1f, stackResourceSortingKey.ReleaseResource);
		whistle.Play();
		flagAnimator.SetTrigger("Wave");
	}

	private void PassBallToPlayer()
	{
		Vector3 destination;
		float time;
		if (hits < simpleHits || EASY_MODE)
		{
			destination = player.transform.position + (ball.transform.position - player.transform.position).normalized;
			time = enemyPassTimeSimple.Random();
		}
		else if (hits < simpleHits + easyHits)
		{
			destination = playerAimRegionEasy.RandomWithin();
			time = enemyPassTimeEasy.Random();
		}
		else if (hits < simpleHits + easyHits + mediumHits)
		{
			destination = playerAimRegionMedium.RandomWithin();
			time = enemyPassTimeMedium.Random();
		}
		else
		{
			destination = playerAimRegionHard.RandomWithin();
			time = enemyPassTimeHard.Random();
		}
		ball.body.linearVelocity = CalculateBallVelocity(ball.transform.position, destination, time);
		TwirlBall();
		enemyHitTime = Time.time;
		playerShouldCatch = true;
	}

	private void PassBallToEnemy(Vector3 aimPos, float time)
	{
		playerShouldCatch = false;
		TwirlBall();
		StartCoroutine(PassCoroutine(aimPos, time));
		opponent.walkTo = aimPos + (aimPos - ball.transform.position).normalized * enemyBackUpUnits;
	}

	private void PassBallToEnemy()
	{
		float num = playerPassTime.Random();
		if (!player.isGrounded)
		{
			num *= spikeMultiplier;
		}
		Vector3 aimPos = enemyAimRegion.RandomWithin();
		PassBallToEnemy(aimPos, num);
	}

	private IEnumerator PassCoroutine(Vector3 destination, float time)
	{
		Vector3 start = ball.transform.position;
		Vector3 startVelocity = CalculateBallVelocity(start, destination, time);
		float startTime = Time.fixedTime;
		bool armAnimation = false;
		while (true)
		{
			float num = Mathf.Clamp(Time.fixedTime - startTime, 0f, time);
			Vector3 position = start + startVelocity * num + 0.5f * Physics.gravity * num * num;
			ball.transform.position = position;
			if (Time.fixedTime > startTime + time)
			{
				break;
			}
			if (!armAnimation && startTime + time - Time.fixedTime < armAnimationTime)
			{
				armAnimation = true;
				opponent.SwingArms();
			}
			yield return new WaitForFixedUpdate();
		}
		PassBallToPlayer();
	}

	private void OnPlayerHoldableUsed(Holdable heldObject)
	{
		if ((bool)whackingActions)
		{
			whackingActions.ForceNextWhack(null);
		}
		whackingActions = heldObject.GetComponent<WhackingActions>();
		if ((bool)whackingActions && playerShouldCatch && (bool)ball)
		{
			Vector3 position = ball.transform.position;
			position += ball.body.linearVelocity * armAnimationTime;
			if (IsInsideEllipse(position, player.transform.position, Camera.main.transform.right * autoAimRadiusCamX, autoAimRadiusCamZ, autoAimBallHeight))
			{
				player.TurnToFace(ball.transform);
				whackingActions.ForceNextWhack(ball.GetComponent<IWhackable>());
			}
		}
	}

	private void SpawnBall()
	{
		if ((bool)ball)
		{
			ball.Orphan();
			orphanedBall = ball;
			ball = null;
		}
		if ((bool)orphanedBall)
		{
			orphanedBall.Kill();
			orphanedBall = null;
		}
		Vector3 vector = (player.transform.position - opponent.transform.position).SetY(0f).normalized * enemyBackUpUnits;
		GameObject gameObject = ballPrefab.CloneAt(opponent.transform.position + vector);
		ball = gameObject.GetComponent<Volleyball>();
		ball.controller = this;
		Physics.IgnoreCollision(ball.GetComponent<Collider>(), opponent.GetComponent<Collider>());
	}

	private void TwirlBall()
	{
		ball.body.AddTorque(Random.insideUnitSphere * 720f);
	}

	private Vector3 CalculateBallVelocity(Vector3 start, Vector3 destination, float time)
	{
		Vector3 vector = destination - start;
		_ = vector.magnitude;
		return vector / time + -Physics.gravity * time / 2f;
	}

	private bool IsInsideEllipse(Vector3 point, Vector3 center, Vector3 axisX, float axisYLength, float heightLength)
	{
		Vector3 vector = point - center;
		vector = Quaternion.AngleAxis(Vector3.SignedAngle(axisX, Vector3.right, Vector3.up), Vector3.up) * vector;
		float sqrMagnitude = axisX.sqrMagnitude;
		return vector.x.Sqr() / sqrMagnitude + vector.z.Sqr() / axisYLength.Sqr() + vector.y.Sqr() / heightLength.Sqr() < 1f;
	}
}
