using UnityEngine;

[System.Serializable]
public struct Note  {

    [System.Serializable]
    public struct American
    {

        public enum Key
        {
            C = 0,
            Db = 1,
            D = 2,
            Eb = 3,
            E = 4,
            F = 5,
            Gb = 6,
            G = 7,
            Ab = 8,
            A = 9,
            Bb = 10,
            B = 11,
        }

        [Range(0, 10)]
        public int octave;
        public Key key;

        static public implicit operator int(American american)
        {
            return ((int)american.key + american.octave * 12);
        }

        static public implicit operator American(int value)
        {
            American a;
            a.octave = value / 12;
            a.key = (Key)(value - a.octave * 12);
            return a;
        }

        public static American Highest {
            get {
                American h = new American();
                h.octave = 10;
                h.key = Key.Gb;
                return h;
            }
        }

        public static American Lowest
        {
            get
            {
                American l = new American();
                l.octave = 0;
                l.key = Key.A;
                return l;
            }
        }

        public override string ToString()
        {
            return octave.ToString() + " "+key.ToString();
        }
    }

    [Range(0, 1024)]
    public int value;

    public Note(int v) {
        this.value = v;
    }

    #region operators

    static public implicit operator American(Note n) {
        return (American)n.value;
    }

    static public implicit operator Note(American a)
    {
        return new Note((int)a);
    }

    static public implicit operator Note(int value)
    {
        return new Note(value);
    }

    static public implicit operator int(Note note)
    {
        return note.value;
    }

    static public Note operator +(Note n, int i) {
        return new Note(n.value + i);
    }
    #endregion
}
