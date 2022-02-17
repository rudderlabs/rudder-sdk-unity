package com.rudderstack.android.sdk.wrapper;

import android.content.Context;
import android.text.TextUtils;
import com.rudderstack.android.sdk.core.RudderClient;
import com.rudderstack.android.sdk.core.RudderConfig;
import com.rudderstack.android.sdk.core.RudderLogger;
import com.rudderstack.android.sdk.core.RudderMessageBuilder;
import com.rudderstack.android.sdk.core.RudderOption;
import com.rudderstack.android.sdk.core.RudderTraits;
import com.rudderstack.android.sdk.core.RudderTraitsBuilder;
import com.rudderstack.android.sdk.core.util.Utils;

import java.util.List;
import java.util.Locale;
import java.util.Map;

public class RudderClientWrapper {

  private static RudderClient rudderClient;

  public static void _initiateInstance(
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

      if(_anonymousId != null) {
      RudderClient.putAnonymousId(_anonymousId);
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
      RudderLogger.logDebug("Client initiated successfully");
    }
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

    RudderMessageBuilder builder = new RudderMessageBuilder();

      builder.setEventName(_eventName);
      if(_eventPropsJson != null) {
      builder.setProperty(Utils.convertToMap(_eventPropsJson));
      }
      // if(_userPropsJson != null) {
      //   builder.setUserProperty(Utils.convertToMap(_userPropsJson));
      // }
      if(_optionsJson != null) {
        Map<String, Object> optionsMap = Utils.convertToMap(_optionsJson);
       builder.setRudderOption(_getRudderOptionsObject(optionsMap));
      }
      
    switch (_eventType) {
      case "track":
        rudderClient.track(builder.build());
        break;
      case "screen":
        rudderClient.screen(builder.build());
        break;
      case "identify":
        RudderLogger.logError("message type is not supported");
    }
  }

  public static void _identify(
    String _userId,
    String _traitsJson,
    String _optionsJson
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
    
    RudderTraits traits = null;
    if(_traitsJson != null) {
        Map<String, Object> traitsMap = Utils.convertToMap(_traitsJson); 
        traits = _getRudderTraitsObject(traitsMap);
    }
    
    RudderOption option = null;
    if(_optionsJson != null) {
       Map<String, Object> optionsMap = Utils.convertToMap(_optionsJson);
       option = _getRudderOptionsObject(optionsMap);
    }

    rudderClient.identify(_userId, traits, option);
  }

  public static void _reset() {
    if (rudderClient == null) {
      return;
    }

    rudderClient.reset();
  }

  public static void _setAnonymousId(String _anonymousId) {
    RudderLogger.logDebug(String.format(Locale.US, "_setAnonymousId: %s", _anonymousId));
    if (_anonymousId != null) {
      RudderClient.putAnonymousId(_anonymousId);
    }
  }

  public static RudderTraits _getRudderTraitsObject(Map<String, Object> traitsMap) {
        RudderTraitsBuilder builder = new RudderTraitsBuilder();
        if (traitsMap.containsKey("address")) {
            Map<String, Object> addressMap = (Map<String, Object>) traitsMap.get(
                    "address"
            );
            if (addressMap != null) {
                if (addressMap.containsKey("city")) {
                    builder.setCity((String) addressMap.get("city"));
                }
                if (addressMap.containsKey("country")) {
                    builder.setCountry((String) addressMap.get("country"));
                }
                if (addressMap.containsKey("postalCode")) {
                    builder.setPostalCode((String) addressMap.get("postalCode"));
                }
                if (addressMap.containsKey("state")) {
                    builder.setState((String) addressMap.get("state"));
                }
                if (addressMap.containsKey("street")) {
                    builder.setStreet((String) addressMap.get("street"));
                }
            }
        }
        if (traitsMap.containsKey("age") && traitsMap.get("age") != null) {
            builder.setAge(Integer.parseInt((String) traitsMap.get("age")));
        }
        if (traitsMap.containsKey("birthday")) {
            builder.setBirthDay((String) traitsMap.get("birthday"));
        }
        if (traitsMap.containsKey("company")) {
            Map<String, Object> companyMap = (Map<String, Object>) traitsMap.get(
                    "company"
            );
            if (companyMap != null) {
                if (companyMap.containsKey("name")) {
                    builder.setCompanyName((String) companyMap.get("name"));
                }
                if (companyMap.containsKey("id")) {
                    builder.setCompanyId((String) companyMap.get("id"));
                }
                if (companyMap.containsKey("industry")) {
                    builder.setIndustry((String) companyMap.get("industry"));
                }
            }
        }
        if (traitsMap.containsKey("createdAt")) {
            builder.setCreateAt((String) traitsMap.get("createdAt"));
        }
        if (traitsMap.containsKey("description")) {
            builder.setDescription((String) traitsMap.get("description"));
        }
        if (traitsMap.containsKey("email")) {
            builder.setEmail((String) traitsMap.get("email"));
        }
        if (traitsMap.containsKey("firstName")) {
            builder.setFirstName((String) traitsMap.get("firstName"));
        }
        if (traitsMap.containsKey("gender")) {
            builder.setGender((String) traitsMap.get("gender"));
        }
        if (traitsMap.containsKey("id")) {
            builder.setId((String) traitsMap.get("id"));
        }
        if (traitsMap.containsKey("lastName")) {
            builder.setLastName((String) traitsMap.get("lastName"));
        }
        if (traitsMap.containsKey("name")) {
            builder.setName((String) traitsMap.get("name"));
        }
        if (traitsMap.containsKey("phone")) {
            builder.setPhone((String) traitsMap.get("phone"));
        }
        if (traitsMap.containsKey("title")) {
            builder.setTitle((String) traitsMap.get("title"));
        }
        if (traitsMap.containsKey("userName")) {
            builder.setUserName((String) traitsMap.get("userName"));
        }
        RudderTraits traits = builder.build();
        if (traitsMap.containsKey("extras")) {
            Map<String, Object> extras = (Map<String, Object>) traitsMap.get(
                    "extras"
            );
            for (Map.Entry<String, Object> entry : extras.entrySet()) {
                traits.put(entry.getKey(), entry.getValue());
            }
        }
        return traits;
    }

    public static RudderOption _getRudderOptionsObject(
            Map<String, Object> optionsMap
    ) {
        RudderOption option = new RudderOption();
        if (optionsMap.containsKey("externalIds")) {
            List<Map<String, Object>> externalIdsList = (List<Map<String, Object>>) optionsMap.get("externalIds");
            for (int i = 0; i < externalIdsList.size(); i++) {
                Map<String, Object> externalIdMap = (Map<String, Object>) externalIdsList.get(
                        i
                );
                String type = (String) externalIdMap.get("type");
                String id = (String) externalIdMap.get("id");
                option.putExternalId(type, id);
            }
        }

        if (optionsMap.containsKey("integrations")) {
            Map<String, Object> integrationsMap = (Map<String, Object>) optionsMap.get("integrations");
            for (Map.Entry<String, Object> entry : integrationsMap.entrySet()) {
                option.putIntegration(entry.getKey(), (boolean) entry.getValue());
            }
        }
        return option;
    }
}
