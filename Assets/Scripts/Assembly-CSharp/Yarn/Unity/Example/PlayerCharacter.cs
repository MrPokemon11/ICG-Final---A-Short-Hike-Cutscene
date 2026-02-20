using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity.Example
{
	public class PlayerCharacter : MonoBehaviour
	{
		public float minPosition = -5.3f;

		public float maxPosition = 5.3f;

		public float moveSpeed = 1f;

		public float interactionRadius = 2f;

		public float movementFromButtons { get; set; }

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.matrix = Matrix4x4.TRS(base.transform.position, Quaternion.identity, new Vector3(1f, 1f, 0f));
			Gizmos.DrawWireSphere(Vector3.zero, interactionRadius);
		}

		private void Update()
		{
			if (!Object.FindObjectOfType<DialogueRunner>().isDialogueRunning)
			{
				float axis = Input.GetAxis("Horizontal");
				axis += movementFromButtons;
				axis *= moveSpeed * Time.deltaTime;
				Vector3 position = base.transform.position;
				position.x += axis;
				position.x = Mathf.Clamp(position.x, minPosition, maxPosition);
				base.transform.position = position;
				if (Input.GetKeyDown(KeyCode.Space))
				{
					CheckForNearbyNPC();
				}
			}
		}

		public void CheckForNearbyNPC()
		{
			NPC nPC = new List<NPC>(Object.FindObjectsOfType<NPC>()).Find((NPC p) => !string.IsNullOrEmpty(p.talkToNode) && (p.transform.position - base.transform.position).magnitude <= interactionRadius);
			if (nPC != null)
			{
				Object.FindObjectOfType<DialogueRunner>().StartDialogue(nPC.talkToNode);
			}
		}
	}
}
