using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class InputManager : MonoSingleton<InputManager>
    {
        public bool IsInputMode = false;

        public bool AltPressed { get; private set; }

        public float MouseValueX { get; private set; }

        public float MouseValueY { get; private set; }

        public float KeyValueVertical { get; private set; }

        public float KeyValueHorizontal { get; private set; }

        public bool KeyValueJump { get; private set; }


        void Update()
        {
            this.AltPressed = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
            this.MouseValueX = Input.GetAxis("Mouse X");
            this.MouseValueY = Input.GetAxis("Mouse Y");
            this.KeyValueVertical = Input.GetAxis("Vertical");
            this.KeyValueHorizontal = Input.GetAxis("Horizontal");
            this.KeyValueJump = Input.GetButtonDown("Jump");
        }
    }
}
