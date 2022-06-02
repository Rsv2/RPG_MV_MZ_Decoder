using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RPG_MV_MZ_Decoder
{
    /// <summary>
    /// Класс дешифровки файлов RPG Maker MV/MZ
    /// </summary>
    public static class Decriptor
    {
        /// <summary>
        /// Ключ дешифровки.
        /// </summary>
        public static byte[] encriptcode;
        /// <summary>
        /// Текст файла System.json
        /// </summary>
        public static string SystemBackupText;
        /// <summary>
        /// Расшифровка файла.
        /// </summary>
        /// <param name="file">Файл для расшифровки</param>
        public static void Decriptfile(string file)
        {
            List<byte> Output = new List<byte>(File.ReadAllBytes(file));
            Output.RemoveRange(0, 16);
            for (int i = 0; i < 16; i++)
            {
                Output[i] = Convert.ToByte(Output[i] ^ encriptcode[i]);
            }
            string outputFile = "";
            if (file.Contains(".rpg"))
            {
                if (Output[0] == 0xff && Output[1] == 0xd8 && Output[2] == 0xff && Output[3] == 0xe0)
                {
                    outputFile = file.Replace(".rpgmvp", ".jpg");
                }
                else if (Output[0] == 0x42 && Output[1] == 0x4d)
                {
                    outputFile = file.Replace(".rpgmvp", ".bmp");
                }
                else if (Output[0] == 0x4f && Output[1] == 0x67 && Output[2] == 0x67)
                {
                    outputFile = file.Replace(".rpgmvo", ".ogg");
                }
                else if (Output[0] == 0x4d && Output[1] == 0x54)
                {
                    outputFile = file.Replace(".rpgmvo", ".mid");
                }
                else if (Output[0] == 0x30 && Output[1] == 0x26 && Output[2] == 0xb2)
                {
                    outputFile = file.Replace(".rpgmvo", ".wma");
                }
                else if (Output[0] == 0x52 && Output[1] == 0x49 && Output[2] == 0x46)
                {
                    outputFile = file.Replace(".rpgmvo", ".wav");
                }
                else if (Output[0] == 0x49 && Output[1] == 0x44 && Output[2] == 0x33)
                {
                    outputFile = file.Replace(".rpgmvo", ".mp3");
                }
                else if (Output[0] == 0x89 && Output[1] == 0x50 && Output[2] == 0x4e)
                {
                    outputFile = file.Replace(".rpgmvp", ".png");
                }
                else
                {
                    outputFile = file.Replace(".rpgmvp", ".unknown");
                    outputFile = file.Replace(".rpgmvo", ".unknown");
                }
            }
            else
            {
                outputFile = file.Substring(0, file.Length - 1);
            }
            File.WriteAllBytes(outputFile, Output.ToArray());
            File.Delete(file);
        }
        /// <summary>
        /// Получить ключ из файла System.json.
        /// </summary>
        public static void GetEncode()
        {
            string hexString = SystemBackupText.Substring(SystemBackupText.IndexOf("encryptionKey\":\"") + "encryptionKey\":\"".Length);
            hexString = hexString.Substring(0, hexString.IndexOf("\""));
            if (hexString != "")
            {
                encriptcode = new byte[hexString.Length / 2];
                for (int index = 0; index < encriptcode.Length; index++)
                {
                    string byteValue = hexString.Substring(index * 2, 2);
                    encriptcode[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
