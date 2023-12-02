using System;
using System.Collections.Generic;
using UnityEngine;

namespace SKCell
{
	public class SKObjectPool<T>
	{
		private List<SKObjectPoolContainer<T>> list;
		private Dictionary<T, SKObjectPoolContainer<T>> lookup;
		private Func<T> factoryFunc;
		private int lastIndex = 0;

		public SKObjectPool(Func<T> factoryFunc, int initialSize)
		{
			this.factoryFunc = factoryFunc;

			list = new List<SKObjectPoolContainer<T>>(initialSize);
			lookup = new Dictionary<T, SKObjectPoolContainer<T>>(initialSize);

			Warm(initialSize);
		}

		private void Warm(int capacity)
		{
			for (int i = 0; i < capacity; i++)
			{
				CreateContainer();
			}
		}

		private SKObjectPoolContainer<T> CreateContainer()
		{
			var container = new SKObjectPoolContainer<T>();
			container.Item = factoryFunc();
			list.Add(container);
			return container;
		}

		public T GetItem()
		{
			SKObjectPoolContainer<T> container = null;
			for (int i = 0; i < list.Count; i++)
			{
				lastIndex++;
				if (lastIndex > list.Count - 1) lastIndex = 0;
				
				if (list[lastIndex].Used)
				{
					continue;
				}
				else
				{
					container = list[lastIndex];
					break;
				}
			}

			if (container == null)
			{
				container = CreateContainer();
			}

			container.Consume();
			lookup.Add(container.Item, container);
			return container.Item;
		}

		public void ReleaseItem(object item)
		{
			ReleaseItem((T) item);
		}

		public void ReleaseItem(T item)
		{
			if (lookup.ContainsKey(item))
			{
				var container = lookup[item];
				container.Release();
				lookup.Remove(item);
			}
			else
			{
				SKUtils.EditorLogWarning("This object pool does not contain the item provided: " + item);
			}
		}

		public int Count
		{
			get { return list.Count; }
		}

		public int CountUsedItems
		{
			get { return lookup.Count; }
		}
	}
}
