using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class KeyManager : MonoBehaviour
    {
        public static bool WaitingForKeyInput { get; private set; }

        public static KeyCode RollLeftKey = KeyCode.Q;
        public static KeyCode RollRightKey = KeyCode.E;
        public static KeyCode AccelerateKey = KeyCode.W;
        public static KeyCode DecelerateKey = KeyCode.S;
        public static KeyCode StrafeLeftKey = KeyCode.A;
        public static KeyCode StrafeRightKey = KeyCode.D;
        public static KeyCode BrakingKey = KeyCode.X;
        public static KeyCode BoostKey = KeyCode.LeftShift;
        public static KeyCode FlightModeSwitchKey = KeyCode.T;
        public static KeyCode PauseKey = KeyCode.P;

        private static Button _nextBindKey;
        private static KeyCode _mostRecentKey;

        public void NextKeyToBind(Button keyButton)
        {
            if (WaitingForKeyInput) return;
            
            _nextBindKey = keyButton;
            WaitingForKeyInput = true;
            
            _nextBindKey.gameObject.GetComponentInChildren<Text>().text = "<?>";
        }

        public static string GetKeyCodeForName(string keyName) => keyName switch
        {
            "BtnRollLeft" => RollLeftKey.ToString(),
            "BtnRollRight" => RollRightKey.ToString(),
            "BtnAccelerate" => AccelerateKey.ToString(),
            "BtnDecelerate" => DecelerateKey.ToString(),
            "BtnStrafeLeft" => StrafeLeftKey.ToString(),
            "BtnStrafeRight" => StrafeRightKey.ToString(),
            "BtnBraking" => BrakingKey.ToString(),
            "BtnBoost" => BoostKey.ToString(),
            "BtnFlightModeSwitch" => FlightModeSwitchKey.ToString(),
            "BtnPause" => PauseKey.ToString(),
            _ => "NONE"
        };

        private static void BindKey(KeyCode newKey)
        {
            if(newKey == KeyCode.None) return;
            
            switch (_nextBindKey.name)
            {
                case "BtnRollLeft":
                    RollLeftKey = newKey;
                    break;
                case "BtnRollRight":
                    RollRightKey = newKey;
                    break;
                case "BtnAccelerate":
                    AccelerateKey = newKey;
                    break;
                case "BtnDecelerate":
                    DecelerateKey = newKey;
                    break;
                case "BtnStrafeLeft":
                    StrafeLeftKey = newKey;
                    break;
                case "BtnStrafeRight":
                    StrafeRightKey = newKey;
                    break;
                case "BtnBraking":
                    BrakingKey = newKey;
                    break;
                case "BtnBoost":
                    BoostKey = newKey;
                    break;
                case "BtnFlightModeSwitch":
                    FlightModeSwitchKey = newKey;
                    break;
                case "BtnPause":
                    PauseKey = newKey;
                    break;
            }

            _nextBindKey.gameObject.GetComponentInChildren<Text>().text = newKey.ToString();
            _mostRecentKey = newKey;
        }

        private void Update()
        {
            if (_mostRecentKey == 0) return;
            if (!Input.GetKeyUp(_mostRecentKey)) return;

            WaitingForKeyInput = false;
            _mostRecentKey = 0;
        }

        private void OnGUI()
        {
            if (!WaitingForKeyInput) return;
            if (_nextBindKey == null || _nextBindKey.name == string.Empty) return;

            var e = Event.current;
            if (e.isKey)
            {
                BindKey(e.keyCode);
            }
        }
    }
}