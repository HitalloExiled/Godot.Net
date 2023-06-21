namespace Godot.Net.Core;

public partial class SelfList<T> where T : notnull
{
    public class List
    {
        public SelfList<T>? First { get; private set; }
        public SelfList<T>? Last  { get; private set; }

        public void Add(SelfList<T> elements)
        {
            if (ERR_FAIL_COND(elements.Root != null))
            {
                return;
            }

            elements.Root = this;
            elements.Next = this.First;
            elements.Prev = null;

            if (this.First != null)
            {
                this.First.Prev = elements;

            }
            else
            {
                this.Last = elements;
            }

            this.First = elements;
        }

        public void Remove(SelfList<T> elements)
        {
            if (ERR_FAIL_COND(elements.Root != this))
            {
                return;
            }

            if (elements.Next != null)
            {
                elements.Next.Prev = elements.Prev;
            }

            if (elements.Prev != null)
            {
                elements.Prev.Next = elements.Next;
            }

            if (this.First == elements)
            {
                this.First = elements.Next;
            }

            if (this.Last == elements)
            {
                this.Last = elements.Prev;
            }

            elements.Next = null;
            elements.Prev = null;
            elements.Root = null;
        }
    }
}
