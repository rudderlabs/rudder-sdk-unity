package com.rudderlabs.android.sdk.core;

import android.content.Context;
import android.text.TextUtils;

import com.rudderlabs.android.sdk.core.util.Utils;

import java.util.Locale;
import java.util.Map;

public class RudderClientWrapper {

    private static RudderClient rudderClient;

    public static void _initiateInstance(
            Context _context,
            String _anonymousId,
            String _writeKey,
            String _endPointUrl,
            int _flushQueueSize,
            int _dbCountThreshold,
            int _sleepTimeout,
            int _logLevel
    ) {
        if (_context == null) {
            RudderLogger.logError("Context can not be null");
            return;
        }

        if (TextUtils.isEmpty(_writeKey)) {
            RudderLogger.logError("WriteKey can not be null or empty");
            return;
        }

        if (TextUtils.isEmpty(_anonymousId)) {
            _anonymousId = Utils.getDeviceId(_context);
        }
        RudderElementCache.setAnonymousId(_anonymousId);

        RudderConfig config = new RudderConfig.Builder()
                .withEndPointUri(_endPointUrl)
                .withFlushQueueSize(_flushQueueSize)
                .withDbThresholdCount(_dbCountThreshold)
                .withSleepCount(_sleepTimeout)
                .withLogLevel(_logLevel)
                .build();

        rudderClient = RudderClient.getInstance(_context, _writeKey, config);
        RudderLogger.logDebug("Client initiated successfully");
    }

    public static void _logEvent(
            String _eventType,
            String _eventName,
            String _eventPropsJson,
            String _userPropsJson,
            String _optionsJson
    ) {
        if (rudderClient == null) {
            return;
        }

        RudderMessage message = new RudderMessageBuilder()
                .setEventName(_eventName)
                .setProperty(Utils.convertToMap(_eventPropsJson))
                .setUserProperty(Utils.convertToMap(_userPropsJson))
                .setRudderOption(Utils.convertToMap(_optionsJson))
                .build();

        switch (_eventType) {
            case "track":
                rudderClient.track(message);
                break;
            case "screen":
                rudderClient.screen(message);
                break;
            case "identify":
                RudderLogger.logError("Identify is not supported via this endpoint");
        }
    }

    public static void _identify(String _userId, String _traitsJson, String _optionsJson) {
        RudderLogger.logDebug(String.format(Locale.US, "_userId: %s", _userId));
        RudderLogger.logDebug(String.format(Locale.US, "_traitsJson: %s", _traitsJson));
        RudderLogger.logDebug(String.format(Locale.US, "_optionsJson: %s", _optionsJson));

        if (rudderClient == null) {
            return;
        }

        // check for anonymousId
        Map<String, Object> traitsMap = Utils.convertToMap(_traitsJson);
        if (!traitsMap.containsKey("anonymousId")) {
            traitsMap.put("anonymousId", Utils.getDeviceId(rudderClient.getApplication()));
        }
        rudderClient.identify(
                _userId,
                traitsMap,
                Utils.convertToMap(_optionsJson)
        );
    }

    public static void _reset() {
        if (rudderClient == null) {
            return;
        }

        rudderClient.reset();
    }
}
