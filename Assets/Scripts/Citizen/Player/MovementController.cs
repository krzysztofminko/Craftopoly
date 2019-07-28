using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class MovementController : MonoBehaviour
{
	Player player;
	private Vector3 cameraOffset;

    void Awake()
    {
		player = GetComponent<Player>();
		cameraOffset = Camera.main.transform.position - player.transform.position;

	}

    void Update()
    {
		if (!player.controlsEnabled)
		{
			player.animator.SetBool("Walk", false);
		}
		else
		{
			Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (Input.GetButton("Run") ? player.runSpeed : player.walkSpeed) * Time.deltaTime;

			Vector3 direction = Vector3.zero;
			if (Input.GetMouseButton(1))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100.0f))
					direction = hit.point - transform.position;
			}
			else
			{
				direction = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));
			}

			if (direction.sqrMagnitude > 0)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up), Vector3.up), 500 * Time.deltaTime);
			else if (movement.sqrMagnitude > 0)
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(movement, Vector3.up), Vector3.up), 500 * Time.deltaTime);

			transform.position += movement;

			Camera.main.transform.position = transform.position + cameraOffset;

			player.animator.SetBool("Walk", movement.sqrMagnitude > 0);
		}
	}
}
