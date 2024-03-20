using Banking.Extensions;
using Banking.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Banking.Services
{
    public  class CsvFileService : ICsvFileService
    {
        private readonly Encoding _encoding;

        private readonly bool _hasHeaderRecord;

        private readonly string _separator;

        public CsvFileService(Encoding encoding, string separator, bool header)
        {
            _encoding = encoding;
            _separator = separator;
            _hasHeaderRecord = header;
        }

        public ReadOnlyCollection<T> Read<T>(string path) where T : class
        {
            var bytes = File.ReadAllBytes(path);
            List<T> records = ToColecction<T>(bytes);
            return new ReadOnlyCollection<T>(records);
        }

        private List<T> ToColecction<T>(byte[] fileInBytes) where T : class
        {
            List<T> records = new List<T>();
            using (var memoryStream = new MemoryStream(fileInBytes))
            {
                using (var streamReader = new StreamReader(memoryStream, _encoding))
                {
                    var cultureInfo = CultureInfo.InvariantCulture;
                    var configuration = new CsvConfiguration(cultureInfo)
                    {
                        Delimiter = _separator,
                        MissingFieldFound = null,
                        HasHeaderRecord = _hasHeaderRecord,
                        Encoding = _encoding
                    };
                    if (_hasHeaderRecord)
                    {
                        configuration.PrepareHeaderForMatch = configMatch => configMatch.Header.ToLower();
                        configuration.HeaderValidated = headerValidated =>
                        {
                            List<string> errorTrace = new List<string>();
                            Type type = typeof(T);
                            PropertyInfo[] properties = type.GetProperties();
                            string[] readedHeader = headerValidated.Context.Reader.HeaderRecord;
                            bool validHeader = true;
                            if (readedHeader != null && readedHeader.Length > 0 && readedHeader.Length == properties.Length)
                            {
                                foreach (PropertyInfo item in properties)
                                {
                                    List<CustomAttributeData> property = item.CustomAttributes.ToList();
                                    if (property.Count == 2)
                                    {
                                        CustomAttributeData columnIndex = property.FirstOrDefault(i => i.AttributeType.Name.Contains("Index"));
                                        int index = (int)columnIndex.ConstructorArguments.First().Value;
                                        CustomAttributeData columnName = property.FirstOrDefault(i => i.AttributeType.Name.Contains("Name"));
                                        string name = (string)columnName.ConstructorArguments.First().Value;
                                        if (!Compare.StringEqual(readedHeader[index], name))
                                        {
                                            validHeader = false;
                                            errorTrace.Add(string.Format("La columna en la posicion {0} no coincide con la definida en el funcional | Columna esperada: {1} - Columna encontrada: {2}", index + 1, name, readedHeader[index]));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                validHeader = false;
                                string expected = properties.Select(c => c.CustomAttributes.Where(n => n.AttributeType.Name.Contains("Name")).FirstOrDefault().ConstructorArguments.First().Value.ToString()).Aggregate((a, b) => a + ", " + b);
                                string found = readedHeader.Aggregate((a, b) => a + ", " + b);
                                errorTrace.Add("El archivo no tiene la misma cantidad de columnas a las definidas en el funcional || Se esperaba: " + expected + " - se encontró: " + found);
                            }
                            if (!validHeader)
                            {
                                string mensajes = errorTrace.Aggregate((a, b) => a + "\n" + b);
                                throw new Exception("ERROR AL LEER EL ARCHIVO || " + mensajes);
                            }
                        };
                    }
                    using (var csvReader = new CsvReader(streamReader, configuration))
                    {
                        foreach (T item in csvReader.GetRecords<T>())
                        {
                            records.Add(item);
                        }
                    }
                }
            }
            return records;
        }
    }
}
