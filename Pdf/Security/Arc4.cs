namespace Fonet.Pdf.Security
{
    /// <summary>
    ///     ARC4 is a fast, simple stream encryption algorithm that is
    ///     compatible with RSA Security's RC4 algorithm.
    /// </summary>
    internal class Arc4
    {
        private readonly byte[] _state = new byte[ 256 ];
        private int _x;
        private int _y;

        internal Arc4()
        {
        }

        internal Arc4( byte[] key )
        {
            Initialise( key );
        }

        internal Arc4( byte[] key, int offset, int length )
        {
            // Extract the key from the passed array and call initialise.
            var k2 = new byte[ length ];
            for ( var x = 0; x < length; x++ )
                k2[ x ] = key[ offset + x ];
            Initialise( k2 );
        }

        /// <summary>
        ///     Initialises internal state from the passed key.
        /// </summary>
        /// <remarks>
        ///     Can be called again with a new key to reuse an Arc4 instance.
        /// </remarks>
        /// <param name="key">The encryption key.</param>
        internal void Initialise( byte[] key )
        {
            for ( var i = 0; i < 256; i++ )
                _state[ i ] = (byte)i;
            for ( int i = 0, j = 0; i < 256; i++ )
            {
                j = ( j + _state[ i ] + key[ i % key.Length ] ) % 256;
                byte t = _state[ i ];
                _state[ i ] = _state[ j ];
                _state[ j ] = t;
            }
            _x = 0;
            _y = 0;
        }

        /// <summary>
        ///     Encrypts or decrypts the passed byte array.
        /// </summary>
        /// <param name="dataIn">
        ///     The data to be encrypted or decrypted.
        /// </param>
        /// <param name="dataOut">
        ///     The location that the encrypted or decrypted data is to be placed.
        ///     The passed array should be at least the same size as dataIn.
        ///     It is permissible for the same array to be passed for both dataIn
        ///     and dataOut.
        /// </param>
        internal void Encrypt( byte[] dataIn, byte[] dataOut )
        {
            for ( var x = 0; x < dataIn.Length; x++ )
                dataOut[ x ] = (byte)( dataIn[ x ] ^ Arc4Byte() );
        }

        /// <summary>
        ///     Generates a pseudorandom byte used to encrypt or decrypt.
        /// </summary>
        private byte Arc4Byte()
        {
            _x = ( _x + 1 ) % 256;
            _y = ( _y + _state[ _x ] ) % 256;
            byte temp = _state[ _x ];
            _state[ _x ] = _state[ _y ];
            _state[ _y ] = temp;
            return _state[ ( _state[ _x ] + _state[ _y ] ) % 256 ];
        }
    }
}