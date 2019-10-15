package com.rudderlabs.android.sdk.core;

import java.util.Map;

/*
 * builder for RudderElement (alias RudderEvent)
 * */
public class RudderElementBuilder {
    private String eventName = null;

    public RudderElementBuilder setEventName(String eventName) {
        this.eventName = eventName;
        return this;
    }

    private String userId = null;

    public RudderElementBuilder setUserId(String userId) {
        this.userId = userId;
        return this;
    }

    private RudderProperty property;

    public RudderElementBuilder setProperty(RudderProperty property) {
        this.property = property;
        return this;
    }

    public RudderElementBuilder setProperty(RudderPropertyBuilder builder) throws RudderException {
        this.property = builder.build();
        return this;
    }

    public RudderElementBuilder setProperty(Map<String, Object> map) {
        property = new RudderProperty();
        property.setProperty(map);
        return this;
    }

    private RudderUserProperty userProperty;

    public RudderElementBuilder setUserProperty(RudderUserProperty userProperty) {
        this.userProperty = userProperty;
        return this;
    }

    public RudderElementBuilder setUserProperty(Map<String, Object> map) {
        this.userProperty = new RudderUserProperty();
        userProperty.setProperty(map);
        return this;
    }

    public RudderElement build() {
        RudderElement event = new RudderElement();
        event.setUserId(userId);
        event.setEventName(eventName);
        event.setProperty(property);
        event.setUserProperty(userProperty);
        return event;
    }
}
