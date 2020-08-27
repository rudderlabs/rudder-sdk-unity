package com.rudderstack.android.sdk.core;

import android.app.Application;

import java.util.HashMap;
import java.util.Map;

/*
 * RudderContext is populated once and cached through out the lifecycle
 * */
public class RudderElementCache {
    private static RudderContext cachedContext;

    private RudderElementCache() {
        // stop instantiating
    }

    static void initiate(Application application) {
        if (cachedContext == null) {
            RudderLogger.logDebug("RudderElementCache: initiating RudderContext");
            cachedContext = new RudderContext(application);
        }
    }

    static RudderContext getCachedContext() {
        return cachedContext;
    }

    static void reset() {
        cachedContext.updateTraits(null);
        persistTraits();
    }

    static void persistTraits() {
        cachedContext.persistTraits();
    }

    static void updateTraits(Map<String, Object> traits) {
        cachedContext.updateTraitsMap(traits);
    }

    public static void setAnonymousId(String anonymousId) {

        System.out.println("Ruchira Moitra");
        Map<String, Object> traits =new HashMap<>();
        traits.put("anonymousId",anonymousId);
        System.out.println(traits);
        System.out.println(cachedContext);
        cachedContext.updateTraitsMap(traits);
    }
}

