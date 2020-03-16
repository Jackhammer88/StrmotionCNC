using Light.GuardClauses;
using ControllerService.GCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ControllerService
{
    public class NCFile : IDisposable
    {
        private List<byte> _data = new List<byte>();
        private readonly Encoding _encoding = Encoding.Default;
        private readonly object _locker = new object();
        private readonly string _path;
        private List<int> _pos = new List<int> { 0 };
        public NCFile()
        {

        }
        /// <summary>
        /// Создает экземпляр файла
        /// FileNotFoundException
        /// </summary>
        /// <param name="path"></param>
        public NCFile(string path)
        {
            _path = path;
            OpenFile(path);
        }
        public NCFile(string path, bool normalized)
        {
            if (!normalized)
            {
                _path = $"{Path.ChangeExtension(path, null)}-normalized.cnc";
            }
            else
                _path = path;
            OpenFile(path, normalized);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void OpenFile(string path, bool normalized = true)
        {
            if (!(File.Exists(path)))
                throw new FileNotFoundException("NC file not found.");

            if (!normalized)
            {
                using (var reader = new StreamReader(path))
                using (var writer = new StreamWriter(File.OpenWrite(_path)))
                    while (!reader.EndOfStream)
                    {
                        var str = GCodeNormalizer.NormalizeString(reader.ReadLine());
                        if (!string.IsNullOrWhiteSpace(str))
                            writer.WriteLine(str);
                    }
            }
            int totalPos = 0;
            foreach (var item in File.ReadLines(_path))
            {
                totalPos += item.Length + 2;
                _pos.Add(totalPos);
            }
            GC.Collect(2);

            _data = File.ReadAllBytes(_path).ToList();
            _pos.Add(_data.Count);
        }
        protected virtual void Dispose(bool ext)
        {
            _data = null;
            _pos = null;
            GC.Collect(2, GCCollectionMode.Forced);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Методы Dispose должны вызывать SuppressFinalize", Justification = "<Ожидание>")]
        public void Dispose()
        {
            Dispose(true);
        }
        public virtual string GetClearSomeString(int startIndex, int count)
        {
            return RemoveComments(GetSomeString(startIndex, count));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208:Правильно создавайте экземпляры исключений аргументов", Justification = "<Ожидание>")]
        public virtual string GetSomeString(int startIndex, int count)
        {
            _pos.MustNotBeNull(nameof(_pos));
            lock (_locker)
            {
                // Если указанное количество строк доступно для выгрузки
                if (startIndex >= 0 && count > 0 && (startIndex + count) < (_pos.Count - 1))
                    return _encoding.GetString(_data.Skip(_pos[startIndex]).Take(_pos[startIndex + count] - _pos[startIndex]).ToArray());
                // Если строк осталось меньше чем указано, то выгружаем все, что осталось
                if (startIndex >= 0 && count > 0)
                {
                    count = _pos.Count - 1 - startIndex;
                    return _encoding.GetString(_data.Skip(_pos[startIndex]).Take(_pos[startIndex + count] - _pos[startIndex]).ToArray());
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        public static string RemoveComments(string inputString)
        {
            while (inputString.Any(c => c == '('))
            {
                var str1 = inputString.TakeWhile(c => c != '(');
                var str2 = inputString.SkipWhile(c => c != ')').Skip(1);
                inputString = new string(str1.Concat(str2).ToArray());
            }
            return inputString;
        }
        public int StringCount
        {
            get
            {
                if (_pos != null)
                    return _pos.Count - 2;
                return 0;
            }
        }
    }
}
