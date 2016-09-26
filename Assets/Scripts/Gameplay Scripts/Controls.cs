using UnityEngine;
using System.Collections;

public static class Controls {

	public static KeyCode left = KeyCode.A;
	public static KeyCode right = KeyCode.D;
	public static KeyCode up = KeyCode.W;
	public static KeyCode down = KeyCode.S;
	public static KeyCode jump = KeyCode.Space;
	public static KeyCode attack1 = KeyCode.Mouse0;
	public static KeyCode attack2 = KeyCode.Mouse1;
	public static KeyCode shield = KeyCode.LeftShift;
	public static KeyCode interact = KeyCode.E;
	public static KeyCode jumpALT = KeyCode.Joystick1Button0;
	public static KeyCode attack1ALT = KeyCode.Joystick1Button2;
	public static KeyCode attack2ALT = KeyCode.Joystick1Button1; 
	public static KeyCode shieldALT = KeyCode.Joystick1Button4;
	public static KeyCode interactALT = KeyCode.Joystick1Button3;

	static bool KeyPress(KeyCode key1, KeyCode key2)
	{
		if (Input.GetKeyDown (key1) || Input.GetKeyDown (key2)) 
		{
			return true;
		}
		return false;
	}

	static bool KeyStay(KeyCode key1, KeyCode key2)
	{
		if (Input.GetKey (key1) || Input.GetKey (key2)) 
		{
			return true;
		}
		return false;
	}

	static bool KeyRelease(KeyCode key1, KeyCode key2)
	{
		if (Input.GetKeyUp (key1) || Input.GetKeyUp (key2)) 
		{
			return true;
		}
		return false;
	}

	public static bool Jump()
	{
		return KeyStay (jump, jumpALT);
	}
	public static bool JumpPressed()
	{
		return KeyPress (jump, jumpALT);
	}
	public static bool JumpReleased()
	{
		return KeyRelease (jump, jumpALT);
	}

	public static bool Attack1()
	{
		return KeyStay (attack1, attack1ALT);
	}
	public static bool Attack1Pressed()
	{
		return KeyPress (attack1, attack1ALT);
	}
	public static bool Attack1Released()
	{
		return KeyRelease (attack1, attack1ALT);
	}

	public static bool Attack2()
	{
		return KeyStay (attack2, attack2ALT);
	}
	public static bool Attack2Pressed()
	{
		return KeyPress (attack2, attack2ALT);
	}
	public static bool Attack2Released()
	{
		return KeyRelease (attack2, attack2ALT);
	}

	public static bool Shield()
	{
		return KeyStay (shield, shieldALT);
	}
	public static bool ShieldPressed()
	{
		return KeyPress (shield, shieldALT);
	}
	public static bool ShieldReleased()
	{
		return KeyRelease (shield, shieldALT);
	}

	public static bool Interact()
	{
		return KeyStay (interact, interactALT);
	}
	public static bool InteractPressed()
	{
		return KeyPress (interact, interactALT);
	}
	public static bool InteractReleased()
	{
		return KeyRelease (interact, interactALT);
	}

	public static float Horizontal()
	{
		float hor = 0;
		if (Input.GetAxis ("Horizontal") != 0)
			return Input.GetAxis ("Horizontal");
		else 
		{
			if (Input.GetKey (left))
				hor -= 1;
			if (Input.GetKey (right))
				hor += 1; 
			return hor;
		}
	}

	public static float Vertical()
	{
		float ver = 0;
		if (Input.GetAxis ("Vertical") != 0)
			return Input.GetAxis ("Vertical");
		else 
		{
			if (Input.GetKey (down))
				ver -= 1;
			if (Input.GetKey (up))
				ver += 1; 
			return ver;
		}
	}
}
