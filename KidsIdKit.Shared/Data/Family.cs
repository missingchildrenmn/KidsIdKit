﻿namespace KidsIdKit.Shared.Data
{
    public class Family
    {
        public List<Child> Children { get; set; }

        public DateTime LastDateTimeAnyChildWasUpdated { get; set; }

        public Family()
        {
            Children = new List<Child>();
            LastDateTimeAnyChildWasUpdated = DateTime.MinValue;
        }
    }
}
