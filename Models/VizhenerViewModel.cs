using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VizhenerWeb.Models
{
    public enum CipherMode { Encrypt, Decrypt }
    public class VizhenerViewModel
    {
        public VizhenerViewModel()
        {

        }
        public VizhenerViewModel(string message, string key, CipherMode mode, string error = null)
        {
            Message = message;
            Key = key;
            Mode = mode;
            Error = error;
        }
        public string Message { get; set; }
        public string Key { get; set; }
        public CipherMode Mode { get; set; }
        private string result;
        public string Result
        {
            get
            {
                if (result == null)
                {
                    result = Cipher();
                }
                return result;
            }
            set
            {
                result = value;
            }
        }

        public string Error { get; set; }

        public static string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        private string Cipher()
        {
            string result = "";
            string fullKey = GetFullKey();

            int keyIndex = 0;
            if (Validate())
            {
                for (int i = 0; i < Message.Length; i++)
                {
                    if (alphabet.Contains(char.ToLower(Message[i])))
                    {
                        if (char.IsLower(Message[i]))
                        {
                            if (Mode == CipherMode.Encrypt)
                                result += alphabet[(alphabet.IndexOf(Message[i]) + alphabet.IndexOf(fullKey[keyIndex])) % alphabet.Length];
                            else
                                result += alphabet[(alphabet.IndexOf(Message[i]) - alphabet.IndexOf(fullKey[keyIndex]) + alphabet.Length) % alphabet.Length];
                        }
                        else
                        {
                            if (Mode == CipherMode.Encrypt)
                                result += char.ToUpper(alphabet[(alphabet.IndexOf(char.ToLower(Message[i])) + alphabet.IndexOf(fullKey[keyIndex])) % alphabet.Length]);
                            else
                                result += char.ToUpper(alphabet[(alphabet.IndexOf(char.ToLower(Message[i])) - alphabet.IndexOf(fullKey[keyIndex]) + alphabet.Length) % alphabet.Length]);
                        }
                        keyIndex++;
                    }
                    else
                        result += Message[i];
                }
            }
            return result;
        }
        public string GetFullKey()
        {
            string fullKey = "";
            int length = 0;
            for (int i = 0; i < Message.Length; i++)
            {
                if (alphabet.Contains(char.ToLower(Message[i])))
                    length++;
            }
            for (int i = 0; i < length; i++)
                fullKey += char.ToLower(Key[i % Key.Length]);
            return fullKey;
        }

        public bool Validate()
        {
            if (Error != null) 
                return false;
            if (string.IsNullOrEmpty(Message))
            {
                Error = "Вы ввели пустое сообщение!";
                return false;
            }
            else if (string.IsNullOrEmpty(Key))
            {
                Error = "Вы ввели пустой ключ!";
                return false;
            }
            else if (!Key.All(c => alphabet.Contains(char.ToLower(c))))
            {
                Error = "Ключ должен содержать только буквы русского алфавита!";
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
