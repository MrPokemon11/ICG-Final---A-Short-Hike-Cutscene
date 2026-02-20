using System.Collections;
using QuickUnityTools.Input;
using UnityEngine;

public class FishingTutorial : MonoBehaviour
{
	public string castNode = "FishTutCastStart";

	public string chatNode = "FishTutChatStart";

	public string nibbleNode = "FishTutNibbleStart";

	public string biteNode = "FishTutBiteStart";

	public string pullBackNode = "FishTutPullStart";

	public string finishedNode = "FishTutDoneStart";

	public string waterNode = "FishTutWaterStart";

	public Transform npc;

	public Transform[] sitPositions;

	public Transform castDirection;

	public CollectableItem fishingRodItem;

	public FishSpecies tutorialFish;

	public GameObject fishingCamera;

	private NPCIKAnimator npcAnimator;

	private void Awake()
	{
		npcAnimator = npc.GetComponentInChildren<NPCIKAnimator>();
	}

	public void StartTutorial()
	{
		StartCoroutine(TutorialRoutine());
	}

	private IEnumerator TutorialRoutine()
	{
		GameUserInput input = GameUserInput.CreateInput(base.gameObject);
		Player player = Singleton<GameServiceLocator>.instance.levelController.player;
		Transform closest = sitPositions.MinValue((Transform p) => (p.position - player.transform.position).sqrMagnitude);
		DialogueController dialogue = Singleton<ServiceLocator>.instance.Locate<DialogueController>();
		float originalRadius = npcAnimator.lookAtPlayerRadius;
		npcAnimator.lookAtPlayerRadius = 0f;
		player.WalkTo(closest.position, 6f);
		yield return new WaitUntil(() => !player.walkTo.HasValue);
		if ((player.transform.position - closest.position).sqrMagnitude > 2f.Sqr())
		{
			player.body.position = closest.position;
		}
		player.TurnToFace(castDirection);
		yield return new WaitForSeconds(0.5f);
		if (player.heldItem == null || player.heldItem.associatedItem != fishingRodItem)
		{
			Holdable.EquipFromInventory(player, fishingRodItem);
			yield return new WaitForSeconds(0.5f);
		}
		IConversation conversation = dialogue.StartConversation(castNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
		yield return new WaitUntil(() => input.GetUseItemButton().wasPressed);
		player.UseItem();
		FishingActions fishingRod = player.heldItem.GetComponent<FishingActions>();
		fishingRod.tutorialMode = true;
		fishingRod.allowSleeping = false;
		yield return new WaitForSeconds(5f);
		fishingCamera.SetActive(value: true);
		conversation = dialogue.StartConversation(chatNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
		yield return new WaitForSeconds(4f);
		fishingRod.allowSleeping = true;
		yield return new WaitForSeconds(2f);
		if (!fishingRod.isCast)
		{
			Debug.LogError("Fishing rod should be cast!");
			fishingRod.tutorialMode = false;
			fishingRod.allowSleeping = true;
			npcAnimator.lookAtPlayerRadius = originalRadius;
			Object.Destroy(input);
			yield break;
		}
		fishingRod.Nibble();
		yield return new WaitForSeconds(0.25f);
		fishingRod.Nibble();
		yield return new WaitForSeconds(0.25f);
		fishingRod.allowSleeping = false;
		conversation = dialogue.StartConversation(nibbleNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
		fishingCamera.SetActive(value: false);
		yield return new WaitForSeconds(1.5f);
		fishingRod.Nibble();
		yield return new WaitForSeconds(0.5f);
		fishingRod.Nibble();
		yield return new WaitForSeconds(1f);
		fishingRod.fishEncounter = tutorialFish;
		fishingRod.Bite();
		conversation = dialogue.StartConversation(biteNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
		yield return new WaitUntil(() => input.GetUseItemButton().wasPressed);
		player.UseItem();
		Object.Destroy(input);
		player.disableMenu = true;
		yield return new WaitForSeconds(1f);
		if (!fishingRod.isCast)
		{
			conversation = dialogue.StartConversation(waterNode, npc);
			yield return new WaitUntil(() => !conversation.isAlive);
			CleanUpTutorial(player, originalRadius, fishingRod);
			yield break;
		}
		conversation = dialogue.StartConversation(pullBackNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
		yield return new WaitUntil(() => !fishingRod.isCast);
		if (player.isSwimming)
		{
			conversation = dialogue.StartConversation(waterNode, npc);
			yield return new WaitUntil(() => !conversation.isAlive);
			CleanUpTutorial(player, originalRadius, fishingRod);
			yield break;
		}
		yield return new WaitForSeconds(1f);
		yield return new WaitUntil(() => player.input.hasFocus);
		yield return new WaitForSeconds(1f);
		yield return new WaitUntil(() => player.input.hasFocus);
		CleanUpTutorial(player, originalRadius, fishingRod);
		conversation = dialogue.StartConversation(finishedNode, npc);
		yield return new WaitUntil(() => !conversation.isAlive);
	}

	private void CleanUpTutorial(Player player, float originalRadius, FishingActions fishingRod)
	{
		player.disableMenu = false;
		fishingRod.tutorialMode = false;
		fishingRod.allowSleeping = true;
		npcAnimator.lookAtPlayerRadius = originalRadius;
	}
}
