using System.Collections.Generic;

public class RudderElementBuilder
{
    private string eventName;
    public RudderElementBuilder WithEventName(string eventName)
    {
        this.eventName = eventName;
        return this;
    }
    private string userId;
    public RudderElementBuilder WithUserId(string userId)
    {
        this.userId = userId;
        return this;
    }
    private Dictionary<string, object> eventProperties;
    public RudderElementBuilder WithEventProperties(Dictionary<string, object> eventProperties)
    {
        if (eventProperties == null)
        {
            // do not set null value
            return this;
        }
        else
        {
            if (this.eventProperties == null)
            {
                this.eventProperties = new Dictionary<string, object>();
            }
            foreach (var key in eventProperties.Keys)
            {
                var value = eventProperties[key];
                if (value != null)
                {
                    this.eventProperties.Add(key, value);
                }
            }
        }
        return this;
    }
    public RudderElementBuilder WithEventProperty(string key, object value)
    {
        if (this.eventProperties == null)
        {
            this.eventProperties = new Dictionary<string, object>();
        }
        if (value != null)
        {
            this.eventProperties.Add(key, value);
        }
        return this;
    }
    private Dictionary<string, object> userProperties;
    public RudderElementBuilder WithUserProperties(Dictionary<string, object> userProperties)
    {
        if (userProperties == null)
        {
            // do not set null values
            return this;
        }
        else
        {
            if (this.userProperties == null)
            {
                this.userProperties = new Dictionary<string, object>();
            }
            foreach (var key in userProperties.Keys)
            {
                var value = userProperties[key];
                if (value != null)
                {
                    this.userProperties.Add(key, value);
                }
            }
        }
        return this;
    }
    public RudderElementBuilder WithUserProperty(string key, object value)
    {
        if (this.userProperties == null)
        {
            this.userProperties = new Dictionary<string, object>();
        }
        if (value != null)
        {
            this.userProperties.Add(key, value);
        }
        return this;
    }
    public RudderElement Build()
    {
        RudderElement element = new RudderElement();
        element.eventName = this.eventName;
        element.userId = this.userId;
        element.eventProperties = this.eventProperties;
        element.userProperties = this.userProperties;
        return element;
    }

}