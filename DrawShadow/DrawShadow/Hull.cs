
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace DrawShadow
    {

        class Hull
        {
            public List<Trace> Sides;
            public List<Trace> Bound;
            public int id;
            private static int count = 0;
            public Hull(List<Trace> sides)
            {
                this.Sides = sides;
                this.id = Hull.count;
                this.Bound = new List<Trace>();
                Hull.count++;
                HullContainer.hulls.Add(this);
            }
            public bool Equal(Hull another)
            {
                if (this.id == another.id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        public void Update() {
            this.Bound = new List<Trace>();
        }
        }
    }

