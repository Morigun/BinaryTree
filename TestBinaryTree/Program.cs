/*
 * Сделано в SharpDevelop.
 * Пользователь: suvoroda
 * Дата: 02/01/2017
 * Время: 14:22
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BinaryTree.BTArrayType;

namespace TestBinaryTree
{
	class Program
	{
		public static void Main(string[] args)
		{
			//BTClass<String, int> bt = BTClass<String, int>.ReadFromFile(@"C:\MyDOC\Проекты\C#\Developer\BinaryTree\BinaryTree\test.txt");
			BTClass<String, int> bt = new BTClass<String, int>();
			//IEnumerator<KeyValuePair<String, int>> t = bt.GetEnumerator();
			//tmp.Where();
			bt.Add("e",1);
			bt.Add("b",1);
			bt.Add("f",1);
			bt.Add("a",1);
			bt.Add("z",2);
			bt.Add("x",3);
			bt.Add("y",1);
			bt.Add("g",2);
			bt.Add("t",1);
			bt.Add("o",3);
			bt.Add("c",3);
			bt.Add("d",3);
			bt.Add("r",3);
			bt.Add("w",3);
			bt.Add("u",3);
			bt.Add("i",3);
			bt.Add("p",3);
			/*for(int i = 0; i < 1000; i++){
				bt.Add("a"+i,i);
			}*/
			/*bt.Remove("e");
			Console.WriteLine("Del E {0}\n\n",bt.ToString());
			bt.Remove("b");
			Console.WriteLine("Del B {0}\n\n",bt.ToString());
			bt.Remove("a");
			Console.WriteLine("Del A {0}\n\n",bt.ToString());
			bt.Remove("z");
			Console.WriteLine("Del sa {0}\n\n",bt.ToString());
			bt.Remove("y");
			Console.WriteLine("Del Y {0}\n\n",bt.ToString());
			bt.Remove("g");
			Console.WriteLine("Del G {0}\n\n",bt.ToString());*/
			//BTClass<string,int>.ToFile(@"C:\MyDOC\Проекты\C#\Developer\BinaryTree\BinaryTree\test.txt",bt);
			Console.WriteLine("{0}", bt.GetBtToString());
			Stopwatch sw = new Stopwatch();
			sw.Start();
			//Console.WriteLine("{0}\n\n",bt.Where((x) => x.Key == "a999").First());
			Console.WriteLine("{0}\n\n",bt.Where((x) => x.Key == "p").First());
			sw.Stop();
			Console.WriteLine("\n\n{0}",sw.ElapsedMilliseconds);
			/*sw.Reset();
			Dictionary<String, int> dict = new Dictionary<string, int>();
			dict.Add("e",1);
			dict.Add("b",1);
			dict.Add("f",1);
			dict.Add("a",1);
			dict.Add("z",2);
			dict.Add("x",3);
			dict.Add("y",1);
			dict.Add("g",2);
			dict.Add("t",1);
			dict.Add("o",3);
			sw.Start();
			Console.WriteLine("{0}\n\n",dict.Where((x) => x.Key == "x").First());
			sw.Stop();
			Console.WriteLine("\n\n{0}",sw.ElapsedMilliseconds);	*/		
			Console.ReadKey(true);
		}
	}
}