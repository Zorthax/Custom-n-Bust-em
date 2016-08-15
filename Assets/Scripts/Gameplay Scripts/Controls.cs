using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;
    public KeyCode jump;
    public KeyCode attack1;
    public KeyCode attack2;
    public KeyCode shield;
    public KeyCode jumpALT;
    public KeyCode attack1ALT;
    public KeyCode attack2ALT;
    public KeyCode shieldALT;

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);

        left = KeyCode.A;
        right = KeyCode.D;
        up = KeyCode.W;
        down = KeyCode.S;
        jump = KeyCode.Space;
        attack1 = KeyCode.Mouse0;
        attack2 = KeyCode.Mouse1;
        shield = KeyCode.LeftShift;
        jumpALT = KeyCode.Joystick1Button0;
        attack1ALT = KeyCode.Joystick1Button2;
        attack2ALT = KeyCode.Joystick1Button3;
        shieldALT = KeyCode.Joystick1Button4;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
