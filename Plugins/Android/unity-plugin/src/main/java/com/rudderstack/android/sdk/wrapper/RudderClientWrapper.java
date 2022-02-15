package com.rudderstack.android.sdk.wrapper;

import android.app.Application;
import android.content.Context;
import android.text.TextUtils;
import com.rudderstack.android.sdk.core.RudderClient;
import com.rudderstack.android.sdk.core.RudderConfig;
import com.rudderstack.android.sdk.core.RudderElementCache;
import com.rudderstack.android.sdk.core.RudderLogger;
import com.rudderstack.android.sdk.core.RudderMessage;
import com.rudderstack.android.sdk.core.RudderMessageBuilder;
import com.rudderstack.android.sdk.core.RudderOption;
import com.rudderstack.android.sdk.core.RudderTraits;
import com.rudderstack.android.sdk.core.util.Utils;
import java.util.Locale;
import java.util.Map;

public class RudderClientWrapper {

  private static RudderClient rudderClient;

  public static void _initiateInstance(
    Application application,
    Context _context,
    String _anonymousId,
    String _writeKey,
    String _dataPlaneUrl,
    String _controlPlaneUrl,
    int _flushQueueSize,
    int _dbCountThreshold,
    int _sleepTimeout,
    int _configRefreshInterval,
    boolean _trackLifecycleEvents,
    boolean _recordScreenViews,
    int _logLevel
  ) {
    if (rudderClient == null) {
      if (_context == null) {
        RudderLogger.logError("Context can not be null");
        return;
      }

      if (TextUtils.isEmpty(_writeKey)) {
        RudderLogger.logError("WriteKey can not be null or empty");
        return;
      }

      if (TextUtils.isEmpty(_anonymousId)) {
        _anonymousId = Utils.getDeviceId(application);
      }

      RudderConfig config = new RudderConfig.Builder()
        .withDataPlaneUrl(_dataPlaneUrl)
        .withControlPlaneUrl(_controlPlaneUrl)
        .withFlushQueueSize(_flushQueueSize)
        .withDbThresholdCount(_dbCountThreshold)
        .withSleepCount(_sleepTimeout)
        .withLogLevel(_logLevel)
        .withConfigRefreshInterval(_configRefreshInterval)
        .withTrackLifecycleEvents(_trackLifecycleEvents)
        .withRecordScreenViews(_recordScreenViews)
        .build();

      rudderClient = RudderClient.getInstance(_context, _writeKey, config);
      RudderElementCache.setAnonymousId(_anonymousId);
      RudderLogger.logDebug("Client initiated successfully");
    }
  }

  public static void _logEvent(
    String _eventType,
    String _eventName,
    String _eventPropsJson,
    String _userPropsJson,
    RudderOption _optionsJson
  ) {
    if (rudderClient == null) {
      return;
    }

    if (_eventType != "setAnonymousId") {
      RudderMessage message = new RudderMessageBuilder()
        .setEventName(_eventName)
        .setProperty(Utils.convertToMap(_eventPropsJson))
        .setUserProperty(Utils.convertToMap(_userPropsJson))
        .setRudderOption(_optionsJson)
        .build();
    }

    switch (_eventType) {
      case "track":
        rudderClient.track(message);
        break;
      case "screen":
        rudderClient.screen(message);
        break;
      case "identify":
        RudderLogger.logError("Identify is not supported via this endpoint");
        break;
      case "setAnonymousId":
        RudderClient.putAnonymousId(_eventName);
        break;
    }
  }

  public static void _identify(
    String _userId,
    RudderTraits _traitsJson,
    RudderOption _optionsJson
  ) {
    if (rudderClient == null) {
      return;
    }

    RudderLogger.logDebug(String.format(Locale.US, "_userId: %s", _userId));
    RudderLogger.logDebug(
      String.format(Locale.US, "_traitsJson: %s", _traitsJson)
    );
    RudderLogger.logDebug(
      String.format(Locale.US, "_optionsJson: %s", _optionsJson)
    );

    // check for anonymousId
    if (_traitsJson == null) {
      _traitsJson = new RudderTraits();
    }
    rudderClient.identify(_userId, _traitsJson, _optionsJson);
  }

  public static void _reset() {
    if (rudderClient == null) {
      return;
    }

    rudderClient.reset();
  }
}
