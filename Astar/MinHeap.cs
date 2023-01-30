namespace System.Collections.Generic
{
    using System;
    using System.Text;

    public class MinHeap<T>
    {
        private int m_N;
        private List<T> m_DataList;
        private IComparer<T> m_Compare;
        private int m_Size;

        private bool m_Uniqueness = false;
        private Dictionary<T, int> m_DicNode;

        public int N
        {
            get
            {
                return m_N;
            }
            set
            {
                m_N = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Size;
            }
        }

        public T this[int node]
        {
            get
            {
                return m_DataList[node - 1];
            }
            set
            {
                if(node - 1 == m_DataList.Count)
                {
                    m_DataList.Add(value);
                }
                else
                {
                    m_DataList[node - 1] = value;
                }

                m_DicNode[value] = node;
            }
        }

        /// <summary>
        /// 最后一个非叶子节点
        /// </summary>
        public int LastNonLeafNode
        {
            get
            {
                return (m_Size + m_N - 2 ) / m_N;
            }
        }

        public int CompareTimes { get; private set; }
        public int SwapTimes { get; private set; }

        public MinHeap(int n, bool uniqueness = false, IComparer<T> comapre = null, T[] datas = null)
        {
            m_N = n;
            m_Uniqueness = uniqueness;
            m_Compare = comapre ?? Comparer<T>.Default;
            m_DataList = datas == null ? new List<T>(128) : new List<T>(datas);
            m_Size = m_DataList.Count;
            m_DicNode = new Dictionary<T, int>(m_DataList.Capacity);
            for (int i = LastNonLeafNode; i >= 1; i--)
            {
                DownHeap(i);
            }
        }

        /// <summary>
        /// 获取节点的子节点序号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int GetMinChildNode(int node)
        {
            int ret = (node - 1) * m_N + 2;
            for (int i = ret + 1; i < ret + m_N; i++)
            {
                if(i > m_Size)
                {
                    break;
                }
                CompareTimes++;
                if (m_Compare.Compare(this[ret], this[i]) > 0)
                {
                    ret = i;
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取节点的父节点序号
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int GetParentNode(int node)
        {
            if(node <= 1)
            {
                return 0;
            }
            return (node - 2) / m_N + 1;
        }

        /// <summary>
        /// 下层
        /// </summary>
        /// <param name="node">节点序号(根节点为1)</param>
        private void DownHeap(int node)
        {
            int child;
            T val = this[node];
            T child_val;
            while (node <= LastNonLeafNode)
            {
                child     = GetMinChildNode(node);
                child_val = this[child];
                CompareTimes++;
                if (m_Compare.Compare(val, child_val) <= 0)
                {
                    break;
                }
                this[node] = child_val;
                node       = child;
            }
            this[node] = val;
        }

        /// <summary>
        /// 上浮
        /// </summary>
        /// <param name="data"></param>
        /// <param name="node"></param>
        private void UpHeap(T data, int node)
        {
            if (node <= 1)
            {
                return;
            }

            int parent = GetParentNode(node);
            T p_val;
            while (node > 1)
            {
                p_val = this[parent];
                CompareTimes++;
                if (m_Compare.Compare(data, p_val) > 0)
                {
                    break;
                }
                this[node] = p_val;
                node = parent;
                parent = GetParentNode(node);
            }
            this[node] = data;
        }

        /// <summary>
        /// 入堆
        /// </summary>
        /// <param name="data"></param>
        public void Push(T data)
        {
            if(data == null)
            {
                return;
            }

            if (!m_Uniqueness || !m_DicNode.TryGetValue(data, out int node))
            {
                this[++m_Size] = data;
                node = m_Size;
            }

            UpHeap(data, node);
        }

        /// <summary>
        /// 出堆
        /// </summary>
        /// <returns></returns>
        public bool TryPop(ref T result)
        {
            if(m_Size < 1)
            {
                return false;
            }
            result = this[1];
            this[1] = this[m_Size--];
            DownHeap(1);
            return true;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            m_Size = 0;
            m_DicNode.Clear();
            CompareTimes = 0;
            SwapTimes = 0;
        }

        ~MinHeap()
        {
            m_DataList = null;
        }
    }
}
