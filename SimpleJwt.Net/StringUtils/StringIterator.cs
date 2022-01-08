namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // Minimal or zero alloc string iterator
    public struct StringIterator
    {
        private readonly string _target;
        private readonly char _separator;

        private StringIteratorEntry _currentEntry;
        private int _position;
        private bool _ended;


        public StringIterator(string target, char separator)
        {
            _target = target;
            _separator = separator;

            _currentEntry = StringIteratorEntry.Null;
            _position = 0;
            _ended = false;
        }

        public StringIteratorEntry Current()
        {
            return _currentEntry;
        }
        
        public bool MoveNext()
        {
            // Ending condition
            if (_ended)
                return false;

            // Validation
            if (_target == null)
            {
                _currentEntry = StringIteratorEntry.Null;
                _ended = true;
                return true;
            }

            if (_target == string.Empty)
            {
                _currentEntry = StringIteratorEntry.Empty;
                _ended = true;
                return true;
            }

            // Actual iteration through string
            for (int i = _position; i < _target.Length; i++)
            {
                if (_target[i] == _separator)
                {
                    _currentEntry = new StringIteratorEntry(_target, _position, i - _position);
                    _position = i + 1;
                    
                    return true;
                }
            }

            // End of string -> wrap everything that it left
            _currentEntry = new StringIteratorEntry(_target, _position, _target.Length - _position);
            _ended = true;
            
            return true;
        }
    }


    // Entry for string iterator
    public readonly struct StringIteratorEntry
    {
        public static readonly StringIteratorEntry Null = new StringIteratorEntry(null, -1, 0);
        public static readonly StringIteratorEntry Empty = new StringIteratorEntry(string.Empty, 0, -1);
        
        public readonly string OriginalString;
        public readonly int Offset;
        public readonly int Count;


        public StringIteratorEntry(string originalString, int offset, int count)
        {
            OriginalString = originalString;
            Offset = offset;
            Count = count;
        }

        public bool IsNull => Offset == -1;
        public bool IsEmpty => Count == -1;
        public bool IsNullOrEmpty => Offset == -1 || Count == -1;
    }
}