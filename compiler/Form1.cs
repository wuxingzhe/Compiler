using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace compiler
{


    //public enum codeF{lit, opr, lod,sto, cal, inte,jmp, jpc,fre, sta, loa};

    public struct pCode
    {
        public int f;
        public int l;
        public int a;

    };

    public struct Item
    {
        public string name;
        public int type;
        public int val;
        public int level;
        public int addr;
        public int Size;
        public bool ret;                                 
        public int paarNum;                           
    };

    public struct words
    {
        public string name;
        public int type;
        public int line;
    }
    public partial class Form1 : Form
    {
        /*0到15表示保留字，16表示标识符，17表示常数，18表示+，19表示-，20表示*，21表示
         * /、22表示：=23表示<=,24表示>=，25表示=，26表示<，27表示>,28表示.,29表示， 30表示；
         * 31表示（，32表示）
         */

        private string[] codeF;
        private words[] wordList;
        private pCode[] code;
        private Item[] sign;

        private int usedNum;
        private int lineId;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        /*private void errorDeal(int lineNum,char errorCh)
        {
            textBox2.Text = lineNum.ToString();
            textBox3.Text = errorCh.ToString();
            //textBox4.Text = "You input a tan-90 character!!";
 
        }*/

        private void insertWordList(int type, string name, int line)
        {
            wordList[wordNum].type = type;
            wordList[wordNum].name = name;
            wordList[wordNum++].line = line;
        }

        private bool wordJudge(string str, int lineNum)
        {
            int len = str.Length;
            bool numFlag = false;
            bool chFlag = false;
            bool dotFlag = false;
            for (int i = 0; i < len; i++)
            {
                if (str[i] >= '0' && str[i] <= '9')
                    numFlag = true;
                else if (str[i] >= 'a' && str[i] <= 'z')
                    chFlag = true;
                else if (str[i] >= 'A' && str[i] <= 'Z')
                    chFlag = true;
                else if (str[i] == '.')
                    dotFlag = true;
            }

            if (chFlag)
            {
                if (str[0] >= '0' && str[0] <= '9')
                {
                    //errorDeal(lineNum, str[0]);
                    textBox1.Text += "第";
                    textBox1.Text += lineNum.ToString();
                    textBox1.Text += " 行 字符 ";
                    textBox1.Text += str;
                    textBox1.Text += "  ";
                    textBox1.Text = "You input a wrong identifier!!";
                    textBox1.Text += Environment.NewLine;
                    return false;
                }
                else if (dotFlag)
                {
                    //errorDeal(lineNum, str[0]);
                    textBox1.Text += "第";
                    textBox1.Text += lineNum.ToString();
                    textBox1.Text += " 行 字符 ";
                    textBox1.Text += str;
                    textBox1.Text += "  ";
                    textBox1.Text = "You input a wrong identifier!!";
                    textBox1.Text += Environment.NewLine;
                    return false;
                }
                else
                {
                    bool tt = false;
                    for (int i = 0; i < 16; i++)
                    {
                        if (string.Compare(keyWord[i], str) == 0)
                        {
                            tt = true;
                            /*wordFlow[total] = str;
                            kindFlow[total] = "关键字";*/

                            wordText += str;
                            if (str.Length <= 12)
                            {
                                int count = 12 - str.Length;
                                for (int j = 0; j < count; j++)
                                    wordText += " ";
                            }

                            wordText += "  ";
                            wordText += "关键字";
                            wordText += "        ";
                            wordText += str;
                            wordText += Environment.NewLine;
                            insertWordList(i, str, lineNum);
                            return true;


                        }
                    }

                    wordText += str;
                    if (str.Length <= 12)
                    {
                        int count = 12 - str.Length;
                        for (int i = 0; i < count; i++)
                            wordText += " ";
                    }

                    wordText += "  ";
                    wordText += "标识符";
                    wordText += "        ";
                    wordText += str;
                    wordText += Environment.NewLine;
                    insertWordList(16, str, lineNum);
                    return true;
                }
            }
            else
            {
                wordText += str;
                /*wordText += " ";
                wordText += str.Length.ToString();*/
                if (str.Length <= 12)
                {
                    int count = 12 - str.Length;
                    for (int i = 0; i < count; i++)
                        wordText += " ";
                }

                wordText += "  ";
                if (dotFlag)
                {
                    wordText += "浮点数";
                    wordText += "        ";
                    byte[] bArray = BitConverter.GetBytes(Double.Parse(str));
                    string sBin = string.Empty;
                    foreach (byte b in bArray)
                    {
                        sBin += Convert.ToString(b, 2);
                    }
                    wordText += str;
                    //wordText += "（二进制）";
                }
                else
                {
                    wordText += "常量";
                    wordText += "          ";
                    wordText += Convert.ToString(Int32.Parse(str), 2);
                    wordText += "（二进制）";
                    insertWordList(17, str, lineNum);
                }
                wordText += Environment.NewLine;
                return true;
            }

        }

        private bool wordCompile(string str, int lineNum)
        {
            /*string[] words = str.Split(' ');
            int len=0;
            char ch = ' ';
            foreach(string word in words)
            {
                len = word.Length;
                for(int i=0;i<len;i++)
                {
                    ch = word[i];
                    if (ch == ' ')
                        continue;
                    else if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == ':' || ch == ';' || ch == ',' || ch == '=' || ch == '(' || ch == ')' || ch == '%' || ch == '&' || ch == '!' || ch == '|' || ch == '#' || ch == '{' || ch == '}' || ch == '<' || ch == '>' || ch == '~')
                }
            }*/
            bool check = true;
            string tmp = "";
            int len = str.Length;
            bool assign = false;
            bool operatorFlag = false;
            bool greatFlag = false;

            for (int i = 0; i < len; i++)
            {
                char ch = str[i];
                if (ch == ' ' || ch == '\n' || ch == '\r' || ch == '\t')
                {
                    assign = false;
                    greatFlag = false;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        /*if (!cnt)
                            return false;*/
                        check = true;
                        tmp = "";
                    }
                }
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                {
                    assign = false;
                    greatFlag = false;
                    /*if(i==0||operatorFlag)
                    {
                        errorDeal(lineNum, ch);
                        textBox4.Text = "You input a tan90 operator!!";
                        //return false;
                    }*/
                    operatorFlag = true;

                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        /*if (!cnt)
                            return false;*/
                        check = true;
                        tmp = "";
                    }

                    wordText += ch.ToString();
                    wordText += "           ";
                    wordText += "  ";
                    wordText += "单字符运算符";
                    wordText += "  ";
                    wordText += ch.ToString();
                    wordText += Environment.NewLine;

                    int tt = 21;
                    if (ch == '+')
                        tt = 18;
                    else if (ch == '-')
                        tt = 19;
                    else if (ch == '*')
                        tt = 20;
                    insertWordList(tt, ch.ToString(), lineNum);
                }
                else if (ch == '=')
                {
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        /*if (!cnt)
                            return false;*/
                        check = true;
                        tmp = "";
                    }

                    if (i > 0 && str[i - 1] == ':')
                    {
                        wordText += ":=          ";
                        wordText += "  ";
                        wordText += "双字符运算符  ";
                        wordText += ":=";
                        wordText += Environment.NewLine;
                        assign = false;
                        insertWordList(22, ":=", lineNum);
                    }
                    else if (i > 0 && (str[i - 1] == '<' || str[i - 1] == '>'))
                    {
                        wordText += str[i - 1].ToString();
                        wordText += "=          ";
                        wordText += "  ";
                        wordText += "双字符运算符  ";
                        wordText += str[i - 1].ToString();
                        wordText += "=";
                        wordText += Environment.NewLine;
                        greatFlag = false;
                        if (str[i - 1] == '<')
                            insertWordList(23, "<=", lineNum);
                        else if (str[i - 1] == '>')
                            insertWordList(24, ">=", lineNum);
                    }
                    else
                    {
                        /*if(operatorFlag)
                        {
                            errorDeal(lineNum, ch);
                            //textBox4.Text = "You input a tan90 operator!!";
                            //return false;
                        }*/

                        wordText += "=           ";
                        wordText += "  ";
                        wordText += "单字符运算符  ";
                        wordText += "=";
                        wordText += Environment.NewLine;
                        insertWordList(25, "=", lineNum);
                    }

                    operatorFlag = true;
                }
                else if (ch == ':')
                {
                    if (i < len - 2 && str[i + 1] == '=')
                    {
                        assign = true;

                    }
                    else
                    {
                        //errorDeal(lineNum, ch);
                        textBox1.Text += "第";
                        textBox1.Text += lineNum.ToString();
                        textBox1.Text += " 行 字符 ";
                        textBox1.Text += ":";
                        textBox1.Text += "  ";
                        textBox1.Text = "You input a tan90 operator!!";
                        textBox1.Text += Environment.NewLine;
                        //return false;
                    }
                    operatorFlag = true;
                }
                else if (ch == '>' || ch == '<')
                {
                    /*if (i == 0 || operatorFlag)
                    {
                        errorDeal(lineNum, ch);
                        textBox4.Text = "You input a tan90 operator!!";
                        return false;
                    }*/
                    operatorFlag = true;

                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);
                        /*if (!cnt)
                            return false;*/
                        check = true;
                        tmp = "";
                    }

                    greatFlag = true;
                    if (i > 0 && str[i - 1] == '<' && str[i] == '>')
                    {
                        insertWordList(33, "<>", lineNum);
                    }
                    else if (i == len - 1 || (str[i + 1] != '=' && (str[i] == '>' || (str[i] == '<' && str[i + 1] != '>'))))
                    {
                        textBox1.Text = str[i + 1].ToString();
                        wordText += ch.ToString();
                        wordText += "           ";
                        wordText += "  ";
                        wordText += "单字符运算符";
                        wordText += "  ";
                        wordText += ch.ToString();
                        wordText += Environment.NewLine;

                        if (ch == '<')
                            insertWordList(26, "<", lineNum);
                        else
                            insertWordList(27, ">", lineNum);
                    }
                }
                else if (ch == '.')
                {
                    if (i == len - 1 || i == 0)
                    {
                        if (!check && tmp != "")
                        {
                            bool cnt = wordJudge(tmp, lineNum);

                            check = true;
                            tmp = "";
                        }

                        wordText += ch.ToString();
                        wordText += "           ";
                        wordText += "  ";
                        wordText += "分界符";
                        wordText += "        ";
                        wordText += ch.ToString();
                        wordText += Environment.NewLine;
                        insertWordList(28, ".", lineNum);
                    }

                    /*else if(str[i-1]>='0'&&str[i-1]<='9'&& str[i + 1] >= '0' && str[i + 1] <= '9')
                    {
                        tmp += ".";
                    }*/
                    else
                    {
                        if (!check && tmp != "")
                        {
                            bool cnt = wordJudge(tmp, lineNum);

                            check = true;
                            tmp = "";
                        }

                        wordText += ch.ToString();
                        wordText += "           ";
                        wordText += "  ";
                        wordText += "分界符";
                        wordText += "        ";
                        wordText += ch.ToString();
                        wordText += Environment.NewLine;
                        insertWordList(28, ".", lineNum);
                    }
                }
                else if (ch == ',' || ch == ';' || ch == '(' || ch == ')')
                {
                    operatorFlag = true;
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);

                        check = true;
                        tmp = "";
                    }

                    wordText += ch.ToString();
                    wordText += "           ";
                    wordText += "  ";
                    wordText += "分界符";
                    wordText += "        ";
                    wordText += ch.ToString();
                    wordText += Environment.NewLine;

                    if (ch == ',')
                        insertWordList(29, ",", lineNum);
                    else if (ch == ';')
                        insertWordList(30, ";", lineNum);
                    else if (ch == '(')
                        insertWordList(31, "(", lineNum);
                    else
                        insertWordList(32, ")", lineNum);
                }
                else if (ch >= '0' && ch <= '9')
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else if (ch >= 'a' && ch <= 'z')
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else if (ch >= 'A' && ch <= 'Z')
                {
                    tmp += ch.ToString();
                    check = false;
                    assign = false;
                    operatorFlag = false;
                    greatFlag = false;
                }
                else
                {
                    //errorDeal(lineNum, ch);
                    textBox1.Text += "第";
                    textBox1.Text += lineNum.ToString();
                    textBox1.Text += " 行 字符 ";
                    textBox1.Text += ch.ToString();
                    textBox1.Text += "  ";
                    textBox1.Text = "you input a tan90 character!!";
                    textBox1.Text += Environment.NewLine;
                    //return false;
                }

                if (i == len - 1)
                {
                    if (!check && tmp != "")
                    {
                        bool cnt = wordJudge(tmp, lineNum);

                        check = true;
                        tmp = "";
                    }
                }
            }

            return true;
        }

        void dealError(int id, int lineId, string name)
        {
            textBox1.Text += errorNum;
            textBox1.Text += " ";
            textBox1.Text += "第";
            textBox1.Text += lineId.ToString();
            textBox1.Text += "行： 字符 ";
            textBox1.Text += name;
            textBox1.Text += ":  ";
            switch (id)
            {
                case 1:
                    textBox1.Text += "应该用：=而不是=";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 2:
                    textBox1.Text += "\"=\"后应该是整数.";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 3:
                    textBox1.Text += "标识符后缺少\"=\".  ";
                    break;
                case 4:
                    textBox1.Text += "const ，var ，procedure后应为标识符.  ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 5:
                    textBox1.Text += "漏掉了\",\"或者是\";\".  ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 6:
                    textBox1.Text += "过程说明后的符号不正确";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 7:
                    textBox1.Text += "声明顺序有误，应为[<变量说明部分>][<常量说明部分>] [<过程说明部分>]<语句>。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 8:
                    textBox1.Text += "程序体内的语句部分的符不正确";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 9:
                    textBox1.Text += "程序的末尾丢掉了句号\".\" ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 10:
                    textBox1.Text += "句子之间漏掉了\";\"。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 11:
                    textBox1.Text += "标识符未说明";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 12:
                    textBox1.Text += "赋值号左端应为变量";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 13:
                    textBox1.Text += "应为赋值运算符\":=\"";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 14:
                    textBox1.Text += "call 后应为标识符 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 15:
                    textBox1.Text += "call不能调用常量或者变量 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 16:
                    textBox1.Text += "应该为\"then\"。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 17:
                    textBox1.Text += "缺少\"end\"或\";\"。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 18:
                    textBox1.Text += "do while 型循环语句缺少while 。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 19:
                    textBox1.Text += "语句后的标号不正确。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 20:
                    textBox1.Text += "应为关系运算符 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 21:
                    textBox1.Text += "表达式中不能有过程标识符 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 22:
                    textBox1.Text += "表达式中漏掉右括号";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 23:
                    textBox1.Text += "非法符号。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 24:
                    textBox1.Text += "表达式不能用这个符号开头 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 25:
                    textBox1.Text += "运算符的后边是常量。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 26:
                    textBox1.Text += "不存在此操作符 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 27:
                    textBox1.Text += "变量定义的长度应为常量或者是常数 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 28:
                    textBox1.Text += "变量定义重复 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 29:
                    textBox1.Text += "未找到对应过程名 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 30:
                    textBox1.Text += "不支持过程的判断 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 31:
                    textBox1.Text += "这个数太大";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 32:
                    textBox1.Text += "read 语句括号中的标识符不是变量。 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 33:
                    textBox1.Text += "缺少标识符，无法进行条件判断";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 34:
                    textBox1.Text += "read内应当为变量值 ";
                    textBox1.Text += Environment.NewLine;
                    break;

                case 35:
                    textBox1.Text += "此处不应该出现过程说明标识符";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 36:
                    textBox1.Text += "缺少until";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 37:
                    textBox1.Text += "read括号内应该是变量标识符";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 41:
                    textBox1.Text += "此处应该为标识符 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 42:
                    textBox1.Text += "常量说明部分出现在错误位置 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 43:
                    textBox1.Text += "常量说明部分多于一个 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 44:
                    textBox1.Text += "变量说明部分出现在错误位置 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 45:
                    textBox1.Text += "变量说明部分多于一个 ";
                    textBox1.Text += Environment.NewLine;
                    break;
                case 40:
                    textBox1.Text += "应为左括号";
                    textBox1.Text += Environment.NewLine;
                    break;
                default:
                    textBox1.Text += " ";
                    textBox1.Text += Environment.NewLine;
                    break;
            }
            isError = true;
            errorNum++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init();
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            richTextBox1.Text = "";
            textBox5.Text = "";
            textBox1.Text = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "../../";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            string str = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                str += openFileDialog.FileName.ToString();
            else
                MessageBox.Show("请选择文件");
            try
            {
                StreamReader sR = new StreamReader(str);
                string nextLine;
                int lineNum = 1;
                bool cnt = true;
                while ((nextLine = sR.ReadLine()) != null)
                {
                    Console.WriteLine(nextLine);
                    cnt = wordCompile(nextLine, lineNum);
                    /*if (!cnt)
                        break;*/
                    lineNum++;
                }

                /*if (cnt)
                    textBox5.Text = wordText;*/
                sR.Close();

                if (getsym())
                    block(ref follow, 0, ref tx);

                if (sym != 28)
                    dealError(9, linePos, ss);

                /*for (int i = 0; i < cx; i++)
                {
                    if (code[i].f == 20)
                        continue;
                    textBox5.Text += (i + 1).ToString();
                    textBox5.Text += " ";
                    textBox5.Text += codeF[code[i].f];
                    textBox5.Text += " ";
                    textBox5.Text += code[i].l.ToString();
                    textBox5.Text += " ";
                    textBox5.Text += code[i].a.ToString();
                    textBox5.Text += Environment.NewLine;
                }*/
                if (!isError) isCompile = true;
                else
                    isCompile = false;
            }
            catch (IOException)
            {
                MessageBox.Show("The file not exits!!");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private string wordText;
        private string[] keyWord;
        private string[] wordFlow;
        private string[] kindFlow;
        private string[] valueFlow;
        private int total;

        //first集合
        private bool[] decPre;
        private bool[] statePre;
        private bool[] factorPre;

        //程序是否有错误
        private bool isError;
        private bool isCompile;

        private int pcodeNum;

        private int wordNum;

        private int sym;
        private string ss;
        private int symNum;

        private bool[] follow;

        private int cx;
        private int tx;

        private int ttop;
        private string[] input;

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private bool gen(int x, int y, int z)
        {
            if (pcodeNum >= 1005)
            {

                textBox1.Text = "Program too long";
                return false;
            }
            code[pcodeNum].f = x;
            code[pcodeNum].l = y;
            code[pcodeNum].a = z;
            pcodeNum++;
            cx++;
            return true;
        }



        public void init()
        {
            keyWord = new string[20];
            wordFlow = new string[20005];
            kindFlow = new string[20005];
            valueFlow = new string[20005];
            codeF = new string[15];
            wordList = new words[2005];
            sign = new Item[2005];
            code = new pCode[2005];
            follow = new bool[55];
            decPre = new bool[55];
            statePre = new bool[55];
            factorPre = new bool[55];
            

            total = 0;
            pcodeNum = 0;
            wordNum = 0;
            usedNum = 0;
            cx = 0;
            errorNum = 0;
            symNum = 55;
            tx = 0;
            ttop = 0;
            isError = false;
            isCompile = false;

            //wordText = "单词          类别          值";
            //wordText += Environment.NewLine;
            keyWord[0] = "const";
            keyWord[1] = "var";
            keyWord[2] = "procedure";
            keyWord[3] = "odd";
            keyWord[4] = "if";
            keyWord[5] = "then";
            keyWord[6] = "else";
            keyWord[7] = "while";
            keyWord[8] = "do";
            keyWord[9] = "call";
            keyWord[10] = "begin";
            keyWord[11] = "end";
            keyWord[12] = "repeat";
            keyWord[13] = "until";
            keyWord[14] = "read";
            keyWord[15] = "write";

            codeF[0] = "lit";
            codeF[1] = "opr";
            codeF[2] = "lod";
            codeF[3] = "sto";
            codeF[4] = "cal";
            codeF[5] = "int";
            codeF[6] = "jmp";
            codeF[7] = "jpc";
            codeF[8] = "wrt";
            codeF[9] = "red";
            //codeF[10] = "loa";

            for (int i = 0; i < 55; i++)
                decPre[i] = false;
            decPre[0] = true;
            decPre[1] = true;
            decPre[2] = true;

            for (int i = 0; i < 55; i++)
                factorPre[i] = false;
            factorPre[16] = true;
            factorPre[17] = true;
            factorPre[31] = true;

            for (int i = 0; i < 55; i++)
                statePre[i] = false;
            statePre[4] = true;
            statePre[7] = true;
            statePre[9] = true;
            statePre[10] = true;
            statePre[12] = true;
            statePre[16] = true;

            for (int i = 0; i < symNum; i++)
                follow[i] = false;
            for (int i = 0; i < 55; i++)
            {
                if (decPre[i])
                    follow[i] = true;
                if (statePre[i])
                    follow[i] = true;
            }
            follow[28] = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private int linePos;

        private bool getsym()
        {
            if (usedNum > wordNum - 1)
                return false;
            ss = wordList[usedNum].name;
            linePos = wordList[usedNum].line;
            sym = wordList[usedNum++].type;
            return true;
        }



        private bool block(ref bool[] fs, int level, ref int tx)
        {
            int dx = 3;
            int ttmp = tx;
            sign[tx].addr = cx;
            if (!gen(6, 0, 0))
                return false;
            bool[] foll = new bool[symNum];

            if (level > 5)
            {
                dealError(32, linePos, ss);
                return false;
            }

            int varNum = 0, conNum = 0, proNum = 0;

            do
            {
                if (sym == 1)
                {
                    if (proNum > 0)
                        dealError(44, linePos, ss);
                    if (varNum > 0)
                        dealError(45, linePos, ss);
                    varNum++;

                    if (!getsym())
                        return false;

                    vardeclaration(ref tx, level, ref dx);
                    while (sym == 29)
                    {
                        if (!getsym())
                            return false;
                        vardeclaration(ref tx, level, ref dx);
                    }
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);

                }
                else if (sym == 0)
                {
                    if (varNum > 0 || proNum > 0)
                        dealError(42, linePos, ss);
                    if (conNum > 0)
                        dealError(43, linePos, ss);
                    conNum++;

                    if (!getsym())
                        return false;
                    constdeclaration(ref tx, level, ref dx);

                    while (sym == 29)
                    {
                        if (!getsym())
                            return false;
                        constdeclaration(ref tx, level, ref dx);
                    }

                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);
                }

                while (sym == 2)
                {
                    proNum++;
                    if (!getsym())
                        return false;
                    int tdx = -1;
                    if (sym == 16)
                    {
                        enter(2, ref tx, level, ref dx, 1, ss);
                        tdx = tx;
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(4, linePos, ss);
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(5, linePos, ss);

                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    foll[30] = true;
                    if (!block(ref foll, level + 1, ref tx))
                        return false;

                    if (sym == 30)
                        getsym();
                    else
                        dealError(5, linePos, ss);

                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    foll[16] = true;
                    foll[2] = true;
                    if (!test(ref foll, ref fs, 6))
                        return false;
                    //if()
                }
            }
            while (decPre[sym]);

            code[sign[ttmp].addr].a = cx;
            sign[ttmp].addr = cx;
            sign[ttmp].Size = dx;

            gen(5, 0, dx);
            textBox2.Text += "符号表如下：";
            textBox2.Text += Environment.NewLine;
            if (ttmp >= tx)
            {
                textBox2.Text += "NULL";
                textBox2.Text += Environment.NewLine;
            }
            else
            {
                for (int i = ttmp + 1; i <= tx; i++)
                {
                    textBox2.Text += i.ToString();
                    textBox2.Text += ": ";
                    switch (sign[i].type)
                    {
                        case 0:
                            textBox2.Text += "const ";
                            textBox2.Text += sign[i].name;
                            textBox2.Text += " val= ";
                            textBox2.Text += (sign[i].val).ToString();
                            textBox2.Text += Environment.NewLine;
                            break;
                        case 1:
                            textBox2.Text += "var ";
                            textBox2.Text += sign[i].name;
                            textBox2.Text += " level= ";
                            textBox2.Text += (sign[i].level).ToString();
                            textBox2.Text += " addr= ";
                            textBox2.Text += (sign[i].addr).ToString();
                            textBox2.Text += Environment.NewLine;
                            break;
                        case 2:
                            textBox2.Text += "proc ";
                            textBox2.Text += sign[i].name;
                            textBox2.Text += " level= ";
                            textBox2.Text += (sign[i].level).ToString();
                            textBox2.Text += " addr= ";
                            textBox2.Text += (sign[i].addr).ToString();
                            textBox2.Text += " size= ";
                            textBox2.Text += (sign[i].Size).ToString();
                            textBox2.Text += Environment.NewLine;
                            break;
                    }
                }


            }

            for (int i = 0; i < symNum; i++)
                foll[i] = fs[i];
            foll[30] = true;
            foll[11] = true;
            statement(ref foll, ref tx, level);
            gen(1, 0, 0);
            for (int i = 0; i < symNum; i++)
                foll[i] = false;
            if (!test(ref fs, ref foll, 8))
                return false;

            

            tx = ttmp;
            return true;
        }

        private void interpret()
        {
            int[] sta = new int[305];
            for (int i = 0; i < 305; i++)
                sta[i] = 0;
            int pc = 0, bp = 0, sp = 0;

            do
            {

                pCode currentCode = code[pc++];
                switch (currentCode.f)
                {
                    case 0:
                        sta[sp++] = currentCode.a;
                        break;
                    case 1:
                        switch (currentCode.a)
                        {
                            case 0:
                                sp = bp;
                                pc = sta[sp + 2];
                                bp = sta[sp + 1];
                                break;
                            case 1:
                                sta[sp - 1] = -sta[sp - 1];
                                break;
                            case 2:
                                sp--;
                                sta[sp - 1] += sta[sp];
                                break;
                            case 3:
                                sp--;
                                sta[sp - 1] -= sta[sp];
                                break;
                            case 4:
                                sp--;
                                sta[sp - 1] = sta[sp - 1] * sta[sp];
                                break;
                            case 5:
                                sp--;
                                sta[sp - 1] /= sta[sp];
                                break;
                            case 6:
                                sta[sp - 1] %= 2;
                                break;
                            case 7:
                                sp--;
                                sta[sp - 1] %= sta[sp];
                                break;
                            case 8:                                                             //OPR 0 8;==判断相等
                                sp--;
                                sta[sp - 1] = (sta[sp] == sta[sp - 1] ? 1 : 0);
                                break;
                            case 9:                                                                //OPR 0 9;!=判断不相等
                                sp--;
                                sta[sp - 1] = (sta[sp] != sta[sp - 1] ? 1 : 0);
                                break;
                            case 10:                                                               //OPR 0 10;<判断小于
                                sp--;
                                sta[sp - 1] = (sta[sp-1] < sta[sp] ? 1 : 0);
                                break;
                            case 11:                                                                //OPR 0 11;>=判断大于等于
                                sp--;
                                sta[sp - 1] = (sta[sp-1] >= sta[sp] ? 1 : 0);
                                break;
                            case 12:                                                                //OPG 0 12;>判断大于
                                sp--;
                                sta[sp - 1] = (sta[sp-1] > sta[sp] ? 1 : 0);
                                break;
                            case 13:                                                                 //OPG 0 13;<=判断小于等于
                                sp--;
                                sta[sp - 1] = (sta[sp-1] <= sta[sp] ? 1 : 0);
                                break;
                               
                        }
                        break;
                    case 2:                                          //lod
                        sta[sp] = sta[Base(currentCode.l, sta, bp) + currentCode.a];
                        sp++;
                        break;
                    case 3:                                        
                        sp--;
                        sta[Base(currentCode.l, sta, bp) + currentCode.a] = sta[sp];
                        break;
                    case 4:                                                                
                        sta[sp] = Base(currentCode.l, sta, bp);           
                        sta[sp + 1] = bp;                                                
                        sta[sp + 2] = pc;                                               
                        bp = sp;                                                                      
                        pc = currentCode.a;                                                               
                        break;
                    case 5:
                        sp += currentCode.a;
                        break;
                    case 6:
                        pc = currentCode.a;
                        break;
                    case 7:
                        sp--;
                        if (sta[sp] == 0)
                        {
                            pc = currentCode.a;
                        }
                        break;
                    case 8:
                        textBox3.Text += sta[sp - 1].ToString();
                        textBox3.Text += Environment.NewLine;
                        sp--;
                        break;
                    case 9:
                        int tmp = int.Parse(input[ttop++]);
                        sta[sp] = tmp;
                        sta[Base(currentCode.l, sta, bp) + currentCode.a] = sta[sp];
                        break;
                }
            } while (pc != 0);


        }

        private int Base(int l, int[] sta, int bp) 
        {
            while (l > 0)
            {                                                     
                bp = sta[bp];
                l--;
            }
            return bp;
        }

        private bool test(ref bool[] a,ref bool[] b,int id)
        {
            if(!a[sym])
            {
                dealError(id, linePos, ss);
                while(!b[sym])
                {
                    if (!getsym())
                        return false;
                }
            }

            return true;
        }

       

        private bool statement(ref bool[] fs,ref int tx,int level)
        {
            bool[] foll=new bool[symNum];
            if (sym == 16)
            {
                int pos = position(ss, tx);
                if (pos == -1)
                {
                    dealError(11, linePos, ss);
                }
                else if (sign[pos].type != 1)
                {
                    dealError(12, linePos, ss);
                    pos = -1;
                }
                else
                {
                    if (!getsym())
                        return false;
                    if (sym == 22)
                    {
                        
                    }
                    else
                        dealError(13, linePos, ss);

                    if (!getsym())
                        return false;
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    
                    expression(ref foll, level, ref tx);
                    gen(3, level - sign[pos].level, sign[pos].addr);
                }
            }
            else if (sym == 15)
            {
                write(tx, level);
            }
            else if (sym == 14)
                read(tx, level);
            else if(sym==9)//call
            {
                if (!getsym())
                    return false;
                if (sym == 16)
                {
                    int pos = position(ss, tx);
                    if (pos == -1)
                        dealError(29, linePos, ss);
                    else if(sign[pos].type!=2)
                    {
                        dealError(15, linePos, ss);
                    }
                    else
                    {
                        gen(4, level - sign[pos].level, sign[pos].addr);
                    }

                    if (!getsym())
                        return false;
                }
                else
                {
                    dealError(14, linePos, ss);
                    if (!getsym())
                        return false;
                }

            }
            else if(sym==7)//while
            {
                int t1 = cx;
                if (!getsym())
                    return false;

                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[8] = true;
                condition(ref fs, ref tx, level);

                int t2 = cx;
                gen(7, 0, 0);
                if (sym == 8)
                {
                    if (!getsym())
                        return false;
                }
                else
                    dealError(18, linePos, ss);

                statement(ref foll, ref tx, level);
                gen(6, 0, t1);
                code[t2].a = cx;
            }
            else if(sym==4)//if
            {
                if (!getsym())
                    return false;

                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[5] = true;
                condition(ref foll, ref tx, level);
                if (sym ==5)
                {
                    if (!getsym())
                        return false;
                }
                else
                {
                    dealError(16, linePos, ss);
                    if (!getsym())
                        return false;
                }
                    

                int cur = cx;
                gen(7, 0, 0);
                statement(ref foll, ref tx, level);
                code[cur].a = cx; 

                if(sym==6)
                {
                    code[cur].a++;
                    if (!getsym())
                        return false;
                    gen(6, 0, 0);
                    int temp = cx - 1;
                    statement(ref foll, ref tx, level);
                    code[temp].a = cx;
                }
            }
            else if(sym==10)//begin
            {
                if (!getsym())
                    return false;

                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[30] = true;
                foll[11] = true;

                statement(ref foll, ref tx, level);
                while(statePre[sym]||sym==30)
                {
                    if(sym==30)
                    {
                        if (!getsym())
                            return false;
                    } 
                    else
                    {
                        dealError(10, linePos, ss);
                       
                    }

                    statement(ref foll, ref tx, level);
                }

                if (sym == 11)
                {
                    if (!getsym())
                        return false;
                }
                else
                    dealError(17, linePos, ss);
            }
            else if(sym==12)//repeat
            {
                if (!getsym())
                    return false;

                int cur = cx;
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[30] = true;
                foll[13] = true;
                statement(ref foll, ref tx, level);

                while(statePre[sym]||sym==30)
                {
                    if (sym == 30)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                        dealError(10, linePos, ss);

                    statement(ref foll, ref tx, level);
                }

                if (sym == 13)
                {
                    if (!getsym())
                        return false;
                    condition(ref foll, ref tx, level);
                    gen(7, 0, cur);
                }
                else
                    dealError(36, linePos, ss);
            }
            return true;
        }

        private bool read(int tx,int level)
        {
            if (!getsym())
                return false;
            if (sym != 31)
                dealError(40, linePos, ss);

            do
            {
                getsym();
                if (sym != 16)
                    dealError(41, linePos, ss);
                int pos = position(ss, tx);
                if (pos == -1)
                    dealError(11, linePos, ss);
                else if (sign[pos].type !=1)
                    dealError(37, linePos, ss);
                else
                {
                    int addr = sign[pos].addr;
                    //gen(2, level-sign[pos].level, addr);
                    gen(9, level - sign[pos].level, addr);
                }
                getsym();
            } while (sym == 29);

            if (sym == 32)
            {
                if (!getsym())
                    return false;
            }
            else
                dealError(22, linePos, ss);
            return true;
        }

        private bool write(int tx,int level)
        {
            if (!getsym())
                return false;
            if (sym!=31)
                dealError(40, linePos, ss);

            do
            {
                getsym();
                if (sym != 16)
                    dealError(41, linePos, ss);
                int pos = position(ss, tx);
                if (pos == -1)
                    dealError(11, linePos, ss);
                else if (sign[pos].type == 2)
                    dealError(35, linePos, ss);
                else if(sign[pos].type==1)
                {
                    int addr = sign[pos].addr;
                    gen(2, level-sign[pos].level, addr);
                    gen(8,0,0);
                }
                else
                {
                    gen(0, 0, sign[pos].val);
                    gen(8, 0, 0);
                }
                getsym();
            } while (sym == 29);

            if (sym == 32)
            {
                if (!getsym())
                    return false;
            }
            else
                dealError(22, linePos, ss);


            return true;
        }

        private bool expression(ref bool[] fs,int level,ref int tx)
        {
            bool[] foll = new bool[symNum];
            if(sym==18||sym==19)
            {
                int tmp = sym;
                if (!getsym())
                    return false;
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);
                if (tmp  == 19)
                    gen(1, 0, 1);
            }
            else
            {
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);
            }

            while(sym==18||sym==19)
            {
                int tmp = sym;
                if (!getsym())
                    return false;
                for (int i = 0; i < 55; i++)
                    foll[i] = fs[i];
                foll[18] = true;
                foll[19] = true;
                term(ref foll, ref tx, level);

                if (tmp == 18)
                    gen(1, 0, 2);
                else
                    gen(1, 0, 3);
            }
            return true;
        }
        

        private bool term(ref bool[] fs,ref int tx,int level)
        {
            bool[] foll = new bool[symNum];
            for (int i = 0; i < 55; i++)
                foll[i] = fs[i];
            foll[20] = true;
            foll[21] = true;
            foll[17] = true;
            factor(ref foll, ref tx, level);

            if (linePos == 8)
                textBox3.Text += ss;

            bool heihei = false;
            while(sym==20||sym==21)
            {
                heihei = true;
                if(sym==20)
                {
                    if (!getsym())
                        return false;
                    
                    factor(ref foll, ref tx, level);
                    gen(1, 0, 4);
                }
                else
                {
                    if (!getsym())
                        return false;
                     
                    factor(ref foll, ref tx, level);
                    gen(1, 0, 5);
                }
            }

            if(sym!=30&&linePos==8)
            {
                dealError(26, linePos, ss);
                if (!getsym())
                    return false;
            }
            return true;
        }

        private int errorNum;
        private bool factor(ref bool[] fs,ref int tx,int level)
        {
            bool[] foll = new bool[symNum];
            test(ref factorPre, ref fs, 24);

                if (sym == 16)
                {
                    int pos = position(ss, tx);
                    if (pos == -1)
                        dealError(11, linePos, ss);
                    else
                    {
                        switch (sign[pos].type)
                        {
                            case 0:
                                gen(0, 0, sign[pos].val);
                                break;
                            case 1:
                                gen(2, level - sign[pos].level, sign[pos].addr);
                                break;
                            case 2:
                                dealError(21, linePos, ss);
                                break;
                        }
                    }

                    if (!getsym())
                        return false;
                }
                else if (sym == 17)
                {
                    int num = int.Parse(ss);
                    if (num > 20000005)
                    {
                        dealError(31, linePos, ss);
                    }

                    gen(0, 0, num);
                    if (!getsym())
                        return false;
                }
                else if (sym == 31)
                {
                    if (!getsym())
                        return false;
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    foll[32] = true;
                    expression(ref foll, level, ref tx);

                    if (sym == 32)
                    {
                        if (!getsym())
                            return false;
                    }
                    else
                {
                    dealError(22, linePos, ss);
                    if(linePos==17)
                    {
                        usedNum += 8;
                        getsym();
                    }
                }
                        
                }
                else
                {
                    for (int i = 0; i < symNum; i++)
                        foll[i] = fs[i];
                    test(ref fs, ref foll, 23);
                }
            return true;
        }

        private int position(string ss,int tx)
        {
            for(int i=tx;i>=1;i--)
            {
                if (ss == sign[i].name)
                    return i;
            }

            return -1;
        }

        private bool constdeclaration(ref int tx,int level,ref int dx)
        {
            if (sym == 16)
            {
                string sTmp = ss;
                if (!getsym())
                    return false;
                if (sym == 22 || sym == 25)
                {
                    if (sym == 22)
                        dealError(1, linePos, ss);
                    if (!getsym())
                        return false;
                    if(sym==17)
                    {
                        enter(0, ref tx, level, ref dx,int.Parse(ss),sTmp);
                    }
                    else
                    {
                        dealError(2, linePos, ss);
                    }
                }
                else
                    dealError(3, linePos, ss);

            }
            else
                dealError(4, linePos, ss);

            if (!getsym())
                return false;

            return true;
        }


        private bool vardeclaration(ref int tx,int level,ref int dx)

       {

            if (sym == 16)
            {
                
                enter(1, ref tx, level, ref dx, 1,ss);

                
            }
            else
                dealError(4, linePos, ss);

            if (!getsym())
                return false;
            return true;
        }

        private void enter(int type,ref int tx,int level,ref int dx,int val,string name)
        {
                for (int i = tx; i >= 1; i--)
                {
                    if (sign[i].name == name && level == sign[i].level)
                    {
                        dealError(28, linePos, ss);
                        break;
                    }
                }
            

            tx++;
            sign[tx].name = name;
            sign[tx].type = type;
            switch(type)
            {
                case 0:
                    if(val>20000005)
                    {
                        dealError(31, linePos, ss);
                        val=0;
                    }
                    sign[tx].val = val;
                    sign[tx].level = level;
                    break;
                case 1:
                    sign[tx].addr = dx;
                    sign[tx].level = level;
                    dx++;
                    break;
                case 2:
                    sign[tx].level = level;
                    break;

            }
        }

        private bool condition(ref bool[] fs,ref int tx,int level)
        {
            bool[] foll = new bool[symNum];

            if(sym==3)
            {
                if (!getsym())
                    return false;
                expression(ref fs, level, ref tx);
                if(!gen(1, 0, 6))
                    return false;
            }
            else
            {
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[23] = true;
                foll[24] = true;
                foll[25] = true;
                foll[26] = true;
                foll[27] = true;
                foll[33] = true;
                expression(ref foll, level, ref tx);

                if (sym != 23 && sym != 24 && sym != 25 && sym != 26 && sym != 27 && sym != 33)
                {
                    dealError(20, linePos, ss);
                }
                else
                {
                    int temp = sym;
                    if (!getsym())
                        return false;
                    expression(ref fs, level, ref tx);

                    switch (temp)
                    {
                        case 23:
                            gen(1, 0, 13);
                            break;
                        case 24:
                            gen(1, 0, 11);
                            break;
                        case 25:
                            gen(1, 0, 8);
                            break;
                        case 26:
                            gen(1, 0, 10);
                            break;
                        case 27:
                            gen(1, 0, 12);
                            break;
                        case 33:
                            gen(1, 0, 9);
                            break;
                    }
                }
                /*if (sym == 16 || sym == 17)
                {
                    int pos = position(ss, tx);
                    if(pos==-1)
                    {
                        dealError(11);
                    }
                    else
                    {
                        switch(sign[pos].type)
                        {
                            case 0:
                                gen(0, 0, sign[pos].val);
                                break;
                            case 1:
                                gen(2, level - sign[pos].level, sign[pos].val);
                                break;
                            case 2:
                                dealError(30);
                                break;
                        }
                    }
                }
                else
                    dealError(33);
                for (int i = 0; i < symNum; i++)
                    foll[i] = fs[i];
                foll[23] = true;
                foll[24] = true;
                foll[25] = true;
                foll[26] = true;
                foll[27] = true;
                foll[33] = true;

                if(sym!=23&&sym!=24&&sym!=25&&sym!=26&&sym!=27&&sym!=33)
                {
                    dealError(20);
                }
                else
                {
                    
                    int cnt = sym;
                    if (!getsym())
                        return false;
                    expression(ref foll, level, ref tx);

                    switch(cnt)
                    {
                        case 23:
                            gen(1, 0, 13);
                            break;
                        case 24:
                            gen(1, 0, 11);
                            break;
                        case 25:
                            gen(1, 0, 8);
                            break;
                        case 26:
                            gen(1, 0, 10);
                            break;
                        case 27:
                            gen(1, 0, 12);
                            break;
                        case 33:
                            gen(1, 0, 9);
                            break;
                    }
                }*/
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            init();
            textBox2.Text = "";
            textBox1.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            wordText = "单词          类别          值";
            wordText += Environment.NewLine;
            string str = textBox1.Text;
                int lineNum = 1;
                bool cnt = true;
                foreach(string nextLine in richTextBox1.Lines)
                {
                    //Console.WriteLine(nextLine);
                    cnt = wordCompile(nextLine, lineNum);
                    /*if (!cnt)
                        break;*/
                    lineNum++;
                }

            /*for (int i = 0; i < wordNum; i++)
            {
                textBox5.Text += wordList[i].name;
                textBox5.Text += " ";
                textBox5.Text+=(wordList[i].type).ToString();
                textBox5.Text += " ";
                textBox5.Text += (wordList[i].line).ToString();
                textBox5.Text += Environment.NewLine;
            }*/

            if(getsym())
                block(ref follow,0, ref tx);


            if (sym != 28)
                dealError(9, linePos, ss);

            for (int i = 0; i < cx; i++)
            {
                if (code[i].f == 20)
                    continue;
                textBox5.Text += (i + 1).ToString();
                textBox5.Text += " ";
                textBox5.Text += codeF[code[i].f];
                textBox5.Text += " ";
                textBox5.Text += code[i].l.ToString();
                textBox5.Text += " ";
                textBox5.Text += code[i].a.ToString();
                textBox5.Text += Environment.NewLine;
            }


            if (!isError) isCompile = true;
            else
                isCompile = false;
                //interpret();

                
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            if (!isCompile || isError)
                MessageBox.Show("请先正确编译");
            else
            {
                string str = textBox4.Text;
                ttop = 0;
                input = str.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                interpret();
                /*for(int i=0;i<input.Length;i++)
                {
                    textBox3.Text += input[i];
                    textBox3.Text += "";
                }*/
            }
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            string str = "";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                str += saveFileDialog1.FileName.ToString();
            }

            try
            {
                StreamWriter sw = new StreamWriter(str);
                //textBox4.Text += textBox5.Text;
                sw.Write(textBox3.Text);
                sw.Close();
            }
            catch (IOException)
            {
                MessageBox.Show("打开文件失败");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            string str = "";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                str+= saveFileDialog1.FileName.ToString();
            }

            try
            {
                StreamWriter sw = new StreamWriter(str);
                //textBox4.Text += textBox5.Text;
                sw.Write(textBox5.Text);
                sw.Close();
            }
            catch(IOException)
            {
                MessageBox.Show("打开文件失败");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
