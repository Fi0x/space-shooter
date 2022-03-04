using System.Collections.Generic;
using Targeting;

namespace Manager
{
    public class TargetableManager
    {
        private List<Targetable> targetables = new List<Targetable>();

        public void NotifyAboutNewTargetable(Targetable targetable)
        {
            this.targetables.Add(targetable);
        }

        public void NotifyAboutTargetableGone(Targetable targetable)
        {
            this.targetables.Remove(targetable);
        }

        public IEnumerable<Targetable> Targets => this.targetables;
    }
}