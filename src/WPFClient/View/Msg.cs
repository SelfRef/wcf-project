using System.Windows;

namespace WPFClient.View
{
    /// <summary>
    /// This class provides message boxes in client-side app.
    /// </summary>
    public static class Msg
    {
        /// <summary>
        /// Set of message contents for information message box.
        /// </summary>
        public enum InfoMsgs
        {
            LoginSuccess
        }
        /// <summary>
        /// Set of message contents for error message box.
        /// </summary>
        public enum ErrorMsgs
        {
            LoginFailure,
            DisconnectionError,
            WrongPass,
            NotRegistered,
            Banned,
            OperationFailed
        }
        /// <summary>
        /// Set of message contents for warning message box.
        /// </summary>
        public enum WarningMsgs
        {
            AlreadyLoggedIn,
            Banned,
            Kicked,
            ConnectionWarning,
            NotConnected,
            EmptyInput
        }
        /// <summary>
        /// Set of message contents for question message box.
        /// </summary>
        public enum AskMsgs
        {
            
        }
        
        /// <summary>
        /// Show information type message.
        /// </summary>
        /// <param name="msg">Message content.</param>
        public static void Info(InfoMsgs msg)
        {
            string strMsg;
            switch((int)msg)
            {
                case 0:
                    strMsg = Lang.Strings["login_success"];
                    break;
                default:
                    strMsg = "<info_message>";
                    break;
            }
            MessageBox.Show(strMsg, Lang.Strings["caption_operation_success"], MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Show error type message.
        /// </summary>
        /// <param name="msg">Message content.</param>
        /// <param name="btns">Message buttons.</param>
        /// <returns></returns>
        public static MessageBoxResult Error(ErrorMsgs msg, MessageBoxButton btns)
        {
            string strMsg;
            switch((int)msg)
            {
                case 0:
                    strMsg = Lang.Strings["login_failure"];
                    break;
                case 1:
                    strMsg = Lang.Strings["disconnection_error"];
                    break;
                case 2:
                    strMsg = Lang.Strings["wrong_pass"];
                    break;
                case 3:
                    strMsg = Lang.Strings["not_registered"];
                    break;
                case 4:
                    strMsg = Lang.Strings["banned"];
                    break;
                case 5:
                    strMsg = Lang.Strings["operation_failed"];
                    break;
                default:
                    strMsg = "<error_message>";
                    break;
            }
            return MessageBox.Show(strMsg, Lang.Strings["caption_operation_error"], btns, MessageBoxImage.Error);
        }
        /// <summary>
        /// Show warning type message.
        /// </summary>
        /// <param name="msg">Message content.</param>
        /// <param name="btns">Message buttons.</param>
        /// <returns></returns>
        public static MessageBoxResult Warning(WarningMsgs msg, MessageBoxButton btns)
        {
            string strMsg;
            switch ((int)msg)
            {
                case 0:
                    strMsg = Lang.Strings["login_already_loggedin"];
                    break;
                case 1:
                    strMsg = Lang.Strings["login_banned"];
                    break;
                case 2:
                    strMsg = Lang.Strings["server_kicked"];
                    break;
                case 3:
                    strMsg = Lang.Strings["connect_warning"];
                    break;
                case 4:
                    strMsg = Lang.Strings["not_connected"];
                    break;
                case 5:
                    strMsg = Lang.Strings["empty_input"];
                    break;
                default:
                    strMsg = "<warning_message>";
                    break;
            }
            return MessageBox.Show(strMsg, Lang.Strings["caption_warning"], btns, MessageBoxImage.Warning);
        }
        /// <summary>
        /// Show ask type message.
        /// </summary>
        /// <param name="msg">Message content.</param>
        /// <param name="btns">Message buttons.</param>
        /// <returns></returns>
        public static MessageBoxResult Ask(AskMsgs msg, MessageBoxButton btns)
        {
            string strMsg;
            switch ((int)msg)
            {
                default:
                    strMsg = "<ask_message>";
                    break;
            }
            return MessageBox.Show(strMsg, Lang.Strings["caption_warning"], btns, MessageBoxImage.Warning);
        }
    }
}
