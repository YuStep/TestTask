using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        /// <summary>
        /// Считывание символов из потока байтов.
        /// </summary>
        private StreamReader LocalStream { get; set; }

        /// <summary>
        /// Содержимое файла, прочитанного из потока.
        /// </summary>
        private string CharFromStream { get; set; }

        /// <summary>
        /// Флаг вызова Dispose.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Флаг конца потока.
        /// </summary>
        private bool IsEndStream;

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get { return IsEndStream; }
            // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set
            {
                if (LocalStream.EndOfStream)
                    IsEndStream = value;
            }
        }

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            if (File.Exists(fileFullPath))
            {
                LocalStream = new StreamReader(fileFullPath, encoding: Encoding.Default);
                CharFromStream = LocalStream.ReadToEnd();
                ResetPositionToStart();
            }
            else
            {
                throw new FileNotFoundException("Не найден файл по указанному пути.");
            }
        }

        /// <summary>
        /// Подсчитывает число вхождений аргумента в строке, заполненной из потока.
        /// </summary>
        /// <param name="ch">Символ, считываемый ReadNextChar.</param>
        /// <returns>Число вхождений (int).</returns>
        public int NumberOfOccurrences(char ch)
        {
            int count = 0;
            foreach (char i in CharFromStream)
                if (i == ch) count++;
            return count;
        }


        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (LocalStream.EndOfStream) IsEof = true;
            // TODO : Необходимо считать очередной символ из LocalStream.
            try
            {
                return (char)LocalStream.Read();
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Ф-ция получения из потока пар (одинаковых) регистронезависимых букв. 
        /// </summary>
        /// <returns>Список строк из пар одинаковых букв.</returns>
        public List<string> FindPairLetter()
        {
            List<string> res = new List<string>();
            string pattern = @"\w{2}";
            Regex pat = new Regex(pattern, RegexOptions.IgnoreCase);
            if (pat.IsMatch(CharFromStream))
            {
                MatchCollection matchcol = pat.Matches(CharFromStream);
                foreach (Match item in matchcol)
                {
                    res.Add(item.Value.ToLower());
                }
            }
            return res;
        }


        /// <summary>
        /// Публичная реализации шаблона очистки объекта.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Защищенная реализации шаблона очистки объекта.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    LocalStream.Dispose();
                }
                // освобождаем неуправляемые объекты
                disposed = true;
            }
        }
    }
}
