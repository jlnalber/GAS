﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticFramework;
using System.IO;

namespace GAS
{
    public class CSVReader
    {
        public readonly string[,] Table;

        public CSVReader(string path)
        {
            StreamReader streamReader = new(path);
            List<string> lines = new();
            while (!streamReader.EndOfStream)
            {
                lines.Add(streamReader.ReadLine());
            }

            string[][] csv = (from line in lines select line.Split(';').ToArray().Modify((string s) => s.Replace("\"", ""))).ToArray();

            this.Table = new string[csv[0].Length, csv.Length];

            for (int i = 0; i < csv.Length; i++)
            {
                for (int j = 0; j < csv[0].Length; j++)
                {
                    this.Table[j, i] = csv[i][j];
                }
            }
        }

        public string this[int column, int row]
        {
            get
            {
                return this.Table[column, row];
            }
        }

        public int Columns
        {
            get
            {
                return this.Table.GetLength(0);
            }
        }

        public int Rows
        {
            get
            {
                return this.Table.GetLength(1);
            }
        }
    }
}
