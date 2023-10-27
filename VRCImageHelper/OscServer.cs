namespace VRCImageHelper
{
    public class OscEventArgs : EventArgs
    {
        public OscEventArgs(string path, int data)
        {
            Path = path;
            Data = data;
        }

        public string Path { get; }
        public int Data { get; }
    }
    public delegate void OscEventHandler(object sender, OscEventArgs e);

    internal class OscServer
    {
        private bool _enabled = false;
        public bool Enable
        {
            set
            {
                if (value == _enabled) return;

                if (value)
                {
                    // open
                }
                else
                {
                    // close
                }

                _enabled = value;
            }
            get
            {
                return _enabled;
            }
        }


        public event OscEventHandler? Received;
    }
}
