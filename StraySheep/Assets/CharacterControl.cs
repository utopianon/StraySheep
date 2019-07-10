using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test
{
    public class CharacterControl : MonoBehaviour
    {
        public int maxJumps = 2;
        public int currentJumps;

        public bool grounded;

        public int bufferSize = 12;     //How many frames the input buffer keeps checking for new inputs / The Size of the Buffer
        public InputBufferItem[] inputBuffer;

        public Vector3 velocity;
        public float jumpPow = 0.4f;
        public float gravity = -0.025f;

        // Use this for initialization
        void Start()
        {
            inputBuffer = new InputBufferItem[bufferSize];
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                inputBuffer[i] = new InputBufferItem();
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateBuffer();
            UpdateCommand();
            //UpdateAction();
            UpdatePhysics();
        }

        public void UpdateBuffer()
        {
            //Get the Input for the current Frame -- "Jump" is defined in  Edit > Project Settings > Input. Use whatever button/string you like
            if (Input.GetAxisRaw("Jump") > 0) { inputBuffer[inputBuffer.Length - 1].Hold(); }
            else { inputBuffer[inputBuffer.Length - 1].ReleaseHold(); }

            //Go through each Input Buffer item and copy the previous frame
            for (int i = 0; i < inputBuffer.Length - 1; i++)
            {
                inputBuffer[i].hold = inputBuffer[i + 1].hold;
                inputBuffer[i].used = inputBuffer[i + 1].used;
            }

        }

        public void UpdateCommand()
        {
            for (int i = 0; i < inputBuffer.Length; i++)
            {
                if (inputBuffer[i].CanExecute()) { if (Jump()) { inputBuffer[i].Execute(); break; } }
            }
        }

        public bool Jump()
        {
            if (currentJumps > 0)
            {
                velocity.y = jumpPow;
                currentJumps--;
                return true;
            }
            return false;
        }

        //This is really really basic physics just for demo purposes. You'll have your own physics I'm assuming.
        public void UpdatePhysics()
        {
            velocity.y += gravity;
            transform.position += velocity;
            if (transform.position.y < 0)
            {
                grounded = true;
                currentJumps = maxJumps;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            else { grounded = false; }
        }
    }

    [System.Serializable]
    public class InputBufferItem
    {
        public int hold;
        public bool used;

        public bool CanExecute()
        {
            if (hold == 1 && !used) { return true; }
            return false;
        }

        public void Execute()
        {
            used = true;
        }

        public void Hold()
        {
            if (hold < 0) { hold = 1; }
            else { hold += 1; }
        }

        public void ReleaseHold()
        {
            if (hold > 0) { hold = -1; used = false; }
            else { hold = 0; }
        }

        public void ForceHold()
        {
            hold = 2;
        }

        public void Reset()
        {
            hold = 0;
            used = false;
        }
    }
}
