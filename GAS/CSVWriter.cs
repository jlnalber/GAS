using System.IO;

namespace GAS
{
    public class CSVWriter
    {
        public string[,] Table;
        public int Columns { get; }
        public int Rows { get; }

        public CSVWriter(int columns, int rows)
        {
            this.Table = new string[columns, rows];
            this.Columns = columns;
            this.Rows = rows;
        }

        public string this[int column, int row]
        {
            get
            {
                return this.Table[column, row];
            }
            set
            {
                this.Table[column, row] = value;
            }
        }

        public void Save(string path)
        {
            StreamWriter streamWriter = new(path);

            for (int i = 0; i < this.Rows; i++)
            {
                string line = "";
                for (int j = 0; j < this.Columns; j++)
                {
                    if (j != 0)
                    {
                        line += ";";
                    }
                    line += "\"" + this.Table[j, i] + "\"";
                }
                streamWriter.WriteLine(line);
            }

            streamWriter.Close();
        }
    }
}
