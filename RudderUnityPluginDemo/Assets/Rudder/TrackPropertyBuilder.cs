using System.Collections.Generic;
namespace RudderStack
{
    public class TrackPropertyBuilder
    {
        private string category;
        public TrackPropertyBuilder SetCategory(string category)
        {
            this.category = category;
            return this;
        }

        private string label;
        public TrackPropertyBuilder SetLabel(string label)
        {
            this.label = label;
            return this;
        }

        private string value;
        public TrackPropertyBuilder SetValue(string value)
        {
            this.value = value;
            return this;
        }

        public Dictionary<string, object> Build()
        {
            if (category == null)
            {
                throw new RudderException("Key \"category\" is required for track event");
            }

            Dictionary<string, object> property = new Dictionary<string, object>();
            property.Add("category", this.category);
            if (this.label != null)
            {
                property.Add("label", this.label);
            }
            if (this.value != null)
            {
                property.Add("value", this.value);
            }
            return property;
        }
    }
}