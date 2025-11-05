using System;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>
    {
        public bool IsInputMode = false;

        public bool AltPressed { get; private set; }
        public float MouseValueX { get; private set; }
        public float MouseValueY { get; private set; }
        public float KeyValueVertical { get; private set; }
        public float KeyValueHorizontal { get; private set; }
        public bool KeyValueJump { get; private set; }     
        public bool KeyValueLockEnemy { get; private set; } 

        void Update()
        {
            // 1. 持续状态（Should use GetKey / GetAxis）
            bool altDown = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);
            float mouseDeltaX = Input.GetAxis("Mouse X");
            float mouseDeltaY = Input.GetAxis("Mouse Y");
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            // 2. 瞬时动作（Should use GetButtonDown）
            bool jumpDown = Input.GetButtonDown("Jump");
            bool lockEnemyDown = Input.GetKeyDown(KeyCode.Q);

            // 3. 日志：只在有意义时输出
            if (jumpDown)
            {
                LogHelper.Log("Jump key triggered", LogUser.InputManager);
            }
            if (lockEnemyDown)
            {
                LogHelper.Log("Lock enemy key triggered", LogUser.InputManager);
            }

            // 4. 更新属性
            this.AltPressed = altDown;
            this.MouseValueX = mouseDeltaX;
            this.MouseValueY = mouseDeltaY;
            this.KeyValueVertical = vertical;
            this.KeyValueHorizontal = horizontal;
            this.KeyValueJump = jumpDown;           
            this.KeyValueLockEnemy = lockEnemyDown; 
        }
    }
}