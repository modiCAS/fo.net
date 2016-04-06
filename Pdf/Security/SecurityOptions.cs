using System.Collections.Specialized;

namespace Fonet.Pdf.Security
{
    public class SecurityOptions
    {
        /// <summary>
        ///     Password that disables all security permissions
        /// </summary>
        private string _ownerPassword;

        /// <summary>
        ///     Collection of flags describing permissions granted to user who opens
        ///     a file with the user password.
        /// </summary>
        /// <remarks>
        ///     The given initial value zero's out first two bits.
        ///     The PDF specification dictates that these entries must be 0.
        /// </remarks>
        private BitVector32 _permissions = new BitVector32( -4 );

        /// <summary>
        ///     The user password
        /// </summary>
        private string _userPassword;

        /// <summary>
        ///     Returns the owner password as a string.
        /// </summary>
        /// <value>
        ///     The default value is null
        /// </value>
        public string OwnerPassword
        {
            get { return _ownerPassword; }
            set { _ownerPassword = value; }
        }

        /// <summary>
        ///     Returns the user password as a string.
        /// </summary>
        /// <value>
        ///     The default value is null
        /// </value>
        public string UserPassword
        {
            get { return _userPassword; }
            set { _userPassword = value; }
        }

        /// <summary>
        ///     The document access privileges encoded in a 32-bit unsigned integer
        /// </summary>
        /// <value>
        ///     The default access priviliges are:
        ///     <ul>
        ///         <li>Printing disallowed</li>
        ///         <li>Modifications disallowed</li>
        ///         <li>Copy and Paste disallowed</li>
        ///         <li>Addition or modification of annotation/form fields disallowed</li>
        ///     </ul>
        ///     To override any of these priviliges see the <see cref="EnablePrinting" />,
        ///     <see cref="EnableChanging" />, <see cref="EnableCopying" />,
        ///     <see cref="EnableAdding" /> methods
        /// </value>
        public int Permissions
        {
            get { return _permissions.Data; }
            set { _permissions = new BitVector32( value ); }
        }

        /// <summary>
        ///     Enables or disables printing.
        /// </summary>
        /// <param name="enable">If true enables printing otherwise false</param>
        public void EnablePrinting( bool enable )
        {
            _permissions[ 4 ] = enable;
        }

        /// <summary>
        ///     Enable or disable changing the document other than by adding or
        ///     changing text notes and AcroForm fields.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableChanging( bool enable )
        {
            _permissions[ 8 ] = enable;
        }

        /// <summary>
        ///     Enable or disable copying of text and graphics from the document.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableCopying( bool enable )
        {
            _permissions[ 16 ] = enable;
        }

        /// <summary>
        ///     Enable or disable adding and changing text notes and AcroForm fields.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableAdding( bool enable )
        {
            _permissions[ 32 ] = enable;
        }
    }
}