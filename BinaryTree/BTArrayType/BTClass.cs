/*
 * Сделано в SharpDevelop.
 * Пользователь: suvoroda
 * Дата: 02.02.2017
 * Время: 8:45
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryTree.BTArrayType
{
	/// <summary>
	/// Description of BTClass.
	/// </summary>
	[Serializable]
	public class BTClass<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{		
		#region private param's
		internal static BTClass<TKey, TValue>[] elements;
		private BTClass<TKey, TValue>[] _branchs;
		private BTClass<TKey, TValue> _parent;
		private Entry entry;
		private static int count;
		private int version;
		#endregion
		
		#region internal nested class
		[Serializable]
		internal class Entry{
			public TKey _key;
			public TValue _value;
			public Entry(TKey key, TValue val){
				this._key = key;
				this._value = val;
			}
			public Entry(){}
		}
		#endregion
		
		[Serializable]
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, System.Collections.IDictionaryEnumerator, System.Collections.IEnumerator
		{
			internal const int DictEntry = 1;
			internal const int KeyValuePair = 2;
			private BTClass<TKey, TValue> dictionary;
			private int version;
			private int index;
			private KeyValuePair<TKey, TValue> current;
			private int getEnumeratorRetType;
			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this.current;
				}
			}
			object System.Collections.IEnumerator.Current
			{
				get
				{
					if (this.index == 0 || this.index == BTClass<TKey, TValue>.count + 1)
					{
						throw new Exception("InvalidOperation_EnumOpCantHappen");
					}
					if (this.getEnumeratorRetType == 1)
					{
						return new DictionaryEntry(this.current.Key, this.current.Value);
					}
					return new KeyValuePair<TKey, TValue>(this.current.Key, this.current.Value);
				}
			}
			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					if (this.index == 0 || this.index == BTClass<TKey, TValue>.count + 1)
					{
						throw new Exception("InvalidOperation_EnumOpCantHappen");
					}
					return new DictionaryEntry(this.current.Key, this.current.Value);
				}
			}
			object IDictionaryEnumerator.Key
			{
				get
				{
					if (this.index == 0 || this.index == BTClass<TKey, TValue>.count + 1)
					{
						throw new Exception("InvalidOperation_EnumOpCantHappen");
					}
					return this.current.Key;
				}
			}
			object IDictionaryEnumerator.Value
			{
				get
				{
					if (this.index == 0 || this.index == BTClass<TKey, TValue>.count + 1)
					{
						throw new Exception("InvalidOperation_EnumOpCantHappen");
					}
					return this.current.Value;
				}
			}
			internal Enumerator(BTClass<TKey, TValue> dictionary, int getEnumeratorRetType)
			{
				this.dictionary = dictionary;
				this.version = dictionary.version;
				this.index = 0;
				this.getEnumeratorRetType = getEnumeratorRetType;
				this.current = default(KeyValuePair<TKey, TValue>);
			}
			public bool MoveNext()
			{
				if (this.version != this.dictionary.version)
				{
					throw new Exception("InvalidOperation_EnumFailedVersion");
				}
				while (this.index < BTClass<TKey, TValue>.count/*BTClass<TKey, TValue>.elements.Length*/)
				{
					TKey tmpKey = dictionary.getNextKey(this.current.Key);
					this.current = new KeyValuePair<TKey, TValue>(tmpKey, dictionary.Get(tmpKey));//new KeyValuePair<TKey, TValue>(BTClass<TKey, TValue>.elements[index].entry._key, BTClass<TKey, TValue>.elements[index].entry._value);
					this.index++;
					return true;
				}
				this.index = BTClass<TKey, TValue>.count + 1;
				this.current = default(KeyValuePair<TKey, TValue>);
				return false;
			}
			public void Dispose()
			{
			}
			void System.Collections.IEnumerator.Reset()
			{
				if (this.version != this.dictionary.version)
				{
					throw new Exception("InvalidOperation_EnumFailedVersion");
				}
				this.index = 0;
				this.current = default(KeyValuePair<TKey, TValue>);
			}
		}
		
		#region public Field's
		public BTClass<TKey, TValue> LeftBranch{
			get {
				if(this._branchs != null)
					if(this._branchs.Length > 0)
						if(this._branchs[0] != null)
							return this._branchs[0];
				return null;
			}
			set {
				if(this._branchs == null)
					this._branchs = new BTClass<TKey, TValue>[2];
				this._branchs[0] = value;
			}
		}
		
		public BTClass<TKey, TValue> RightBranch{
			get {
				if(this._branchs != null)
					if(this._branchs.Length > 0)
						if(this._branchs[1] != null)
							return this._branchs[1];
				return null;
			}
			set {
				if(this._branchs == null)
					this._branchs = new BTClass<TKey, TValue>[2];
				this._branchs[1] = value;
			}
		}
		#endregion
		
		#region .ctor's
		public BTClass(){}
		#endregion
		
		#region public Func's
		/// <summary>
		/// Добавить элемент в дерево
		/// </summary>
		/// <param name="key">Ключ для поиска</param>
		/// <param name="val">Значение</param>
		/// <param name="replace">Заменять, если есть такой ключь, иначе Exception</param>
		public virtual void Add(TKey key, TValue val, bool replace = false, BTClass<TKey, TValue> parent = null){
			if(this.entry == null){
				this.entry = new BTClass<TKey,TValue>.Entry();
				this.entry._key = key;
				this.entry._value = val;
				this._parent = parent;
				/*if(elements == null)
					elements = new BTClass<TKey, TValue>[1];
				if(!Array.Exists(elements,x => (x != null) ? x.compare(this.entry._key) == 0 : false)){
					if(elements[0] != null)
						Array.Resize(ref elements,elements.Length+1);
					elements[elements.Length-1] = this;
				}*/
				count++;
				return;
			}
			int result = compare(key);
			if(this._branchs == null)
				this._branchs = new BTClass<TKey, TValue>[2];
			if(result < 0){
				if(this._branchs[0]==null){
					this._branchs[0] = new BTClass<TKey, TValue>();					
				}
				this._branchs[0].Add(key, val, replace, this);
			} else if (result > 0){
				if(this._branchs[1]==null){
					this._branchs[1] = new BTClass<TKey, TValue>();
				}
				this._branchs[1].Add(key, val, replace, this);
			} else {
				if(replace){
					this.entry._value = val;
				} else {
					throw new Exception("Error key");
				}
			}
		}
		/// <summary>
		/// Вернуть значение по ключу
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns>Значение</returns>
		public TValue Get(TKey key){
			if(key == null)
				throw new Exception("Key is null!");
			if(this.entry == null)
				throw new Exception("Value is not find");
			int result = compare(key);
			if(result == 0)
				return this.entry._value;
			else{ 
				if(this._branchs == null)
					throw new Exception("Branchs is null!");
				if(result < 0){
					if(this._branchs[0] == null)
						throw new Exception("Left Branch is null!");
					return this._branchs[0].Get(key);
				}
				else{
					if(this._branchs[1] == null)
						throw new Exception("Right Branch is null!");
					return this._branchs[1].Get(key);
				}
			}
		}
		/// <summary>
		/// Проверить есть ли элемент с таким ключом
		/// </summary>
		/// <param name="key">Ключ</param>
		/// <returns>True - есть такой элемент/False - нет такого элемента</returns>
		public bool ContainKey(TKey key){
			if(key == null)
				throw new Exception("Key is null!");
			if(this.entry == null)
				return false;
			int result = compare(key);
			if(result == 0)
				return true;
			if(this._branchs == null)
					return false;
			if(result < 0){
				if(this._branchs[0] == null)
					return false;
				return this._branchs[0].ContainKey(key);
			} else {
				if(this._branchs[1] == null)
					return false;
				return this._branchs[1].ContainKey(key);
			}
		}
		/// <summary>
		/// Удалить элемент по ключу
		/// </summary>
		/// <param name="key">Ключ</param>
		public virtual void Remove(TKey key){
			if(key == null)
				throw new Exception("Key is null!");
			if(this.entry != null){
				if(ContainKey(key)){//такого ключа нету, удалять нечего
					int result = compare(key);
					if(result == 0){
						if(this._branchs == null){//Веток нет, удаляем значения текущей ветки
							this.entry = null;
							this._branchs = null;
						} else {//Ветки есть, переносим на место удаленной
							BranchUp();
						}
					} else {
						if(result < 0){
							if(this._branchs != null){
								if(this._branchs[0] != null){
									this._branchs[0].Remove(key);
								} 
							} 
						} else {
							if(this._branchs != null){
								if(this._branchs[1] != null){
									this._branchs[1].Remove(key);
								}
							}
						}
					}
				}
			}
			clearEmptyBranch();
		}
		/// <summary>
		/// Возвращает все значения бинарного дерева в виде строки(Внимание, при большом содержании значений может долго работать)
		/// </summary>
		/// <returns>Строка</returns>
		public string GetBtToString()
		{
			return string.Format("[[Left[{0}]];[Right[{1}]];[Key:{2};Type:{3}];[Val:{4};Type:{5}]]",(_branchs != null) ? (_branchs[0] != null) ? _branchs[0].GetBtToString() : "" : "", 
			                     (_branchs != null) ? (_branchs[1] != null) ? _branchs[1].GetBtToString() : "" : "",
			                     (entry == null) ? "" : entry._key.ToString(), (entry == null) ? "" : entry._key.GetType().ToString(),
			                     (entry == null) ? "" : entry._value.ToString(), (entry == null) ? "" : entry._value.GetType().ToString());
		}
		
		public BTClass<TKey, TValue> GetMin(){
			if(this.entry == null)
				throw new Exception("Value is not find");
			if(this._branchs != null){
				if(this._branchs[0] != null)
					return this._branchs[0].GetMin();
				else
					return this;
			}
			else
				return this;
		}
		
		public BTClass<TKey, TValue> GetMax(){
			if(this.entry == null)
				throw new Exception("Value is not find");
			if(this._branchs != null){
				if(this._branchs[1] != null)
					return this._branchs[1].GetMax();
				else
					return this;
			}
			else
				return this;
		}
		
		public BTClass<TKey, TValue> GetParent(){
			return this._parent;
		}
		
		public BTClass<TKey, TValue> GetNextRight(TKey key){
			if(this._branchs[1].compare(key) > 0)
				return this._parent.GetNextRight(key);
			return this;
		}
		
		public TKey GetNextBranch(TKey key){
			Debug.Print("key {0} compare {1} this key {2}",key, compare(key), this.entry._key);			
			switch(compare(key)){
				case 0:
					if(this._branchs != null){
						if(this._branchs[1] != null){
							return this._branchs[1].GetMin().entry._key;					
						}
					}
					if(this != GetMax())
						return this._parent.GetNextRight(key).entry._key;
					else
						return this.entry._key;
				case -1:
					if(this._branchs[0] != null & this._branchs[0].compare(key) == 0){
						if(this._branchs[0]._branchs != null/* & this._branchs[0]._branchs[1] != null*/)
							if(this._branchs[0]._branchs[1] != null)
								return this._branchs[0].GetNextBranch(key);
						return this.entry._key;
					}
					return this._branchs[0].GetNextBranch(key);
				default:
					/*if(this._branchs[1].compare(key) == 0){
						return this.GetNextRight(
					}*/
					return this._branchs[1].GetNextBranch(key);
			}
		}
		#endregion
		
		#region internal Func's
		/// <summary>
		/// Помним, что все значения правой ветки всегда больше значений левой ветки, значит ищем самое левое значение правой ветки
		/// </summary>
		internal void BranchUp(){
			if(this._branchs[0] == null){//Если левой ветки нету, то просто переносим значение правой ветки в текущую
				this.entry = this._branchs[1].entry;
				this._branchs = this._branchs[1]._branchs;
			} else {//Если есть левая ветка, то сначала опускаем ее в самый низ правой ветки а затем заносим значения в текущую
				if(this._branchs[1] != null){
					this._branchs[1].BranchDown(this._branchs[0]);
					this.entry = this._branchs[1].entry;
					this._branchs = this._branchs[1]._branchs;
				}//Если правой ветки нету, то поднимаем левую вверх
				else {
					this.entry = this._branchs[0].entry;
					this._branchs = this._branchs[0]._branchs;
				}
			}
		}
		
		internal void BranchDown(BTClass<TKey, TValue> bt){//ERROR
			if(this._branchs != null){
				if(this._branchs[0] == null){
					this._branchs[0] = new BTClass<TKey, TValue>();
					this._branchs[0]._branchs = bt._branchs;
					this._branchs[0].entry = bt.entry;
				} else {
					this._branchs[0].BranchDown(bt);
				}
			} else {
				this._branchs = new BTClass<TKey, TValue>[2];
				this._branchs[0] = new BTClass<TKey, TValue>();
				this._branchs[0]._branchs = bt._branchs;
				this._branchs[0].entry = bt.entry;
			}
		}
		
		internal bool checkLeftBranch(){
			if(_branchs != null)
				return (_branchs[0] != null);
			return false;
		}
		
		internal bool checkRightBranch(){
			if(_branchs != null)
				return (_branchs[1] != null);
			return false;
		}
		
		internal TKey getNextKey(TKey curKey){
			if(curKey == null)
				return this.GetMin().entry._key;
			return this.GetNextBranch(curKey);
		}
		#endregion
		
		#region private Func's
		private int compare(TKey key){
			return (key.GetHashCode() > entry._key.GetHashCode()) ? 1 : (key.GetHashCode() == entry._key.GetHashCode()) ? 0 : -1;
		}
		
		private void clearEmptyBranch(){
			if(this._branchs != null){
				if(this._branchs[0] != null){
					if(this._branchs[0].entry == null){
						this._branchs[0] = null;
					} else {
						this._branchs[0].clearEmptyBranch();
					}
				}
				if(this._branchs[1] != null){
					if(this._branchs[1].entry == null){
						this._branchs[1] = null;
					} else {
						this._branchs[1].clearEmptyBranch();
					}
				}
				if(this._branchs[0] == null && this._branchs[1] == null)
					this._branchs = null;
			}
		}
		#endregion
		
		#region public static Func's
		/// <summary>
		/// Сохранить данные в файл
		/// </summary>
		/// <param name="PathToFile">Путь к файлу</param>
		/// <param name="bt">Данные</param>
		public static void ToFile(string PathToFile, BTClass<TKey, TValue> bt){
			File.WriteAllBytes(PathToFile,ToByteArr(bt));
		}
		/// <summary>
		/// Загрузить данные из файла
		/// </summary>
		/// <param name="PathToFile">Путь к файлу</param>
		/// <returns>Данные</returns>
		public static BTClass<TKey, TValue> ReadFromFile(string PathToFile){
			return ReadFromByte(File.ReadAllBytes(PathToFile));
		}
		#endregion
		
		#region private static Func's 
		private static byte[] ToByteArr(BTClass<TKey, TValue> bt){
			if(bt == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using(MemoryStream ms = new MemoryStream()){
				bf.Serialize(ms, bt);
				return ms.ToArray();
			}
		}
		
		private static BTClass<TKey, TValue> ReadFromByte(byte[] byteArr){
			MemoryStream ms = new MemoryStream();
			BinaryFormatter bf = new BinaryFormatter();
			ms.Write(byteArr,0, byteArr.Length);
			ms.Seek(0, SeekOrigin.Begin);
			BTClass<TKey, TValue> res = (BTClass<TKey, TValue>) bf.Deserialize(ms);
			return res;
		}
		#endregion
		
		#region public override Func's
		/// <summary>
		/// Возвращает BTClass в виде строки
		/// </summary>
		/// <returns>BTClass в виде строки</returns>
		public override string ToString()
		{
			return string.Format("[BTClass Branchs={0}, Entry={1}]", _branchs, entry);
		}

		#endregion
		
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			//throw new NotImplementedException();
			return new BTClass<TKey, TValue>.Enumerator(this, 2);
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
