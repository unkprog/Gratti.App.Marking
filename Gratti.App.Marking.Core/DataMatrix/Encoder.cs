using System;

namespace Gratti.App.Marking.Core.DataMatrix
{
    public partial class Encoder
    {
        private bool autoSize = true;

        private bool autoSquareSize = true;

        private int[] matrix = new int[17424];

        private int size;

        private int[] CI_CONST_ARRAY_1 =
        {
            10, 12, 14, 16, 18, 20, 22, 24, 26, 32, 36, 40, 44, 48, 52, 64, 72, 80, 88, 96, 104, 120, 132, 144, 18, 32, 26, 36, 36, 48
        };

        private int[] CI_CONST_ARRAY_2 =
        {
            10, 12, 14, 16, 18, 20, 22, 24, 26, 32, 36, 40, 44, 48, 52, 64, 72, 80, 88, 96, 104, 120, 132, 144, 8, 8, 12, 12, 16, 16
        };

        private int[] CI_CONST_ARRAY_3 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 4, 6, 6, 6, 1, 2, 1, 2, 2, 2 };

        private int[] CI_CONST_ARRAY_4 = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4, 4, 6, 6, 6, 1, 1, 1, 1, 1, 1 };

        private int[] CI_CONST_ARRAY_5 =
        {
            3, 5, 8, 12, 18, 22, 30, 36, 44, 62, 86, 114, 144, 174, 204, 280, 368, 456, 576, 696, 816, 1050, 1304, 1558, 5, 10, 16, 22,
            32, 49
        };

        private int[] CI_CONST_ARRAY_6 =
        {
            5, 7, 10, 12, 14, 18, 20, 24, 28, 36, 42, 48, 56, 68, 42, 56, 36, 48, 56, 68, 56, 68, 62, 62, 7, 11, 14, 18, 24, 28
        };

        private int[] CI_CONST_ARRAY_7 =
        {
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 4, 4, 4, 4, 6, 6, 8, 10, 1, 1, 1, 1, 1, 1
        };

        private int MAX_MESSAGE_LENGTH = 3116;

        public int[] Encode(string message)
        {
            var codewords = new int[6232];
            var messageCodewords = this.EncodeMessage(message, codewords);
            if (this.autoSize)
            {
                this.FitSize(messageCodewords);
            }
            else if (messageCodewords > this.CI_CONST_ARRAY_5[this.size])
            {
                throw new ArgumentException("Message too long for this symbol size");
            }
            this.PadCodewords(messageCodewords, codewords);
            this.CalculateReedSolomonCode(codewords);
            this.FillMatrix(codewords);

            return codewords;
        }

        public int GetColumns()
        {
            return this.CI_CONST_ARRAY_1[this.size];
        }

        public int GetRows()
        {
            return this.CI_CONST_ARRAY_2[this.size];
        }

        public int GetModule(int column, int row)
        {
            var symbolColumns = this.CI_CONST_ARRAY_1[this.size];
            var symbolRows = this.CI_CONST_ARRAY_2[this.size];
            if (column < 0 || column >= symbolColumns || row < 0 || row >= symbolRows)
            {
                throw new Exception("Coordinates out of bounds");
            }
            var regionColumns = Math.Floor((double)symbolColumns / this.CI_CONST_ARRAY_3[this.size]);
            var regionRows = Math.Floor((double)symbolRows / this.CI_CONST_ARRAY_4[this.size]);
            if ((row + 1) % regionRows == 0 || column % regionColumns == 0)
            {
                return 1;
            }

            if (row % regionRows == 0)
            {
                return ~column & 1;
            }

            if ((column + 1) % regionColumns == 0)
            {
                return row & 1;
            }

            column -= 1 + (int)Math.Floor(column / regionColumns) * 2;
            row -= 1 + (int)Math.Floor(row / regionRows) * 2;
            return this.matrix[row * 132 + column];
        }

        private void CalculateReedSolomonCode(int[] codewords)
        {
            var exp = new int[511];
            var log = new int[256];
            var e = 1;
            for (var x = 0; x < 256; x++)
            {
                exp[255 + x] = exp[x] = e;
                log[e] = x;
                e <<= 1;
                if (e >= 256)
                {
                    e ^= 301;
                }
            }

            var dataCodewords = this.CI_CONST_ARRAY_5[this.size];
            var errorCodewords = this.CI_CONST_ARRAY_6[this.size];
            var poly = new int[69];
            poly[0] = 1;
            for (var i = 1; i <= errorCodewords; i++)
            {
                poly[i] = poly[i - 1];
                for (var j = i - 1; j >= 0; j--)
                {
                    var k = exp[log[poly[j]] + i];
                    if (j > 0)
                    {
                        k ^= poly[j - 1];
                    }

                    poly[j] = k;
                }
            }

            for (var i = 0; i < errorCodewords; i++)
            {
                poly[i] = log[poly[i]];
            }

            var blocks = this.CI_CONST_ARRAY_7[this.size];

            var totalCodewords = dataCodewords + blocks * errorCodewords;

            for (var i = dataCodewords; i < totalCodewords; i++)
            {
                codewords[i] = 0;
            }

            for (var block = 0; block < blocks; block++)
            {
                for (var i = block; i < dataCodewords; i += blocks)
                {
                    var j = dataCodewords + block;
                    var k = codewords[j] ^ codewords[i];
                    for (var l = errorCodewords; --l >= 0; j += blocks)
                    {
                        var checkword = k == 0 ? 0 : exp[log[k] + poly[l]];
                        if (j + blocks < totalCodewords)
                        {
                            checkword ^= codewords[j + blocks];
                        }

                        codewords[j] = checkword;
                    }
                }
            }
        }

        private int EncodeMessage(string message, int[] codewords)
        {
            var messageLength = message.Length;
            if (messageLength > this.MAX_MESSAGE_LENGTH)
            {
                throw new ArgumentException("Message too long");
            }

            var j = 0;
            for (var i = 0; i < messageLength; i++)
            {
                var c = message[i];
                if (c >= 128)
                {
                    if (c >= 256)
                    {
                        throw new NotSupportedException("Unsupported Unicode character");
                    }

                    codewords[j++] = 235;
                    codewords[j++] = c - 127;
                    continue;
                }

                if (c >= 48 && c <= 57 && i + 1 < messageLength)
                {
                    var d = message[i + 1];
                    if (d >= 48 && d <= 57)
                    {
                        i++;
                        codewords[j++] = 130 + (c - 48) * 10 + d - 48;
                        continue;
                    }
                }

                codewords[j++] = c + 1;
            }

            return j;
        }

        private void FillMatrix(int[] codewords)
        {
            var matrixColumns = this.CI_CONST_ARRAY_1[this.size] - this.CI_CONST_ARRAY_3[this.size] * 2;
            var matrixRows = this.CI_CONST_ARRAY_2[this.size] - this.CI_CONST_ARRAY_4[this.size] * 2;
            for (var i = 0; i < matrixRows; i++)
            {
                for (var j = 0; j < matrixColumns; j++)
                {
                    this.matrix[i * 132 + j] = 2;
                }
            }

            var codewordsIndex = 0;
            var column = 0;
            var row = 4;
            do
            {
                if (column == 0)
                {
                    if (row == matrixRows)
                    {
                        this.SetCorner1(matrixColumns, matrixRows, codewords[codewordsIndex++]);
                    }
                    else if (row == matrixRows - 2)
                    {
                        if ((matrixColumns & 3) != 0)
                        {
                            this.SetCorner2(matrixColumns, matrixRows, codewords[codewordsIndex++]);
                        }
                        else if ((matrixColumns & 7) == 4)
                        {
                            this.SetCorner3(matrixColumns, matrixRows, codewords[codewordsIndex++]);
                        }
                    }
                }
                else if (column == 2 && row == matrixRows + 4 && (matrixColumns & 7) == 0)
                {
                    this.SetCorner4(matrixColumns, matrixRows, codewords[codewordsIndex++]);
                }

                do
                {
                    if (row < matrixRows && this.matrix[row * 132 + column] == 2)
                    {
                        this.SetUtah(column, row, matrixColumns, matrixRows, codewords[codewordsIndex++]);
                    }

                    column += 2;
                    row -= 2;
                }
                while (column < matrixColumns && row >= 0);

                column += 3;
                row++;
                do
                {
                    if (column < matrixColumns && row >= 0 && this.matrix[row * 132 + column] == 2)
                    {
                        this.SetUtah(column, row, matrixColumns, matrixRows, codewords[codewordsIndex++]);
                    }

                    column -= 2;
                    row += 2;
                }
                while (column >= 0 && row < matrixRows);

                column++;
                row += 3;
            }
            while (column < matrixColumns || row < matrixRows);

            if (row == matrixRows + 6)
            {
                this.matrix[(matrixRows - 2) * 132 + matrixColumns - 2] = 1;
                this.matrix[(matrixRows - 2) * 132 + matrixColumns - 1] = 0;
                this.matrix[(matrixRows - 1) * 132 + matrixColumns - 2] = 0;
                this.matrix[(matrixRows - 1) * 132 + matrixColumns - 1] = 1;
            }
        }

        private void FitSize(int usedCodewords)
        {
            for (var i = this.autoSquareSize ? 0 : 24; i < 30; i++)
            {
                if (this.CI_CONST_ARRAY_5[i] >= usedCodewords)
                {
                    this.size = i;
                    return;
                }
            }

            throw new ArgumentException("Message too long");
        }

        private void PadCodewords(int usedCodewords, int[] codewords)
        {
            var dataCodewords = this.CI_CONST_ARRAY_5[this.size];
            if (usedCodewords < dataCodewords)
            {
                codewords[usedCodewords++] = 129;
                while (usedCodewords < dataCodewords)
                {
                    var pad = 130 + (usedCodewords + 1) * 149 % 253;
                    if (pad > 254)
                    {
                        pad -= 254;
                    }

                    codewords[usedCodewords++] = pad;
                }
            }
        }

        private void SetModule(int column, int row, int value)
        {
            this.matrix[row * 132 + column] = value & 1;
        }

        private void SetCorner1(int matrixColumns, int matrixRows, int value)
        {
            this.SetModule(0, matrixRows - 1, value >> 7);
            this.SetModule(1, matrixRows - 1, value >> 6);
            this.SetModule(2, matrixRows - 1, value >> 5);
            this.SetModule(matrixColumns - 2, 0, value >> 4);
            this.SetModule(matrixColumns - 1, 0, value >> 3);
            this.SetModule(matrixColumns - 1, 1, value >> 2);
            this.SetModule(matrixColumns - 1, 2, value >> 1);
            this.SetModule(matrixColumns - 1, 3, value);
        }

        private void SetCorner2(int matrixColumns, int matrixRows, int value)
        {
            this.SetModule(0, matrixRows - 3, value >> 7);
            this.SetModule(0, matrixRows - 2, value >> 6);
            this.SetModule(0, matrixRows - 1, value >> 5);
            this.SetModule(matrixColumns - 4, 0, value >> 4);
            this.SetModule(matrixColumns - 3, 0, value >> 3);
            this.SetModule(matrixColumns - 2, 0, value >> 2);
            this.SetModule(matrixColumns - 1, 0, value >> 1);
            this.SetModule(matrixColumns - 1, 1, value);
        }

        private void SetCorner3(int matrixColumns, int matrixRows, int value)
        {
            this.SetModule(0, matrixRows - 3, value >> 7);
            this.SetModule(0, matrixRows - 2, value >> 6);
            this.SetModule(0, matrixRows - 1, value >> 5);
            this.SetModule(matrixColumns - 2, 0, value >> 4);
            this.SetModule(matrixColumns - 1, 0, value >> 3);
            this.SetModule(matrixColumns - 1, 1, value >> 2);
            this.SetModule(matrixColumns - 1, 2, value >> 1);
            this.SetModule(matrixColumns - 1, 3, value);
        }

        private void SetCorner4(int matrixColumns, int matrixRows, int value)
        {
            this.SetModule(0, matrixRows - 1, value >> 7);
            this.SetModule(matrixColumns - 1, matrixRows - 1, value >> 6);
            this.SetModule(matrixColumns - 3, 0, value >> 5);
            this.SetModule(matrixColumns - 2, 0, value >> 4);
            this.SetModule(matrixColumns - 1, 0, value >> 3);
            this.SetModule(matrixColumns - 3, 1, value >> 2);
            this.SetModule(matrixColumns - 2, 1, value >> 1);
            this.SetModule(matrixColumns - 1, 1, value);
        }

        private void SetModuleWrapped(int column, int row, int matrixColumns, int matrixRows, int value)
        {
            if (row < 0)
            {
                row += matrixRows;
                column += 4 - (matrixRows + 4 & 7);
            }

            if (column < 0)
            {
                column += matrixColumns;
                row += 4 - (matrixColumns + 4 & 7);
            }

            this.SetModule(column, row, value);
        }

        private void SetUtah(int column, int row, int matrixColumns, int matrixRows, int value)
        {
            this.SetModuleWrapped(column - 2, row - 2, matrixColumns, matrixRows, value >> 7);
            this.SetModuleWrapped(column - 1, row - 2, matrixColumns, matrixRows, value >> 6);
            this.SetModuleWrapped(column - 2, row - 1, matrixColumns, matrixRows, value >> 5);
            this.SetModuleWrapped(column - 1, row - 1, matrixColumns, matrixRows, value >> 4);
            this.SetModuleWrapped(column, row - 1, matrixColumns, matrixRows, value >> 3);
            this.SetModuleWrapped(column - 2, row, matrixColumns, matrixRows, value >> 2);
            this.SetModuleWrapped(column - 1, row, matrixColumns, matrixRows, value >> 1);
            this.SetModuleWrapped(column, row, matrixColumns, matrixRows, value);
        }
    }
}
