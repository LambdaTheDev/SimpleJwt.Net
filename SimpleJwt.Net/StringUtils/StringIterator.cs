namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // Minimal or zero alloc string iterator
    public struct StringIterator
    {
        private readonly string _target;
        private readonly char _separator;

        private StringSegment _currentEntry;
        private int _position;
        private bool _ended;


        public StringIterator(string target, char separator)
        {
            _target = target;
            _separator = separator;

            _currentEntry = StringSegment.Null;
            _position = 0;
            _ended = false;
        }

        public StringSegment Current()
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
                _currentEntry = StringSegment.Null;
                _ended = true;
                return true;
            }

            if (_target == string.Empty)
            {
                _currentEntry = StringSegment.Empty;
                _ended = true;
                return true;
            }

            // Actual iteration through string
            for (int i = _position; i < _target.Length; i++)
            {
                if (_target[i] == _separator)
                {
                    _currentEntry = new StringSegment(_target, _position, i - _position);
                    _position = i + 1;
                    
                    return true;
                }
            }

            // End of string -> wrap everything that it left
            _currentEntry = new StringSegment(_target, _position, _target.Length - _position);
            _ended = true;
            
            return true;
        }
    }
}