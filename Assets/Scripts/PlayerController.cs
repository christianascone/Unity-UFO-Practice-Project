using UnityEngine;
using System.Collections;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed; //Floating point variable to store the player's movement speed.

    public Text
        countText; //Store a reference to the UI Text component which will display the number of pickups collected.

    public Text winText; //Store a reference to the UI Text component which will display the 'You win' message.

    private Rigidbody2D rb2d; //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private int count; //Integer to store the number of pickups collected so far.
    
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

    // Use this for initialization
    void Start()
    {
        //Get and store a reference to the Rigidbody2D component so that we can access it.
        rb2d = GetComponent<Rigidbody2D>();

        //Initialize count to zero.
        count = 0;

        //Initialze winText to a blank string since we haven't won yet at beginning.
        winText.text = "";

        //Call our SetCountText function which will update the text with the current value for count.
        SetCountText();
        print("Started");
    }

    //FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        float moveHorizontal = 0;
        float moveVertical = 0; 
        
        //Check if we are running either in the Unity editor or in a standalone build.
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        //Store the current horizontal input in the float moveHorizontal.
        moveHorizontal = Input.GetAxis("Horizontal");

        //Store the current vertical input in the float moveVertical.
        moveVertical = Input.GetAxis("Vertical");

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        //Check if Input has registered more than zero touches
            if (Input.touchCount > 0)
            {
                //Store the first touch detected.
                Touch myTouch = Input.touches[0];
                
                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began)
                {
                    print("Began");
                    //If so, set touchOrigin to the position of that touch
                    touchOrigin = myTouch.position;
                }
                
                //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
                {
                    print("Ended");
                    //Set touchEnd to equal the position of this touch
                    Vector2 touchEnd = myTouch.position;
                    
                    //Calculate the difference between the beginning and end of the touch on the x axis.
                    float x = touchEnd.x - touchOrigin.x;
                    
                    //Calculate the difference between the beginning and end of the touch on the y axis.
                    float y = touchEnd.y - touchOrigin.y;
                    
                    //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                    touchOrigin.x = -1;
                    
                    //Check if the difference along the x axis is greater than the difference along the y axis.
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                        //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                        moveHorizontal = x > 0 ? 1 : -1;
                    else
                        //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                        moveVertical = y > 0 ? 1 : -1;
                }
            }

        #endif

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.AddForce(movement * speed);
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (other.gameObject.CompareTag("PickUp"))

            //... then set the other object we just collided with to inactive.
            other.gameObject.SetActive(false);

        //Add one to the current value of our count variable.
        count = count + 1;

        //Update the currently displayed count by calling the SetCountText function.
        SetCountText();
    }

    //This function updates the text displaying the number of objects we've collected and displays our victory message if we've collected all of them.
    void SetCountText()
    {
        //Set the text property of our our countText object to "Count: " followed by the number stored in our count variable.
        countText.text = "Count: " + count;

        //Check if we've collected all 12 pickups. If we have...
        if (count >= 3)
            //... then set the text property of our winText object to "You win!"
            winText.text = "You win!";
    }
}