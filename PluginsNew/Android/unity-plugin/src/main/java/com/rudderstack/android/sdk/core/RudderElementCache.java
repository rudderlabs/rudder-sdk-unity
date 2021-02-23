package com.rudderstack.android.sdk.core;

import android.app.Application;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/*
 * RudderContext is populated once and cached through out the lifecycle
 * */
public class RudderElementCache {
    static RudderContext cachedContext;

    private RudderElementCache() {
        // stop instantiating
    }

    static void initiate(Application application, String anonymousId, String advertisingId) {
        if (cachedContext == null) {
            RudderLogger.logDebug("RudderElementCache: initiating RudderContext");
            cachedContext = new RudderContext(application, anonymousId, advertisingId);
            cachedContext.updateDeviceWithAdId();
        }
    }

    static RudderContext getCachedContext() {
        return cachedContext.copy();
    }

    static void reset() {
        cachedContext.updateTraits(null);
        cachedContext.updateExternalIds(null);
        persistTraits();
    }

    static void persistTraits() {
        cachedContext.persistTraits();
    }

    static void updateTraits(Map<String, Object> traits) {
        cachedContext.updateTraitsMap(traits);
    }

    public static void updateExternalIds(List<Map<String, Object>> externalIds) {
        cachedContext.updateExternalIds(externalIds);
    }

    public static void setAnonymousId(String anonymousId) {
        Map<String, Object> traits =new HashMap<>();
        traits.put("anonymousId",anonymousId);
        cachedContext.updateTraitsMap(traits);
    }
}

